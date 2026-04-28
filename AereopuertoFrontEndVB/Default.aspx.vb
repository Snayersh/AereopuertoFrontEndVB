Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            AplicarSeguridadYBienvenida()
            CargarDatosDashboard()

            ' 🔥 AQUÍ ESTABA EL PROBLEMA: Faltaba llamar a la función que pinta el clima
            CargarClima()
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
        Dim respuesta = DashboardService.ObtenerRadarEnVivo()

        ' Extraemos usando CallByName por si estás usando Option Strict On
        Dim exito As Boolean = Convert.ToBoolean(CallByName(respuesta, "success", CallType.Get))

        If exito Then
            ' 1. Llenar Estadísticas
            Dim stats = CallByName(respuesta, "estadisticas", CallType.Get)
            lblVuelosActivos.Text = CallByName(stats, "activos", CallType.Get).ToString()
            lblLlegadas.Text = CallByName(stats, "llegadas", CallType.Get).ToString()
            lblSalidas.Text = CallByName(stats, "salidas", CallType.Get).ToString()

            ' 2. Llenar Tabla de Vuelos
            rptVuelos.DataSource = CallByName(respuesta, "radar_vuelos", CallType.Get)
            rptVuelos.DataBind()
        Else
            Dim mensaje As String = CallByName(respuesta, "mensaje", CallType.Get).ToString()
            lblSaludo.Text = "Error de sincronización con la torre de control: " & mensaje
        End If
    End Sub

    ' -------------------------------------------------------------
    ' 🌤️ CARGA DEL CLIMA EN TIEMPO REAL (NUEVO MÉTODO)
    ' -------------------------------------------------------------
    Private Sub CargarClima()
        Try
            ' 1. Llamamos al servicio (que ya dejaste listo para la App)
            Dim resultadoClima As Object = DashboardService.ObtenerClimaEnVivo()

            ' 2. Verificamos si conectó exitosamente a OpenWeather
            Dim exito As Boolean = Convert.ToBoolean(CallByName(resultadoClima, "success", CallType.Get))

            If exito Then
                ' 3. Pintamos los datos en la pantalla
                lblClimaTemp.Text = CallByName(resultadoClima, "temperatura", CallType.Get).ToString()
                lblClimaCondicion.Text = CallByName(resultadoClima, "condicion", CallType.Get).ToString()

                Dim icono As String = CallByName(resultadoClima, "icono", CallType.Get).ToString()
                litClimaIcono.Text = "<img src='https://openweathermap.org/img/wn/" & icono & "@2x.png' width='60' />"
            Else
                ' Si falló la API Key o no hay internet, muestra el error en el widget
                Dim errorMsg As String = CallByName(resultadoClima, "mensaje", CallType.Get).ToString()
                lblClimaCondicion.Text = errorMsg
                lblClimaTemp.Text = "--"
                litClimaIcono.Text = "⚠️"
            End If
        Catch ex As Exception
            lblClimaCondicion.Text = "Falla de conexión local"
            lblClimaTemp.Text = "--"
            litClimaIcono.Text = "⚠️"
        End Try
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