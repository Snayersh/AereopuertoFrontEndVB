Public Class DetalleFactura
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD MEJORADA: Verificamos el rol sin riesgos de que explote por valores nulos
        Dim idRol As Integer = 0
        If Session("IdRol") IsNot Nothing Then idRol = Convert.ToInt32(Session("IdRol"))

        If Session("UserEmail") Is Nothing OrElse (idRol <> 2) Then
            Response.Redirect("~/Account/Login.aspx")
            Return ' Obligatorio para detener la carga de la página
        End If

        If Not IsPostBack Then
            Dim idFacturaStr As String = Request.QueryString("id")
            If Not String.IsNullOrEmpty(idFacturaStr) Then
                CargarFactura(Convert.ToInt32(idFacturaStr))
            Else
                MostrarError("No se especificó el número de factura.")
            End If
        End If
    End Sub

    Private Sub CargarFactura(idFactura As Integer)
        ' 🔥 Llamamos a nuestro nuevo servicio consolidado
        Dim respuesta = ClienteFacturaService.ObtenerDetalleFactura(idFactura)

        If respuesta.success Then
            ' 1. Llenamos los Labels de la Cabecera
            Dim cabecera = respuesta.cabecera
            lblNumeroFactura.Text = cabecera("numero_factura").ToString()
            lblFecha.Text = cabecera("fecha_emision").ToString()
            lblCliente.Text = cabecera("cliente").ToString()
            lblCorreo.Text = cabecera("correo").ToString()
            lblTotal.Text = Convert.ToDecimal(cabecera("total")).ToString("N2")

            ' 2. Llenamos la Tabla (Repeater) con los Detalles
            rptDetalles.DataSource = respuesta.detalles
            rptDetalles.DataBind()
        Else
            MostrarError(respuesta.mensaje)
        End If
    End Sub

    Private Sub MostrarError(mensaje As String)
        pnlFactura.Visible = False
        pnlError.Visible = True
        lblError.Text = mensaje
    End Sub

End Class