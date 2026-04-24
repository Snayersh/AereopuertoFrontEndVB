Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class GestionAsistencia
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 CORRECCIÓN DE SEGURIDAD: Solo Admin (1) y Recursos Humanos (4)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 1 AndAlso idRol <> 4) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            txtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd") ' Fecha por defecto hoy
            CargarEmpleados()
            CargarHistorialAsistencia()
        End If
    End Sub

    ' --- CARGAR LISTA DE EMPLEADOS (100% SP REUTILIZADO) ---
    Private Sub CargarEmpleados()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                ' Reusamos el SP que ya tienes creado en Oracle
                Using cmd As New OracleCommand("SP_OBTENER_EMPLEADOS_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlEmpleados.DataSource = reader
                        ddlEmpleados.DataTextField = "NOMBRE_COMPLETO"
                        ddlEmpleados.DataValueField = "ID_EMPLEADO"
                        ddlEmpleados.DataBind()
                        ddlEmpleados.Items.Insert(0, New ListItem("-- Seleccione un Empleado --", ""))
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar empleados: " & ex.Message, False)
        End Try
    End Sub

    ' --- GUARDAR ASISTENCIA ---
    Protected Sub btnRegistrarAsistencia_Click(sender As Object, e As EventArgs) Handles btnRegistrarAsistencia.Click
        If String.IsNullOrEmpty(ddlEmpleados.SelectedValue) OrElse String.IsNullOrEmpty(ddlEstado.SelectedValue) Then
            MostrarMensaje("⚠️ Por favor complete todos los campos.", False)
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_ASISTENCIA", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.Add("p_fecha", OracleDbType.Date).Value = Convert.ToDateTime(txtFecha.Text)
                    cmd.Parameters.Add("p_estado", OracleDbType.Varchar2).Value = ddlEstado.SelectedValue
                    cmd.Parameters.Add("p_id_empleado", OracleDbType.Int32).Value = Convert.ToInt32(ddlEmpleados.SelectedValue)

                    Dim outRes As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outRes.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outRes)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outRes.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Registro de asistencia guardado correctamente.", True)
                        CargarHistorialAsistencia()
                        ddlEmpleados.SelectedIndex = 0
                        ddlEstado.SelectedIndex = 0
                    Else
                        MostrarMensaje("⚠️ Error: " & outRes.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error interno: " & ex.Message, False)
        End Try
    End Sub

    ' --- CARGAR TABLA DE HISTORIAL ---
    Private Sub CargarHistorialAsistencia()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_ASISTENCIA_RECIENTE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptAsistencia.DataSource = dt
                        rptAsistencia.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Fallo silencioso en tabla
        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success fw-bold text-center mb-4", "alert alert-danger fw-bold text-center mb-4")
    End Sub

End Class