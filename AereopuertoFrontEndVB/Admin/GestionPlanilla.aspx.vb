Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class GestionPlanilla
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD ESTRICTA: Solo Administradores (Rol 1) pueden ver finanzas
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse idRol <> 1 Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            ' Sugerir el mes y año actual por defecto
            ddlMes.SelectedValue = DateTime.Now.Month.ToString()
            txtAnio.Text = DateTime.Now.Year.ToString()
            CargarHistorial()
        End If
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If String.IsNullOrEmpty(txtAnio.Text) OrElse String.IsNullOrEmpty(txtTotal.Text) Then
            MostrarMensaje("Complete todos los campos obligatorios.", False)
            Return
        End If

        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_PLANILLA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_mes", OracleDbType.Int32).Value = Convert.ToInt32(ddlMes.SelectedValue)
                    cmd.Parameters.Add("p_anio", OracleDbType.Int32).Value = Convert.ToInt32(txtAnio.Text)
                    cmd.Parameters.Add("p_total", OracleDbType.Decimal).Value = Convert.ToDecimal(txtTotal.Text)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outResultado.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Planilla del período registrada y cerrada exitosamente.", True)
                        txtTotal.Text = ""
                        CargarHistorial()
                    Else
                        MostrarMensaje(outResultado.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error interno: " & ex.Message, False)
        End Try
    End Sub

    Private Sub CargarHistorial()
        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_PLANILLAS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptPlanillas.DataSource = dt
                            rptPlanillas.DataBind()
                            rptPlanillas.Visible = True
                            pnlVacio.Visible = False
                        Else
                            rptPlanillas.Visible = False
                            pnlVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar historial: " & ex.Message, False)
        End Try
    End Sub

    ' =================================================================
    ' EVENTO: Transformar números en meses de texto para UX
    ' =================================================================
    Protected Sub rptPlanillas_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPlanillas.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then

            Dim mesNum As Integer = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "MES"))
            Dim anioNum As Integer = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "ANIO"))

            Dim lblPeriodo As Label = CType(e.Item.FindControl("lblPeriodo"), Label)

            ' Convertir el número del mes a su nombre en español
            Dim nombreMes As String = System.Globalization.CultureInfo.CreateSpecificCulture("es-ES").DateTimeFormat.GetMonthName(mesNum)
            ' Capitalizar la primera letra
            nombreMes = Char.ToUpper(nombreMes(0)) + nombreMes.Substring(1)

            lblPeriodo.Text = nombreMes & " " & anioNum
        End If
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm")
    End Sub
End Class