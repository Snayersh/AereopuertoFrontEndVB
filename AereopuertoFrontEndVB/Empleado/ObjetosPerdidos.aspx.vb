Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ObjetosPerdidos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Validación de seguridad: Solo Empleados y Admins
        If Session("UserRole") Is Nothing OrElse (Session("UserRole").ToString() <> "Empleado" AndAlso Session("UserRole").ToString() <> "Admin") Then
            Response.Redirect("~/Default.aspx")
        End If

        If Not IsPostBack Then
            ' Magia UX: Al cargar la página por primera vez, mostramos solo lo que estorba en bodega
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
                    cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = txtDescripcion.Text.Trim()
                    cmd.Parameters.Add("p_lugar", OracleDbType.Varchar2).Value = ddlLugar.SelectedValue

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If paramOut.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Objeto registrado en bodega.", True)
                        ' Limpiamos el formulario de arriba
                        txtDescripcion.Text = ""
                        ddlLugar.SelectedIndex = 0

                        ' Refrescamos la tabla manteniendo lo que sea que tengan seleccionado en los filtros
                        CargarObjetos(txtBusqueda.Text.Trim(), ddlFiltroEstado.SelectedValue)
                    Else
                        MostrarMensaje("⚠️ Error: " & paramOut.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error: " & ex.Message, False)
        End Try
    End Sub

    ' --- Buscar y Filtrar Objetos ---
    Protected Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        ' Toma el texto del buscador y el valor del menú desplegable
        CargarObjetos(txtBusqueda.Text.Trim(), ddlFiltroEstado.SelectedValue)
    End Sub

    ' --- Método Principal que se conecta a Oracle ---
    Private Sub CargarObjetos(busqueda As String, estado As String)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_BUSCAR_OBJETOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' 👇 ESTA ES LA LÍNEA MÁGICA QUE FALTABA 👇
                    cmd.BindByName = True

                    ' Inyectamos los filtros
                    cmd.Parameters.Add("p_busqueda", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(busqueda), DBNull.Value, busqueda)
                    cmd.Parameters.Add("p_estado", OracleDbType.Varchar2).Value = estado

                    ' Parámetro de salida (el cursor con la tabla)
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        rptObjetos.DataSource = dt
                        rptObjetos.DataBind()

                        ' Detalle de UX
                        If dt.Rows.Count = 0 Then
                            MostrarMensaje("ℹ️ No hay registros que coincidan con la búsqueda actual.", True)
                        Else
                            pnlMensaje.Visible = False
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error cargando tabla: " & ex.Message, False)
        End Try
    End Sub

    ' --- Entregar Objeto (Botón que está dentro del Repeater) ---
    Protected Sub rptObjetos_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Entregar" Then
            Dim idObjeto As Integer = Convert.ToInt32(e.CommandArgument)

            ' Buscamos la caja de texto que está en la MISMA fila del botón que presionaron
            Dim txtEntregarA As TextBox = CType(e.Item.FindControl("txtEntregarA"), TextBox)
            Dim nombreDueno As String = txtEntregarA.Text.Trim()

            If String.IsNullOrEmpty(nombreDueno) Then
                MostrarMensaje("⚠️ Debes ingresar el nombre de la persona a quien se le entrega el objeto.", False)
                Return
            End If

            Dim db As New ConexionDB()
            Try
                Using conn As OracleConnection = db.ObtenerConexion()
                    Using cmd As New OracleCommand("SP_ENTREGAR_OBJETO", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.Add("p_id_objeto", OracleDbType.Int32).Value = idObjeto
                        cmd.Parameters.Add("p_entregado_a", OracleDbType.Varchar2).Value = nombreDueno

                        Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                        paramOut.Direction = ParameterDirection.Output
                        cmd.Parameters.Add(paramOut)

                        conn.Open()
                        cmd.ExecuteNonQuery()

                        If paramOut.Value.ToString() = "EXITO" Then
                            MostrarMensaje("🎉 Objeto entregado exitosamente.", True)

                            ' Volvemos a cargar la tabla respetando los filtros actuales
                            CargarObjetos(txtBusqueda.Text.Trim(), ddlFiltroEstado.SelectedValue)
                        Else
                            MostrarMensaje("Error: " & paramOut.Value.ToString(), False)
                        End If
                    End Using
                End Using
            Catch ex As Exception
                MostrarMensaje("Error al actualizar: " & ex.Message, False)
            End Try
        End If
    End Sub

    ' --- Función para mostrar alertas en pantalla ---
    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm")
    End Sub
End Class