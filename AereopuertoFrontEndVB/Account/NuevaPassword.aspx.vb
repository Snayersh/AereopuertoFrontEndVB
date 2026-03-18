Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Security.Cryptography
Imports System.Text

Public Class NuevaPassword
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Validamos que el enlace traiga un token
            If String.IsNullOrEmpty(Request.QueryString("token")) Then
                pnlFormulario.Visible = False
                pnlMensaje.Visible = True
                pnlMensaje.CssClass = "alert alert-danger"
                lblMensaje.Text = "Enlace inválido o expirado. Vuelve a solicitar la recuperación de contraseña."
            End If
        End If
    End Sub

    Protected Sub btnActualizar_Click(sender As Object, e As EventArgs) Handles btnActualizar.Click
        Dim token As String = Request.QueryString("token")

        If txtPass1.Text <> txtPass2.Text Then
            pnlMensaje.Visible = True
            pnlMensaje.CssClass = "alert alert-warning"
            lblMensaje.Text = "Las contraseñas no coinciden. Intenta de nuevo."
            Return
        End If

        ' Encriptamos la nueva contraseña antes de mandarla a Oracle
        Dim nuevaPassHasheada As String = EncriptarSHA256(txtPass1.Text)

        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ACTUALIZAR_PASSWORD_TOKEN", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_token", OracleDbType.Varchar2).Value = token
                    cmd.Parameters.Add("p_nuevo_hash", OracleDbType.Varchar2).Value = nuevaPassHasheada

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = paramOut.Value.ToString()

                    If resultado = "EXITO" Then
                        pnlFormulario.Visible = False
                        pnlMensaje.Visible = True
                        pnlMensaje.CssClass = "alert alert-success"
                        lblMensaje.Text = "¡Tu contraseña ha sido actualizada exitosamente! Ya puedes iniciar sesión."
                    Else
                        pnlMensaje.Visible = True
                        pnlMensaje.CssClass = "alert alert-danger"
                        lblMensaje.Text = "El enlace ya fue utilizado o no es válido."
                    End If
                End Using
            End Using

        Catch ex As Exception
            pnlMensaje.Visible = True
            pnlMensaje.CssClass = "alert alert-danger"
            lblMensaje.Text = "Error: " & ex.Message
        End Try
    End Sub

    Private Function EncriptarSHA256(texto As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto))
            Dim builder As New StringBuilder()
            For i As Integer = 0 To bytes.Length - 1
                builder.Append(bytes(i).ToString("x2"))
            Next
            Return builder.ToString()
        End Using
    End Function
End Class