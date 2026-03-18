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
            CargarAerolineas()
            CargarDestinosConMinutos()
        End If
    End Sub

    ' -------------------------------------------------------------
    ' 1. CARGAR CATÁLOGOS
    ' -------------------------------------------------------------
    Private Sub CargarAerolineas()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SELECT id_aerolinea, nombre FROM AUR_AEROLINEA ORDER BY nombre", conn)
                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlAerolinea.DataSource = reader
                        ddlAerolinea.DataTextField = "nombre"
                        ddlAerolinea.DataValueField = "id_aerolinea"
                        ddlAerolinea.DataBind()
                    End Using
                End Using
            End Using
            ddlAerolinea.Items.Insert(0, New ListItem("-- Seleccione Aerolínea --", ""))
        Catch ex As Exception
            MostrarMensaje("Error cargando Aerolíneas: " & ex.Message, False)
        End Try
    End Sub

    ' Carga los destinos e inyecta los minutos para que JavaScript los lea
    Private Sub CargarDestinosConMinutos()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_DESTINOS_RUTA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlDestino.Items.Clear()
                        ddlDestino.Items.Add(New ListItem("-- Seleccione Destino --", ""))

                        While reader.Read()
                            Dim item As New ListItem(reader("detalle").ToString(), reader("id_aeropuerto").ToString())
                            item.Attributes.Add("data-minutos", reader("minutos").ToString())
                            ddlDestino.Items.Add(item)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar destinos: " & ex.Message, False)
        End Try
    End Sub

    ' Filtro de Aviones (Depende de la Aerolínea)
    Protected Sub ddlAerolinea_SelectedIndexChanged(sender As Object, e As EventArgs)
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
    ' 2. GUARDAR EL VUELO (USANDO EL PROCEDIMIENTO AUTOMÁTICO)
    ' -------------------------------------------------------------
    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Dim fechaSalida As DateTime

        If String.IsNullOrEmpty(ddlAerolinea.SelectedValue) OrElse String.IsNullOrEmpty(ddlAeronave.SelectedValue) OrElse String.IsNullOrEmpty(ddlDestino.SelectedValue) Then
            MostrarMensaje("Complete todos los campos obligatorios.", False)
            Return
        End If

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
                        MostrarMensaje("Error en base de datos: " & resultado, False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error de procesamiento: " & ex.Message, False)
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
        ddlAeronave.Items.Insert(0, New ListItem("-- Primero seleccione aerolínea --", ""))
        ddlDestino.SelectedIndex = 0
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