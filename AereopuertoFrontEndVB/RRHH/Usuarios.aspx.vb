Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Usuarios
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Validación de Rol 4
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 4) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            pnlEditarRol.Visible = False
            CargarDirectorioUsuarios()
            CargarCatalogoRoles()
        End If
    End Sub

    ' =========================================================
    ' CARGAR LA TABLA DE USUARIOS
    ' =========================================================
    Private Sub CargarDirectorioUsuarios()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_USUARIOS_ROLES", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptUsuarios.DataSource = dt
                        rptUsuarios.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar usuarios: " & ex.Message, False)
        End Try
    End Sub

    ' =========================================================
    ' CARGAR EL COMBOBOX DE ROLES
    ' =========================================================
    Private Sub CargarCatalogoRoles()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_ROLES", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlRoles.DataSource = reader
                        ddlRoles.DataTextField = "nombre"
                        ddlRoles.DataValueField = "id_rol"
                        ddlRoles.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Silencioso
        End Try
    End Sub

    ' =========================================================
    ' LÓGICA DE COLORES Y BOTONES (SIN ERRORES HTML)
    ' =========================================================
    Protected Sub rptUsuarios_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptUsuarios.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then

            ' 1. Badge del Rol
            Dim rolStr As String = DataBinder.Eval(e.Item.DataItem, "nombre_rol").ToString()
            Dim lblBadgeRol As Label = CType(e.Item.FindControl("lblBadgeRol"), Label)
            lblBadgeRol.Text = rolStr

            If rolStr = "Administrador" Then
                lblBadgeRol.CssClass = "badge-admin shadow-sm"
            ElseIf rolStr = "Empleado" Then
                lblBadgeRol.CssClass = "badge-empleado shadow-sm"
            Else
                lblBadgeRol.CssClass = "badge-cliente shadow-sm"
            End If

            ' 2. Badge del Estado
            Dim estadoStr As String = DataBinder.Eval(e.Item.DataItem, "estado").ToString().ToUpper()
            Dim lblBadgeEstado As Label = CType(e.Item.FindControl("lblBadgeEstado"), Label)
            lblBadgeEstado.Text = estadoStr

            If estadoStr = "ACTIVO" Then
                lblBadgeEstado.CssClass = "badge bg-success shadow-sm px-3 py-2"
            Else
                lblBadgeEstado.CssClass = "badge bg-danger shadow-sm px-3 py-2"
            End If

            ' 3. Botón Activar/Desactivar
            Dim btnToggleEstado As LinkButton = CType(e.Item.FindControl("btnToggleEstado"), LinkButton)
            If estadoStr = "ACTIVO" Then
                btnToggleEstado.Text = "🚫 Desactivar"
                btnToggleEstado.CssClass = "btn btn-sm btn-outline-danger fw-bold mb-1 shadow-sm"
                btnToggleEstado.OnClientClick = "return confirm('¿Estás seguro que deseas DESACTIVAR a este usuario?');"
            Else
                btnToggleEstado.Text = "✅ Activar"
                btnToggleEstado.CssClass = "btn btn-sm btn-outline-success fw-bold mb-1 shadow-sm"
                btnToggleEstado.OnClientClick = "return confirm('¿Estás seguro que deseas ACTIVAR a este usuario?');"
            End If
        End If
    End Sub

    ' =========================================================
    ' ACCIONES DE LA TABLA (EDITAR / TOGGLE)
    ' =========================================================
    Protected Sub rptUsuarios_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptUsuarios.ItemCommand
        pnlMensaje.Visible = False

        If e.CommandName = "EditarRol" Then
            Dim datos As String() = e.CommandArgument.ToString().Split("|"c)
            Dim idUsuario As String = datos(0)
            Dim nombre As String = datos(1)
            Dim idRolActual As String = datos(2)

            lblNombreEdicion.Text = nombre
            hfUsuarioEditando.Value = idUsuario

            If ddlRoles.Items.FindByValue(idRolActual) IsNot Nothing Then
                ddlRoles.ClearSelection()
                ddlRoles.Items.FindByValue(idRolActual).Selected = True
            End If

            pnlEditarRol.Visible = True

        ElseIf e.CommandName = "ToggleEstado" Then
            Dim datos As String() = e.CommandArgument.ToString().Split("|"c)
            Dim idUsuario As String = datos(0)
            Dim estadoActual As String = datos(1).ToUpper()
            Dim nuevoEstado As String = If(estadoActual = "ACTIVO", "Inactivo", "Activo")
            CambiarEstadoUsuario(idUsuario, nuevoEstado)
        End If
    End Sub

    Private Sub CambiarEstadoUsuario(idUsuario As String, nuevoEstado As String)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CAMBIAR_ESTADO_USUARIO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_usuario", OracleDbType.Int32).Value = Convert.ToInt32(idUsuario)
                    cmd.Parameters.Add("p_nuevo_estado", OracleDbType.Varchar2).Value = nuevoEstado

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        MostrarMensaje("✅ El estado del usuario se actualizó a: " & nuevoEstado, True)
                        CargarDirectorioUsuarios()
                        pnlEditarRol.Visible = False
                    Else
                        MostrarMensaje("⚠️ Error al cambiar estado: " & resultado, False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error de conexión: " & ex.Message, False)
        End Try
    End Sub

    ' =========================================================
    ' GUARDAR EL CAMBIO DE ROL
    ' =========================================================
    Protected Sub btnGuardarCambios_Click(sender As Object, e As EventArgs) Handles btnGuardarCambios.Click
        Dim idUsuario As String = hfUsuarioEditando.Value
        Dim nuevoIdRol As String = ddlRoles.SelectedValue

        If String.IsNullOrEmpty(idUsuario) OrElse String.IsNullOrEmpty(nuevoIdRol) Then Return

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ACTUALIZAR_ROL_USUARIO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_usuario", OracleDbType.Int32).Value = Convert.ToInt32(idUsuario)
                    cmd.Parameters.Add("p_id_rol", OracleDbType.Int32).Value = Convert.ToInt32(nuevoIdRol)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        pnlEditarRol.Visible = False
                        MostrarMensaje("✅ Rol de usuario actualizado correctamente.", True)
                        CargarDirectorioUsuarios()
                    Else
                        MostrarMensaje("⚠️ Error en base de datos: " & resultado, False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error de conexión: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnCancelarEdicion_Click(sender As Object, e As EventArgs) Handles btnCancelarEdicion.Click
        pnlEditarRol.Visible = False
        pnlMensaje.Visible = False
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm")
    End Sub
End Class