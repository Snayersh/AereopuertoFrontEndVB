Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class GestionTurnos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 4) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarCatalogos()
            CargarListaTurnos()
        End If
    End Sub

    ' --- CARGAR DROPDOWNS ---
    Private Sub CargarCatalogos()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                conn.Open()

                ' 1. Lista de Empleados (Nombre viene de AUR_PERSONA)
                Dim sqlEmpleados As String = "SELECT E.ID_EMPLEADO, P.PRIMER_NOMBRE || ' ' || P.PRIMER_APELLIDO AS NOMBRE FROM AUR_EMPLEADO E INNER JOIN AUR_PERSONA P ON E.ID_PERSONA = P.ID_PERSONA"
                Using cmdEmp As New OracleCommand(sqlEmpleados, conn)
                    ddlEmpleados.DataSource = cmdEmp.ExecuteReader()
                    ddlEmpleados.DataTextField = "NOMBRE"
                    ddlEmpleados.DataValueField = "ID_EMPLEADO"
                    ddlEmpleados.DataBind()
                    ddlEmpleados.Items.Insert(0, New ListItem("-- Seleccionar Empleado --", ""))
                End Using

                ' 2. Tipos de Turnos (Mañana, Tarde, Noche)
                Dim sqlTurnos As String = "SELECT ID_TURNO, NOMBRE_TURNO FROM AUR_TURNO"
                Using cmdTur As New OracleCommand(sqlTurnos, conn)
                    ddlTiposTurno.DataSource = cmdTur.ExecuteReader()
                    ddlTiposTurno.DataTextField = "NOMBRE_TURNO"
                    ddlTiposTurno.DataValueField = "ID_TURNO"
                    ddlTiposTurno.DataBind()
                    ddlTiposTurno.Items.Insert(0, New ListItem("-- Seleccionar Turno --", ""))
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