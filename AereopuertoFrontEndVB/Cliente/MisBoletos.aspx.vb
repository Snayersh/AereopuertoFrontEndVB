Public Class MisBoletos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = 0
        If Session("IdRol") IsNot Nothing Then idRol = Convert.ToInt32(Session("IdRol"))

        ' 🔥 SEGURIDAD: Solo clientes
        If Session("UserEmail") Is Nothing OrElse (idRol <> 2) Then
            Response.Redirect("~/Account/Login.aspx")
            Return
        End If

        If Not IsPostBack Then
            ' La primera vez carga con el valor por defecto del DropDown
            CargarDataBoletos(Convert.ToInt32(ddlFiltroEstado.SelectedValue))
        End If
    End Sub

    ' Evento del DropDown: Al cambiar el filtro, recarga la lista
    Protected Sub ddlFiltroEstado_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlFiltroEstado.SelectedIndexChanged
        CargarDataBoletos(Convert.ToInt32(ddlFiltroEstado.SelectedValue))
    End Sub

    ' =================================================================
    ' FUNCIÓN UNIFICADA PARA CARGAR BOLETOS (CON FILTRO)
    ' =================================================================
    Private Sub CargarDataBoletos(idEstadoFiltro As Integer)
        Dim correoUsuario As String = Session("UserEmail").ToString()
        pnlError.Visible = False

        ' 🔥 Llamamos al nuevo servicio
        Dim respuesta = ClienteBoletoService.ObtenerMisBoletos(correoUsuario, idEstadoFiltro)

        If respuesta.success Then
            Dim listaBoletos = CType(respuesta.boletos, List(Of Dictionary(Of String, Object)))

            If listaBoletos.Count > 0 Then
                rptBoletos.DataSource = listaBoletos
                rptBoletos.DataBind()
                rptBoletos.Visible = True
                pnlVacio.Visible = False
            Else
                rptBoletos.Visible = False
                pnlVacio.Visible = True
            End If
        Else
            pnlError.Visible = True
            lblError.Text = respuesta.mensaje
            rptBoletos.Visible = False
            pnlVacio.Visible = False
        End If
    End Sub

    ' =================================================================
    ' MANEJO DE BOTONES (CANCELAR)
    ' =================================================================
    Protected Sub rptBoletos_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptBoletos.ItemCommand
        If e.CommandName = "CancelarReserva" Then
            Dim codigoReserva As String = e.CommandArgument.ToString()
            ProcesarCancelacion(codigoReserva)
        End If
    End Sub

    Private Sub ProcesarCancelacion(codigoReserva As String)
        Dim correoUsuario As String = Session("UserEmail").ToString()

        ' 🔥 Llamamos al servicio para cancelar
        Dim respuesta = ClienteBoletoService.CancelarReserva(codigoReserva, correoUsuario)

        If respuesta.success Then
            ' REFRESCAMOS la lista usando el mismo filtro que está seleccionado
            CargarDataBoletos(Convert.ToInt32(ddlFiltroEstado.SelectedValue))
        Else
            pnlError.Visible = True
            lblError.Text = respuesta.mensaje
        End If
    End Sub

End Class