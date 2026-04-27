Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class AccountLoginService

    ''' <summary>
    ''' Valida las credenciales y devuelve un objeto con la identidad del usuario o el error correspondiente.
    ''' </summary>
    Public Shared Function IniciarSesion(email As String, passwordPlana As String) As Object
        Dim db As New ConexionDB()
        ' Usamos la utilidad compartida que ya creamos
        Dim passHasheada As String = UtilidadesService.EncriptarSHA256(passwordPlana)

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_VALIDAR_LOGIN", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = email.ToLower()
                    cmd.Parameters.Add("p_password_hash", OracleDbType.Varchar2).Value = passHasheada

                    Dim outRol As New OracleParameter("p_id_rol", OracleDbType.Int32)
                    outRol.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outRol)

                    Dim outNombre As New OracleParameter("p_nombre_completo", OracleDbType.Varchar2, 200)
                    outNombre.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outNombre)

                    Dim outToken As New OracleParameter("p_token_sesion", OracleDbType.Varchar2, 255)
                    outToken.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outToken)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    ' --- EVALUACIÓN DE RESULTADOS ---
                    If resultado = "EXITO" Then
                        Dim idRol As Integer = Convert.ToInt32(outRol.Value.ToString())

                        Return New With {
                            .success = True,
                            .id_rol = idRol,
                            .nombre_completo = outNombre.Value.ToString(),
                            .token_sesion = If(IsDBNull(outToken.Value), "", outToken.Value.ToString()),
                            .rol_nombre = TraducirRol(idRol)
                        }
                    Else
                        ' Manejo centralizado de mensajes de error
                        Dim mensajeAmigable As String = "Error al iniciar sesión."
                        Select Case resultado
                            Case "BLOQUEO_TEMPORAL" : mensajeAmigable = "⛔ Demasiados intentos fallidos. Tu cuenta ha sido bloqueada temporalmente por 1 minuto."
                            Case "CUENTA_PENDIENTE" : mensajeAmigable = "Tu cuenta aún no está activada. Revisa tu correo electrónico."
                            Case "CUENTA_INACTIVA" : mensajeAmigable = "⛔ Tu cuenta ha sido desactivada. Contacta a la administración."
                            Case "CREDENCIALES_INVALIDAS" : mensajeAmigable = "Correo o contraseña incorrectos."
                            Case Else : mensajeAmigable = resultado
                        End Select

                        Return New With {.success = False, .mensaje = mensajeAmigable}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error de sistema: " & ex.Message}
        End Try
    End Function

    ' Lógica centralizada de nombres de roles
    Private Shared Function TraducirRol(idRol As Integer) As String
        Select Case idRol
            Case 1 : Return "Admin"
            Case 2 : Return "Pasajero"
            Case 3 : Return "Operaciones"
            Case 4 : Return "Recursos_Humanos"
            Case 5 : Return "Seguridad"
            Case 6 : Return "Servicio_Al_Cliente"
            Case 7 : Return "Mantenimiento_Tecnico"
            Case Else : Return "Invitado"
        End Select
    End Function

End Class