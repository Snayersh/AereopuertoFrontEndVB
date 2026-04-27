Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ClientePagoService

    ''' <summary>
    ''' Procesa el pago de una reserva en la base de datos y devuelve el número de factura formateado.
    ''' </summary>
    Public Shared Function ProcesarPago(codigoReserva As String, correo As String, idMetodoPago As Integer) As Object
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_PROCESAR_PAGO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo_reserva", OracleDbType.Varchar2).Value = codigoReserva
                    cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = correo
                    cmd.Parameters.Add("p_id_metodo_pago", OracleDbType.Int32).Value = idMetodoPago

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    ' Leemos la respuesta bruta de Oracle (Ej: "EXITO|123")
                    Dim resultadoCompleto As String = outResultado.Value.ToString()
                    Dim partes() As String = resultadoCompleto.Split("|"c)

                    If partes(0) = "EXITO" Then
                        ' Formateamos la factura directamente en el servicio.
                        ' Así la App Móvil también recibe el "FAC-000123" ya listo para mostrar en pantalla.
                        Dim numFactura As String = "FAC-" & partes(1).PadLeft(6, "0"c)

                        Return New With {.success = True, .factura = numFactura}
                    ElseIf partes(0) = "ERROR_RESERVA_NO_VALIDA" Then
                        Return New With {.success = False, .mensaje = "No se encontró una reserva pendiente de pago con ese código o ya fue pagada."}
                    Else
                        Return New With {.success = False, .mensaje = "Error en base de datos: " & resultadoCompleto}
                    End If
                End Using
            End Using

        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error interno al procesar el pago: " & ex.Message}
        End Try
    End Function

End Class