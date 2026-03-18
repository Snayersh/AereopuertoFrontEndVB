Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class RegistroTripulacion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Protección de seguridad: Solo Admins pueden registrar personal
        If Session("UserRole") Is Nothing OrElse Session("UserRole").ToString() <> "Admin" Then
            Response.Redirect("~/Default.aspx")
        End If

        If Not IsPostBack Then
            pnlMensaje.Visible = False
        End If
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_TRIPULANTE", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' ESTA LÍNEA ES VITAL: Evita el error de "argumentos erróneos" en Oracle
                    cmd.BindByName = True

                    ' Agregamos los parámetros tal cual los pide el SP
                    cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = txtNombre.Text.Trim()
                    cmd.Parameters.Add("p_puesto", OracleDbType.Varchar2).Value = ddlPuesto.SelectedValue

                    ' Si la licencia viene vacía, mandamos NULL a la DB
                    If String.IsNullOrEmpty(txtLicencia.Text.Trim()) Then
                        cmd.Parameters.Add("p_licencia", OracleDbType.Varchar2).Value = DBNull.Value
                    Else
                        cmd.Parameters.Add("p_licencia", OracleDbType.Varchar2).Value = txtLicencia.Text.Trim().ToUpper()
                    End If

                    ' Parámetro de salida para confirmar el éxito
                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = paramOut.Value.ToString()

                    If resultado = "EXITO" Then
                        MostrarMensaje("✅ Personal registrado correctamente en el sistema.", True)
                        LimpiarCampos()
                    Else
                        MostrarMensaje("⚠️ Error al registrar: " & resultado, False)
                    End If
                End Using
            End Using

        Catch ex As Exception
            MostrarMensaje("❌ Error de sistema: " & ex.Message, False)
        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm")
    End Sub

    Private Sub LimpiarCampos()
        txtNombre.Text = ""
        ddlPuesto.SelectedIndex = 0
        txtLicencia.Text = ""
    End Sub
End Class