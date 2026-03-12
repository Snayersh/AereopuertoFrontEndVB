Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Text

Public Class Reservas
    Inherits System.Web.UI.Page

    ' Variable para guardar el correo del usuario que inició sesión
    Private CorreoUsuario As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Seguridad: Validamos que haya una sesión activa
        If Session("UserRole") Is Nothing Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        ' Recuperamos el correo
        CorreoUsuario = If(Session("UserEmail") IsNot Nothing, Session("UserEmail").ToString(), "cliente@prueba.com")

        If Not IsPostBack Then
            pnlError.Visible = False
            pnlExito.Visible = False
            CargarVuelosDisponibles()
            CargarClases()
        End If
    End Sub

    ' ====================================================================
    ' 1. CARGA DE CATÁLOGOS INICIALES
    ' ====================================================================
    Private Sub CargarVuelosDisponibles()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Dim query As String = "SELECT v.id_vuelo, " &
                                      "v.codigo_vuelo || ' : ' || o.codigo_iata || ' ✈️ ' || d.codigo_iata || ' (' || TO_CHAR(v.fecha_salida, 'DD/MM/YYYY HH24:MI') || ')' AS DETALLE " &
                                      "FROM AUR_VUELO v " &
                                      "INNER JOIN AUR_AEROPUERTO o ON v.id_origen = o.id_aeropuerto " &
                                      "INNER JOIN AUR_AEROPUERTO d ON v.id_destino = d.id_aeropuerto " &
                                      "WHERE v.id_estado_vuelo = 1 " &
                                      "ORDER BY v.fecha_salida ASC"
                Using cmd As New OracleCommand(query, conn)
                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlVuelos.DataSource = reader
                        ddlVuelos.DataTextField = "DETALLE"
                        ddlVuelos.DataValueField = "ID_VUELO"
                        ddlVuelos.DataBind()
                    End Using
                End Using
            End Using
            ddlVuelos.Items.Insert(0, New ListItem("-- Selecciona tu vuelo --", ""))
        Catch ex As Exception
            MostrarError("Error cargando vuelos: " & ex.Message)
        End Try
    End Sub

    Private Sub CargarClases()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Dim query As String = "SELECT id_tipo_boleto, nombre FROM AUR_TIPO_BOLETO ORDER BY id_tipo_boleto"
                Using cmd As New OracleCommand(query, conn)
                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlClase.DataSource = reader
                        ddlClase.DataTextField = "NOMBRE"
                        ddlClase.DataValueField = "ID_TIPO_BOLETO"
                        ddlClase.DataBind()
                    End Using
                End Using
            End Using
            ddlClase.Items.Insert(0, New ListItem("-- Selecciona la clase --", ""))
        Catch ex As Exception
            MostrarError("Error cargando clases: " & ex.Message)
        End Try
    End Sub

    ' ====================================================================
    ' 2. LA FUNCIÓN QUE FALTABA: SE DISPARA AL ELEGIR UN VUELO
    ' ====================================================================
    Protected Sub ddlVuelos_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlVuelos.SelectedIndexChanged
        If String.IsNullOrEmpty(ddlVuelos.SelectedValue) Then
            panelAvion.Visible = False
            Return
        End If

        Dim idVuelo As Integer = Convert.ToInt32(ddlVuelos.SelectedValue)
        GenerarMapaDeAsientos(idVuelo)
    End Sub

    ' ====================================================================
    ' 3. DIBUJAR EL AVIÓN DINÁMICAMENTE DESDE ORACLE
    ' ====================================================================
    Private Sub GenerarMapaDeAsientos(idVuelo As Integer)
        Dim db As New ConexionDB()
        Dim capacidad As Integer = 0
        Dim asientosOcupados As New List(Of String)()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_MAPA_ASIENTOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = idVuelo

                    Dim outCapacidad As New OracleParameter("p_capacidad", OracleDbType.Int32)
                    outCapacidad.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outCapacidad)

                    Dim cursorParam As New OracleParameter("p_cursor_ocupados", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()

                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            ' Ahora leemos la columna "numero" que trae el texto "1A"
                            asientosOcupados.Add(reader("numero").ToString().ToUpper())
                        End While
                    End Using

                    capacidad = Convert.ToInt32(outCapacidad.Value.ToString())
                End Using
            End Using

            ' Armamos el HTML de los cuadritos
            Dim htmlAvion As New StringBuilder()
            Dim letras() As String = {"A", "B", "C", "D"}
            Dim totalFilas As Integer = Math.Ceiling(capacidad / 4.0)
            Dim asientoActual As Integer = 1

            For fila As Integer = 1 To totalFilas
                htmlAvion.Append("<div class='seat-row'>")

                For col As Integer = 0 To 3
                    If asientoActual > capacidad Then Exit For

                    Dim codigoAsiento As String = fila.ToString() & letras(col)

                    If asientosOcupados.Contains(codigoAsiento) Then
                        htmlAvion.Append($"<div class='seat occupied' onclick=""alert('El asiento {codigoAsiento} ya está ocupado. Elige otro.');"">{codigoAsiento}</div>")
                    Else
                        htmlAvion.Append($"<div class='seat available' onclick=""seleccionarAsiento(this, '{codigoAsiento}')"">{codigoAsiento}</div>")
                    End If

                    If col = 1 Then htmlAvion.Append("<div class='aisle'></div>")

                    asientoActual += 1
                Next
                htmlAvion.Append("</div>")
            Next

            litMapaAsientos.Text = htmlAvion.ToString()
            panelAvion.Visible = True

            ' Limpiar la selección anterior en el HTML
            hfAsientoSeleccionado.Value = ""
            ClientScript.RegisterStartupScript(Me.GetType(), "LimpiarAsiento", "document.getElementById('lblAsientoMostrado').innerText = 'Ninguno';", True)

        Catch ex As Exception
            MostrarError("Error al cargar el mapa de asientos: " & ex.Message)
            panelAvion.Visible = False
        End Try
    End Sub

    ' ====================================================================
    ' 4. GUARDAR LA RESERVA
    ' ====================================================================
    Protected Sub btnReservar_Click(sender As Object, e As EventArgs) Handles btnReservar.Click
        ' 1. Capturamos todos los asientos separados por comas (Ej: "1A,1B")
        Dim asientosElegidos As String = hfAsientoSeleccionado.Value

        If String.IsNullOrEmpty(ddlVuelos.SelectedValue) OrElse String.IsNullOrEmpty(ddlClase.SelectedValue) Then
            MostrarError("Debes seleccionar un vuelo y la clase de cabina.")
            Return
        End If

        If String.IsNullOrEmpty(asientosElegidos) Then
            MostrarError("No se han detectado asientos. Haz clic en el mapa del avión.")
            Return
        End If

        ' 2. Convertimos el texto en una lista de asientos y generamos UN solo código de reserva
        Dim listaAsientos As String() = asientosElegidos.Split(","c)
        Dim random As New Random()
        Dim codigoReservaUnico As String = "B-" & Guid.NewGuid().ToString().Substring(0, 5).ToUpper()

        Dim db As New ConexionDB()
        Dim errores As New StringBuilder()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                conn.Open()

                ' 3. Hacemos un ciclo (loop) para guardar cada asiento en la base de datos
                For Each asiento In listaAsientos
                    Dim asientoLimpio As String = asiento.Trim()

                    Using cmd As New OracleCommand("SP_RESERVAR_BOLETO", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.BindByName = True

                        cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = CorreoUsuario
                        cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = Convert.ToInt32(ddlVuelos.SelectedValue)
                        cmd.Parameters.Add("p_id_tipo_boleto", OracleDbType.Int32).Value = Convert.ToInt32(ddlClase.SelectedValue)
                        cmd.Parameters.Add("p_asiento", OracleDbType.Varchar2).Value = asientoLimpio
                        cmd.Parameters.Add("p_codigo_reserva", OracleDbType.Varchar2).Value = codigoReservaUnico

                        Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                        outResultado.Direction = ParameterDirection.Output
                        cmd.Parameters.Add(outResultado)

                        cmd.ExecuteNonQuery()

                        Dim resultado As String = outResultado.Value.ToString()
                        If resultado <> "EXITO" Then
                            errores.AppendLine($"No se pudo reservar el asiento {asientoLimpio}: {resultado}")
                        End If
                    End Using
                Next
            End Using

            ' 4. Mostramos el resultado final
            If errores.Length = 0 Then
                pnlError.Visible = False
                ddlVuelos.Enabled = False
                ddlClase.Enabled = False
                btnReservar.Enabled = False
                panelAvion.Visible = False

                lblCodigoBoleto.Text = codigoReservaUnico
                lblAsientoConfirmado.Text = String.Join(", ", listaAsientos)

                ' =========================================================
                ' NUEVA LÍNEA: Le inyectamos el código al botón de Pagar
                hlPagarAhora.NavigateUrl = "Pagos.aspx?codigo=" & codigoReservaUnico
                ' =========================================================

                pnlExito.Visible = True
            Else
                MostrarError("Hubo problemas con algunos asientos: " & errores.ToString())
            End If

        Catch ex As Exception
            MostrarError("Error al procesar la reserva múltiple: " & ex.Message)
        End Try
    End Sub
    Private Sub MostrarError(mensaje As String)
        pnlExito.Visible = False
        pnlError.Visible = True
        lblError.Text = mensaje
    End Sub

End Class