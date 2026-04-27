Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ClientePerfilService

    ''' <summary>
    ''' Obtiene los datos del perfil del usuario para llenarlos en la Web o la App.
    ''' </summary>
    Public Shared Function ObtenerPerfil(correo As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_PERFIL", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            ' Usamos nuestra utilidad para convertir la tabla a un formato amigable
                            Return New With {.success = True, .perfil = UtilidadesService.ConvertirDataTableALista(dt)(0)}
                        Else
                            Return New With {.success = False, .mensaje = "No se encontraron datos del perfil."}
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error de conexión: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Actualiza los datos del perfil (Unificado para Web y App Móvil).
    ''' </summary>
    Public Shared Function ActualizarPerfil(correo As String, pNombre As String, sNombre As String, pApellido As String, sApellido As String, telefono As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ACTUALIZAR_PERFIL", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo
                    cmd.Parameters.Add("p_primer_nombre", OracleDbType.Varchar2).Value = pNombre
                    cmd.Parameters.Add("p_segundo_nombre", OracleDbType.Varchar2).Value = sNombre
                    cmd.Parameters.Add("p_primer_apellido", OracleDbType.Varchar2).Value = pApellido
                    cmd.Parameters.Add("p_segundo_apellido", OracleDbType.Varchar2).Value = sApellido
                    cmd.Parameters.Add("p_telefono", OracleDbType.Varchar2).Value = telefono

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        Return New With {.success = True, .mensaje = "¡Tu perfil se ha actualizado correctamente!"}
                    Else
                        Return New With {.success = False, .mensaje = resultado}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al actualizar: " & ex.Message}
        End Try
    End Function

End Class