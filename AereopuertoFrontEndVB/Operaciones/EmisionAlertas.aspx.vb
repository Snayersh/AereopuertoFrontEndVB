Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Net ' Agregado para credenciales de red
Imports System.Net.Mail ' Agregado para envío de correos

Public Class EmisionAlertas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Solo Admin (1) o Personal de Operaciones/Servicio (3)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 1 AndAlso idRol <> 3) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarUsuarios()
            CargarHistorialAlertas()
        End If
    End Sub

    Private Sub CargarUsuarios()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_USUARIOS_NOTIF", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cur As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cur.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cur)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlUsuarioDestino.DataSource = reader
                        ddlUsuarioDestino.DataTextField = "DATOS_USUARIO" ' Viene como: Nombre Apellido (correo@dominio.com)
                        ddlUsuarioDestino.DataValueField = "ID_USUARIO"
                        ddlUsuarioDestino.DataBind()
                    End Using
                End Using
            End Using
            ddlUsuarioDestino.Items.Insert(0, New ListItem("-- Seleccione un Destinatario --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar la libreta de contactos: " & ex.Message, False)
        End Try
    End Sub

    Private Sub CargarHistorialAlertas()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_ALERTAS_EMITIDAS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptAlertas.DataSource = dt
                            rptAlertas.DataBind()
                            rptAlertas.Visible = True
                            pnlVacio.Visible = False
                        Else
                            rptAlertas.Visible = False
                            pnlVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Error silencioso en la carga inicial
        End Try
    End Sub

    Protected Sub btnEnviar_Click(sender As Object, e As EventArgs) Handles btnEnviar.Click
        If String.IsNullOrEmpty(ddlUsuarioDestino.SelectedValue) OrElse String.IsNullOrEmpty(txtMensaje.Text) Then
            MostrarMensaje("Por favor, seleccione un destinatario y escriba un mensaje.", False)
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ENVIAR_NOTIFICACION", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_mensaje", OracleDbType.Varchar2).Value = txtMensaje.Text.Trim()
                    cmd.Parameters.Add("p_id_usuario", OracleDbType.Int32).Value = Convert.ToInt32(ddlUsuarioDestino.SelectedValue)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outResultado.Value.ToString() = "EXITO" Then

                        ' ==========================================
                        ' LÓGICA DE EXTRACCIÓN Y ENVÍO DE CORREO
                        ' ==========================================
                        Dim textoDestinatario As String = ddlUsuarioDestino.SelectedItem.Text
                        Dim correoExtraido As String = ExtraerCorreo(textoDestinatario)

                        ' Intentamos mandar el correo. Si falla, al menos ya guardó en la BD
                        Dim envioCorreoExitoso As Boolean = EnviarCorreoSMTP(correoExtraido, txtMensaje.Text.Trim())

                        If envioCorreoExitoso Then
                            MostrarMensaje("✅ Alerta guardada en el sistema y enviada por correo electrónico.", True)
                        Else
                            MostrarMensaje("⚠️ Alerta guardada en el sistema, pero hubo un problema al enviar el correo.", True)
                        End If

                        txtMensaje.Text = ""
                        ddlUsuarioDestino.SelectedIndex = 0
                        CargarHistorialAlertas() ' Refrescar tabla
                    Else
                        MostrarMensaje("Error: " & outResultado.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error interno al enviar alerta: " & ex.Message, False)
        End Try
    End Sub

    ' -------------------------------------------------------------
    ' FUNCIÓN PARA EXTRAER EL CORREO ENTRE PARÉNTESIS
    ' -------------------------------------------------------------
    Private Function ExtraerCorreo(texto As String) As String
        Try
            Dim inicio As Integer = texto.LastIndexOf("(")
            Dim fin As Integer = texto.LastIndexOf(")")
            If inicio <> -1 AndAlso fin <> -1 AndAlso fin > inicio Then
                Return texto.Substring(inicio + 1, fin - inicio - 1).Trim()
            End If
        Catch ex As Exception
        End Try
        Return "" ' Retorna vacío si no lo encuentra
    End Function

    ' -------------------------------------------------------------
    ' FUNCIÓN OFICIAL PARA EL ENVÍO DE CORREOS SMTP (LA AURORA)
    ' -------------------------------------------------------------
    Private Function EnviarCorreoSMTP(correoDestino As String, mensajeNotificacion As String) As Boolean
        If String.IsNullOrEmpty(correoDestino) Then Return False

        Try
            Dim smtp As New SmtpClient("smtp.gmail.com", 587)
            smtp.EnableSsl = True
            smtp.Credentials = New NetworkCredential("proyectoaeroupuertoaurora@gmail.com", "ezkk vcci gsgy qguh")

            Dim mail As New MailMessage()
            mail.From = New MailAddress("proyectoaeroupuertoaurora@gmail.com", "Aeropuerto La Aurora")
            mail.To.Add(correoDestino)
            mail.Subject = "🔔 Importante: Nueva Notificación de La Aurora"

            ' Cuerpo del correo con HTML básico para que se vea profesional
            Dim cuerpoHTML As String = "
                <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px; max-width: 600px;'>
                    <h2 style='color: #0d47a1;'>Aeropuerto Internacional La Aurora</h2>
                    <p style='font-size: 16px; color: #333;'>Estimado pasajero/usuario,</p>
                    <p style='font-size: 16px; color: #333;'>Tiene una nueva notificación operativa en su expediente:</p>
                    <div style='background-color: #f8f9fa; padding: 15px; border-left: 4px solid #0d47a1; margin: 20px 0;'>
                        <p style='font-size: 18px; font-weight: bold; margin: 0;'>" & mensajeNotificacion & "</p>
                    </div>
                    <p style='font-size: 14px; color: #777;'>Por favor, si tiene dudas acérquese a nuestro mostrador de atención al cliente o revise su portal web.</p>
                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='font-size: 12px; color: #aaa; text-align: center;'>Este es un mensaje automático generado por el Centro de Control de La Aurora. No responda a este correo.</p>
                </div>"

            mail.Body = cuerpoHTML
            mail.IsBodyHtml = True

            smtp.Send(mail)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success fw-bold text-center rounded-3 mb-4 shadow-sm", "alert alert-danger fw-bold text-center rounded-3 mb-4 shadow-sm")
    End Sub
End Class