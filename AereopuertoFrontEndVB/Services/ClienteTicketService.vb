Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ClienteTicketService

    ''' <summary>
    ''' Obtiene las categorías de soporte (Ej: Equipaje, Vuelos, Quejas, etc.)
    ''' </summary>
    Public Shared Function ObtenerTiposTicket() As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_TIPOS_TICKET_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cur As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cur.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cur)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .tipos = UtilidadesService.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al cargar categorías: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Lista todos los tickets abiertos y cerrados de un cliente específico.
    ''' </summary>
    Public Shared Function ObtenerTicketsCliente(correo As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_TICKETS_CLIENTE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .tickets = UtilidadesService.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al cargar tickets: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Abre un nuevo caso de soporte.
    ''' </summary>
    Public Shared Function CrearTicket(asunto As String, correo As String, idTipo As Integer) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CREAR_TICKET", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_asunto", OracleDbType.Varchar2).Value = asunto
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo
                    cmd.Parameters.Add("p_id_tipo", OracleDbType.Int32).Value = idTipo

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()
                    If resultado = "EXITO" Then
                        Return New With {.success = True, .mensaje = "✅ Ticket abierto exitosamente. El equipo lo revisará pronto."}
                    Else
                        Return New With {.success = False, .mensaje = "⚠️ Error DB: " & resultado}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "❌ Error interno: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Carga el historial de conversación de un ticket específico.
    ''' </summary>
    Public Shared Function ObtenerHiloRespuestas(idTicket As Integer) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_RESPUESTAS_TICKET", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_id_ticket", OracleDbType.Int32).Value = idTicket

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .respuestas = UtilidadesService.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al cargar la conversación: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Agrega un nuevo mensaje al chat del ticket.
    ''' </summary>
    Public Shared Function AgregarRespuesta(idTicket As Integer, mensaje As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_AGREGAR_RESPUESTA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_id_ticket", OracleDbType.Int32).Value = idTicket
                    cmd.Parameters.Add("p_mensaje", OracleDbType.Varchar2).Value = mensaje

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()
                    If resultado = "EXITO" Then
                        Return New With {.success = True, .mensaje = "Respuesta enviada."}
                    Else
                        Return New With {.success = False, .mensaje = "⚠️ Error al enviar: " & resultado}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "❌ Error interno: " & ex.Message}
        End Try
    End Function

End Class