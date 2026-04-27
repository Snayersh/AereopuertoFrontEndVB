Public Class SiteMaster
    Inherits MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ' 1. Evitamos validar en páginas públicas si por error usan el Master
        ' (Agregué las de recuperar contraseña por seguridad, por si alguna vez las enlazas al Master)
        Dim paginaActual As String = Request.AppRelativeCurrentExecutionFilePath.ToLower()
        If paginaActual.Contains("login.aspx") OrElse
           paginaActual.Contains("registro.aspx") OrElse
           paginaActual.Contains("recuperarpassword.aspx") OrElse
           paginaActual.Contains("nuevapassword.aspx") Then
            Return
        End If

        ' 2. Verificamos si las variables de sesión web existen en memoria
        If Session("UserEmail") IsNot Nothing AndAlso Session("TokenSesion") IsNot Nothing Then
            Dim correo As String = Session("UserEmail").ToString()
            Dim token As String = Session("TokenSesion").ToString()

            ' 🔥 3. Llamamos a nuestro nuevo servicio centralizado de validación
            Dim respuesta = AccountValidacionService.ValidarTokenSesion(correo, token)

            ' Evaluamos el objeto respuesta (verificamos la propiedad .success)
            If Not respuesta.success Then
                ' ¡ALERTA! El token ya no coincide (inició sesión en otro lado o expiró en BD)
                Session.Clear()
                Session.Abandon()

                ' Redirigimos al login con un parámetro para mostrar la alerta correspondiente
                Response.Redirect("~/Account/Login.aspx?error=sesion_expirada", False)
                Context.ApplicationInstance.CompleteRequest() ' Buena práctica para evitar errores de interrupción de hilo
            End If
        Else
            ' Si no hay sesión en absoluto, lo mandamos al login
            Response.Redirect("~/Account/Login.aspx", False)
            Context.ApplicationInstance.CompleteRequest()
        End If
    End Sub

    ' ====================================================================
    ' Aquí puedes poner tu evento del botón de Cerrar Sesión (Si lo tienes)
    ' ====================================================================
    ' Protected Sub btnCerrarSesion_Click(sender As Object, e As EventArgs)
    '     Session.Clear()
    '     Session.Abandon()
    '     Response.Redirect("~/Account/Login.aspx", False)
    '     Context.ApplicationInstance.CompleteRequest()
    ' End Sub

End Class