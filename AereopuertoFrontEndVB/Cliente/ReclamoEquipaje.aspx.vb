Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ReclamoEquipaje
    Inherits System.Web.UI.Page

    Private CorreoUsuario As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        ' 🔥 SEGURIDAD: Solo Clientes / Pasajeros (Rol 2)
        If Session("UserEmail") Is Nothing OrElse (idRol <> 2) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        CorreoUsuario = Session("UserEmail").ToString()

        If Not IsPostBack Then
            CargarEquipajeCliente()
            CargarHistorialReclamos()
        End If
    End Sub

    ' =======================================================
    ' CARGAR SOLO LAS MALETAS DE ESTE CLIENTE EN EL SELECTOR
    ' =======================================================
    Private Sub CargarEquipajeCliente()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_EQUIPAJE_CLIENTE_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = CorreoUsuario

                    Dim cur As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cur.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cur)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlEquipaje.DataSource = reader
                        ddlEquipaje.DataTextField = "INFO_EQUIPAJE" ' Asegurar que el SP devuelve este alias
                        ddlEquipaje.DataValueField = "ID_EQUIPAJE"
                        ddlEquipaje.DataBind()
                    End Using
                End Using
            End Using
            ddlEquipaje.Items.Insert(0, New ListItem("-- Seleccione la Maleta --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar su equipaje: " & ex.Message, False)
        End Try
    End Sub

    ' =======================================================
    ' GUARDAR EL REPORTE
    ' =======================================================
    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If String.IsNullOrEmpty(ddlEquipaje.SelectedValue) OrElse String.IsNullOrEmpty(txtDescripcion.Text) Then
            MostrarMensaje("⚠️ Por favor, seleccione la maleta y detalle el problema.", False)
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_RECLAMO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = txtDescripcion.Text.Trim()
                    cmd.Parameters.Add("p_id_equipaje", OracleDbType.Int32).Value = Convert.ToInt32(ddlEquipaje.SelectedValue)

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If paramOut.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Reclamo enviado. Nuestro equipo de Servicio al Cliente lo revisará a la brevedad.", True)
                        txtDescripcion.Text = ""
                        ddlEquipaje.SelectedIndex = 0
                        CargarHistorialReclamos() ' Refrescar la tabla
                    Else
                        MostrarMensaje("⚠️ " & paramOut.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error en el servidor: " & ex.Message, False)
        End Try
    End Sub

    ' =======================================================
    ' MOSTRAR HISTORIAL DE RECLAMOS DEL CLIENTE
    ' =======================================================
    Private Sub CargarHistorialReclamos()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_RECLAMOS_CLIENTE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = CorreoUsuario

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptReclamos.DataSource = dt
                            rptReclamos.DataBind()
                            rptReclamos.Visible = True
                            pnlVacio.Visible = False
                        Else
                            rptReclamos.Visible = False
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
    ' LÓGICA DE COLORES PARA ESTADOS EN EL REPEATER
    ' =======================================================
    Protected Sub rptReclamos_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptReclamos.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim estado As String = DataBinder.Eval(e.Item.DataItem, "ESTADO").ToString().ToUpper()
            Dim lblBadge As Label = CType(e.Item.FindControl("lblBadgeEstado"), Label)

            lblBadge.Text = estado

            If estado.Contains("PENDIENTE") OrElse estado.Contains("RECIBIDO") Then
                lblBadge.CssClass = "badge-pendiente shadow-sm"
            ElseIf estado.Contains("INVESTIGACIÓN") OrElse estado.Contains("PROCESO") Then
                lblBadge.CssClass = "badge-investigacion shadow-sm"
            ElseIf estado.Contains("RESUELTO") OrElse estado.Contains("CERRADO") OrElse estado.Contains("ENTREGADO") Then
                lblBadge.CssClass = "badge-resuelto shadow-sm"
            Else
                lblBadge.CssClass = "badge bg-secondary text-white shadow-sm"
            End If
        End If
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger fw-bold rounded-3 mb-4 shadow-sm")
    End Sub
End Class