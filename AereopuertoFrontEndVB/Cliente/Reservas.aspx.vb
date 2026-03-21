Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Text
Imports System.Web.Services

Public Class Reservas
    Inherits System.Web.UI.Page

    Private CorreoUsuario As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("UserRole") Is Nothing Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        CorreoUsuario = If(Session("UserEmail") IsNot Nothing, Session("UserEmail").ToString(), "cliente@prueba.com")

        If Not IsPostBack Then
            pnlError.Visible = False
            pnlExito.Visible = False
            CargarVuelosDisponibles()
            CargarClases()
        End If
    End Sub

    ' ====================================================================
    ' 1. CARGA DE CATÁLOGOS
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

            ' TRUCO: Le decimos a ASP.NET que este control no haga PostBack al servidor,
            ' sino que llame únicamente a nuestra función de JavaScript
            ddlClase.Attributes.Add("onchange", "actualizarFiltroYPrecio();")

        Catch ex As Exception
            MostrarError("Error cargando clases: " & ex.Message)
        End Try
    End Sub

    ' ====================================================================
    ' 2. AL ELEGIR UN VUELO
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
    ' 3. DIBUJAR EL AVIÓN INTELIGENTE (Inyecta precio y clase)
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
                            asientosOcupados.Add(reader("numero").ToString().ToUpper())
                        End While
                    End Using
                    capacidad = Convert.ToInt32(outCapacidad.Value.ToString())
                End Using
            End Using

            Dim htmlAvion As New StringBuilder()
            Dim letras() As String = {"A", "B", "C", "D"}
            Dim totalFilas As Integer = Math.Ceiling(capacidad / 4.0)
            Dim asientoActual As Integer = 1

            Dim limitePrimera As Integer = Math.Ceiling(totalFilas * 0.1)
            Dim limiteEjecutiva As Integer = Math.Ceiling(totalFilas * 0.3)

            For fila As Integer = 1 To totalFilas
                htmlAvion.Append("<div class='seat-row'>")

                Dim claseCSS As String
                Dim precioAsiento As Decimal
                Dim idClaseAsiento As Integer

                ' Asignamos los datos reales a cada bloque del avión
                If fila <= limitePrimera Then
                    claseCSS = "primera"
                    precioAsiento = 950.0
                    idClaseAsiento = 3
                ElseIf fila <= limiteEjecutiva Then
                    claseCSS = "ejecutiva"
                    precioAsiento = 550.0
                    idClaseAsiento = 2
                Else
                    claseCSS = "economica"
                    precioAsiento = 250.0
                    idClaseAsiento = 1
                End If

                For col As Integer = 0 To 3
                    If asientoActual > capacidad Then Exit For
                    Dim codigoAsiento As String = fila.ToString() & letras(col)

                    If asientosOcupados.Contains(codigoAsiento) Then
                        htmlAvion.Append($"<div class='seat occupied {claseCSS}' onclick=""alert('Ocupado.');"">{codigoAsiento}</div>")
                    Else
                        ' ATENCIÓN AQUÍ: Inyectamos data-precio y data-idclase al HTML
                        htmlAvion.Append($"<div class='seat available {claseCSS}' data-precio='{precioAsiento}' data-idclase='{idClaseAsiento}' onclick=""seleccionarAsiento(this, '{codigoAsiento}')"">{codigoAsiento}</div>")
                    End If

                    If col = 1 Then htmlAvion.Append("<div class='aisle'></div>")
                    asientoActual += 1
                Next
                htmlAvion.Append("</div>")
            Next

            litMapaAsientos.Text = htmlAvion.ToString()
            panelAvion.Visible = True
            hfAsientoSeleccionado.Value = ""
            ClientScript.RegisterStartupScript(Me.GetType(), "LimpiarAsiento", "document.getElementById('lblAsientoMostrado').innerText = 'Ninguno'; siExisteFuncion('actualizarFiltroYPrecio');", True)

        Catch ex As Exception
            MostrarError("Error al cargar el mapa: " & ex.Message)
            panelAvion.Visible = False
        End Try
    End Sub

    ' ====================================================================
    ' 4. GUARDAR RESERVA MULTI-CLASE
    ' ====================================================================
    Protected Sub btnReservar_Click(sender As Object, e As EventArgs) Handles btnReservar.Click
        ' Ahora los asientos vienen así: "1A:3,12B:1" (Asiento : IdClase)
        Dim asientosElegidos As String = hfAsientoSeleccionado.Value

        If String.IsNullOrEmpty(ddlVuelos.SelectedValue) Then
            MostrarError("Debes seleccionar un vuelo.")
            Return
        End If

        If String.IsNullOrEmpty(asientosElegidos) Then
            MostrarError("Haz clic en el mapa para elegir tus asientos.")
            Return
        End If

        Dim listaAsientos As String() = asientosElegidos.Split(","c)
        Dim codigoReservaUnico As String = "B-" & Guid.NewGuid().ToString().Substring(0, 5).ToUpper()
        Dim db As New ConexionDB()
        Dim errores As New StringBuilder()
        Dim asientosParaMostrar As New List(Of String)()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                conn.Open()

                For Each item In listaAsientos
                    ' Desempaquetamos el dato (Ej. "1A:3")
                    Dim partes() As String = item.Split(":"c)
                    Dim asientoLimpio As String = partes(0).Trim()
                    Dim idClaseElegida As Integer = Convert.ToInt32(partes(1))

                    asientosParaMostrar.Add(asientoLimpio)

                    Using cmd As New OracleCommand("SP_RESERVAR_BOLETO", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.BindByName = True

                        cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = CorreoUsuario
                        cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = Convert.ToInt32(ddlVuelos.SelectedValue)

                        ' AHORA USAMOS LA CLASE ESPECÍFICA DE ESE ASIENTO, NO LA DEL COMBOBOX
                        cmd.Parameters.Add("p_id_tipo_boleto", OracleDbType.Int32).Value = idClaseElegida

                        cmd.Parameters.Add("p_asiento", OracleDbType.Varchar2).Value = asientoLimpio
                        cmd.Parameters.Add("p_codigo_reserva", OracleDbType.Varchar2).Value = codigoReservaUnico

                        Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                        outResultado.Direction = ParameterDirection.Output
                        cmd.Parameters.Add(outResultado)

                        cmd.ExecuteNonQuery()

                        If outResultado.Value.ToString() <> "EXITO" Then
                            errores.AppendLine($"Error en asiento {asientoLimpio}: {outResultado.Value.ToString()}")
                        End If
                    End Using
                Next
            End Using

            If errores.Length = 0 Then
                pnlError.Visible = False
                ddlVuelos.Enabled = False
                ddlClase.Enabled = False
                btnReservar.Enabled = False
                panelAvion.Visible = False

                lblCodigoBoleto.Text = codigoReservaUnico
                lblAsientoConfirmado.Text = String.Join(", ", asientosParaMostrar)
                hlPagarAhora.NavigateUrl = "Pagos.aspx?codigo=" & codigoReservaUnico
                pnlExito.Visible = True
            Else
                MostrarError("Problemas al reservar: " & errores.ToString())
            End If

        Catch ex As Exception
            MostrarError("Error crítico: " & ex.Message)
        End Try
    End Sub

    Private Sub MostrarError(mensaje As String)
        pnlExito.Visible = False
        pnlError.Visible = True
        lblError.Text = mensaje
    End Sub
    ' ====================================================================
    ' 5. SINCRONIZACIÓN SILENCIOSA CADA 30 SEGUNDOS
    ' ====================================================================
    <WebMethod()>
    Public Shared Function ObtenerAsientosOcupados(idVuelo As Integer) As List(Of String)
        Dim ocupados As New List(Of String)()
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_MAPA_ASIENTOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
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
                            ' Guardamos solo el número (Ej: "1A", "2B")
                            ocupados.Add(reader("numero").ToString().ToUpper())
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Como es un proceso silencioso de fondo, si hay un micro-corte de red,
            ' simplemente ignoramos el error y devolverá la lista vacía para no asustar al usuario.
        End Try

        Return ocupados
    End Function
End Class