Public Class MiPerfil
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Si no hay sesión, lo mandamos al login
        If Session("UserEmail") Is Nothing Then
            Response.Redirect("~/Account/Login.aspx")
            Return ' Añadimos Return para detener la ejecución por seguridad
        End If

        If Not IsPostBack Then
            CargarDatosPerfil()
        End If
    End Sub

    Private Sub CargarDatosPerfil()
        Dim correoUsuario As String = Session("UserEmail").ToString()

        ' 🔥 Llamamos a nuestro nuevo servicio
        Dim respuesta = ClientePerfilService.ObtenerPerfil(correoUsuario)

        If respuesta.success Then
            Dim perfilData = respuesta.perfil
            ' El diccionario convierte las columnas a minúsculas
            txtPrimerNombre.Text = perfilData("primer_nombre")?.ToString()
            txtSegundoNombre.Text = perfilData("segundo_nombre")?.ToString()
            txtPrimerApellido.Text = perfilData("primer_apellido")?.ToString()
            txtSegundoApellido.Text = perfilData("segundo_apellido")?.ToString()
            txtTelefono.Text = perfilData("telefono")?.ToString()
        Else
            MostrarMensaje(respuesta.mensaje, False)
        End If
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Dim correoUsuario As String = Session("UserEmail").ToString()

        ' 🔥 Llamamos al servicio para guardar
        Dim respuesta = ClientePerfilService.ActualizarPerfil(
            correoUsuario,
            txtPrimerNombre.Text.Trim(),
            txtSegundoNombre.Text.Trim(),
            txtPrimerApellido.Text.Trim(),
            txtSegundoApellido.Text.Trim(),
            txtTelefono.Text.Trim()
        )

        If respuesta.success Then
            MostrarMensaje(respuesta.mensaje, True)
            ' TRUCO DE ORO: Actualizamos la variable de sesión para que el saludo cambie en toda la web
            Session("UserName") = txtPrimerNombre.Text.Trim()
        Else
            MostrarMensaje(respuesta.mensaje, False)
        End If
    End Sub

    Private Sub MostrarMensaje(mensaje As String, exito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(exito, "alert alert-success text-center fw-bold", "alert alert-danger text-center fw-bold")
    End Sub
End Class