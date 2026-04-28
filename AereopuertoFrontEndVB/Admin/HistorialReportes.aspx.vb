Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class HistorialReportes
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Solo Administradores (Rol 1)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse idRol <> 1 Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarHistorial()
        End If
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If String.IsNullOrEmpty(txtNombreReporte.Text) Then
            MostrarMensaje("⚠️ Por favor, ingrese un nombre para el reporte.", False)
            Return
        End If

        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_REPORTE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = txtNombreReporte.Text.Trim()

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outResultado.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Reporte registrado exitosamente en la bitácora.", True)
                        txtNombreReporte.Text = ""
                        CargarHistorial()
                    Else
                        MostrarMensaje("⚠️ " & outResultado.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error interno: " & ex.Message, False)
        End Try
    End Sub

    Private Sub CargarHistorial()
        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_REPORTES", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptReportes.DataSource = dt
                            rptReportes.DataBind()
                            rptReportes.Visible = True
                            pnlVacio.Visible = False
                        Else
                            rptReportes.Visible = False
                            pnlVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar historial: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub rptReportes_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptReportes.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim fechaStr As String = ""
            Dim fechaObj = DataBinder.Eval(e.Item.DataItem, "FECHA")

            If Not IsDBNull(fechaObj) Then
                fechaStr = Convert.ToDateTime(fechaObj).ToString("dd/MM/yyyy HH:mm")
            End If

            Dim lblBadgeFecha As Label = CType(e.Item.FindControl("lblBadgeFecha"), Label)
            lblBadgeFecha.Text = "📅 " & fechaStr
            lblBadgeFecha.CssClass = "badge-date shadow-sm"
        End If
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm")
    End Sub
End Class