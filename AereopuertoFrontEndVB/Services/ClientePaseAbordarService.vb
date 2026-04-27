Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ClientePaseAbordarService

    ''' <summary>
    ''' Obtiene los pases de abordar (tickets) vinculados a un código de reserva.
    ''' </summary>
    Public Shared Function ObtenerPasesDeAbordar(codigoReserva As String, correo As String) As Object
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_PASE_ABORDAR", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo_reserva", OracleDbType.Varchar2).Value = codigoReserva
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correo

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            ' Convertimos a formato diccionario/JSON para Web y App
                            Return New With {.success = True, .pases = UtilidadesService.ConvertirDataTableALista(dt)}
                        Else
                            Return New With {.success = False, .mensaje = "No se encontró el pase de abordar. Verifica que sea tuyo y esté pagado."}
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al generar el pase: " & ex.Message}
        End Try
    End Function

End Class