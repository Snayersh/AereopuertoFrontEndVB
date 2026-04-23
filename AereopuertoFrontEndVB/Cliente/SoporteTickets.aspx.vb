Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class SoporteTickets
    Inherits System.Web.UI.Page

    Private CorreoUsuario As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Solo Clientes (Rol 2) pueden abrir tickets propios
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse idRol <> 2 Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        CorreoUsuario = Session("UserEmail").ToString()

        If Not IsPostBack Then
            ' 1. Cargar datos iniciales
            CargarTiposTicket()
            CargarTicketsCliente()

            ' 2. ¡MAGIA URL! Si viene un ID en la URL, ocultamos el listado y mostramos la conversación
            If Request.QueryString("id") IsNot Nothing Then
                pnlListado.Visible = False
                pnlConversacion.Visible = True
                CargarHiloRespuestas(Request.QueryString("id").ToString())
            End If
        End If
    End Sub

    ' =======================================================
    ' MÉTODOS DEL PANEL 1 (LISTADO Y CREACIÓN)
    ' =======================================================
    Private Sub CargarTiposTicket()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_TIPOS_TICKET_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cur As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cur.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cur)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlTipoTicket.DataSource = reader
                        ddlTipoTicket.DataTextField = "NOMBRE"
                        ddlTipoTicket.DataValueField = "ID_TIPO_TICKET"
                        ddlTipoTicket.DataBind()
                    End Using
                End Using
            End Using
            ddlTipoTicket.Items.Insert(0, New ListItem("-- Seleccione Tipo --", ""))
        Catch ex As Exception
            ' Error silencioso en carga
        End Try
    End Sub

    Private Sub CargarTicketsCliente()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_TICKETS_CLIENTE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = CorreoUsuario

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptTickets.DataSource = dt
                            rptTickets.DataBind()
                            rptTickets.Visible = True
                            pnlVacioTickets.Visible = False
                        Else
                            rptTickets.Visible = False
                            pnlVacioTickets.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar sus tickets: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnGuardarTicket_Click(sender As Object, e As EventArgs) Handles btnGuardarTicket.Click
        If String.IsNullOrEmpty(txtAsunto.Text) OrElse String.IsNullOrEmpty(ddlTipoTicket.SelectedValue) Then
            MostrarMensaje("⚠️ Complete todos los campos para enviar el ticket.", False)
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CREAR_TICKET", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_asunto", OracleDbType.Varchar2).Value = txtAsunto.Text.Trim()
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = CorreoUsuario
                    cmd.Parameters.Add("p_id_tipo", OracleDbType.Int32).Value = Convert.ToInt32(ddlTipoTicket.SelectedValue)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outResultado.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Ticket abierto exitosamente. El equipo lo revisará pronto.", True)
                        txtAsunto.Text = ""
                        ddlTipoTicket.SelectedIndex = 0
                        CargarTicketsCliente()
                    Else
                        MostrarMensaje("⚠️ Error DB: " & outResultado.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error interno: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub rptTickets_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptTickets.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim estado As String = DataBinder.Eval(e.Item.DataItem, "ESTADO").ToString().ToUpper()
            Dim lblBadge As Label = CType(e.Item.FindControl("lblBadgeEstado"), Label)

            lblBadge.Text = estado
            If estado = "ABIERTO" Then
                lblBadge.CssClass = "badge-abierto shadow-sm"
            ElseIf estado = "CERRADO" Then
                lblBadge.CssClass = "badge-cerrado shadow-sm"
            Else
                lblBadge.CssClass = "badge-proceso shadow-sm"
            End If
        End If
    End Sub

    ' =======================================================
    ' MÉTODOS DEL PANEL 2 (CONVERSACIÓN / HILO)
    ' =======================================================
    Private Sub CargarHiloRespuestas(idTicket As String)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_RESPUESTAS_TICKET", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_id_ticket", OracleDbType.Int32).Value = Convert.ToInt32(idTicket)

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptRespuestas.DataSource = dt
                            rptRespuestas.DataBind()
                            rptRespuestas.Visible = True
                            pnlVacioRespuestas.Visible = False
                        Else
                            rptRespuestas.Visible = False
                            pnlVacioRespuestas.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar la conversación: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnEnviarRespuesta_Click(sender As Object, e As EventArgs) Handles btnEnviarRespuesta.Click
        If String.IsNullOrEmpty(txtNuevaRespuesta.Text) Then
            Return
        End If

        Dim idTicket As String = Request.QueryString("id").ToString()
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_AGREGAR_RESPUESTA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_ticket", OracleDbType.Int32).Value = Convert.ToInt32(idTicket)
                    cmd.Parameters.Add("p_mensaje", OracleDbType.Varchar2).Value = txtNuevaRespuesta.Text.Trim()

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outResultado.Value.ToString() = "EXITO" Then
                        txtNuevaRespuesta.Text = ""
                        CargarHiloRespuestas(idTicket) ' Recargar el chat
                    Else
                        MostrarMensaje("⚠️ Error al enviar: " & outResultado.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error interno: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub rptRespuestas_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptRespuestas.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim fechaObj = DataBinder.Eval(e.Item.DataItem, "FECHA")
            Dim lblFecha As Label = CType(e.Item.FindControl("lblFechaMensaje"), Label)

            If Not IsDBNull(fechaObj) Then
                lblFecha.Text = Convert.ToDateTime(fechaObj).ToString("dd MMM yyyy, HH:mm") & " hrs"
            End If
        End If
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm")
    End Sub
End Class