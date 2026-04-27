Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            AplicarSeguridadYBienvenida()
            CargarDatosDashboard()
        End If
    End Sub

    ' -------------------------------------------------------------
    ' LÓGICA DE CONTROL DE ACCESO (RBAC) - Roles Separados
    ' -------------------------------------------------------------
    Private Sub AplicarSeguridadYBienvenida()
        pnlAdmin.Visible = False
        pnlCliente.Visible = False
        pnlEmpleado.Visible = False
        pnlRRHH.Visible = False
        pnlSeguridadSoporte.Visible = False

        If Session("UserEmail") IsNot Nothing Then
            Dim rolStr As String = If(Session("UserRole") IsNot Nothing, Session("UserRole").ToString(), "")
            Dim nombre As String = If(Session("UserName") IsNot Nothing, Session("UserName").ToString(), "Usuario")

            lblSaludo.Text = $"Hola, {nombre}"
            pnlBotonesAcceso.Visible = False
            pnlBotonSalir.Visible = True

            Select Case rolStr
                Case "Admin" : pnlAdmin.Visible = True
                Case "Pasajero" : pnlCliente.Visible = True
                Case "Operaciones", "Mantenimiento_Tecnico" : pnlEmpleado.Visible = True
                Case "Recursos_Humanos" : pnlRRHH.Visible = True
                Case "Seguridad", "Servicio_Al_Cliente" : pnlSeguridadSoporte.Visible = True
            End Select
        Else
            lblSaludo.Text = "Bienvenido, Invitado. Inicia sesión para más opciones."
            pnlBotonesAcceso.Visible = True
            pnlBotonSalir.Visible = False
        End If
    End Sub

    ' -------------------------------------------------------------
    ' CARGA INTEGRADA: Estadísticas + Vuelos con Rutas Reales
    ' -------------------------------------------------------------
    Private Sub CargarDatosDashboard()
        ' 🔥 Llamamos al nuevo servicio público
        Dim respuesta = DashboardService.ObtenerRadarEnVivo()

        If respuesta.success Then
            ' 1. Llenar Estadísticas
            Dim stats = respuesta.estadisticas
            lblVuelosActivos.Text = stats.activos.ToString()
            lblLlegadas.Text = stats.llegadas.ToString()
            lblSalidas.Text = stats.salidas.ToString()

            ' 2. Llenar Tabla de Vuelos
            ' (Asegúrate de que en el Default.aspx los <%# Eval() %> de esta tabla estén en minúsculas)
            rptVuelos.DataSource = respuesta.radar_vuelos
            rptVuelos.DataBind()
        Else
            lblSaludo.Text = "Error de sincronización con la torre de control: " & respuesta.mensaje
        End If
    End Sub

    ' -------------------------------------------------------------
    ' EVENTO LOGOUT
    ' -------------------------------------------------------------
    Protected Sub btnLogout_Click(sender As Object, e As EventArgs)
        Session.Clear()
        Session.Abandon()
        Response.Redirect("Default.aspx")
    End Sub

    ' -------------------------------------------------------------
    ' FUNCIONES VISUALES (Se mantienen igual para usarlas en el Repeater)
    ' -------------------------------------------------------------
    Public Function ObtenerClaseEstado(estado As String) As String
        If String.IsNullOrEmpty(estado) Then Return "badge bg-secondary"
        Select Case estado.ToUpper()
            Case "PROGRAMADO" : Return "badge bg-info text-dark"
            Case "ABORDANDO" : Return "badge bg-warning text-dark"
            Case "EN VUELO" : Return "badge bg-primary"
            Case "ATERRIZADO", "ATERRIZÓ" : Return "badge bg-success"
            Case "CANCELADO" : Return "badge bg-danger"
            Case "RETRASADO" : Return "badge bg-warning text-dark border border-danger"
            Case Else : Return "badge bg-secondary"
        End Select
    End Function

    Public Function ObtenerIconoEstado(estado As String) As String
        If String.IsNullOrEmpty(estado) Then Return "⚪"
        Select Case estado.ToUpper()
            Case "PROGRAMADO" : Return "📅"
            Case "ABORDANDO" : Return "🚶"
            Case "EN VUELO" : Return "✈️"
            Case "ATERRIZADO", "ATERRIZÓ" : Return "🛬"
            Case "CANCELADO" : Return "❌"
            Case "RETRASADO" : Return "⏳"
            Case Else : Return "⚪"
        End Select
    End Function

End Class