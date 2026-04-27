Imports System.Security.Cryptography
Imports System.Text
Imports System.Net.Mail
Imports System.Net

Public Class UtilidadesService

    ' 1. FUNCIÓN DE ENCRIPTADO COMPARTIDA
    Public Shared Function EncriptarSHA256(texto As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto))
            Dim builder As New StringBuilder()
            For i As Integer = 0 To bytes.Length - 1
                builder.Append(bytes(i).ToString("x2"))
            Next
            Return builder.ToString()
        End Using
    End Function

    ' 2. FUNCIÓN DE CORREO COMPARTIDA
    Public Shared Sub EnviarCorreoActivacion(correoDestino As String, nombre As String, linkActivacion As String)
        Try
            Dim smtp As New SmtpClient("smtp.gmail.com", 587)
            smtp.EnableSsl = True
            ' Considera poner estas credenciales en el Web.config en el futuro por seguridad
            smtp.Credentials = New NetworkCredential("proyectoaeroupuertoaurora@gmail.com", "ezkk vcci gsgy qguh")

            Dim mensaje As New MailMessage()
            mensaje.From = New MailAddress("no-reply@laaurora.com", "Aeropuerto La Aurora")
            mensaje.To.Add(correoDestino)
            mensaje.Subject = "Activa tu cuenta en La Aurora GUA"
            mensaje.IsBodyHtml = True

            mensaje.Body = $"
                <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <h2 style='color: #0d47a1;'>¡Hola {nombre}!</h2>
                    <p>Gracias por registrarte en el sistema de La Aurora. Para poder iniciar sesión, necesitamos que confirmes tu dirección de correo electrónico.</p>
                    <br>
                    <a href='{linkActivacion}' style='background-color: #0d47a1; color: white; padding: 10px 20px; text-decoration: none; font-weight: bold; border-radius: 5px;'>Activar mi Cuenta</a>
                    <br><br>
                    <p style='color: #777; font-size: 12px;'>Si el botón no funciona, copia y pega este enlace en tu navegador:<br>{linkActivacion}</p>
                </div>"

            smtp.Send(mensaje)
        Catch ex As Exception
            Throw New Exception("No se pudo enviar el correo de activación.")
        End Try
    End Sub

End Class