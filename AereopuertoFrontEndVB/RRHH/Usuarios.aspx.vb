Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Usuarios
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 4) Then
            Response.Redirect("~/Account/Login.aspx")
        End If
        If Not IsPostBack Then
            pnlExito.Visible = False
            pnlError.Visible = False
            pnlEditarRol.Visible = False

            ' Cargamos las tablas de forma normal
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
            MostrarError("Error al cargar usuarios: " & ex.Message)
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
            MostrarError("Error al cargar roles: " & ex.Message)
        End Try
    End Sub

    ' =========================================================
    ' CUANDO SE HACE CLIC EN "MODIFICAR ROL" EN LA TABLA
    ' =========================================================
    Protected Sub rptUsuarios_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptUsuarios.ItemCommand
        ' Limpiamos mensajes anteriores
        pnlExito.Visible = False
        pnlError.Visible = False

        ' === SI HIZO CLIC EN EDITAR ROL ===
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

            ' === SI HIZO CLIC EN DESACTIVAR / ACTIVAR ===
        ElseIf e.CommandName = "ToggleEstado" Then
            Dim datos As String() = e.CommandArgument.ToString().Split("|"c)
            Dim idUsuario As String = datos(0)
            Dim estadoActual As String = datos(1).ToUpper()

            ' Lógica de interruptor: Si está activo, lo volvemos inactivo, y viceversa
            Dim nuevoEstado As String = If(estadoActual = "ACTIVO", "Inactivo", "Activo")

            CambiarEstadoUsuario(idUsuario, nuevoEstado)
        End If
    End Sub

    ' Nueva subrutina para comunicarse con Oracle
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
                        MostrarExito("El estado del usuario se actualizó a: " & nuevoEstado)
                        CargarDirectorioUsuarios() ' Refrescamos la tabla
                        pnlEditarRol.Visible = False
                    Else
                        MostrarError("Error al cambiar estado: " & resultado)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarError("Error de conexión al cambiar estado: " & ex.Message)
        End Try
    End Sub

    ' =========================================================
    ' GUARDAR EL CAMBIO EN ORACLE
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
                        MostrarExito("Rol actualizado correctamente.")
                        CargarDirectorioUsuarios() ' Recargamos la tabla para ver el cambio
                    Else
                        MostrarError("Error en base de datos al guardar: " & resultado)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarError("Error de conexión al guardar: " & ex.Message)
        End Try
    End Sub

    Protected Sub btnCancelarEdicion_Click(sender As Object, e As EventArgs) Handles btnCancelarEdicion.Click
        pnlEditarRol.Visible = False
        pnlExito.Visible = False
        pnlError.Visible = False
    End Sub

    Private Sub MostrarError(mensaje As String)
        pnlExito.Visible = False
        pnlError.Visible = True
        lblError.Text = mensaje
    End Sub

    Private Sub MostrarExito(mensaje As String)
        pnlError.Visible = False
        pnlExito.Visible = True
        lblExito.Text = mensaje
    End Sub

End Class