Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Text

Public Class ReporteObjetos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("UserRole") Is Nothing OrElse Session("UserRole").ToString() <> "Admin" Then
            Response.Redirect("~/Default.aspx")
        End If

        If Not IsPostBack Then
            GenerarReporte()
        End If
    End Sub

    Private Sub GenerarReporte()
        Dim db As New ConexionDBReplica() ' ⚠️ RÉPLICA CONECTADA
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REPORTE_OBJETOS", conn)
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

                        ' Agrupar datos para la gráfica
                        Dim conteoEstados As New Dictionary(Of String, Integer)
                        For Each row As DataRow In dt.Rows
                            Dim estado As String = row("estado_reclamo").ToString()
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