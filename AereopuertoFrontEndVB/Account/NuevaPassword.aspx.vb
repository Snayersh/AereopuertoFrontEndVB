Public Class NuevaPassword
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Validamos que el enlace traiga un token
            If String.IsNullOrEmpty(Request.QueryString("token")) Then
                pnlFormulario.Visible = False
                MostrarMensaje(False, "Enlace inválido o expirado. Vuelve a solicitar la recuperación de contraseña.")
            End If
        End If
    End Sub

    Protected Sub btnActualizar_Click(sender As Object, e As EventArgs) Handles btnActualizar.Click
        Dim token As String = Request.QueryString("token")

        ' Validación visual: Las contraseñas deben ser iguales
        If txtPass1.Text <> txtPass2.Text Then
            pnlMensaje.Visible = True
            pnlMensaje.CssClass = "alert alert-warning"
            lblMensaje.Text = "Las contraseñas no coinciden. Intenta de nuevo."
            Return
        End If

        ' 🔥 Llamamos a nuestro nuevo servicio unificado
        Dim respuesta = AccountPasswordService.ActualizarPasswordConToken(token, txtPass1.Text)

        If respuesta.success Then
            pnlFormulario.Visible = False
            MostrarMensaje(True, respuesta.mensaje)
        Else
            MostrarMensaje(False, respuesta.mensaje)
        End If
    End Sub

    ' Método auxiliar para mostrar alertas de forma limpia
    Private Sub MostrarMensaje(esExito As Boolean, texto As String)
        pnlMensaje.Visible = True
        lblMensaje.Text = texto
        pnlMensaje.CssClass = If(esExito, "alert alert-success", "alert alert-danger")
    End Sub

End Class