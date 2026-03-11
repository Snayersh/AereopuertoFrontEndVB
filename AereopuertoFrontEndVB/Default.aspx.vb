Imports Oracle.ManagedDataAccess.Client

Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            AplicarSeguridadYBienvenida()
            CargarVuelos()
            CargarEstadisticas() ' ¡Agregamos esta línea aquí!
        End If
    End Sub

    ' -------------------------------------------------------------
    ' CARGA DE TARJETAS ESTADÍSTICAS
    ' -------------------------------------------------------------
    Private Sub CargarEstadisticas()
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ESTADISTICAS_DASHBOARD", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' Preparamos los parámetros de salida (OUT)
                    Dim outActivos As New OracleParameter("p_activos", OracleDbType.Int32)
                    outActivos.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outActivos)

                    Dim outLlegadas As New OracleParameter("p_llegadas", OracleDbType.Int32)
                    outLlegadas.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outLlegadas)

                    Dim outSalidas As New OracleParameter("p_salidas", OracleDbType.Int32)
                    outSalidas.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outSalidas)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    ' Llenamos las etiquetas de la pantalla con los resultados de Oracle
                    lblVuelosActivos.Text = outActivos.Value.ToString()
                    lblLlegadas.Text = outLlegadas.Value.ToString()
                    lblSalidas.Text = outSalidas.Value.ToString()
                End Using
            End Using
        Catch ex As Exception
            lblVuelosActivos.Text = "0"
            lblLlegadas.Text = "0"
            lblSalidas.Text = "0"
        End Try
    End Sub

    ' -------------------------------------------------------------
    ' LÓGICA DE CONTROL DE ACCESO BASADO EN ROLES (RBAC)
    ' -------------------------------------------------------------
    Private Sub AplicarSeguridadYBienvenida()
        ' Verificamos si alguien inició sesión
        If Session("UserRole") IsNot Nothing Then
            Dim rol As String = Session("UserRole").ToString()
            Dim nombre As String = Session("UserName").ToString()

            ' Cambiamos el saludo
            lblSaludo.Text = $"Hola, {nombre} | Perfil: {rol}"

            ' Cambiamos los botones de la esquina superior derecha
            pnlBotonesAcceso.Visible = False
            pnlBotonSalir.Visible = True

            ' Habilitamos el menú lateral según el rol
            Select Case rol
                Case "Admin"
                    pnlAdmin.Visible = True
                    pnlMiCuenta.Visible = True
                Case "Empleado"
                    pnlAdmin.Visible = False
                    pnlMiCuenta.Visible = True
                Case "Cliente"
                    pnlAdmin.Visible = False
                    pnlMiCuenta.Visible = True
            End Select
        Else
            ' Si entra un Invitado (sin sesión)
            lblSaludo.Text = "Bienvenido, Invitado. Inicia sesión para más opciones."
            pnlBotonesAcceso.Visible = True
            pnlBotonSalir.Visible = False
            pnlAdmin.Visible = False
            pnlMiCuenta.Visible = False
        End If
    End Sub

    ' -------------------------------------------------------------
    ' EVENTO PARA CERRAR SESIÓN
    ' -------------------------------------------------------------
    Protected Sub btnLogout_Click(sender As Object, e As EventArgs)
        Session.Clear()
        Session.Abandon()
        Response.Redirect("Default.aspx")
    End Sub

    ' -------------------------------------------------------------
    ' CARGA DE DATOS REALES DESDE ORACLE
    ' -------------------------------------------------------------
    Private Sub CargarVuelos()
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_VUELOS_DASHBOARD", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' Le decimos a Oracle que esperamos un Cursor lleno de datos
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()

                    ' Usamos un DataAdapter para convertir los datos de Oracle a una Tabla de VB.NET
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        ' Enlazamos la tabla real a nuestro diseño HTML
                        rptVuelos.DataSource = dt
                        rptVuelos.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Si hay un error, lo mostramos discretamente en el saludo
            lblSaludo.Text = "Error al cargar los vuelos: " & ex.Message
        End Try
    End Sub


    ' -------------------------------------------------------------
    ' FUNCIONES VISUALES PARA LA TABLA
    ' -------------------------------------------------------------
    Public Function ObtenerClaseEstado(estado As String) As String
        If estado = "Aterrizó" Then Return "text-primary fw-bold"
        If estado = "En Horario" Then Return "text-success fw-bold"
        Return "text-warning fw-bold"
    End Function

    Public Function ObtenerIconoEstado(estado As String) As String
        If estado = "Aterrizó" Then Return "🔵"
        If estado = "En Horario" Then Return "🟢"
        Return "🟡"
    End Function



End Class