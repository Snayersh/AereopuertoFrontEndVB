Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ClienteEquipajeService

    ''' <summary>
    ''' Obtiene el catálogo de tipos de equipaje (Ej: Maleta de mano, Bodega 23kg, etc.)
    ''' </summary>
    Public Shared Function ObtenerTiposEquipaje() As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_TIPOS_EQUIPAJE_CBX", conn)
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
            Return New With {.success = False, .mensaje = "Error al cargar tipos: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Obtiene las reservas activas del usuario a las que les puede agregar equipaje.
    ''' </summary>
    Public Shared Function ObtenerBoletosDisponibles(correo As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CARGAR_BOLETOS_EQUIPAJE", conn)
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
                        Return New With {.success = True, .boletos = UtilidadesService.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al cargar reservas: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Lista las maletas ya registradas bajo un código de boleto específico.
    ''' </summary>
    Public Shared Function ObtenerEquipajeRegistrado(codigoBoleto As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_EQUIPAJE_BOLETO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_codigo_boleto", OracleDbType.Varchar2).Value = codigoBoleto

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .equipaje = UtilidadesService.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al cargar equipaje: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Registra una nueva maleta. Verifica excesos de peso y genera recargos si aplica.
    ''' </summary>
    Public Shared Function RegistrarNuevaMaleta(codigoBoleto As String, peso As Decimal, descripcion As String, idTipoEquipaje As Integer) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_EQUIPAJE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo_boleto", OracleDbType.Varchar2).Value = codigoBoleto
                    cmd.Parameters.Add("p_peso", OracleDbType.Decimal).Value = peso
                    cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = descripcion
                    cmd.Parameters.Add("p_id_tipo_equipaje", OracleDbType.Int32).Value = idTipoEquipaje

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado.StartsWith("EXITO") Then
                        Dim partes = resultado.Split("|"c)
                        Dim mensajeExito As String = If(partes.Length > 1, "✅ " & partes(1), "¡Equipaje registrado exitosamente!")
                        Return New With {.success = True, .mensaje = mensajeExito}
                    Else
                        Return New With {.success = False, .mensaje = "No se pudo registrar: " & resultado}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error de conexión al registrar: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Obtiene la línea de tiempo de una maleta (Ej: Check-In -> Cargada al Avión -> Banda de reclamo)
    ''' </summary>
    Public Shared Function RastrearEquipaje(idEquipaje As Integer) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_VER_TRACKING_EQUIPAJE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_id_equipaje", OracleDbType.Int32).Value = idEquipaje

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .tracking = UtilidadesService.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al cargar rastreo: " & ex.Message}
        End Try
    End Function

End Class