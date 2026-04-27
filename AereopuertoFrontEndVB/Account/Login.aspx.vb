Public Class Login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            pnlError.Visible = False
            If Request.QueryString("registro") = "exitoso" Then pnlExito.Visible = True
        End If
    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        pnlExito.Visible = False
        Dim email As String = txtEmail.Text.Trim()
        Dim pass As String = txtPassword.Text.Trim()

        If String.IsNullOrEmpty(email) OrElse String.IsNullOrEmpty(pass) Then
            MostrarError("Por favor, ingrese correo y contraseña.")
            Return
        End If

        ' 🔥 LLAMADA AL SERVICIO
        Dim respuesta = AccountLoginService.IniciarSesion(email, pass)

        If respuesta.success Then
            ' Guardamos en sesión lo que el servicio ya masticó
            Session("IdRol") = respuesta.id_rol
            Session("UserName") = respuesta.nombre_completo
            Session("UserEmail") = email.ToLower()
            Session("TokenSesion") = respuesta.token_sesion
            Session("UserRole") = respuesta.rol_nombre

            Response.Redirect("~/Default.aspx")
        Else
            MostrarError(respuesta.mensaje)
        End If
    End Sub

    Private Sub MostrarError(mensaje As String)
        pnlError.Visible = True
        lblError.Text = mensaje
    End Sub
End Class