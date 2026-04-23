Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ControlSeguridad
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Operaciones/Seguridad (Rol 3) y Administradores (Rol 1)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 1 AndAlso idRol <> 3) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarTiposRevision()
            CargarHistorial()
        End If
    End Sub

    ' =======================================================
    ' CARGAR SELECTOR DE PUNTOS DE CONTROL
    ' =======================================================
    Private Sub CargarTiposRevision()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_TIPOS_REVISION_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cur As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cur.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cur)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlTipoRevision.DataSource = reader
                        ddlTipoRevision.DataTextField = "NOMBRE"
                        ddlTipoRevision.DataValueField = "ID_TIPO_REVISION"
                        ddlTipoRevision.DataBind()
                    End Using
                End Using
            End Using
            ddlTipoRevision.Items.Insert(0, New ListItem("-- Seleccione Punto --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar puntos de revisión: " & ex.Message, False)
        End Try
    End Sub

    ' =======================================================
    ' REGISTRAR INSPECCIÓN
    ' =======================================================
    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If String.IsNullOrEmpty(txtIdPersona.Text) OrElse String.IsNullOrEmpty(ddlTipoRevision.SelectedValue) OrElse String.IsNullOrEmpty(ddlResultado.SelectedValue) Then
            MostrarMensaje("⚠️ Por favor, complete todos los campos de la inspección.", False)
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_REVISION", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_resultado", OracleDbType.Varchar2).Value = ddlResultado.SelectedValue
                    cmd.Parameters.Add("p_id_persona", OracleDbType.Int32).Value = Convert.ToInt32(txtIdPersona.Text.Trim())
                    cmd.Parameters.Add("p_id_tipo", OracleDbType.Int32).Value = Convert.ToInt32(ddlTipoRevision.SelectedValue)

                    Dim outResultado As New OracleParameter("p_resultado_out", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outResultado.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Registro de seguridad guardado correctamente.", True)
                        txtIdPersona.Text = ""
                        ddlTipoRevision.SelectedIndex = 0
                        ddlResultado.SelectedIndex = 0
                        CargarHistorial() ' Refrescar tabla
                    Else
                        MostrarMensaje("⚠️ Error en base de datos: " & outResultado.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error interno: " & ex.Message, False)
        End Try
    End Sub

    ' =======================================================
    ' CARGAR TABLA HISTÓRICA
    ' =======================================================
    Private Sub CargarHistorial()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_REVISIONES", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptRevisiones.DataSource = dt
                            rptRevisiones.DataBind()
                            rptRevisiones.Visible = True
                            pnlVacio.Visible = False
                        Else
                            rptRevisiones.Visible = False
                            pnlVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar historial: " & ex.Message, False)
        End Try
    End Sub

    ' =======================================================
    ' LÓGICA DE COLORES Y FECHAS SIN ROMPER EL HTML
    ' =======================================================
    Protected Sub rptRevisiones_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptRevisiones.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then

            ' Formatear Fecha
            Dim fechaObj = DataBinder.Eval(e.Item.DataItem, "FECHA")
            Dim lblFecha As Label = CType(e.Item.FindControl("lblFecha"), Label)
            If Not IsDBNull(fechaObj) Then
                lblFecha.Text = Convert.ToDateTime(fechaObj).ToString("dd/MM/yyyy HH:mm")
            End If

            ' Formatear Badge de Resultado
            Dim resultadoStr As String = DataBinder.Eval(e.Item.DataItem, "RESULTADO").ToString().ToUpper()
            Dim lblBadge As Label = CType(e.Item.FindControl("lblBadgeResultado"), Label)

            lblBadge.Text = resultadoStr

            If resultadoStr = "APROBADO" Then
                lblBadge.CssClass = "badge-aprobado shadow-sm"
            ElseIf resultadoStr.Contains("ALERTA") Then
                lblBadge.CssClass = "badge-alerta shadow-sm"
            Else
                lblBadge.CssClass = "badge-revision shadow-sm"
            End If

        End If
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm")
    End Sub
End Class