Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Collections.Generic
Imports System.Security.Cryptography
Imports System.Text
Imports System.Net.Mail
Imports System.Net

' ====================================================================
' UTILIDADES (Funciones compartidas para ahorrar código)
' ====================================================================
Public Class UtilidadesAPI
    ' Encripta la contraseña usando la misma lógica de tu página web
    Public Shared Function EncriptarSHA256(texto As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto))
            Dim builder As New StringBuilder()
            For i As Integer = 0 To bytes.Length - 1
                builder.Append(bytes(i).ToString("x2"))
            Next
            Return builder.ToString()
        End Using
    End Function

    ' Convierte cualquier DataTable de Oracle a una Lista de Diccionarios (JSON Perfecto)
    Public Shared Function ConvertirDataTableALista(dt As DataTable) As List(Of Dictionary(Of String, Object))
        Dim filas As New List(Of Dictionary(Of String, Object))
        For Each row As DataRow In dt.Rows
            Dim dict As New Dictionary(Of String, Object)
            For Each col As DataColumn In dt.Columns
                Dim valor = row(col)
                ' 🔥 TRUCO 1: Convertimos la columna a minúsculas para que React Native no sufra
                Dim nombreColumna = col.ColumnName.ToLower()

                If TypeOf valor Is DateTime Then
                    ' 🔥 TRUCO 2: Formato ISO perfecto para evitar el "Invalid Date"
                    dict(nombreColumna) = DirectCast(valor, DateTime).ToString("yyyy-MM-ddTHH:mm:ss")
                ElseIf IsDBNull(valor) Then
                    dict(nombreColumna) = Nothing
                Else
                    dict(nombreColumna) = valor
                End If
            Next
            filas.Add(dict)
        Next
        Return filas
    End Function
End Class


' ==========================================
' SERVICIO DE AUTENTICACIÓN Y PERFIL
' ==========================================
Public Class AuthService
    Public Shared Function ValidarUsuario(correo As String, passwordPlana As String) As Object
        Dim db As New ConexionDB()
        Dim passHasheada As String = UtilidadesAPI.EncriptarSHA256(passwordPlana)

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_VALIDAR_LOGIN", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' 🔥 IMPORTANTE: Aquí QUITAMOS el BindByName = True para que funcione por posición exacto como en tu Web.
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo.ToLower()
                    cmd.Parameters.Add("p_password_hash", OracleDbType.Varchar2).Value = passHasheada

                    Dim outRol As New OracleParameter("p_id_rol", OracleDbType.Int32)
                    outRol.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outRol)

                    Dim outNombre As New OracleParameter("p_nombre_completo", OracleDbType.Varchar2, 200)
                    outNombre.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outNombre)

                    ' 🔥 NUEVO: Parámetro para atrapar el Token de Sesión Única
                    Dim outToken As New OracleParameter("p_token_sesion", OracleDbType.Varchar2, 255)
                    outToken.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outToken)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        ' 🔥 TRAMPA PARA ORACLE: Limpiamos por si devuelve el objeto nulo
                        Dim tokenGenerado As String = outToken.Value.ToString()
                        If tokenGenerado.ToLower() = "null" Then tokenGenerado = ""

                        Return New With {
        .success = True,
        .token_sesion = tokenGenerado,
        .usuario = New With {
            .nombre = outNombre.Value.ToString(),
            .correo = correo,
            .rol = Convert.ToInt32(outRol.Value.ToString())
        }
    }
                    ElseIf resultado = "BLOQUEO_TEMPORAL" Then
                        Return New With {.success = False, .mensaje = "Demasiados intentos fallidos. Tu cuenta ha sido bloqueada temporalmente por 1 minuto por seguridad."}
                    ElseIf resultado = "CUENTA_PENDIENTE" Then
                        Return New With {.success = False, .mensaje = "Tu cuenta aún no está activada."}
                    ElseIf resultado = "CUENTA_INACTIVA" Then
                        Return New With {.success = False, .mensaje = "Tu cuenta ha sido desactivada."}
                    ElseIf resultado = "CREDENCIALES_INVALIDAS" Then
                        Return New With {.success = False, .mensaje = "Correo o contraseña incorrectos."}
                    Else
                        Return New With {.success = False, .mensaje = resultado}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error de BD: " & ex.Message}
        End Try
    End Function

    ' ... (El resto de funciones como RegistrarUsuario y RecuperarPassword siguen igualitas abajo)

    ' 🔥 ACTUALIZADO: Agregamos el parámetro fechaNacimiento
    Public Shared Function RegistrarUsuario(pNombre As String, pApellido As String, pasaporte As String, telefono As String, pais As String, correo As String, password As String, fechaNacimiento As String) As Object
        Dim db As New ConexionDB()
        Dim contrasenaHasheada As String = UtilidadesAPI.EncriptarSHA256(password)
        Dim tokenActivacion As String = Guid.NewGuid().ToString()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_CLIENTE_COMPLETO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    ' Mapeamos los campos que vienen del celular
                    cmd.Parameters.Add("p_primer_nombre", OracleDbType.Varchar2).Value = pNombre
                    cmd.Parameters.Add("p_segundo_nombre", OracleDbType.Varchar2).Value = ""
                    cmd.Parameters.Add("p_tercer_nombre", OracleDbType.Varchar2).Value = ""
                    cmd.Parameters.Add("p_primer_apellido", OracleDbType.Varchar2).Value = pApellido
                    cmd.Parameters.Add("p_segundo_apellido", OracleDbType.Varchar2).Value = ""
                    cmd.Parameters.Add("p_apellido_casada", OracleDbType.Varchar2).Value = ""

                    ' 🔥 Convertimos el string "YYYY-MM-DD" a una fecha real en Oracle
                    cmd.Parameters.Add("p_fecha_nacimiento", OracleDbType.Date).Value = Convert.ToDateTime(fechaNacimiento)

                    cmd.Parameters.Add("p_telefono", OracleDbType.Varchar2).Value = telefono
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo.ToLower()

                    cmd.Parameters.Add("p_pais", OracleDbType.Varchar2).Value = pais
                    cmd.Parameters.Add("p_departamento", OracleDbType.Varchar2).Value = ""
                    cmd.Parameters.Add("p_municipio", OracleDbType.Varchar2).Value = ""
                    cmd.Parameters.Add("p_zona", OracleDbType.Varchar2).Value = ""
                    cmd.Parameters.Add("p_colonia", OracleDbType.Varchar2).Value = ""
                    cmd.Parameters.Add("p_calle", OracleDbType.Varchar2).Value = ""
                    cmd.Parameters.Add("p_avenida", OracleDbType.Varchar2).Value = DBNull.Value
                    cmd.Parameters.Add("p_numero_casa", OracleDbType.Varchar2).Value = ""

                    cmd.Parameters.Add("p_password_hash", OracleDbType.Varchar2).Value = contrasenaHasheada
                    cmd.Parameters.Add("p_token", OracleDbType.Varchar2).Value = tokenActivacion
                    cmd.Parameters.Add("p_pasaporte", OracleDbType.Varchar2).Value = pasaporte.ToUpper()

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If paramOut.Value.ToString() = "EXITO" Then
                        Dim linkActivacion As String = "https://localhost:44356/Account/Activar.aspx?token=" & tokenActivacion

                        Try
                            ServicioCorreo.EnviarCorreoActivacion(correo, pNombre, linkActivacion)
                        Catch exCorreo As Exception
                            Return New With {.success = True, .mensaje = "Usuario registrado, pero hubo un problema enviando el correo de activación."}
                        End Try

                        Return New With {.success = True, .mensaje = "Usuario registrado correctamente. Revisa tu correo para activar la cuenta."}
                    Else
                        Return New With {.success = False, .mensaje = paramOut.Value.ToString()}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al registrar: " & ex.Message}
        End Try
    End Function
    Public Shared Function RecuperarPassword(correo As String) As Object
        Dim db As New ConexionDB()
        Dim tokenRecuperacion As String = Guid.NewGuid().ToString()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_GENERAR_TOKEN_RECUPERACION", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo
                    cmd.Parameters.Add("p_token", OracleDbType.Varchar2).Value = tokenRecuperacion

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    ' Por seguridad, siempre devolvemos éxito para evitar escaneo de correos
                    Return New With {.success = True, .mensaje = "Si el correo existe, te hemos enviado un enlace para recuperar tu contraseña."}
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error del sistema: " & ex.Message}
        End Try
    End Function

    ' Ponla dentro de la clase AuthService, justo debajo de ValidarUsuario por ejemplo:
    Public Shared Function ValidarToken(correo As String, token As String) As String
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_VALIDAR_TOKEN", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' 🔥 QUITAMOS EL BindByName = True igual que en el Login
                    ' cmd.BindByName = True 

                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo.ToLower()
                    cmd.Parameters.Add("p_token_sesion", OracleDbType.Varchar2).Value = token

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Return outResultado.Value.ToString()
                End Using
            End Using
        Catch ex As Exception
            ' 🔥 Ahora nos dirá exactamente por qué falló la base de datos
            Return "ERROR_DB: " & ex.Message
        End Try
    End Function
End Class

' ==========================================
' SERVICIO DE CLIENTE
' ==========================================
Public Class ClienteService
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
                            Return New With {.success = True, .perfil = UtilidadesAPI.ConvertirDataTableALista(dt)(0)}
                        End If
                    End Using
                End Using
            End Using
            Return New With {.success = False, .mensaje = "Perfil no encontrado."}
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function

    Public Shared Function ActualizarPerfil(correo As String, pNombre As String, pApellido As String, telefono As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ACTUALIZAR_PERFIL", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo
                    cmd.Parameters.Add("p_primer_nombre", OracleDbType.Varchar2).Value = pNombre
                    cmd.Parameters.Add("p_segundo_nombre", OracleDbType.Varchar2).Value = ""
                    cmd.Parameters.Add("p_primer_apellido", OracleDbType.Varchar2).Value = pApellido
                    cmd.Parameters.Add("p_segundo_apellido", OracleDbType.Varchar2).Value = ""
                    cmd.Parameters.Add("p_telefono", OracleDbType.Varchar2).Value = telefono

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outResultado.Value.ToString() = "EXITO" Then
                        Return New With {.success = True, .mensaje = "Perfil actualizado"}
                    Else
                        Return New With {.success = False, .mensaje = outResultado.Value.ToString()}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function
End Class

' ==========================================
' SERVICIO DE VUELOS
' ==========================================
Public Class VueloService

    Public Shared Function ListarVuelosActivos() As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_VUELOS_ACTIVOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .vuelos = UtilidadesAPI.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function

    Public Shared Function ListarClases() As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_CLASES_BOLETO", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .clases = UtilidadesAPI.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function

    Public Shared Function ObtenerMapaAsientos(idVuelo As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_MAPA_ASIENTOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = Convert.ToInt32(idVuelo)

                    Dim outCapacidad As New OracleParameter("p_capacidad", OracleDbType.Int32)
                    outCapacidad.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outCapacidad)

                    Dim cursorParam As New OracleParameter("p_cursor_ocupados", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        Dim ocupados As New List(Of String)
                        For Each row As DataRow In dt.Rows
                            ocupados.Add(row("numero").ToString())
                        Next

                        Return New With {
                            .success = True,
                            .capacidad = Convert.ToInt32(outCapacidad.Value.ToString()),
                            .ocupados = ocupados
                        }
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function
End Class
' ==========================================
' SERVICIO DE RESERVAS
' ==========================================
Public Class ReservaService
    Public Shared Function CrearReservaMasiva(correo As String, idVuelo As String, asientos As Object) As Object
        Dim db As New ConexionDB()
        Dim codigoReservaUnico As String = "B-" & Guid.NewGuid().ToString().Substring(0, 5).ToUpper()
        Dim errores As New List(Of String)

        Try
            ' 🔥 CORRECCIÓN 1: Casteamos el JSON a ArrayList, que es lo que realmente manda React
            Dim listaAsientos As ArrayList = CType(asientos, ArrayList)

            Using conn As OracleConnection = db.ObtenerConexion()
                conn.Open()

                ' Recorremos el ArrayList
                For Each objItem In listaAsientos
                    Dim item As Dictionary(Of String, Object) = CType(objItem, Dictionary(Of String, Object))

                    ' 🔥 CORRECCIÓN 2: Leemos "asiento" en lugar de "numero"
                    Dim asientoLimpio As String = item("asiento").ToString()
                    Dim idClaseElegida As Integer = Convert.ToInt32(item("id_clase"))

                    Using cmd As New OracleCommand("SP_RESERVAR_BOLETO", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.BindByName = True

                        cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = correo
                        cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = Convert.ToInt32(idVuelo)
                        cmd.Parameters.Add("p_id_tipo_boleto", OracleDbType.Int32).Value = idClaseElegida
                        cmd.Parameters.Add("p_asiento", OracleDbType.Varchar2).Value = asientoLimpio
                        cmd.Parameters.Add("p_codigo_reserva", OracleDbType.Varchar2).Value = codigoReservaUnico

                        Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                        outResultado.Direction = ParameterDirection.Output
                        cmd.Parameters.Add(outResultado)

                        cmd.ExecuteNonQuery()

                        If outResultado.Value.ToString() <> "EXITO" Then
                            errores.Add($"Asiento {asientoLimpio}: {outResultado.Value.ToString()}")
                        End If
                    End Using
                Next
            End Using

            If errores.Count = 0 Then
                Return New With {.success = True, .codigo_reserva = codigoReservaUnico}
            Else
                Return New With {.success = False, .mensaje = String.Join(" | ", errores)}
            End If
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function

    Public Shared Function ObtenerMisBoletos(correo As String, estado As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CONSULTAR_MIS_BOLETOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo
                    cmd.Parameters.Add("p_id_estado_filtro", OracleDbType.Int32).Value = Convert.ToInt32(estado)

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .boletos = UtilidadesAPI.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function

    Public Shared Function CancelarReserva(codigoReserva As String, correo As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CANCELAR_RESERVA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_codigo_reserva", OracleDbType.Varchar2).Value = codigoReserva
                    cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = correo

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outResultado.Value.ToString() = "EXITO" Then
                        Return New With {.success = True, .mensaje = "Reserva cancelada exitosamente."}
                    Else
                        Return New With {.success = False, .mensaje = outResultado.Value.ToString()}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function
End Class

' ==========================================
' SERVICIO DE PAGOS Y FACTURAS
' ==========================================
Public Class PagoService
    Public Shared Function ProcesarPago(codigoReserva As String, correo As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_PROCESAR_PAGO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_codigo_reserva", OracleDbType.Varchar2).Value = codigoReserva
                    cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = correo
                    cmd.Parameters.Add("p_id_metodo_pago", OracleDbType.Int32).Value = 1

                    Dim paramRes As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramRes.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramRes)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultadoBD As String = paramRes.Value.ToString()
                    Dim partes = resultadoBD.Split("|"c)

                    If partes(0) = "EXITO" Then
                        Dim numFactura = "FAC-" & partes(1).PadLeft(6, "0"c)
                        Return New With {.success = True, .factura = numFactura, .mensaje = "Pago procesado"}
                    Else
                        Return New With {.success = False, .mensaje = resultadoBD}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function

    Public Shared Function ListarFacturas(correo As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CONSULTAR_MIS_FACTURAS", conn)
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
                        Return New With {.success = True, .facturas = UtilidadesAPI.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function

    Public Shared Function ObtenerDetalleFactura(idFactura As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                conn.Open()

                ' Cabecera
                Dim objCabecera As Dictionary(Of String, Object) = Nothing
                Using cmdCabecera As New OracleCommand("SP_FACTURA_CABECERA", conn)
                    cmdCabecera.CommandType = CommandType.StoredProcedure
                    cmdCabecera.BindByName = True
                    cmdCabecera.Parameters.Add("p_id_factura", OracleDbType.Int32).Value = Convert.ToInt32(idFactura)

                    Dim cursorCabecera As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorCabecera.Direction = ParameterDirection.Output
                    cmdCabecera.Parameters.Add(cursorCabecera)

                    Using da As New OracleDataAdapter(cmdCabecera)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        If dt.Rows.Count > 0 Then
                            objCabecera = UtilidadesAPI.ConvertirDataTableALista(dt)(0)
                        End If
                    End Using
                End Using

                ' Detalle
                Dim objDetalles As List(Of Dictionary(Of String, Object)) = Nothing
                Using cmdDetalle As New OracleCommand("SP_FACTURA_DETALLE", conn)
                    cmdDetalle.CommandType = CommandType.StoredProcedure
                    cmdDetalle.BindByName = True
                    cmdDetalle.Parameters.Add("p_id_factura", OracleDbType.Int32).Value = Convert.ToInt32(idFactura)

                    Dim cursorDetalle As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorDetalle.Direction = ParameterDirection.Output
                    cmdDetalle.Parameters.Add(cursorDetalle)

                    Using da As New OracleDataAdapter(cmdDetalle)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        objDetalles = UtilidadesAPI.ConvertirDataTableALista(dt)
                    End Using
                End Using

                Return New With {.success = True, .cabecera = objCabecera, .detalles = objDetalles}
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function
End Class


' ==========================================
' SERVICIO DE EQUIPAJE (API MÓVIL)
' ==========================================
Public Class EquipajeService
    Public Shared Function ObtenerBoletosPagados(correo As String) As Object
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
                        Return New With {.success = True, .boletos = UtilidadesAPI.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function

    Public Shared Function ListarMaletasPorBoleto(codigoBoleto As String) As Object
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
                        Return New With {.success = True, .equipaje = UtilidadesAPI.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function

    ' 🔥 ACTUALIZADO: Ahora recibe el tipoEquipaje y maneja los recargos
    Public Shared Function RegistrarMaleta(codigoBoleto As String, peso As String, desc As String, tipoEquipaje As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_EQUIPAJE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo_boleto", OracleDbType.Varchar2).Value = codigoBoleto
                    cmd.Parameters.Add("p_peso", OracleDbType.Decimal).Value = Convert.ToDecimal(peso)
                    cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = desc
                    ' Nuevo parámetro requerido por la BD
                    cmd.Parameters.Add("p_id_tipo_equipaje", OracleDbType.Int32).Value = Convert.ToInt32(tipoEquipaje)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    ' Manejo de respuesta con posible cobro extra
                    If resultado.StartsWith("EXITO") Then
                        Dim partes = resultado.Split("|"c)
                        Dim mensajeFinal As String = "Maleta registrada exitosamente."

                        If partes.Length > 1 Then
                            mensajeFinal = partes(1) ' Aquí viene el texto "Tienes un recargo de Q..."
                        End If

                        Return New With {.success = True, .mensaje = mensajeFinal}
                    Else
                        Return New With {.success = False, .mensaje = resultado}
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error en API: " & ex.Message}
        End Try
    End Function
End Class
' ==========================================
' SERVICIO DE RADAR Y DASHBOARD
' ==========================================
' ==========================================
' SERVICIO DE RADAR Y DASHBOARD
' ==========================================
Public Class RadarService

    ' 1. Esta es para la pantalla de INICIO (Trae los textos de estados, aerolíneas, etc.)
    Public Shared Function ObtenerVuelosEnVivo() As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_VUELOS_DASHBOARD", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .vuelos = UtilidadesAPI.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function

    ' 2. Esta es NUEVA y es solo para el MAPA interactivo (Trae coordenadas)
    Public Shared Function ObtenerVuelosMapa() As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_RADAR", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .vuelos = UtilidadesAPI.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function

    ' === TUS OTRAS DOS FUNCIONES INTACTAS ===
    Public Shared Function ObtenerEstadisticasDashboard() As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ESTADISTICAS_DASHBOARD", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim outActivos As New OracleParameter("p_activos", OracleDbType.Int32)
                    outActivos.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outActivos)
                    Dim outLlegadas As New OracleParameter("p_llegadas", OracleDbType.Int32)
                    outLlegadas.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outLlegadas)
                    Dim outSalidas As New OracleParameter("p_salidas", OracleDbType.Int32)
                    outSalidas.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outSalidas)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Return New With {
                        .success = True,
                        .data = New With {
                            .activos = Convert.ToInt32(outActivos.Value.ToString()),
                            .llegadas = Convert.ToInt32(outLlegadas.Value.ToString()),
                            .salidas = Convert.ToInt32(outSalidas.Value.ToString())
                        }
                    }
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function

    Public Shared Function ObtenerDetalleVueloEspecifico(idVuelo As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_DETALLE_VUELO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = Convert.ToInt32(idVuelo)

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        If dt.Rows.Count > 0 Then
                            Return New With {.success = True, .vuelo = UtilidadesAPI.ConvertirDataTableALista(dt)(0)}
                        Else
                            Return New With {.success = False, .mensaje = "Vuelo no encontrado"}
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function
End Class

' ==========================================
' SERVICIO DE OPERACIONES
' ==========================================
Public Class OperacionesService
    Public Shared Function ObtenerPaseAbordar(codigo As String, correo As String) As Object
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_PASE_ABORDAR", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_codigo_reserva", OracleDbType.Varchar2).Value = codigo
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return New With {.success = True, .pases = UtilidadesAPI.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = ex.Message}
        End Try
    End Function

End Class