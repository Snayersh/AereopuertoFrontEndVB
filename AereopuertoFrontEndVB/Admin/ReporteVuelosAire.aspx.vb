Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Text

Public Class ReporteVuelosAire
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 1) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            GenerarReporte()
        End If
    End Sub

    Private Sub GenerarReporte()
        Dim db As New ConexionDBReplica() ' ⚠️ RÉPLICA CONECTADA
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REPORTE_VUELOS_AIRE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        rptDatos.DataSource = dt
                        rptDatos.DataBind()

                        ' Agrupar datos para la gráfica (Cuántos vuelos en el aire tiene cada aerolínea)
                        Dim conteoAerolineas As New Dictionary(Of String, Integer)
                        For Each row As DataRow In dt.Rows
                            Dim aerolinea As String = row("aerolinea").ToString()
                            If conteoAerolineas.ContainsKey(aerolinea) Then
                                conteoAerolineas(aerolinea) += 1
                            Else
                                conteoAerolineas.Add(aerolinea, 1)
                            End If
                        Next

                        Dim jsonLabels As New StringBuilder("[")
                        Dim jsonDatos As New StringBuilder("[")
                        Dim index As Integer = 0

                        For Each kvp In conteoAerolineas
                            jsonLabels.Append($"""{kvp.Key}""")
                            jsonDatos.Append(kvp.Value.ToString())
                            If index < conteoAerolineas.Count - 1 Then
                                jsonLabels.Append(",")
                                jsonDatos.Append(",")
                            End If
                            index += 1
                        Next

                        jsonLabels.Append("]")
                        jsonDatos.Append("]")

                        hfLabels.Value = jsonLabels.ToString()
                        hfDatos.Value = jsonDatos.ToString()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            hfLabels.Value = "[]"
            hfDatos.Value = "[]"
        End Try
    End Sub
End Class