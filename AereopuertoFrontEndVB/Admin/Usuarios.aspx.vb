Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Usuarios
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' SEGURIDAD NIVEL 1: Si no está logueado o NO es Administrador (Rol 1), lo echamos.
        If Session("UserRole") Is Nothing OrElse Session("UserRole").ToString() <> "1" Then
            Response.Redirect("~/Default.aspx") ' O lo mandas a una página de "Acceso Denegado"
        End If

        If Not IsPostBack Then
            pnlExito.Visible = False
            pnlError.Visible = False
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
        If e.CommandName = "EditarRol" Then
            ' Limpiamos mensajes anteriores
            pnlExito.Visible = False
            pnlError.Visible = False

            ' Recibimos los datos separados por la barra "|" (ID|Nombre|IdRolActual)
            Dim datos As String() = e.CommandArgument.ToString().Split("|"c)
            Dim idUsuario As String = datos(0)
            Dim nombre As String = datos(1)
            Dim idRolActual As String = datos(2)

            ' Preparamos el panel lateral
            lblNombreEdicion.Text = nombre
            hfUsuarioEditando.Value = idUsuario

            ' Seleccionamos el rol actual en el dropdown
            If ddlRoles.Items.FindByValue(idRolActual) IsNot Nothing Then
                ddlRoles.ClearSelection()
                ddlRoles.Items.FindByValue(idRolActual).Selected = True
            End If

            ' Mostramos el panel
            pnlEditarRol.Visible = True
        End If
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