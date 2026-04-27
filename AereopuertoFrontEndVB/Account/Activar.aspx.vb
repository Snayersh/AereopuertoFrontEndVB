Public Class Activar
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim token As String = Request.QueryString("token")

            If String.IsNullOrEmpty(token) Then
                MostrarPantalla(False, "No se proporcionó ningún código de activación.")
            Else
                ' 🔥 Llamamos al servicio centralizado
                Dim respuesta = AccountActivationService.ActivarCuentaUsuario(token)

                If respuesta.success Then
                    MostrarPantalla(True, "")
                Else
                    MostrarPantalla(False, respuesta.mensaje)
                End If
            End If
        End If
    End Sub

    Private Sub MostrarPantalla(esExito As Boolean, mensajeError As String)
        pnlExito.Visible = esExito
        pnlError.Visible = Not esExito
        If Not esExito Then
            lblMensajeError.Text = mensajeError
        End If
    End Sub
End Class