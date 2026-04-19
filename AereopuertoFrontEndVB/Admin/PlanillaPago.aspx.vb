Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class PlanillaPago
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 1 AndAlso idRol <> 4) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarCatalogos()
            If ddlPlanilla.Items.Count > 1 Then
                ddlPlanilla.SelectedIndex = 1
                CargarTablaPlanilla(ddlPlanilla.SelectedValue)
            End If
        End If
    End Sub

    ' Evento que se dispara al cambiar el mes en el DropDownList
    Protected Sub ddlPlanilla_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlPlanilla.SelectedIndexChanged
        CargarTablaPlanilla(ddlPlanilla.SelectedValue)
    End Sub

    Protected Sub btnGuardarPago_Click(sender As Object, e As EventArgs) Handles btnGuardarPago.Click
        If String.IsNullOrEmpty(ddlPlanilla.SelectedValue) OrElse String.IsNullOrEmpty(ddlEmpleado.SelectedValue) OrElse String.IsNullOrEmpty(txtSalario.Text) Then
            MostrarMensaje("⚠️ Completa los campos obligatorios.", False)
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_DETALLE_PLANILLA", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.Add("p_id_planilla", OracleDbType.Int32).Value = Convert.ToInt32(ddlPlanilla.SelectedValue)
                    cmd.Parameters.Add("p_id_empleado", OracleDbType.Int32).Value = Convert.ToInt32(ddlEmpleado.SelectedValue)
                    cmd.Parameters.Add("p_salario", OracleDbType.Decimal).Value = Convert.ToDecimal(txtSalario.Text)
                    cmd.Parameters.Add("p_bonificacion", OracleDbType.Decimal).Value = If(String.IsNullOrEmpty(txtBonificacion.Text), 0, Convert.ToDecimal(txtBonificacion.Text))

                    Dim outRes As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outRes.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outRes)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outRes.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Pago registrado en la nómina exitosamente.", True)
                        txtSalario.Text = ""
                        txtBonificacion.Text = "0.00"
                        ddlEmpleado.SelectedIndex = 0
                        ' Refrescar la tabla
                        CargarTablaPlanilla(ddlPlanilla.SelectedValue)
                    Else
                        MostrarMensaje("⚠️ Error: " & outRes.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error interno: " & ex.Message, False)
        End Try
    End Sub

    Private Sub CargarCatalogos()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                conn.Open()

                ' 1. Cargar Empleados
                Dim sqlEmp As String = "SELECT E.ID_EMPLEADO, P.PRIMER_NOMBRE || ' ' || P.PRIMER_APELLIDO AS NOMBRE FROM AUR_EMPLEADO E INNER JOIN AUR_PERSONA P ON E.ID_PERSONA = P.ID_PERSONA"
                Using cmd As New OracleCommand(sqlEmp, conn)
                    ddlEmpleado.DataSource = cmd.ExecuteReader()
                    ddlEmpleado.DataTextField = "NOMBRE"
                    ddlEmpleado.DataValueField = "ID_EMPLEADO"
                    ddlEmpleado.DataBind()
                    ddlEmpleado.Items.Insert(0, New ListItem("-- Seleccione Empleado --", ""))
                End Using

                ' 2. Cargar Planillas Maestras (Ej: 'Planilla Enero 2026')
                ' Asumimos que la tabla AUR_PLANILLA tiene un campo DESCRIPCION o MES_ANIO
                Dim sqlPla As String = "SELECT ID_PLANILLA, DESCRIPCION FROM AUR_PLANILLA ORDER BY ID_PLANILLA DESC"
                Using cmd As New OracleCommand(sqlPla, conn)
                    ddlPlanilla.DataSource = cmd.ExecuteReader()
                    ddlPlanilla.DataTextField = "DESCRIPCION"
                    ddlPlanilla.DataValueField = "ID_PLANILLA"
                    ddlPlanilla.DataBind()
                    ddlPlanilla.Items.Insert(0, New ListItem("-- Seleccione Periodo --", ""))
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar datos: " & ex.Message, False)
        End Try
    End Sub

    Private Sub CargarTablaPlanilla(idPlanilla As String)
        If String.IsNullOrEmpty(idPlanilla) Then Return

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_DETALLE_PLANILLA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_id_planilla", OracleDbType.Int32).Value = Convert.ToInt32(idPlanilla)

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    Dim outTotal As New OracleParameter("p_total_planilla", OracleDbType.Decimal)
                    outTotal.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outTotal)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptDetallePlanilla.DataSource = dt
                        rptDetallePlanilla.DataBind()

                        ' Mostrar el total sumado por la base de datos
                        If Not IsDBNull(outTotal.Value) Then
                            lblTotalPlanilla.Text = Convert.ToDecimal(outTotal.Value.ToString()).ToString("N2")
                        Else
                            lblTotalPlanilla.Text = "0.00"
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Silencioso para la vista
        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success fw-bold text-center mb-4", "alert alert-danger fw-bold text-center mb-4")
    End Sub
End Class