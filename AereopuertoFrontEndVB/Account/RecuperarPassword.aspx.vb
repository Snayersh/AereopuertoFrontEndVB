Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Net.Mail
Imports System.Net

Public Class RecuperarPassword
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        pnlMensaje.Visible = False
    End Sub

    Protected Sub btnEnviar_Click(sender As Object, e As EventArgs) Handles btnEnviar.Click
        Dim correo As String = txtEmail.Text.Trim()
        Dim tokenRecuperacion As String = Guid.NewGuid().ToString() ' Generamos un token mágico de 36 caracteres

        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_GENERAR_TOKEN_RECUPERACION", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo
                    cmd.Parameters.Add("p_token", OracleDbType.Varchar2).Value = tokenRecuperacion

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = paramOut.Value.ToString()

                    If resultado = "EXITO" Then
                        EnviarCorreoRecuperacion(correo, tokenRecuperacion)

                        pnlMensaje.Visible = True
                        pnlMensaje.CssClass = "alert alert-success text-center rounded-3 mb-4"
                        lblMensaje.Text = "¡Listo! Si el correo existe, te hemos enviado un enlace para recuperar tu contraseña."
                        txtEmail.Text = "" ' Limpiamos por seguridad
                    Else
                        ' Por seguridad, no le decimos al hacker si el correo existe o no, le mostramos el mismo mensaje.
                        pnlMensaje.Visible = True
                        pnlMensaje.CssClass = "alert alert-success text-center rounded-3 mb-4"
                        lblMensaje.Text = "¡Listo! Si el correo existe, te hemos enviado un enlace para recuperar tu contraseña."
                    End If
                End Using
            End Using

        Catch ex As Exception
            pnlMensaje.Visible = True
            pnlMensaje.CssClass = "alert alert-danger"
            lblMensaje.Text = "Error del sistema: " & ex.Message
        End Try
    End Sub

    Private Sub EnviarCorreoRecuperacion(correoDestino As String, token As String)
        Try
            Dim smtp As New SmtpClient("smtp.gmail.com", 587)
            smtp.EnableSsl = True
            smtp.Credentials = New NetworkCredential("proyectoaeroupuertoaurora@gmail.com", "ezkk vcci gsgy qguh")

            Dim mensaje As New MailMessage()
            mensaje.From = New MailAddress("no-reply@laaurora.com", "Soporte La Aurora")
            mensaje.To.Add(correoDestino)
            mensaje.Subject = "Recuperación de Contraseña - La Aurora"
            mensaje.IsBodyHtml = True

            ' La página a la que los vamos a mandar para que escriban su nueva clave
            Dim urlRecuperacion As String = "https://localhost:44356/Account/NuevaPassword.aspx?token=" & token

            mensaje.Body = $"
                <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <h2 style='color: #0d47a1;'>Recuperación de Contraseña</h2>
                    <p>Hemos recibido una solicitud para cambiar la contraseña de tu cuenta en La Aurora.</p>
                    <p>Haz clic en el botón de abajo para asignar una nueva contraseña. Si tú no hiciste esta solicitud, ignora este correo.</p>
                    <br>
                    <a href='{urlRecuperacion}' style='background-color: #f57c00; color: white; padding: 10px 20px; text-decoration: none; font-weight: bold; border-radius: 5px;'>Cambiar Contraseña</a>
                </div>"

            smtp.Send(mensaje)
        Catch ex As Exception
            ' Silencioso
        End Try
    End Sub
End Class