Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            AplicarSeguridadYBienvenida()
            ' 🔥 CAMBIO: Ahora un solo método carga todo (Estadísticas + Tabla con Rutas)
            CargarDatosDashboard()
        End If
    End Sub

    ' -------------------------------------------------------------
    ' LÓGICA DE CONTROL DE ACCESO (RBAC) - Roles Separados
    ' -------------------------------------------------------------
    Private Sub AplicarSeguridadYBienvenida()
        ' 1. Ocultar todos los paneles por defecto por seguridad
        pnlAdmin.Visible = False
        pnlCliente.Visible = False
        pnlEmpleado.Visible = False
        pnlRRHH.Visible = False
        pnlSeguridadSoporte.Visible = False

        If Session("UserEmail") IsNot Nothing Then

            ' 2. Leer el ROL usando "UserRole" (que es lo que manda Login.aspx)
            Dim rolStr As String = ""
            If Session("UserRole") IsNot Nothing Then
                rolStr = Session("UserRole").ToString()
            End If

            ' 3. Leer el NOMBRE usando "UserName" (que es lo que manda Login.aspx)
            Dim nombre As String = "Usuario"
            If Session("UserName") IsNot Nothing Then
                nombre = Session("UserName").ToString()
            End If

            lblSaludo.Text = $"Hola, {nombre}"
            pnlBotonesAcceso.Visible = False
            pnlBotonSalir.Visible = True

            ' 4. RBAC por Nombre de Rol (Totalmente separados)
            Select Case rolStr
                Case "Admin"
                    pnlAdmin.Visible = True

                Case "Pasajero"
                    pnlCliente.Visible = True

                Case "Operaciones"
                    pnlEmpleado.Visible = True

                Case "Recursos_Humanos"
                    pnlRRHH.Visible = True

                Case "Seguridad"
                    pnlSeguridadSoporte.Visible = True

                Case "Servicio_Al_Cliente"
                    ' A servicio al cliente le habilitamos el panel de Soporte/Tickets
                    pnlSeguridadSoporte.Visible = True

                Case "Mantenimiento_Tecnico"
                    ' Los técnicos necesitan ver el módulo de Mantenimiento de Aeronaves que está en Operaciones
                    pnlEmpleado.Visible = True

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
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                ' Usaremos el SP unificado que maneja las Rutas dinámicas
                Using cmd As New OracleCommand("SP_DATOS_DASHBOARD_PRINCIPAL", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' Parámetros de salida para las tarjetas (Stat-Cards)
                    cmd.Parameters.Add("p_vuelos_activos", OracleDbType.Int32).Direction = ParameterDirection.Output
                    cmd.Parameters.Add("p_llegadas_hoy", OracleDbType.Int32).Direction = ParameterDirection.Output
                    cmd.Parameters.Add("p_salidas_hoy", OracleDbType.Int32).Direction = ParameterDirection.Output

                    ' Cursor para la tabla principal (con el JOIN de AUR_RUTA)
                    Dim cursorParam As New OracleParameter("p_cursor_vuelos", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    ' 1. Llenar Estadísticas
                    lblVuelosActivos.Text = cmd.Parameters("p_vuelos_activos").Value.ToString()
                    lblLlegadas.Text = cmd.Parameters("p_llegadas_hoy").Value.ToString()
                    lblSalidas.Text = cmd.Parameters("p_salidas_hoy").Value.ToString()

                    ' 2. Llenar Tabla de Vuelos
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptVuelos.DataSource = dt
                        rptVuelos.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Fallo silencioso o log de error
            lblSaludo.Text = "Error de sincronización: " & ex.Message
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
    ' FUNCIONES VISUALES 
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