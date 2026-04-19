Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Text

Public Class ReporteArrestos
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
        Dim db As New ConexionDBReplica() ' ⚠️ SIEMPRE LA RÉPLICA
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REPORTE_ARRESTOS", conn)
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

                        ' Agrupar datos para la gráfica (Contar cuántos hay por Estado)
                        Dim conteoEstados As New Dictionary(Of String, Integer)
                        For Each row As DataRow In dt.Rows
                            Dim estado As String = row("estado").ToString()
                            If conteoEstados.ContainsKey(estado) Then
                                conteoEstados(estado) += 1
                            Else
                                conteoEstados.Add(estado, 1)
                            End If
                        Next

                        Dim jsonLabels As New StringBuilder("[")
                        Dim jsonDatos As New StringBuilder("[")
                        Dim index As Integer = 0

                        For Each kvp In conteoEstados
                            jsonLabels.Append($"""{kvp.Key}""")
                            jsonDatos.Append(kvp.Value.ToString())
                            If index < conteoEstados.Count - 1 Then
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