Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class GestionPuertas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        ' Seguridad: Solo Admin (1) o Empleado (3)
        If Session("UserEmail") Is Nothing OrElse (idRol <> 1 AndAlso idRol <> 3) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarAeropuertos()
            CargarPuertas()
        End If
    End Sub

    ' =======================================================
    ' CARGAR CATÁLOGO DE AEROPUERTOS EN EL DROPDOWN
    ' =======================================================
    Private Sub CargarAeropuertos()
        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_AEROPUERTOS_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cur As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cur.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cur)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlAeropuerto.DataSource = reader
                        ddlAeropuerto.DataTextField = "NOMBRE"
                        ddlAeropuerto.DataValueField = "ID_AEROPUERTO"
                        ddlAeropuerto.DataBind()
                    End Using
                End Using
            End Using
            ddlAeropuerto.Items.Insert(0, New ListItem("-- Seleccione Aeropuerto --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar aeropuertos: " & ex.Message, False)
        End Try
    End Sub

    ' =======================================================
    ' CARGAR LA TABLA DE PUERTAS
    ' =======================================================
    Private Sub CargarPuertas()
        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_PUERTAS_ADMIN", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)
                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptPuertas.DataSource = dt
                        rptPuertas.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar puertas: " & ex.Message, False)
        End Try
    End Sub

    ' =======================================================
    ' LÓGICA DE COLORES VISUALES SIN ERRORES EN EL HTML
    ' =======================================================
    Protected Sub rptPuertas_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPuertas.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim estado As String = DataBinder.Eval(e.Item.DataItem, "ESTADO").ToString().ToUpper()
            Dim lblBadge As Label = CType(e.Item.FindControl("lblBadgeEstado"), Label)

            lblBadge.Text = estado
            Select Case estado
                Case "DISPONIBLE"
                    lblBadge.CssClass = "badge-disponible"
                Case "OCUPADA"
                    lblBadge.CssClass = "badge-ocupada"
                Case "MANTENIMIENTO"
                    lblBadge.CssClass = "badge-mantenimiento"
                Case Else
                    lblBadge.CssClass = "badge bg-secondary text-white"
            End Select
        End If
    End Sub

    ' =======================================================
    ' GUARDAR NUEVA PUERTA
    ' =======================================================
    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If String.IsNullOrEmpty(txtCodigo.Text) OrElse String.IsNullOrEmpty(ddlAeropuerto.SelectedValue) Then
            MostrarMensaje("⚠️ Por favor, complete todos los campos.", False)
            Return
        End If

        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_PUERTA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo", OracleDbType.Varchar2).Value = txtCodigo.Text.Trim().ToUpper()
                    cmd.Parameters.Add("p_id_aeropuerto", OracleDbType.Int32).Value = Convert.ToInt32(ddlAeropuerto.SelectedValue)

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If paramOut.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Puerta registrada exitosamente.", True)
                        txtCodigo.Text = ""
                        ddlAeropuerto.SelectedIndex = 0
                        CargarPuertas() ' Actualiza la tabla para ver la puerta nueva
                    Else
                        MostrarMensaje("⚠️ Error en DB: " & paramOut.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error en el servidor: " & ex.Message, False)
        End Try
    End Sub

    ' =======================================================
    ' MENSAJES DE ALERTA
    ' =======================================================
    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success fw-bold rounded-3 mb-4", "alert alert-danger fw-bold rounded-3 mb-4")
    End Sub
End Class