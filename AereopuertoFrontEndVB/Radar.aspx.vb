Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Text

Public Class Radar
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Opcional: Proteger la página
        ' If Session("UserRole") Is Nothing Then Response.Redirect("~/Default.aspx")

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

                        ' Convertir la tabla de Oracle a un formato JSON manual
                        Dim jsonBuilder As New StringBuilder()
                        jsonBuilder.Append("[")

                        For i As Integer = 0 To dt.Rows.Count - 1
                            Dim row As DataRow = dt.Rows(i)
                            jsonBuilder.Append("{")

                            ' --- AGREGAMOS EL ID DEL VUELO AQUÍ ---
                            jsonBuilder.Append($"""id_vuelo"": {row("id_vuelo")},")

                            jsonBuilder.Append($"""codigo_vuelo"": ""{row("codigo_vuelo")}"",")
                            jsonBuilder.Append($"""origen_iata"": ""{row("origen_iata")}"",")
                            jsonBuilder.Append($"""destino_iata"": ""{row("destino_iata")}"",")

                            ' Reemplazar la coma decimal por punto para que JavaScript lo entienda bien
                            jsonBuilder.Append($"""orig_lat"": {row("orig_lat").ToString().Replace(",", ".")},")
                            jsonBuilder.Append($"""orig_lng"": {row("orig_lng").ToString().Replace(",", ".")},")
                            jsonBuilder.Append($"""dest_lat"": {row("dest_lat").ToString().Replace(",", ".")},")
                            jsonBuilder.Append($"""dest_lng"": {row("dest_lng").ToString().Replace(",", ".")},")

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

                        ' Enviamos la cadena JSON al HTML
                        hfDatosVuelos.Value = jsonBuilder.ToString()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Manejo de error silencioso para el mapa
            hfDatosVuelos.Value = "[]"
        End Try
    End Sub
End Class