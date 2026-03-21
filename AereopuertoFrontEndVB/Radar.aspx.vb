Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Text

Public Class Radar
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CargarVuelosParaRadar()
        End If
    End Sub

    Private Sub CargarVuelosParaRadar()
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

                        Dim jsonBuilder As New StringBuilder()
                        jsonBuilder.Append("[")

                        For i As Integer = 0 To dt.Rows.Count - 1
                            Dim row As DataRow = dt.Rows(i)
                            jsonBuilder.Append("{")

                            ' 👇 AQUÍ ESTÁ LA MAGIA: Ahora sí pasamos el ID numérico real
                            jsonBuilder.Append($"""id_vuelo"": {row("id_vuelo")},")

                            jsonBuilder.Append($"""codigo_vuelo"": ""{row("codigo_vuelo")}"",")
                            jsonBuilder.Append($"""origen_iata"": ""{row("origen_iata")}"",")
                            jsonBuilder.Append($"""destino_iata"": ""{row("destino_iata")}"",")

                            jsonBuilder.Append($"""orig_lat"": {row("orig_lat").ToString().Replace(",", ".")},")
                            jsonBuilder.Append($"""orig_lng"": {row("orig_lng").ToString().Replace(",", ".")},")
                            jsonBuilder.Append($"""dest_lat"": {row("dest_lat").ToString().Replace(",", ".")},")
                            jsonBuilder.Append($"""dest_lng"": {row("dest_lng").ToString().Replace(",", ".")},")

                            If IsDBNull(row("escala_lat")) Then
                                jsonBuilder.Append("""escala_iata"": null,")
                                jsonBuilder.Append("""escala_lat"": null,")
                                jsonBuilder.Append("""escala_lng"": null,")
                            Else
                                jsonBuilder.Append($"""escala_iata"": ""{row("escala_iata")}"",")
                                jsonBuilder.Append($"""escala_lat"": {row("escala_lat").ToString().Replace(",", ".")},")
                                jsonBuilder.Append($"""escala_lng"": {row("escala_lng").ToString().Replace(",", ".")},")
                            End If

                            jsonBuilder.Append($"""fecha_salida"": ""{row("fecha_salida")}"",")
                            jsonBuilder.Append($"""fecha_llegada"": ""{row("fecha_llegada")}"",")
                            jsonBuilder.Append($"""id_estado_vuelo"": {row("id_estado_vuelo")}")

                            If i < dt.Rows.Count - 1 Then
                                jsonBuilder.Append("},")
                            Else
                                jsonBuilder.Append("}")
                            End If
                        Next

                        jsonBuilder.Append("]")
                        hfDatosVuelos.Value = jsonBuilder.ToString()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            hfDatosVuelos.Value = "[]"
        End Try
    End Sub
End Class