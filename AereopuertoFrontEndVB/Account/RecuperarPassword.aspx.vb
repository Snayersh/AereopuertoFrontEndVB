Public Class RecuperarPassword
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        pnlMensaje.Visible = False
    End Sub

    Protected Sub btnEnviar_Click(sender As Object, e As EventArgs) Handles btnEnviar.Click
        Dim correo As String = txtEmail.Text.Trim()

        If String.IsNullOrEmpty(correo) Then
            MostrarMensaje("Por favor, ingresa tu correo electrónico.", False)
            Return
        End If

        ' 🔥 Llamamos al servicio centralizado
        Dim respuesta = AccountPasswordService.SolicitarRecuperacion(correo)

        If respuesta.success Then
            MostrarMensaje(respuesta.mensaje, True)
            txtEmail.Text = "" ' Limpiamos por seguridad
        Else
            MostrarMensaje(respuesta.mensaje, False)
        End If
    End Sub

    Private Sub MostrarMensaje(mensaje As String, exito As Boolean)
        pnlMensaje.Visible = True
        pnlMensaje.CssClass = If(exito, "alert alert-success text-center rounded-3 mb-4", "alert alert-danger")
        lblMensaje.Text = mensaje
    End Sub
End Class