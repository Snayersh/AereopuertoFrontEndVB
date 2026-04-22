Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ObjetosPerdidos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Solo Admins (1) o Empleados/Operaciones (3)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 1 AndAlso idRol <> 3) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            ' Cargar inicial: Solo los que estorban en bodega
            CargarObjetos("", "EN BODEGA")
        End If
    End Sub

    ' --- Guardar Nuevo Objeto ---
    Protected Sub btnGuardarObjeto_Click(sender As Object, e As EventArgs) Handles btnGuardarObjeto.Click
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_OBJETO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = txtDescripcion.Text.Trim()
                    cmd.Parameters.Add("p_lugar", OracleDbType.Varchar2).Value = ddlLugar.SelectedValue

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If paramOut.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Objeto registrado e ingresado a bodega correctamente.", True)
                        txtDescripcion.Text = ""
                        ddlLugar.SelectedIndex = 0
                        CargarObjetos(txtBusqueda.Text.Trim(), ddlFiltroEstado.SelectedValue)
                    Else
                        MostrarMensaje("⚠️ Error en base de datos: " & paramOut.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error interno: " & ex.Message, False)
        End Try
    End Sub

    ' --- Buscar y Filtrar ---
    Protected Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        CargarObjetos(txtBusqueda.Text.Trim(), ddlFiltroEstado.SelectedValue)
    End Sub

    ' --- Cargar Tabla (Lógica Principal) ---
    Private Sub CargarObjetos(busqueda As String, estado As String)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_BUSCAR_OBJETOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_busqueda", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(busqueda), DBNull.Value, busqueda)
                    cmd.Parameters.Add("p_estado", OracleDbType.Varchar2).Value = estado

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptObjetos.DataSource = dt
                            rptObjetos.DataBind()
                            rptObjetos.Visible = True
                            pnlVacio.Visible = False
                        Else
                            rptObjetos.Visible = False
                            pnlVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error cargando el inventario: " & ex.Message, False)
        End Try
    End Sub

    ' =================================================================
    ' EVENTO: Control Visual Fila por Fila (Paneles y Colores)
    ' =================================================================
    Protected Sub rptObjetos_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptObjetos.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then

            Dim estadoStr As String = DataBinder.Eval(e.Item.DataItem, "estado_reclamo").ToString().ToUpper()

            ' 1. Manejo del Badge (Color)
            Dim lblBadgeEstado As Label = CType(e.Item.FindControl("lblBadgeEstado"), Label)
            lblBadgeEstado.Text = estadoStr

            If estadoStr = "EN BODEGA" Then
                lblBadgeEstado.CssClass = "badge bg-warning text-dark shadow-sm px-2 py-1"
            ElseIf estadoStr = "RECLAMADO" Then
                lblBadgeEstado.CssClass = "badge bg-success shadow-sm px-2 py-1"
            Else
                lblBadgeEstado.CssClass = "badge bg-secondary shadow-sm px-2 py-1"
            End If

            ' 2. Manejo de Paneles (Visibilidad)
            Dim pnlEntregar As Panel = CType(e.Item.FindControl("pnlEntregar"), Panel)
            Dim pnlEntregado As Panel = CType(e.Item.FindControl("pnlEntregado"), Panel)

            If estadoStr = "EN BODEGA" Then
                pnlEntregar.Visible = True
                pnlEntregado.Visible = False
            Else
                pnlEntregar.Visible = False
                pnlEntregado.Visible = True
            End If
        End If
    End Sub

    ' --- Procesar Entrega ---
    Protected Sub rptObjetos_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Entregar" Then
            Dim idObjeto As Integer = Convert.ToInt32(e.CommandArgument)
            Dim txtEntregarA As TextBox = CType(e.Item.FindControl("txtEntregarA"), TextBox)
            Dim nombreDueno As String = txtEntregarA.Text.Trim()

            If String.IsNullOrEmpty(nombreDueno) Then
                MostrarMensaje("⚠️ Por favor, ingrese el nombre de la persona que reclama el objeto.", False)
                Return
            End If

            Dim db As New ConexionDB()
            Try
                Using conn As OracleConnection = db.ObtenerConexion()
                    Using cmd As New OracleCommand("SP_ENTREGAR_OBJETO", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.BindByName = True
                        cmd.Parameters.Add("p_id_objeto", OracleDbType.Int32).Value = idObjeto
                        cmd.Parameters.Add("p_entregado_a", OracleDbType.Varchar2).Value = nombreDueno

                        Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                        paramOut.Direction = ParameterDirection.Output
                        cmd.Parameters.Add(paramOut)

                        conn.Open()
                        cmd.ExecuteNonQuery()

                        If paramOut.Value.ToString() = "EXITO" Then
                            MostrarMensaje("🎉 Objeto marcado como entregado exitosamente.", True)
                            CargarObjetos(txtBusqueda.Text.Trim(), ddlFiltroEstado.SelectedValue)
                        Else
                            MostrarMensaje("Error al actualizar: " & paramOut.Value.ToString(), False)
                        End If
                    End Using
                End Using
            Catch ex As Exception
                MostrarMensaje("Error de sistema al entregar: " & ex.Message, False)
            End Try
        End If
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm")
    End Sub
End Class