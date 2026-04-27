Public Class SoporteTickets
    Inherits System.Web.UI.Page

    Private CorreoUsuario As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = 0
        If Session("IdRol") IsNot Nothing Then idRol = Convert.ToInt32(Session("IdRol"))

        If Session("UserEmail") Is Nothing OrElse idRol <> 2 Then
            Response.Redirect("~/Account/Login.aspx")
            Return
        End If

        CorreoUsuario = Session("UserEmail").ToString()

        If Not IsPostBack Then
            CargarTiposTicket()
            CargarTicketsCliente()

            If Request.QueryString("id") IsNot Nothing Then
                pnlListado.Visible = False
                pnlConversacion.Visible = True
                CargarHiloRespuestas(Convert.ToInt32(Request.QueryString("id")))
            End If
        End If
    End Sub

    ' =======================================================
    ' MÉTODOS DEL PANEL 1 (LISTADO Y CREACIÓN)
    ' =======================================================
    Private Sub CargarTiposTicket()
        Dim respuesta = ClienteTicketService.ObtenerTiposTicket()

        If respuesta.success Then
            ddlTipoTicket.Items.Clear()
            ddlTipoTicket.Items.Add(New ListItem("-- Seleccione Tipo --", ""))

            For Each item In respuesta.tipos
                ddlTipoTicket.Items.Add(New ListItem(item("nombre").ToString(), item("id_tipo_ticket").ToString()))
            Next
        End If
    End Sub

    Private Sub CargarTicketsCliente()
        Dim respuesta = ClienteTicketService.ObtenerTicketsCliente(CorreoUsuario)

        If respuesta.success AndAlso respuesta.tickets.Count > 0 Then
            rptTickets.DataSource = respuesta.tickets
            rptTickets.DataBind()
            rptTickets.Visible = True
            pnlVacioTickets.Visible = False
        Else
            rptTickets.Visible = False
            pnlVacioTickets.Visible = True
        End If
    End Sub

    Protected Sub btnGuardarTicket_Click(sender As Object, e As EventArgs) Handles btnGuardarTicket.Click
        If String.IsNullOrEmpty(txtAsunto.Text) OrElse String.IsNullOrEmpty(ddlTipoTicket.SelectedValue) Then
            MostrarMensaje("⚠️ Complete todos los campos para enviar el ticket.", False)
            Return
        End If

        Dim respuesta = ClienteTicketService.CrearTicket(
            txtAsunto.Text.Trim(),
            CorreoUsuario,
            Convert.ToInt32(ddlTipoTicket.SelectedValue)
        )

        If respuesta.success Then
            MostrarMensaje(respuesta.mensaje, True)
            txtAsunto.Text = ""
            ddlTipoTicket.SelectedIndex = 0
            CargarTicketsCliente()
        Else
            MostrarMensaje(respuesta.mensaje, False)
        End If
    End Sub

    Protected Sub rptTickets_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptTickets.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            ' Recuperamos el dato como diccionario (minúsculas por nuestra utilidad)
            Dim itemData As Dictionary(Of String, Object) = CType(e.Item.DataItem, Dictionary(Of String, Object))
            Dim estado As String = itemData("estado")?.ToString().ToUpper()

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
    Private Sub CargarHiloRespuestas(idTicket As Integer)
        Dim respuesta = ClienteTicketService.ObtenerHiloRespuestas(idTicket)

        If respuesta.success AndAlso respuesta.respuestas.Count > 0 Then
            rptRespuestas.DataSource = respuesta.respuestas
            rptRespuestas.DataBind()
            rptRespuestas.Visible = True
            pnlVacioRespuestas.Visible = False
        Else
            rptRespuestas.Visible = False
            pnlVacioRespuestas.Visible = True
        End If
    End Sub

    Protected Sub btnEnviarRespuesta_Click(sender As Object, e As EventArgs) Handles btnEnviarRespuesta.Click
        If String.IsNullOrEmpty(txtNuevaRespuesta.Text) Then Return

        Dim idTicket As Integer = Convert.ToInt32(Request.QueryString("id"))

        Dim respuesta = ClienteTicketService.AgregarRespuesta(idTicket, txtNuevaRespuesta.Text.Trim())

        If respuesta.success Then
            txtNuevaRespuesta.Text = ""
            CargarHiloRespuestas(idTicket) ' Recargar el chat
        Else
            MostrarMensaje(respuesta.mensaje, False)
        End If
    End Sub

    Protected Sub rptRespuestas_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptRespuestas.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim itemData As Dictionary(Of String, Object) = CType(e.Item.DataItem, Dictionary(Of String, Object))
            Dim fechaObj = itemData("fecha")

            Dim lblFecha As Label = CType(e.Item.FindControl("lblFechaMensaje"), Label)

            If fechaObj IsNot Nothing Then
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