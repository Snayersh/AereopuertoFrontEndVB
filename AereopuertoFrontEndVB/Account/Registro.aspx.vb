Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Security.Cryptography
Imports System.Text
Imports System.Net.Mail
Imports System.Net

Public Class Registro
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            pnlMensaje.Visible = False
        End If
    End Sub

    Protected Sub btnRegistrar_Click(sender As Object, e As EventArgs) Handles btnRegistrar.Click
        Dim db As New ConexionDB()

        ' 1. Ciframos la contraseña usando SHA-256
        Dim contrasenaHasheada As String = EncriptarSHA256(txtPassword.Text.Trim())

        ' 2. Generamos un código único para activar la cuenta
        Dim tokenActivacion As String = Guid.NewGuid().ToString()

        ' ... dentro del bloque del botón registrar ...

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                ' Importante: Verifica que tu SP_REGISTRAR_CLIENTE_COMPLETO en Oracle 
                ' ahora acepte el parámetro p_pasaporte.
                Using cmd As New OracleCommand("SP_REGISTRAR_CLIENTE_COMPLETO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True ' Recomendado para evitar errores de orden

                    ' --- Parámetros de Persona ---
                    cmd.Parameters.Add("p_primer_nombre", OracleDbType.Varchar2).Value = txtPrimerNombre.Text.Trim()
                    cmd.Parameters.Add("p_segundo_nombre", OracleDbType.Varchar2).Value = txtSegundoNombre.Text.Trim()
                    cmd.Parameters.Add("p_tercer_nombre", OracleDbType.Varchar2).Value = txtTercerNombre.Text.Trim()
                    cmd.Parameters.Add("p_primer_apellido", OracleDbType.Varchar2).Value = txtPrimerApellido.Text.Trim()
                    cmd.Parameters.Add("p_segundo_apellido", OracleDbType.Varchar2).Value = txtSegundoApellido.Text.Trim()
                    cmd.Parameters.Add("p_apellido_casada", OracleDbType.Varchar2).Value = txtApellidoCasada.Text.Trim()
                    cmd.Parameters.Add("p_fecha_nacimiento", OracleDbType.Date).Value = Convert.ToDateTime(txtFechaNac.Text)
                    cmd.Parameters.Add("p_telefono", OracleDbType.Varchar2).Value = txtTelefono.Text.Trim()
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = txtEmail.Text.Trim()

                    ' --- Parámetros de Dirección ---
                    cmd.Parameters.Add("p_pais", OracleDbType.Varchar2).Value = txtPais.Text.Trim()
                    cmd.Parameters.Add("p_departamento", OracleDbType.Varchar2).Value = txtDepartamento.Text.Trim()
                    cmd.Parameters.Add("p_municipio", OracleDbType.Varchar2).Value = txtMunicipio.Text.Trim()
                    cmd.Parameters.Add("p_zona", OracleDbType.Varchar2).Value = txtZona.Text.Trim()
                    cmd.Parameters.Add("p_colonia", OracleDbType.Varchar2).Value = txtColonia.Text.Trim()
                    cmd.Parameters.Add("p_calle", OracleDbType.Varchar2).Value = txtCalleAvenida.Text.Trim()
                    cmd.Parameters.Add("p_avenida", OracleDbType.Varchar2).Value = DBNull.Value
                    cmd.Parameters.Add("p_numero_casa", OracleDbType.Varchar2).Value = txtNumCasa.Text.Trim()

                    ' --- Parámetros de Usuario Seguro y Pasaporte ---
                    cmd.Parameters.Add("p_password_hash", OracleDbType.Varchar2).Value = contrasenaHasheada
                    cmd.Parameters.Add("p_token", OracleDbType.Varchar2).Value = tokenActivacion
                    cmd.Parameters.Add("p_pasaporte", OracleDbType.Varchar2).Value = txtPasaporte.Text.Trim().ToUpper()

                    ' --- Salida ---
                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    ' ... resto del código de éxito y envío de correo ...

                    Dim resultado As String = paramOut.Value.ToString()

                    If resultado = "EXITO" Then
                        ' Si guardó en Oracle, procedemos a enviar el correo
                        EnviarCorreoActivacion(txtEmail.Text.Trim(), txtPrimerNombre.Text.Trim(), tokenActivacion)

                        pnlMensaje.Visible = True
                        pnlMensaje.CssClass = "alert alert-success text-center rounded-3 mb-4"
                        lblMensaje.Text = "¡Cuenta creada! Hemos enviado un enlace de activación a tu correo electrónico."
                    Else
                        MostrarError(resultado)
                    End If
                End Using
            End Using

        Catch ex As Exception
            MostrarError("Error del sistema: " & ex.Message)
        End Try
    End Sub

    ' =========================================================
    ' FUNCIÓN PARA ENCRIPTAR LA CONTRASEÑA (SHA-256)
    ' =========================================================
    Private Function EncriptarSHA256(texto As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto))
            Dim builder As New StringBuilder()
            For i As Integer = 0 To bytes.Length - 1
                builder.Append(bytes(i).ToString("x2"))
            Next
            Return builder.ToString() ' Devuelve la cadena alfanumérica ilegible
        End Using
    End Function

    ' =========================================================
    ' FUNCIÓN PARA ENVIAR CORREO ELECTRÓNICO
    ' =========================================================
    Private Sub EnviarCorreoActivacion(correoDestino As String, nombre As String, token As String)
        Try
            ' Configuración del SMTP (Ejemplo usando Gmail)
            ' Nota: Para que Gmail te deje, necesitas generar una "Contraseña de Aplicación" en tu cuenta de Google.
            Dim smtp As New SmtpClient("smtp.gmail.com", 587)
            smtp.EnableSsl = True
            smtp.Credentials = New NetworkCredential("proyectoaeroupuertoaurora@gmail.com", "ezkk vcci gsgy qguh")

            Dim mensaje As New MailMessage()
            mensaje.From = New MailAddress("no-reply@laaurora.com", "Aeropuerto La Aurora")
            mensaje.To.Add(correoDestino)
            mensaje.Subject = "Activa tu cuenta en La Aurora GUA"
            mensaje.IsBodyHtml = True

            ' URL a la que le darán clic (Crearemos esta página Activar.aspx después)
            Dim urlActivacion As String = "https://localhost:44356/Account/Activar.aspx?token=" & token

            mensaje.Body = $"
                <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <h2 style='color: #0d47a1;'>¡Hola {nombre}!</h2>
                    <p>Gracias por registrarte en el sistema de La Aurora. Para poder iniciar sesión, necesitamos que confirmes tu dirección de correo electrónico.</p>
                    <br>
                    <a href='{urlActivacion}' style='background-color: #0d47a1; color: white; padding: 10px 20px; text-decoration: none; font-weight: bold; border-radius: 5px;'>Activar mi Cuenta</a>
                    <br><br>
                    <p style='color: #777; font-size: 12px;'>Si el botón no funciona, copia y pega este enlace en tu navegador:<br>{urlActivacion}</p>
                </div>"

            smtp.Send(mensaje)
        Catch ex As Exception
            ' Si falla el envío de correo, lo capturamos silenciosamente o mostramos un error
            ' Dependiendo del entorno, puede que el firewall bloquee el puerto 587
        End Try
    End Sub

    Private Sub MostrarError(mensaje As String)
        pnlMensaje.Visible = True
        pnlMensaje.CssClass = "alert alert-danger text-center rounded-3 mb-4"
        lblMensaje.Text = mensaje
    End Sub

End Class