Public Class PaseAbordar
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = 0
        If Session("IdRol") IsNot Nothing Then idRol = Convert.ToInt32(Session("IdRol"))

        ' 🔥 SEGURIDAD: Validamos sesión y rol de cliente
        If Session("UserEmail") Is Nothing OrElse (idRol <> 2) Then
            Response.Redirect("~/Account/Login.aspx")
            Return
        End If

        If Not IsPostBack Then
            Dim codigoReserva As String = Request.QueryString("codigo")

            If Not String.IsNullOrEmpty(codigoReserva) Then
                CargarDatosPase(codigoReserva)
            Else
                MostrarError("No se proporcionó un código de reserva válido en la URL.")
            End If
        End If
    End Sub

    Private Sub CargarDatosPase(codigoReserva As String)
        Dim correoUsuario As String = Session("UserEmail").ToString()

        ' 🔥 Llamada a nuestro nuevo servicio
        Dim respuesta = ClientePaseAbordarService.ObtenerPasesDeAbordar(codigoReserva, correoUsuario)

        If respuesta.success Then
            ' Si todo sale bien, atamos los datos al Repeater
            rptPases.DataSource = respuesta.pases
            rptPases.DataBind()
            rptPases.Visible = True
            pnlError.Visible = False
        Else
            ' Si falla (no existe, no es suyo, etc.), mostramos el error
            MostrarError(respuesta.mensaje)
        End If
    End Sub

    Private Sub MostrarError(mensaje As String)
        rptPases.Visible = False
        pnlError.Visible = True
        lblError.Text = mensaje
    End Sub

End Class