Public Class SiteMaster
    Inherits MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ' 1. Evitamos validar en páginas públicas si por error usan el Master
        Dim paginaActual As String = Request.AppRelativeCurrentExecutionFilePath.ToLower()
        If paginaActual.Contains("login.aspx") OrElse paginaActual.Contains("registro.aspx") Then
            Return
        End If

        ' 2. Verificamos si las variables de sesión existen
        If Session("UserEmail") IsNot Nothing AndAlso Session("TokenSesion") IsNot Nothing Then
            Dim correo As String = Session("UserEmail").ToString()
            Dim token As String = Session("TokenSesion").ToString()

            ' 3. Consultamos a Oracle si este token sigue siendo el válido
            Dim validacion As String = AuthService.ValidarToken(correo, token)

            If validacion <> "EXITO" Then
                ' ¡ALERTA! El token ya no coincide (inició sesión en otro lado)
                Session.Clear()
                Session.Abandon()

                ' Redirigimos al login con un parámetro para mostrar una alerta si quieres
                Response.Redirect("~/Login.aspx?error=sesion_expirada")
            End If
        Else
            ' Si no hay sesión, lo mandamos al login
            Response.Redirect("~/Login.aspx")
        End If
    End Sub

    ' ... (Cualquier otro código que tengas en tu MasterPage, como botones de cerrar sesión) ...
End Class