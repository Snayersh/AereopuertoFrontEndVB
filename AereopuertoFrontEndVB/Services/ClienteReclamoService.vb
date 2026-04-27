Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ClienteReclamoService

    ''' <summary>
    ''' Obtiene la lista de maletas registradas del cliente para que pueda seleccionar cuál tiene problemas.
    ''' </summary>
    Public Shared Function ObtenerEquipajeCliente(correo As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_EQUIPAJE_CLIENTE_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = correo

                    Dim cur As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cur.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cur)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .equipaje = UtilidadesService.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al cargar su equipaje: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Registra un nuevo reclamo asociado a un equipaje específico.
    ''' </summary>
    Public Shared Function RegistrarReclamo(idEquipaje As Integer, descripcion As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_RECLAMO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = descripcion
                    cmd.Parameters.Add("p_id_equipaje", OracleDbType.Int32).Value = idEquipaje

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If paramOut.Value.ToString() = "EXITO" Then
                        Return New With {.success = True, .mensaje = "✅ Reclamo enviado. Nuestro equipo de Servicio al Cliente lo revisará a la brevedad."}
                    Else
                        Return New With {.success = False, .mensaje = "⚠️ " & paramOut.Value.ToString()}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "❌ Error en el servidor: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Obtiene el historial de todos los reclamos hechos por el usuario para ver su estado.
    ''' </summary>
    Public Shared Function ObtenerHistorialReclamos(correo As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_RECLAMOS_CLIENTE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = correo

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .reclamos = UtilidadesService.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al cargar historial: " & ex.Message}
        End Try
    End Function

End Class