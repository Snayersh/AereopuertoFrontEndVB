Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Text

Public Class ReporteAerolineas
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
                Using cmd As New OracleCommand("SP_REPORTE_AEROLINEAS_ACTIVAS", conn)
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

                        Dim jsonLabels As New StringBuilder("[")
                        Dim jsonDatos As New StringBuilder("[")

                        For i As Integer = 0 To dt.Rows.Count - 1
                            Dim row As DataRow = dt.Rows(i)
                            jsonLabels.Append($"""{row("aerolinea")}""")
                            jsonDatos.Append(row("total_vuelos_asignados").ToString())

                            If i < dt.Rows.Count - 1 Then
                                jsonLabels.Append(",")
                                jsonDatos.Append(",")
                            End If
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