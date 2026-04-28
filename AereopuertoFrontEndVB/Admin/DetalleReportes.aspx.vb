Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class DetalleReportes
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Solo Administradores (Rol 1)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse idRol <> 1 Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarReportesPadre()

            ' ========================================================
            ' ¡MAGIA AQUÍ! Atrapamos el ID que viene de la página padre
            ' ========================================================
            If Request.QueryString("id") IsNot Nothing Then
                Dim idReporteUrl As String = Request.QueryString("id").ToString()

                ' Seleccionamos el reporte en el DropDownList
                If ddlReportes.Items.FindByValue(idReporteUrl) IsNot Nothing Then
                    ddlReportes.SelectedValue = idReporteUrl
                    ' Simulamos el clic del botón para que cargue la tabla de inmediato
                    btnVerDetalles_Click(Nothing, Nothing)
                End If
            End If
        End If
    End Sub

    Private Sub CargarReportesPadre()
        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_REPORTES_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cur As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cur.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cur)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlReportes.DataSource = reader
                        ddlReportes.DataTextField = "INFO_REPORTE"
                        ddlReportes.DataValueField = "ID_REPORTE"
                        ddlReportes.DataBind()
                    End Using
                End Using
            End Using
            ddlReportes.Items.Insert(0, New ListItem("-- Seleccione un Reporte --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar listado de reportes: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnVerDetalles_Click(sender As Object, e As EventArgs) Handles btnVerDetalles.Click
        If String.IsNullOrEmpty(ddlReportes.SelectedValue) Then
            MostrarMensaje("⚠️ Por favor, seleccione un reporte de la lista.", False)
            pnlGestionDetalles.Visible = False
            Return
        End If

        pnlMensaje.Visible = False
        pnlGestionDetalles.Visible = True
        CargarTablaDetalles()
    End Sub

    Private Sub CargarTablaDetalles()
        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_DETALLE_REPORTE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_reporte", OracleDbType.Int32).Value = Convert.ToInt32(ddlReportes.SelectedValue)

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptDetalles.DataSource = dt
                            rptDetalles.DataBind()
                            rptDetalles.Visible = True
                            pnlVacio.Visible = False
                        Else
                            rptDetalles.Visible = False
                            pnlVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar el contenido del reporte: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnGuardarDetalle_Click(sender As Object, e As EventArgs) Handles btnGuardarDetalle.Click
        If String.IsNullOrEmpty(txtContenido.Text) OrElse String.IsNullOrEmpty(ddlReportes.SelectedValue) Then
            MostrarMensaje("⚠️ Escriba el contenido de la línea a anexar.", False)
            Return
        End If

        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_REPORTE_DETALLE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_contenido", OracleDbType.Varchar2).Value = txtContenido.Text.Trim()
                    cmd.Parameters.Add("p_id_reporte", OracleDbType.Int32).Value = Convert.ToInt32(ddlReportes.SelectedValue)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outResultado.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Línea anexada al reporte exitosamente.", True)
                        txtContenido.Text = ""
                        CargarTablaDetalles()
                    Else
                        MostrarMensaje("⚠️ Error en base de datos: " & outResultado.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error interno: " & ex.Message, False)
        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm")
    End Sub
End Class