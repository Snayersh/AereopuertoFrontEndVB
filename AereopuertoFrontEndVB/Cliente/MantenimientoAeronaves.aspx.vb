Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class MantenimientoAeronaves
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Solo Admin (1) o Personal de Operaciones/Mantenimiento (3)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 1 AndAlso idRol <> 3) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            ' Por defecto, ponemos la fecha de hoy en el formulario
            txtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd")
            CargarSelectores()
            CargarHistorial()
        End If
    End Sub

    Private Sub CargarSelectores()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                ' Llenar Aeronaves
                Using cmd As New OracleCommand("SP_OBTENER_AERONAVES_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cur As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cur.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cur)
                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlAeronave.DataSource = reader
                        ddlAeronave.DataTextField = "NOMBRE_AERONAVE"
                        ddlAeronave.DataValueField = "ID_AERONAVE"
                        ddlAeronave.DataBind()
                    End Using
                    conn.Close()
                End Using

                ' Llenar Tipos de Mantenimiento
                Using cmd2 As New OracleCommand("SP_OBTENER_TIPOS_MANT_CBX", conn)
                    cmd2.CommandType = CommandType.StoredProcedure
                    Dim cur2 As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cur2.Direction = ParameterDirection.Output
                    cmd2.Parameters.Add(cur2)
                    conn.Open()
                    Using reader2 As OracleDataReader = cmd2.ExecuteReader()
                        ddlTipo.DataSource = reader2
                        ddlTipo.DataTextField = "NOMBRE"
                        ddlTipo.DataValueField = "ID_TIPO_MANTENIMIENTO"
                        ddlTipo.DataBind()
                    End Using
                End Using
            End Using

            ddlAeronave.Items.Insert(0, New ListItem("-- Seleccione Aeronave --", ""))
            ddlTipo.Items.Insert(0, New ListItem("-- Seleccione Tipo --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar catálogos: " & ex.Message, False)
        End Try
    End Sub

    Private Sub CargarHistorial()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_MANTENIMIENTOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptMantenimientos.DataSource = dt
                            rptMantenimientos.DataBind()
                            rptMantenimientos.Visible = True
                            pnlVacio.Visible = False
                        Else
                            rptMantenimientos.Visible = False
                            pnlVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Error silencioso en la carga inicial
        End Try
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If String.IsNullOrEmpty(ddlAeronave.SelectedValue) OrElse String.IsNullOrEmpty(ddlTipo.SelectedValue) OrElse String.IsNullOrEmpty(txtFecha.Text) Then
            MostrarMensaje("Complete todos los campos obligatorios.", False)
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_MANTENIMIENTO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_fecha", OracleDbType.Date).Value = Convert.ToDateTime(txtFecha.Text)
                    cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = txtDescripcion.Text.Trim()
                    cmd.Parameters.Add("p_id_aeronave", OracleDbType.Int32).Value = Convert.ToInt32(ddlAeronave.SelectedValue)
                    cmd.Parameters.Add("p_id_tipo", OracleDbType.Int32).Value = Convert.ToInt32(ddlTipo.SelectedValue)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outResultado.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Mantenimiento registrado correctamente.", True)
                        txtDescripcion.Text = ""
                        ddlAeronave.SelectedIndex = 0
                        ddlTipo.SelectedIndex = 0
                        CargarHistorial() ' Refrescar tabla
                    Else
                        MostrarMensaje("Error: " & outResultado.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error interno: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub rptMantenimientos_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptMantenimientos.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then

            Dim lblBadgeTipo As Label = CType(e.Item.FindControl("lblBadgeTipo"), Label)
            Dim tipoObj As Object = DataBinder.Eval(e.Item.DataItem, "TIPO_MANTENIMIENTO")

            If IsDBNull(tipoObj) OrElse tipoObj Is Nothing Then
                lblBadgeTipo.Text = "NO DEFINIDO"
                lblBadgeTipo.CssClass = "badge-preventivo shadow-sm"
            Else
                Dim tipoTexto As String = tipoObj.ToString()
                lblBadgeTipo.Text = tipoTexto

                ' Lógica de colores corporativos
                If tipoTexto.ToUpper().Contains("CORRECTIVO") OrElse tipoTexto.ToUpper().Contains("EMERGENCIA") Then
                    lblBadgeTipo.CssClass = "badge-correctivo shadow-sm"
                ElseIf tipoTexto.ToUpper().Contains("RUTINA") OrElse tipoTexto.ToUpper().Contains("INSPECCIÓN") Then
                    lblBadgeTipo.CssClass = "badge-rutina shadow-sm"
                Else
                    lblBadgeTipo.CssClass = "badge-preventivo shadow-sm"
                End If
            End If

        End If
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success fw-bold text-center rounded-3 mb-4 shadow-sm", "alert alert-danger fw-bold text-center rounded-3 mb-4 shadow-sm")
    End Sub
End Class