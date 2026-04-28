Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ClienteReservaService

    ''' <summary>
    ''' Obtiene los vuelos disponibles para reserva en el combo.
    ''' </summary>
    Public Shared Function ObtenerVuelosDisponibles() As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_VUELOS_RESERVA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .vuelos = UtilidadesService.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error cargando vuelos: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Obtiene las clases de boleto (Primera, Ejecutiva, Económica).
    ''' </summary>
    Public Shared Function ObtenerClases() As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_CLASES_BOLETO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .clases = UtilidadesService.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error cargando clases: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Obtiene la información Pura del avión para que la Web o la App dibujen sus propios mapas.
    ''' </summary>
    Public Shared Function ObtenerDatosMapaAsientos(idVuelo As Integer) As Object
        Dim db As New ConexionDB()
        Dim capacidad As Integer = 0
        Dim ocupados As New List(Of String)()

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
                            ocupados.Add(reader("numero").ToString().ToUpper())
                        End While
                    End Using
                    capacidad = Convert.ToInt32(outCapacidad.Value.ToString())
                End Using
            End Using
            Return New With {.success = True, .capacidad = capacidad, .ocupados = ocupados}
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al obtener mapa: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Procesa una reserva de múltiples asientos.
    ''' </summary>
    Public Shared Function ProcesarReservaMasiva(correo As String, idVuelo As Integer, listaAsientos As String()) As Object
        Dim codigoReservaUnico As String = "B-" & Guid.NewGuid().ToString().Substring(0, 5).ToUpper()
        Dim db As New ConexionDB()
        Dim errores As New List(Of String)()
        Dim asientosConfirmados As New List(Of String)()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                conn.Open()
                For Each item In listaAsientos
                    Dim partes() As String = item.Split(":"c)
                    Dim asientoLimpio As String = partes(0).Trim()
                    Dim idClaseElegida As Integer = Convert.ToInt32(partes(1))

                    asientosConfirmados.Add(asientoLimpio)

                    Using cmd As New OracleCommand("SP_RESERVAR_BOLETO", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.BindByName = True

                        cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = correo
                        cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = idVuelo
                        cmd.Parameters.Add("p_id_tipo_boleto", OracleDbType.Int32).Value = idClaseElegida
                        cmd.Parameters.Add("p_asiento", OracleDbType.Varchar2).Value = asientoLimpio
                        cmd.Parameters.Add("p_codigo_reserva", OracleDbType.Varchar2).Value = codigoReservaUnico

                        Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                        outResultado.Direction = ParameterDirection.Output
                        cmd.Parameters.Add(outResultado)

                        cmd.ExecuteNonQuery()

                        If outResultado.Value.ToString() <> "EXITO" Then
                            errores.Add($"Asiento {asientoLimpio}: {outResultado.Value.ToString()}")
                        End If
                    End Using
                Next
            End Using

            If errores.Count = 0 Then
                Return New With {
                    .success = True,
                    .codigo = codigoReservaUnico,
                    .asientos = String.Join(", ", asientosConfirmados)
                }
            Else
                Return New With {.success = False, .mensaje = String.Join(" | ", errores)}
            End If
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error crítico: " & ex.Message}
        End Try
    End Function

End Class