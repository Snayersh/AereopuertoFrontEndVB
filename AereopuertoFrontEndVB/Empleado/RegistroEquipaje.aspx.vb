Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class RegistroEquipaje
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("UserRole") Is Nothing OrElse (Session("UserRole").ToString() <> "Empleado" AndAlso Session("UserRole").ToString() <> "Admin") Then
            Response.Redirect("~/Default.aspx")
        End If

        If Not IsPostBack Then
            txtCodigo.Focus()
        End If
    End Sub

    Protected Sub btnRegistrar_Click(sender As Object, e As EventArgs) Handles btnRegistrar.Click
        Dim codigo As String = txtCodigo.Text.Trim().ToUpper()
        Dim peso As String = txtPeso.Text.Trim()
        Dim descripcion As String = txtDescripcion.Text.Trim()

        If String.IsNullOrEmpty(codigo) OrElse String.IsNullOrEmpty(peso) Then
            MostrarAlerta("Completa todos los campos obligatorios.", "alert-warning")
            Return
        End If

        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_EQUIPAJE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo_reserva", OracleDbType.Varchar2).Value = codigo
                    cmd.Parameters.Add("p_peso", OracleDbType.Decimal).Value = Convert.ToDecimal(peso)
                    cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = descripcion

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim res As String = outResultado.Value.ToString()

                    If res = "EXITO" Then
                        MostrarAlerta("¡Equipaje registrado correctamente! Etiqueta generada.", "alert-success")
                        ' Limpiamos los campos para la siguiente maleta
                        txtCodigo.Text = ""
                        txtPeso.Text = ""
                        txtDescripcion.Text = ""
                        txtCodigo.Focus()
                    ElseIf res = "ERROR_NO_EXISTE_O_NO_PAGADO" Then
                        MostrarAlerta("Error: La reserva no existe o el pasajero aún no ha pagado sus boletos.", "alert-danger")
                    Else
                        MostrarAlerta("Error al registrar: " & res, "alert-danger")
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarAlerta("Error del sistema: " & ex.Message, "alert-danger")
        End Try
    End Sub

    Private Sub MostrarAlerta(mensaje As String, claseCss As String)
        pnlMensaje.Visible = True
        pnlMensaje.CssClass = "alert text-center fw-bold rounded-3 mb-4 " & claseCss
        lblMensaje.Text = mensaje
    End Sub
End Class