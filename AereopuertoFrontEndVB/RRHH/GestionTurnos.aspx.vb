Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class GestionTurnos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Seguridad: Solo Recursos Humanos (Rol 4)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 4) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarCatalogos()
            CargarListaTurnos()
        End If
    End Sub

    ' --- CARGAR DROPDOWNS (100% PARAMETRIZADO CON SPs) ---
    Private Sub CargarCatalogos()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                conn.Open()

                ' 1. Lista de Empleados mediante SP
                Using cmdEmp As New OracleCommand("SP_OBTENER_EMPLEADOS_CBX", conn)
                    cmdEmp.CommandType = CommandType.StoredProcedure
                    Dim cursorEmp As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorEmp.Direction = ParameterDirection.Output
                    cmdEmp.Parameters.Add(cursorEmp)

                    Using reader As OracleDataReader = cmdEmp.ExecuteReader()
                        ddlEmpleados.DataSource = reader
                        ddlEmpleados.DataTextField = "NOMBRE_COMPLETO"
                        ddlEmpleados.DataValueField = "ID_EMPLEADO"
                        ddlEmpleados.DataBind()
                        ddlEmpleados.Items.Insert(0, New ListItem("-- Seleccionar Empleado --", ""))
                    End Using
                End Using

                ' 2. Tipos de Turnos mediante SP
                Using cmdTur As New OracleCommand("SP_OBTENER_TIPOS_TURNO_CBX", conn)
                    cmdTur.CommandType = CommandType.StoredProcedure
                    Dim cursorTur As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorTur.Direction = ParameterDirection.Output
                    cmdTur.Parameters.Add(cursorTur)

                    Using reader As OracleDataReader = cmdTur.ExecuteReader()
                        ddlTiposTurno.DataSource = reader
                        ddlTiposTurno.DataTextField = "NOMBRE_TURNO"
                        ddlTiposTurno.DataValueField = "ID_TURNO"
                        ddlTiposTurno.DataBind()
                        ddlTiposTurno.Items.Insert(0, New ListItem("-- Seleccionar Turno --", ""))
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar catálogos: " & ex.Message, False)
        End Try
    End Sub

    ' --- GUARDAR ASIGNACIÓN ---
    Protected Sub btnGuardarAsignacion_Click(sender As Object, e As EventArgs) Handles btnGuardarAsignacion.Click
        If String.IsNullOrEmpty(ddlEmpleados.SelectedValue) OrElse String.IsNullOrEmpty(ddlTiposTurno.SelectedValue) OrElse String.IsNullOrEmpty(txtFecha.Text) Then
            MostrarMensaje("⚠️ Por favor completa los campos obligatorios.", False)
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ASIGNAR_TURNO_EMPLEADO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_turno", OracleDbType.Int32).Value = Convert.ToInt32(ddlTiposTurno.SelectedValue)
                    cmd.Parameters.Add("p_id_empleado", OracleDbType.Int32).Value = Convert.ToInt32(ddlEmpleados.SelectedValue)
                    cmd.Parameters.Add("p_fecha", OracleDbType.Date).Value = Convert.ToDateTime(txtFecha.Text)
                    cmd.Parameters.Add("p_observacion", OracleDbType.Varchar2).Value = txtObservacion.Text.Trim()

                    Dim outRes As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outRes.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outRes)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outRes.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Turno asignado exitosamente.", True)
                        txtObservacion.Text = ""
                        txtFecha.Text = ""
                        ddlEmpleados.SelectedIndex = 0
                        ddlTiposTurno.SelectedIndex = 0
                        CargarListaTurnos()
                    Else
                        MostrarMensaje("⚠️ Error: " & outRes.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error de sistema: " & ex.Message, False)
        End Try
    End Sub

    ' --- LISTAR TURNOS ---
    Private Sub CargarListaTurnos()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_TURNOS_ASIGNADOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptTurnos.DataSource = dt
                        rptTurnos.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Error silencioso en carga inicial
        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success fw-bold text-center mb-4", "alert alert-danger fw-bold text-center mb-4")
    End Sub

End Class