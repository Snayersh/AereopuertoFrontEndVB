Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ProgramasVuelo
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("UserRole") Is Nothing OrElse Session("UserRole").ToString() <> "Admin" Then
            Response.Redirect("~/Default.aspx")
        End If

        If Not IsPostBack Then
            pnlMensaje.Visible = False
            CargarCatalogos()
        End If
    End Sub

    ' -------------------------------------------------------------
    ' 1. CARGA DE CATÁLOGOS AL INICIAR LA PÁGINA
    ' -------------------------------------------------------------
    Private Sub CargarCatalogos()
        LlenarCombo("SELECT id_aerolinea, nombre FROM AUR_AEROLINEA ORDER BY nombre", ddlAerolinea, "NOMBRE", "ID_AEROLINEA", "Aerolínea")
        CargarDestinosConTiempo()
    End Sub

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

    ' ESTA ES LA FUNCIÓN CLAVE PARA INYECTAR LOS MINUTOS AL HTML
    Private Sub CargarDestinosConTiempo()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                ' Traemos el aeropuerto y sus minutos desde la nueva tabla
                Dim query As String = "SELECT a.id_aeropuerto, a.nombre || ' (' || a.codigo_iata || ')' AS DETALLE, NVL(r.duracion_minutos, 120) AS MINUTOS " &
                                      "FROM AUR_AEROPUERTO a " &
                                      "LEFT JOIN AUR_RUTA_TIEMPO r ON a.id_aeropuerto = r.id_destino " &
                                      "WHERE a.codigo_iata != 'GUA' ORDER BY a.nombre"
                Using cmd As New OracleCommand(query, conn)
                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlDestino.Items.Clear()
                        ddlDestino.Items.Add(New ListItem("-- Seleccione Destino --", ""))

                        While reader.Read()
                            Dim item As New ListItem(reader("DETALLE").ToString(), reader("ID_AEROPUERTO").ToString())
                            ' Inyectamos los minutos en un atributo oculto para que JavaScript los lea
                            item.Attributes.Add("data-minutos", reader("MINUTOS").ToString())
                            ddlDestino.Items.Add(item)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar destinos: " & ex.Message, False)
        End Try
    End Sub

    ' Cuando seleccionan aerolínea, cargamos solo SUS aviones
    Protected Sub ddlAerolinea_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlAerolinea.SelectedIndexChanged
        ddlAeronave.Items.Clear()
        If String.IsNullOrEmpty(ddlAerolinea.SelectedValue) Then
            ddlAeronave.Items.Insert(0, New ListItem("-- Primero seleccione aerolínea --", ""))
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Dim query As String = "SELECT id_aeronave, modelo || ' (Cap: ' || capacidad || ')' AS DETALLE FROM AUR_AERONAVE WHERE id_aerolinea = :id ORDER BY modelo"
                Using cmd As New OracleCommand(query, conn)
                    cmd.Parameters.Add(New OracleParameter("id", Convert.ToInt32(ddlAerolinea.SelectedValue)))
                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlAeronave.DataSource = reader
                        ddlAeronave.DataTextField = "DETALLE"
                        ddlAeronave.DataValueField = "ID_AERONAVE"
                        ddlAeronave.DataBind()
                    End Using
                End Using
            End Using
            ddlAeronave.Items.Insert(0, New ListItem("-- Seleccione Aeronave --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar aeronaves: " & ex.Message, False)
        End Try
    End Sub

    ' -------------------------------------------------------------
    ' 2. GUARDAR EL VUELO (A PRUEBA DE BALAS)
    ' -------------------------------------------------------------
    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If String.IsNullOrEmpty(ddlAerolinea.SelectedValue) OrElse String.IsNullOrEmpty(ddlAeronave.SelectedValue) OrElse String.IsNullOrEmpty(ddlDestino.SelectedValue) Then
            MostrarMensaje("Por favor, complete todos los campos obligatorios.", False)
            Return
        End If

        Dim fechaSalida As DateTime
        If Not DateTime.TryParse(txtSalida.Text, fechaSalida) Then
            MostrarMensaje("La fecha de salida no es válida.", False)
            Return
        End If

        If fechaSalida < DateTime.Now.AddMinutes(-5) Then
            MostrarMensaje("La fecha de salida no puede ser en el pasado.", False)
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CREAR_VUELO_AUTO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo_vuelo", OracleDbType.Varchar2).Value = txtCodigoVuelo.Text.Trim().ToUpper()
                    cmd.Parameters.Add("p_fecha_salida", OracleDbType.TimeStamp).Value = fechaSalida
                    cmd.Parameters.Add("p_id_aerolinea", OracleDbType.Int32).Value = Convert.ToInt32(ddlAerolinea.SelectedValue)
                    cmd.Parameters.Add("p_id_aeronave", OracleDbType.Int32).Value = Convert.ToInt32(ddlAeronave.SelectedValue)
                    cmd.Parameters.Add("p_id_destino", OracleDbType.Int32).Value = Convert.ToInt32(ddlDestino.SelectedValue)

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
                        MostrarMensaje(resultado, False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error de base de datos: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        LimpiarCampos()
        pnlMensaje.Visible = False
    End Sub

    Private Sub LimpiarCampos()
        txtCodigoVuelo.Text = ""
        txtSalida.Text = ""
        txtLlegada.Text = ""
        ddlAerolinea.SelectedIndex = 0
        ddlAeronave.Items.Clear()
        ddlDestino.SelectedIndex = 0
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center rounded-3 mb-4", "alert alert-danger text-center rounded-3 mb-4")
    End Sub
End Class