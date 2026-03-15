Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ControlVuelos
    Inherits System.Web.UI.Page

    ' Vamos a guardar la lista de estados en la memoria de la página para no ir a Oracle por cada fila
    Private dtEstados As DataTable

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Seguridad: Validamos que sea un administrador o empleado
        If Session("UserRole") Is Nothing OrElse (Session("UserRole").ToString() <> "Admin" AndAlso Session("UserRole").ToString() <> "Empleado") Then
            Response.Redirect("~/Default.aspx")
        End If

        If Not IsPostBack Then
            CargarCatalogoEstados()
            CargarVuelosControl()
        End If
    End Sub

    ' 1. Cargamos los estados posibles (Programado, En Vuelo, Retrasado...)
    Private Sub CargarCatalogoEstados()
        Dim db As New ConexionDB()
        dtEstados = New DataTable()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Dim query As String = "SELECT id_estado_vuelo, nombre FROM AUR_ESTADO_VUELO ORDER BY id_estado_vuelo"
                Using cmd As New OracleCommand(query, conn)
                    Using da As New OracleDataAdapter(cmd)
                        da.Fill(dtEstados)
                    End Using
                End Using
            End Using
            ' Guardamos en ViewState para usarlo al dibujar cada fila de la tabla
            ViewState("EstadosVuelo") = dtEstados
        Catch ex As Exception
            MostrarMensaje("Error al cargar catálogo de estados: " & ex.Message, False)
        End Try
    End Sub

    ' 2. Cargamos los vuelos del día
    Private Sub CargarVuelosControl()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_VUELOS_CONTROL", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptControlVuelos.DataSource = dt
                        rptControlVuelos.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar los vuelos: " & ex.Message, False)
        End Try
    End Sub

    ' 3. Cuando se dibuja CADA FILA de la tabla, le inyectamos los estados al DropDownList
    Protected Sub rptControlVuelos_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim ddlNuevoEstado As DropDownList = CType(e.Item.FindControl("ddlNuevoEstado"), DropDownList)

            ' Obtenemos la tabla de estados que guardamos en memoria
            Dim dt As DataTable = CType(ViewState("EstadosVuelo"), DataTable)

            If dt IsNot Nothing AndAlso ddlNuevoEstado IsNot Nothing Then
                ddlNuevoEstado.DataSource = dt
                ddlNuevoEstado.DataTextField = "nombre"
                ddlNuevoEstado.DataValueField = "id_estado_vuelo"
                ddlNuevoEstado.DataBind()

                ' Seleccionamos el estado actual por defecto
                Dim idEstadoActual As String = DataBinder.Eval(e.Item.DataItem, "id_estado_vuelo").ToString()
                If ddlNuevoEstado.Items.FindByValue(idEstadoActual) IsNot Nothing Then
                    ddlNuevoEstado.SelectedValue = idEstadoActual
                End If
            End If
        End If
    End Sub

    ' 4. Cuando el empleado presiona el botón ACTUALIZAR
    Protected Sub rptControlVuelos_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "ActualizarEstado" Then
            Dim idVuelo As Integer = Convert.ToInt32(e.CommandArgument)

            ' Buscamos el DropDownList de la fila en la que hizo clic
            Dim ddlNuevoEstado As DropDownList = CType(e.Item.FindControl("ddlNuevoEstado"), DropDownList)
            Dim idNuevoEstado As Integer = Convert.ToInt32(ddlNuevoEstado.SelectedValue)
            Dim nombreEstado As String = ddlNuevoEstado.SelectedItem.Text

            Dim db As New ConexionDB()
            Try
                Using conn As OracleConnection = db.ObtenerConexion()
                    Using cmd As New OracleCommand("SP_ACTUALIZAR_ESTADO_VUELO", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = idVuelo
                        cmd.Parameters.Add("p_id_estado_vuelo", OracleDbType.Int32).Value = idNuevoEstado

                        Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                        outResultado.Direction = ParameterDirection.Output
                        cmd.Parameters.Add(outResultado)

                        conn.Open()
                        cmd.ExecuteNonQuery()

                        If outResultado.Value.ToString() = "EXITO" Then
                            MostrarMensaje($"El estado del vuelo se actualizó a: {nombreEstado}", True)
                            CargarVuelosControl() ' Refrescamos la tabla
                        Else
                            MostrarMensaje("No se pudo actualizar: " & outResultado.Value.ToString(), False)
                        End If
                    End Using
                End Using
            Catch ex As Exception
                MostrarMensaje("Error de conexión: " & ex.Message, False)
            End Try
        End If
    End Sub

    Private Sub MostrarMensaje(texto As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = texto
        pnlMensaje.CssClass = If(esExito, "alert alert-success fw-bold text-center", "alert alert-danger fw-bold text-center")
    End Sub
End Class