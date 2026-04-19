Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Text

Public Class ReportePasajeros
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
        ' ⚠️ ESTA ES LA MAGIA PARA EL INGENIERO: USAMOS LA RÉPLICA
        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                ' Asumimos que creaste un SP en tu réplica que devuelve los vuelos y su conteo de pasajeros
                Using cmd As New OracleCommand("SP_REPORTE_PASAJEROS_VUELO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        ' 1. Llenamos la tabla HTML
                        rptDatos.DataSource = dt
                        rptDatos.DataBind()

                        ' 2. Preparamos el JSON para la gráfica (Chart.js)
                        Dim jsonLabels As New StringBuilder("[")
                        Dim jsonDatos As New StringBuilder("[")

                        For i As Integer = 0 To dt.Rows.Count - 1
                            Dim row As DataRow = dt.Rows(i)
                            jsonLabels.Append($"""{row("codigo_vuelo")} ({row("aerolinea")})""")
                            jsonDatos.Append(row("total_pasajeros").ToString())

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
            ' Manejo de error
            hfLabels.Value = "[]"
            hfDatos.Value = "[]"
        End Try
    End Sub
End Class