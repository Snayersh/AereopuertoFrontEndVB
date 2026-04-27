Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class AccountValidacionService

    ''' <summary>
    ''' Verifica en la base de datos si el token de sesión de un usuario sigue siendo válido.
    ''' Ideal para mantener la sesión abierta en la App Móvil (React Native).
    ''' </summary>
    Public Shared Function ValidarTokenSesion(correo As String, token As String) As Object
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_VALIDAR_TOKEN", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' 🔥 Se eliminó cmd.BindByName = True para respetar la posición exacta del SP original
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo.ToLower()
                    cmd.Parameters.Add("p_token_sesion", OracleDbType.Varchar2).Value = token

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    ' Estandarizamos la respuesta para que la API la entienda perfecto
                    If resultado.ToUpper() = "VALIDO" OrElse resultado = "EXITO" Then
                        Return New With {.success = True, .mensaje = "Sesión válida."}
                    Else
                        Return New With {.success = False, .mensaje = resultado}
                    End If

                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "ERROR_DB: " & ex.Message}
        End Try
    End Function

End Class