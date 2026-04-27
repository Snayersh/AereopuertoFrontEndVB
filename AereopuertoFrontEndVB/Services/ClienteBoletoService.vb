Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ClienteBoletoService

    ''' <summary>
    ''' Obtiene la lista de boletos del cliente, filtrada por su estado (Pendientes, Pagados, Cancelados, etc.)
    ''' </summary>
    Public Shared Function ObtenerMisBoletos(correo As String, idEstadoFiltro As Integer) As Object
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CONSULTAR_MIS_BOLETOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo
                    cmd.Parameters.Add("p_id_estado_filtro", OracleDbType.Int32).Value = idEstadoFiltro

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        Return New With {.success = True, .boletos = UtilidadesService.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al obtener tus vuelos: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Procesa la cancelación de una reserva validando el código y el usuario dueño.
    ''' </summary>
    Public Shared Function CancelarReserva(codigoReserva As String, correoUsuario As String) As Object
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CANCELAR_RESERVA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo_reserva", OracleDbType.Varchar2).Value = codigoReserva
                    cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = correoUsuario

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        Return New With {.success = True, .mensaje = "Reserva cancelada correctamente."}
                    Else
                        Return New With {.success = False, .mensaje = "No se pudo cancelar: " & resultado}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al procesar cancelación: " & ex.Message}
        End Try
    End Function

End Class