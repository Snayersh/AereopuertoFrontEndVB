Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class AccountRegistroService

    Public Shared Function RegistrarNuevoCliente(
        pNombre As String, sNombre As String, tNombre As String,
        pApellido As String, sApellido As String, aCasada As String,
        fechaNac As DateTime, telefono As String, correo As String,
        pais As String, depto As String, muni As String, zona As String,
        colonia As String, calle As String, avenida As String, numCasa As String,
        pasaporte As String, passwordPlana As String) As Object

        ' 1. Validar Edad (Centralizamos la regla de negocio aquí)
        Dim edad As Integer = DateTime.Now.Year - fechaNac.Year
        If DateTime.Now < fechaNac.AddYears(edad) Then edad -= 1

        If edad < 18 Then
            Return New With {.success = False, .mensaje = "Debes ser mayor de 18 años para crear una cuenta en La Aurora."}
        End If

        ' 2. Preparar Datos
        Dim db As New ConexionDB()
        Dim contrasenaHasheada As String = UtilidadesService.EncriptarSHA256(passwordPlana)
        Dim tokenActivacion As String = Guid.NewGuid().ToString()

        ' 3. Ejecutar BD
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_CLIENTE_COMPLETO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    ' Parámetros de Persona
                    cmd.Parameters.Add("p_primer_nombre", OracleDbType.Varchar2).Value = pNombre
                    cmd.Parameters.Add("p_segundo_nombre", OracleDbType.Varchar2).Value = sNombre
                    cmd.Parameters.Add("p_tercer_nombre", OracleDbType.Varchar2).Value = tNombre
                    cmd.Parameters.Add("p_primer_apellido", OracleDbType.Varchar2).Value = pApellido
                    cmd.Parameters.Add("p_segundo_apellido", OracleDbType.Varchar2).Value = sApellido
                    cmd.Parameters.Add("p_apellido_casada", OracleDbType.Varchar2).Value = aCasada
                    cmd.Parameters.Add("p_fecha_nacimiento", OracleDbType.Date).Value = fechaNac
                    cmd.Parameters.Add("p_telefono", OracleDbType.Varchar2).Value = telefono
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo.ToLower()

                    ' Parámetros de Dirección
                    cmd.Parameters.Add("p_pais", OracleDbType.Varchar2).Value = pais
                    cmd.Parameters.Add("p_departamento", OracleDbType.Varchar2).Value = depto
                    cmd.Parameters.Add("p_municipio", OracleDbType.Varchar2).Value = muni
                    cmd.Parameters.Add("p_zona", OracleDbType.Varchar2).Value = zona
                    cmd.Parameters.Add("p_colonia", OracleDbType.Varchar2).Value = colonia
                    cmd.Parameters.Add("p_calle", OracleDbType.Varchar2).Value = calle
                    cmd.Parameters.Add("p_avenida", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(avenida), DBNull.Value, avenida)
                    cmd.Parameters.Add("p_numero_casa", OracleDbType.Varchar2).Value = numCasa

                    ' Parámetros de Usuario
                    cmd.Parameters.Add("p_password_hash", OracleDbType.Varchar2).Value = contrasenaHasheada
                    cmd.Parameters.Add("p_token", OracleDbType.Varchar2).Value = tokenActivacion
                    cmd.Parameters.Add("p_pasaporte", OracleDbType.Varchar2).Value = pasaporte.ToUpper()

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If paramOut.Value.ToString() = "EXITO" Then
                        ' Enviamos el correo desde aquí
                        Dim linkActivacion As String = "https://localhost:44356/Account/Activar.aspx?token=" & tokenActivacion
                        Try
                            UtilidadesService.EnviarCorreoActivacion(correo.ToLower(), pNombre, linkActivacion)
                        Catch ex As Exception
                            Return New With {.success = True, .mensaje = "Registrado, pero falló el envío del correo."}
                        End Try

                        Return New With {.success = True, .mensaje = "Registro exitoso."}
                    Else
                        Return New With {.success = False, .mensaje = paramOut.Value.ToString()}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error del sistema: " & ex.Message}
        End Try
    End Function

End Class