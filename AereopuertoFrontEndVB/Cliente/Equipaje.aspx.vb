Public Class Equipaje
    Inherits System.Web.UI.Page

    Private CorreoUsuario As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = 0
        If Session("IdRol") IsNot Nothing Then idRol = Convert.ToInt32(Session("IdRol"))

        If Session("UserEmail") Is Nothing OrElse (idRol <> 2) Then
            Response.Redirect("~/Account/Login.aspx")
            Return
        End If

        CorreoUsuario = Session("UserEmail").ToString()

        If Not IsPostBack Then
            CargarBoletos()
            CargarTiposEquipaje()
        End If
    End Sub

    Private Sub CargarTiposEquipaje()
        Dim respuesta = ClienteEquipajeService.ObtenerTiposEquipaje()

        If respuesta.success Then
            ddlTipoEquipaje.Items.Clear()
            ddlTipoEquipaje.Items.Add(New ListItem("-- Selecciona el Tipo --", ""))

            For Each item In respuesta.tipos
                ' El servicio ya nos devuelve las llaves en minúsculas
                ddlTipoEquipaje.Items.Add(New ListItem(item("nombre").ToString(), item("id_tipo_equipaje").ToString()))
            Next
        Else
            MostrarMensaje(respuesta.mensaje, False)
        End If
    End Sub

    Private Sub CargarBoletos()
        Dim respuesta = ClienteEquipajeService.ObtenerBoletosDisponibles(CorreoUsuario)

        If respuesta.success Then
            ddlBoletos.Items.Clear()
            ddlBoletos.Items.Add(New ListItem("-- Selecciona una de tus reservas --", ""))

            For Each item In respuesta.boletos
                ddlBoletos.Items.Add(New ListItem(item("descripcion_boleto").ToString(), item("codigo_boleto").ToString()))
            Next
        Else
            MostrarMensaje(respuesta.mensaje, False)
        End If
    End Sub

    Protected Sub ddlBoletos_SelectedIndexChanged(sender As Object, e As EventArgs)
        If String.IsNullOrEmpty(ddlBoletos.SelectedValue) Then
            pnlGestionEquipaje.Visible = False
            Return
        End If

        pnlGestionEquipaje.Visible = True
        pnlTracking.Visible = False
        CargarListaEquipaje(ddlBoletos.SelectedValue)
    End Sub

    Private Sub CargarListaEquipaje(codigoBoleto As String)
        Dim respuesta = ClienteEquipajeService.ObtenerEquipajeRegistrado(codigoBoleto)

        If respuesta.success AndAlso respuesta.equipaje.Count > 0 Then
            rptEquipaje.DataSource = respuesta.equipaje
            rptEquipaje.DataBind()
            rptEquipaje.Visible = True
            pnlVacio.Visible = False
        Else
            rptEquipaje.Visible = False
            pnlVacio.Visible = True
        End If
    End Sub

    Protected Sub btnRegistrarEquipaje_Click(sender As Object, e As EventArgs) Handles btnRegistrarEquipaje.Click
        Dim codigoBoleto As String = ddlBoletos.SelectedValue
        Dim tipoEquipaje As String = ddlTipoEquipaje.SelectedValue
        Dim peso As String = txtPeso.Text.Trim()
        Dim descripcion As String = txtDescripcion.Text.Trim()

        If String.IsNullOrEmpty(codigoBoleto) OrElse String.IsNullOrEmpty(tipoEquipaje) OrElse String.IsNullOrEmpty(peso) Then
            MostrarMensaje("Por favor completa todos los campos obligatorios.", False)
            Return
        End If

        Dim respuesta = ClienteEquipajeService.RegistrarNuevaMaleta(
            codigoBoleto,
            Convert.ToDecimal(peso),
            descripcion,
            Convert.ToInt32(tipoEquipaje)
        )

        If respuesta.success Then
            MostrarMensaje(respuesta.mensaje, True)
            txtPeso.Text = ""
            txtDescripcion.Text = ""
            ddlTipoEquipaje.SelectedIndex = 0
            CargarListaEquipaje(codigoBoleto)
            pnlTracking.Visible = False
        Else
            MostrarMensaje(respuesta.mensaje, False)
        End If
    End Sub

    Protected Sub rptEquipaje_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptEquipaje.ItemCommand
        If e.CommandName = "Rastrear" Then
            Dim idEquipaje As String = e.CommandArgument.ToString()
            CargarHistorialTracking(Convert.ToInt32(idEquipaje))
            pnlTracking.Visible = True
        End If
    End Sub

    Private Sub CargarHistorialTracking(idEquipaje As Integer)
        Dim respuesta = ClienteEquipajeService.RastrearEquipaje(idEquipaje)

        If respuesta.success AndAlso respuesta.tracking.Count > 0 Then
            rptTrackingLine.DataSource = respuesta.tracking
            rptTrackingLine.DataBind()
            rptTrackingLine.Visible = True
            pnlTrackingVacio.Visible = False
        Else
            rptTrackingLine.Visible = False
            pnlTrackingVacio.Visible = True
        End If
    End Sub

    Protected Sub btnCerrarTracking_Click(sender As Object, e As EventArgs)
        pnlTracking.Visible = False
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center rounded-3 mb-4 fw-bold", "alert alert-danger text-center rounded-3 mb-4 fw-bold")
    End Sub

End Class