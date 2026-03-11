Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ProgramasVuelo
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Seguridad: Solo Administradores
        If Session("UserRole") Is Nothing OrElse Session("UserRole").ToString() <> "Admin" Then
            Response.Redirect("~/Default.aspx")
        End If

        If Not IsPostBack Then
            pnlMensaje.Visible = False
            CargarTodosLosCatalogos()
        End If
    End Sub

    ' -------------------------------------------------------------
    ' 1. CARGA DE CATÁLOGOS AL INICIAR LA PÁGINA
    ' -------------------------------------------------------------
    Private Sub CargarTodosLosCatalogos()
        ' Usamos la función auxiliar para llenar cada DropDownList con 1 sola línea de código
        LlenarCombo("SELECT id_aerolinea, nombre FROM AUR_AEROLINEA ORDER BY nombre", ddlAerolinea, "NOMBRE", "ID_AEROLINEA", "Aerolínea")
        LlenarCombo("SELECT id_aeronave, modelo || ' (Cap: ' || capacidad || ')' AS DETALLE FROM AUR_AERONAVE ORDER BY modelo", ddlAeronave, "DETALLE", "ID_AERONAVE", "Aeronave")
        LlenarCombo("SELECT id_aeropuerto, nombre || ' (' || codigo_iata || ')' AS DETALLE FROM AUR_AEROPUERTO ORDER BY nombre", ddlOrigen, "DETALLE", "ID_AEROPUERTO", "Origen")
        LlenarCombo("SELECT id_aeropuerto, nombre || ' (' || codigo_iata || ')' AS DETALLE FROM AUR_AEROPUERTO ORDER BY nombre", ddlDestino, "DETALLE", "ID_AEROPUERTO", "Destino")
        LlenarCombo("SELECT id_estado_vuelo, nombre FROM AUR_ESTADO_VUELO ORDER BY id_estado_vuelo", ddlEstado, "NOMBRE", "ID_ESTADO_VUELO", "Estado")
    End Sub

    ' Función auxiliar para no repetir código de conexión al llenar combos
    Private Sub LlenarCombo(query As String, ddl As DropDownList, textField As String, valueField As String, nombreCatalogo As String)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand(query, conn)
                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddl.DataSource = reader
                        ddl.DataTextField = textField
                        ddl.DataValueField = valueField
                        ddl.DataBind()
                    End Using
                End Using
            End Using
            ddl.Items.Insert(0, New ListItem($"-- Seleccione {nombreCatalogo} --", ""))
        Catch ex As Exception
            MostrarMensaje($"Error cargando {nombreCatalogo}: {ex.Message}", False)
        End Try
    End Sub

    ' -------------------------------------------------------------
    ' 2. GUARDAR EL VUELO (CON VALIDACIONES FUERTES)
    ' -------------------------------------------------------------
    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        ' 1. Validación de Ruta (No viajar al mismo lugar)
        If ddlOrigen.SelectedValue = ddlDestino.SelectedValue Then
            MostrarMensaje("El aeropuerto de Origen y Destino no pueden ser el mismo.", False)
            Return
        End If

        ' 2. Validación de Fechas (El corazón de la seguridad temporal)
        Dim fechaSalida As DateTime
        Dim fechaLlegada As DateTime

        ' Intentamos convertir el texto a fecha de forma segura
        If Not DateTime.TryParse(txtSalida.Text, fechaSalida) OrElse Not DateTime.TryParse(txtLlegada.Text, fechaLlegada) Then
            MostrarMensaje("El formato de las fechas no es válido.", False)
            Return
        End If

        ' Regla A: La salida no puede ser en el pasado (con un margen de 5 minutos por si acaso)
        If fechaSalida < DateTime.Now.AddMinutes(-5) Then
            MostrarMensaje("La fecha de salida no puede ser en el pasado.", False)
            Return
        End If

        ' Regla B: La llegada DEBE ser estrictamente mayor a la salida
        If fechaLlegada <= fechaSalida Then
            MostrarMensaje("Incoherencia temporal: La fecha de llegada debe ser posterior a la fecha de salida.", False)
            Return
        End If

        ' Regla C: Un vuelo comercial normal no dura más de 24 horas continuas (ej. Singapur-NY dura ~19h)
        Dim duracionVuelo As TimeSpan = fechaLlegada - fechaSalida
        If duracionVuelo.TotalHours > 24 Then
            MostrarMensaje("Alerta de ruta: La duración programada excede el límite máximo operativo (24 horas). Revise las fechas.", False)
            Return
        End If

        ' Si pasó todas las pruebas, procedemos a guardar
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_INSERTAR_VUELO", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.Add("p_codigo_vuelo", OracleDbType.Varchar2).Value = txtCodigoVuelo.Text.Trim()
                    cmd.Parameters.Add("p_fecha_salida", OracleDbType.TimeStamp).Value = fechaSalida
                    cmd.Parameters.Add("p_fecha_llegada", OracleDbType.TimeStamp).Value = fechaLlegada

                    cmd.Parameters.Add("p_id_aerolinea", OracleDbType.Int32).Value = Convert.ToInt32(ddlAerolinea.SelectedValue)
                    cmd.Parameters.Add("p_id_aeronave", OracleDbType.Int32).Value = Convert.ToInt32(ddlAeronave.SelectedValue)
                    cmd.Parameters.Add("p_id_origen", OracleDbType.Int32).Value = Convert.ToInt32(ddlOrigen.SelectedValue)
                    cmd.Parameters.Add("p_id_destino", OracleDbType.Int32).Value = Convert.ToInt32(ddlDestino.SelectedValue)
                    cmd.Parameters.Add("p_id_estado_vuelo", OracleDbType.Int32).Value = Convert.ToInt32(ddlEstado.SelectedValue)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        MostrarMensaje($"¡Vuelo {txtCodigoVuelo.Text.ToUpper()} programado exitosamente!", True)
                        LimpiarCampos()
                    Else
                        MostrarMensaje("Error en base de datos: " & resultado, False)
                    End If
                End Using
            End Using

        Catch ex As Exception
            MostrarMensaje("Error de procesamiento: " & ex.Message, False)
        End Try
    End Sub
    ' -------------------------------------------------------------
    ' 3. FUNCIONES DE APOYO VISUAL
    ' -------------------------------------------------------------
    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        LimpiarCampos()
        pnlMensaje.Visible = False
    End Sub

    Private Sub LimpiarCampos()
        txtCodigoVuelo.Text = ""
        txtSalida.Text = ""
        txtLlegada.Text = ""
        ddlAerolinea.SelectedIndex = 0
        ddlAeronave.SelectedIndex = 0
        ddlOrigen.SelectedIndex = 0
        ddlDestino.SelectedIndex = 0
        ddlEstado.SelectedIndex = 0
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        If esExito Then
            pnlMensaje.CssClass = "alert alert-success text-center rounded-3 mb-4"
        Else
            pnlMensaje.CssClass = "alert alert-danger text-center rounded-3 mb-4"
        End If
    End Sub



End Class