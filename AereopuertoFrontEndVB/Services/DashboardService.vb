Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class DashboardService

    ''' <summary>
    ''' Obtiene las estadísticas generales (vuelos, llegadas, salidas) y la lista de vuelos en vivo.
    ''' </summary>
    Public Shared Function ObtenerRadarEnVivo() As Object
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_DATOS_DASHBOARD_PRINCIPAL", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' Parámetros de salida para las tarjetas (Stat-Cards)
                    cmd.Parameters.Add("p_vuelos_activos", OracleDbType.Int32).Direction = ParameterDirection.Output
                    cmd.Parameters.Add("p_llegadas_hoy", OracleDbType.Int32).Direction = ParameterDirection.Output
                    cmd.Parameters.Add("p_salidas_hoy", OracleDbType.Int32).Direction = ParameterDirection.Output

                    ' Cursor para la tabla principal (Rutas en vivo)
                    Dim cursorParam As New OracleParameter("p_cursor_vuelos", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    ' Extraemos las estadísticas
                    Dim statActivos As Integer = Convert.ToInt32(cmd.Parameters("p_vuelos_activos").Value.ToString())
                    Dim statLlegadas As Integer = Convert.ToInt32(cmd.Parameters("p_llegadas_hoy").Value.ToString())
                    Dim statSalidas As Integer = Convert.ToInt32(cmd.Parameters("p_salidas_hoy").Value.ToString())

                    ' Extraemos la lista de vuelos en vivo usando nuestra utilidad estándar
                    Dim listaVuelos As Object = Nothing
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        listaVuelos = UtilidadesService.ConvertirDataTableALista(dt)
                    End Using

                    ' Empaquetamos todo en un súper objeto JSON-Ready
                    Return New With {
                        .success = True,
                        .estadisticas = New With {
                            .activos = statActivos,
                            .llegadas = statLlegadas,
                            .salidas = statSalidas
                        },
                        .radar_vuelos = listaVuelos
                    }
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al conectar con el radar: " & ex.Message}
        End Try
    End Function
    ''' <summary>
    ''' Obtiene toda la información técnica y de ruta de un vuelo específico, incluyendo escalas.
    ''' </summary>
    Public Shared Function ObtenerDetalleVuelo(idVuelo As Integer) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_DETALLE_VUELO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = idVuelo

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            ' Devolvemos el primer registro (único) en formato diccionario
                            Return New With {.success = True, .datos = UtilidadesService.ConvertirDataTableALista(dt)(0)}
                        Else
                            Return New With {.success = False, .mensaje = "Vuelo no encontrado."}
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error de conexión con el radar: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Obtiene las coordenadas geográficas de los vuelos activos para dibujarlos en Google Maps.
    ''' </summary>
    Public Shared Function ObtenerVuelosParaRadar() As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_RADAR", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        ' Nuestra utilidad mágica convierte toda la tabla a una lista de diccionarios
                        Return New With {.success = True, .vuelos_radar = UtilidadesService.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al cargar coordenadas del radar: " & ex.Message}
        End Try
    End Function
End Class