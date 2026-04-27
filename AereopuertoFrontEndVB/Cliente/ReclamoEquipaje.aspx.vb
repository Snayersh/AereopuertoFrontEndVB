Public Class ReclamoEquipaje
    Inherits System.Web.UI.Page

    Private CorreoUsuario As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = 0
        If Session("IdRol") IsNot Nothing Then idRol = Convert.ToInt32(Session("IdRol"))

        ' 🔥 SEGURIDAD: Solo Clientes / Pasajeros (Rol 2)
        If Session("UserEmail") Is Nothing OrElse (idRol <> 2) Then
            Response.Redirect("~/Account/Login.aspx")
            Return
        End If

        CorreoUsuario = Session("UserEmail").ToString()

        If Not IsPostBack Then
            CargarEquipajeCliente()
            CargarHistorialReclamos()
        End If
    End Sub

    ' =======================================================
    ' CARGAR SOLO LAS MALETAS DE ESTE CLIENTE EN EL SELECTOR
    ' =======================================================
    Private Sub CargarEquipajeCliente()
        Dim respuesta = ClienteReclamoService.ObtenerEquipajeCliente(CorreoUsuario)

        If respuesta.success Then
            ddlEquipaje.Items.Clear()
            ddlEquipaje.Items.Add(New ListItem("-- Seleccione la Maleta --", ""))

            For Each item In respuesta.equipaje
                ' Recordatorio: Las columnas vienen en minúsculas por nuestra utilidad
                ddlEquipaje.Items.Add(New ListItem(item("info_equipaje").ToString(), item("id_equipaje").ToString()))
            Next
        Else
            MostrarMensaje(respuesta.mensaje, False)
        End If
    End Sub

    ' =======================================================
    ' GUARDAR EL REPORTE
    ' =======================================================
    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If String.IsNullOrEmpty(ddlEquipaje.SelectedValue) OrElse String.IsNullOrEmpty(txtDescripcion.Text) Then
            MostrarMensaje("⚠️ Por favor, seleccione la maleta y detalle el problema.", False)
            Return
        End If

        Dim idEquipaje As Integer = Convert.ToInt32(ddlEquipaje.SelectedValue)

        ' 🔥 Llamamos al servicio centralizado
        Dim respuesta = ClienteReclamoService.RegistrarReclamo(idEquipaje, txtDescripcion.Text.Trim())

        If respuesta.success Then
            MostrarMensaje(respuesta.mensaje, True)
            txtDescripcion.Text = ""
            ddlEquipaje.SelectedIndex = 0
            CargarHistorialReclamos() ' Refrescar la tabla
        Else
            MostrarMensaje(respuesta.mensaje, False)
        End If
    End Sub

    ' =======================================================
    ' MOSTRAR HISTORIAL DE RECLAMOS DEL CLIENTE
    ' =======================================================
    Private Sub CargarHistorialReclamos()
        Dim respuesta = ClienteReclamoService.ObtenerHistorialReclamos(CorreoUsuario)

        If respuesta.success AndAlso respuesta.reclamos.Count > 0 Then
            rptReclamos.DataSource = respuesta.reclamos
            rptReclamos.DataBind()
            rptReclamos.Visible = True
            pnlVacio.Visible = False
        Else
            rptReclamos.Visible = False
            pnlVacio.Visible = True
        End If
    End Sub

    ' =======================================================
    ' LÓGICA DE COLORES PARA ESTADOS EN EL REPEATER
    ' =======================================================
    Protected Sub rptReclamos_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptReclamos.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            ' Recuperamos los datos del diccionario (minúsculas)
            Dim itemData As Dictionary(Of String, Object) = CType(e.Item.DataItem, Dictionary(Of String, Object))
            Dim estado As String = itemData("estado")?.ToString().ToUpper()

            Dim lblBadge As Label = CType(e.Item.FindControl("lblBadgeEstado"), Label)
            lblBadge.Text = estado

            If estado.Contains("PENDIENTE") OrElse estado.Contains("RECIBIDO") Then
                lblBadge.CssClass = "badge-pendiente shadow-sm"
            ElseIf estado.Contains("INVESTIGACIÓN") OrElse estado.Contains("PROCESO") Then
                lblBadge.CssClass = "badge-investigacion shadow-sm"
            ElseIf estado.Contains("RESUELTO") OrElse estado.Contains("CERRADO") OrElse estado.Contains("ENTREGADO") Then
                lblBadge.CssClass = "badge-resuelto shadow-sm"
            Else
                lblBadge.CssClass = "badge bg-secondary text-white shadow-sm"
            End If
        End If
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger fw-bold rounded-3 mb-4 shadow-sm")
    End Sub

End Class