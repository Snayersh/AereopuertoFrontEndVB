Public Class Registro
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            pnlMensaje.Visible = False
        End If
    End Sub

    Protected Sub btnRegistrar_Click(sender As Object, e As EventArgs) Handles btnRegistrar.Click

        Dim fechaNac As DateTime
        If Not DateTime.TryParse(txtFechaNac.Text, fechaNac) Then
            MostrarError("Por favor, ingresa una fecha de nacimiento válida.")
            Return
        End If

        ' Llamamos al nuevo servicio
        Dim resultado = AccountRegistroService.RegistrarNuevoCliente(
            txtPrimerNombre.Text.Trim(), txtSegundoNombre.Text.Trim(), txtTercerNombre.Text.Trim(),
            txtPrimerApellido.Text.Trim(), txtSegundoApellido.Text.Trim(), txtApellidoCasada.Text.Trim(),
            fechaNac, txtTelefono.Text.Trim(), txtEmail.Text.Trim(),
            txtPais.Text.Trim(), txtDepartamento.Text.Trim(), txtMunicipio.Text.Trim(), txtZona.Text.Trim(),
            txtColonia.Text.Trim(), txtCalleAvenida.Text.Trim(), "", txtNumCasa.Text.Trim(),
            txtPasaporte.Text.Trim(), txtPassword.Text.Trim()
        )

        If resultado.success Then
            LimpiarCampos()
            Response.Redirect("Login.aspx?registro=exitoso", False)
        Else
            MostrarError(resultado.mensaje)
        End If
    End Sub

    Private Sub LimpiarCampos()
        ' ... tu código original para limpiar los textbox ...
    End Sub

    Private Sub MostrarError(mensaje As String)
        pnlMensaje.Visible = True
        pnlMensaje.CssClass = "alert alert-danger text-center rounded-3 mb-4"
        lblMensaje.Text = mensaje
    End Sub
End Class