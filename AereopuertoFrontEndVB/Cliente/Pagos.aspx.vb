Public Class Pagos
    Inherits System.Web.UI.Page

    Private CorreoUsuario As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Control de sesión seguro
        Dim idRol As Integer = 0
        If Session("IdRol") IsNot Nothing Then idRol = Convert.ToInt32(Session("IdRol"))

        If Session("UserEmail") Is Nothing OrElse (idRol <> 2) Then
            Response.Redirect("~/Account/Login.aspx")
            Return ' Obligatorio para detener el ciclo de vida de la página
        End If

        CorreoUsuario = Session("UserEmail").ToString()

        If Not IsPostBack Then
            pnlError.Visible = False
            pnlExito.Visible = False

            ' Si viene de hacer la reserva, pre-llenamos la caja de texto
            If Request.QueryString("codigo") IsNot Nothing Then
                txtCodigoReserva.Text = Request.QueryString("codigo").ToString()
            End If
        End If
    End Sub

    Protected Sub btnPagar_Click(sender As Object, e As EventArgs) Handles btnPagar.Click
        Dim codigoReserva As String = txtCodigoReserva.Text.Trim().ToUpper()

        If String.IsNullOrEmpty(codigoReserva) Then
            MostrarError("Por favor, ingresa el código de tu reserva.")
            Return
        End If

        ' 🔥 Llamamos a nuestro nuevo servicio centralizado. 
        ' Enviamos 1 fijo porque asumimos que en la web el pago es con tarjeta.
        Dim respuesta = ClientePagoService.ProcesarPago(codigoReserva, CorreoUsuario, 1)

        If respuesta.success Then
            ' Ocultamos el formulario y mostramos el pase de abordar/factura
            pnlFormulario.Visible = False
            pnlError.Visible = False

            lblFactura.Text = respuesta.factura
            lblLocalizadorExito.Text = codigoReserva
            pnlExito.Visible = True
        Else
            ' Si hubo problemas con la tarjeta o el código, lo mostramos
            MostrarError(respuesta.mensaje)
        End If
    End Sub

    Private Sub MostrarError(mensaje As String)
        pnlExito.Visible = False
        pnlError.Visible = True
        lblError.Text = mensaje
    End Sub

End Class