Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ClienteFacturaService

    ''' <summary>
    ''' Obtiene la cabecera y el detalle de una factura específica.
    ''' </summary>
    Public Shared Function ObtenerDetalleFactura(idFactura As Integer) As Object
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                conn.Open()

                Dim objCabecera As Dictionary(Of String, Object) = Nothing
                Dim objDetalles As List(Of Dictionary(Of String, Object)) = Nothing

                ' ==========================================
                ' 1. OBTENER LA CABECERA (Datos Generales)
                ' ==========================================
                Using cmdCabecera As New OracleCommand("SP_FACTURA_CABECERA", conn)
                    cmdCabecera.CommandType = CommandType.StoredProcedure
                    cmdCabecera.BindByName = True
                    cmdCabecera.Parameters.Add("p_id_factura", OracleDbType.Int32).Value = idFactura

                    Dim cursorCabecera As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorCabecera.Direction = ParameterDirection.Output
                    cmdCabecera.Parameters.Add(cursorCabecera)

                    Using da As New OracleDataAdapter(cmdCabecera)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        If dt.Rows.Count > 0 Then
                            ' Usamos nuestra utilidad para pasarlo a formato diccionario (JSON)
                            objCabecera = UtilidadesService.ConvertirDataTableALista(dt)(0)
                        End If
                    End Using
                End Using

                ' Si no existe la cabecera, detenemos el vuelo aquí
                If objCabecera Is Nothing Then
                    Return New With {.success = False, .mensaje = "No se encontró la factura solicitada."}
                End If

                ' ==========================================
                ' 2. OBTENER LOS DETALLES (Boletos/Maletas)
                ' ==========================================
                Using cmdDetalle As New OracleCommand("SP_FACTURA_DETALLE", conn)
                    cmdDetalle.CommandType = CommandType.StoredProcedure
                    cmdDetalle.BindByName = True
                    cmdDetalle.Parameters.Add("p_id_factura", OracleDbType.Int32).Value = idFactura

                    Dim cursorDetalle As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorDetalle.Direction = ParameterDirection.Output
                    cmdDetalle.Parameters.Add(cursorDetalle)

                    Using da As New OracleDataAdapter(cmdDetalle)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        objDetalles = UtilidadesService.ConvertirDataTableALista(dt)
                    End Using
                End Using

                ' Empaquetamos todo el resultado para enviarlo de vuelta
                Return New With {
                    .success = True,
                    .cabecera = objCabecera,
                    .detalles = objDetalles
                }

            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al cargar el detalle: " & ex.Message}
        End Try
    End Function

    ''' <summary>
    ''' Obtiene el historial completo de facturas de un usuario específico.
    ''' </summary>
    Public Shared Function ObtenerMisFacturas(correo As String) As Object
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

                        Return New With {.success = True, .facturas = UtilidadesService.ConvertirDataTableALista(dt)}
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New With {.success = False, .mensaje = "Error al obtener tus facturas: " & ex.Message}
        End Try
    End Function

End Class