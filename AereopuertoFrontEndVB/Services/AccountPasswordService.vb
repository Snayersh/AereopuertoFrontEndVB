Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class AccountPasswordService

    ''' <summary>
    ''' Paso 1: Genera un token y lo asocia al correo (Viene de tu archivo monolítico).
    ''' </summary>
    Public Shared Function SolicitarRecuperacion(correo As String) As Object
        Dim db As New ConexionDB()
        Dim tokenRecuperacion As String = Guid.NewGuid().ToString()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_GENERAR_TOKEN_RECUPERACION", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo.ToLower()
                    cmd.Parameters.Add("p_token", OracleDbType.Varchar2).Value = tokenRecuperacion

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    ' Nota: Aquí en el futuro podrías usar UtilidadesService para enviar el correo con el token generado.
                    ' Por seguridad, siempre devolvemos éxito para evitar que los hackers escaneen qué correos existen.
                    Return New With {.success = True, .mensaje = "Si el correo existe, te hemos enviado un enlace para recuperar tu contraseña."}
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error del sistema: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Paso 2: Actualiza la contraseña usando el token enviado por correo.
    ''' </summary>
    Public Shared Function ActualizarPasswordConToken(token As String, passwordPlana As String) As Object
        Dim db As New ConexionDB()

        ' 🔥 Usamos nuestra herramienta compartida para no repetir código
        Dim nuevaPassHasheada As String = UtilidadesService.EncriptarSHA256(passwordPlana)

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ACTUALIZAR_PASSWORD_TOKEN", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_token", OracleDbType.Varchar2).Value = token
                    cmd.Parameters.Add("p_nuevo_hash", OracleDbType.Varchar2).Value = nuevaPassHasheada

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = paramOut.Value.ToString()

                    If resultado = "EXITO" Then
                        Return New With {.success = True, .mensaje = "¡Tu contraseña ha sido actualizada exitosamente! Ya puedes iniciar sesión."}
                    Else
                        Return New With {.success = False, .mensaje = "El enlace ya fue utilizado o no es válido."}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error de base de datos: " & ex.Message}
        End Try
    End Function

End Class