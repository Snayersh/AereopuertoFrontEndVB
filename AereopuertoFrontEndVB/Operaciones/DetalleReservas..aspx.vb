Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class DetalleReservas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Empleados (3) y Administradores (1)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 1 AndAlso idRol <> 3) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarReservasPadre()
            CargarCatalogoPromociones()

            ' ========================================================
            ' ¡MAGIA URL! Atrapamos el ID si viene de otra página
            ' ========================================================
            If Request.QueryString("id") IsNot Nothing Then
                Dim idReservaUrl As String = Request.QueryString("id").ToString()
                If ddlReservas.Items.FindByValue(idReservaUrl) IsNot Nothing Then
                    ddlReservas.SelectedValue = idReservaUrl
                    btnVerDetalles_Click(Nothing, Nothing)
                End If
            End If
        End If
    End Sub

    ' =======================================================
    ' 1. CARGAR SELECTOR DE RESERVAS (PADRE)
    ' =======================================================
    Private Sub CargarReservasPadre()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_RESERVAS_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cur As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cur.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cur)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlReservas.DataSource = reader
                        ddlReservas.DataTextField = "INFO_RESERVA"
                        ddlReservas.DataValueField = "ID_RESERVA"
                        ddlReservas.DataBind()
                    End Using
                End Using
            End Using
            ddlReservas.Items.Insert(0, New ListItem("-- Seleccione una Reserva --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar las reservas: " & ex.Message, False)
        End Try
    End Sub

    ' =======================================================
    ' 2. CARGAR SELECTOR DE PROMOCIONES (CATÁLOGO PARA AGREGAR)
    ' =======================================================
    Private Sub CargarCatalogoPromociones()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_PROMOCIONES_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cur As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cur.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cur)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlPromos.DataSource = reader
                        ddlPromos.DataTextField = "INFO_PROMOCION"
                        ddlPromos.DataValueField = "ID_PROMOCION"
                        ddlPromos.DataBind()
                    End Using
                End Using
            End Using
            ddlPromos.Items.Insert(0, New ListItem("-- Elija una Promoción --", ""))
        Catch ex As Exception
            ' Silencioso en carga inicial
        End Try
    End Sub

    ' =======================================================
    ' 3. BOTÓN "CARGAR RESERVA" (MOSTRAR DETALLES)
    ' =======================================================
    Protected Sub btnVerDetalles_Click(sender As Object, e As EventArgs) Handles btnVerDetalles.Click
        If String.IsNullOrEmpty(ddlReservas.SelectedValue) Then
            MostrarMensaje("⚠️ Por favor, seleccione una reserva de la lista.", False)
            pnlGestionDetalles.Visible = False
            Return
        End If

        pnlMensaje.Visible = False
        pnlGestionDetalles.Visible = True
        CargarTablaPromosAplicadas()
    End Sub

    ' =======================================================
    ' 4. CARGAR LA TABLA DE PROMOCIONES APLICADAS A LA RESERVA
    ' =======================================================
    Private Sub CargarTablaPromosAplicadas()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_PROMOS_RESERVA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_reserva", OracleDbType.Int32).Value = Convert.ToInt32(ddlReservas.SelectedValue)

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptPromos.DataSource = dt
                            rptPromos.DataBind()
                            rptPromos.Visible = True
                            pnlVacio.Visible = False
                        Else
                            rptPromos.Visible = False
                            pnlVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar promociones: " & ex.Message, False)
        End Try
    End Sub

    ' =======================================================
    ' 5. GUARDAR LA VINCULACIÓN (INSERTAR EN TABLA PUENTE)
    ' =======================================================
    Protected Sub btnGuardarPromo_Click(sender As Object, e As EventArgs) Handles btnGuardarPromo.Click
        If String.IsNullOrEmpty(ddlPromos.SelectedValue) OrElse String.IsNullOrEmpty(ddlReservas.SelectedValue) Then
            MostrarMensaje("⚠️ Seleccione una promoción válida para aplicar.", False)
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_VINCULAR_PROMO_RESERVA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_reserva", OracleDbType.Int32).Value = Convert.ToInt32(ddlReservas.SelectedValue)
                    cmd.Parameters.Add("p_id_promocion", OracleDbType.Int32).Value = Convert.ToInt32(ddlPromos.SelectedValue)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outResultado.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Promoción aplicada exitosamente a la reserva.", True)
                        ddlPromos.SelectedIndex = 0
                        CargarTablaPromosAplicadas() ' Refrescar la tabla
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
    ' 6. FORMATEO VISUAL SIN ERRORES HTML (ITEMDATABOUND)
    ' =======================================================
    Protected Sub rptPromos_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPromos.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then

            Dim descuentoObj = DataBinder.Eval(e.Item.DataItem, "DESCUENTO")
            Dim lblBadge As Label = CType(e.Item.FindControl("lblBadgeDescuento"), Label)

            If Not IsDBNull(descuentoObj) Then
                Dim descuentoVal As Decimal = Convert.ToDecimal(descuentoObj)
                lblBadge.Text = "- " & descuentoVal.ToString("0.00") & " %"
                lblBadge.CssClass = "discount-badge shadow-sm"
            End If

        End If
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm")
    End Sub
End Class