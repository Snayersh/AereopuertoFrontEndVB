Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class AccountActivationService

    ''' <summary>
    ''' Lógica centralizada para activar la cuenta de un usuario mediante su token único.
    ''' </summary>
    Public Shared Function ActivarCuentaUsuario(token As String) As Object
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ACTIVAR_CUENTA", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' Parámetro de Entrada
                    cmd.Parameters.Add("p_token", OracleDbType.Varchar2).Value = token

                    ' Parámetro de Salida
                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    ' Evaluamos la respuesta del Store Procedure
                    If resultado = "EXITO" Then
                        Return New With {.success = True, .mensaje = "Cuenta activada correctamente."}
                    ElseIf resultado = "TOKEN_INVALIDO" Then
                        Return New With {.success = False, .mensaje = "El enlace de activación no es válido o la cuenta ya fue verificada."}
                    Else
                        Return New With {.success = False, .mensaje = "Aviso del sistema: " & resultado}
                    End If
                End Using
            End Using

        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error de conexión con la base de datos: " & ex.Message}
        End Try
    End Function

End Class