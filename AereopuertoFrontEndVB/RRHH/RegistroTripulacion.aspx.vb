Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class RegistroTripulacion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Solo Recursos Humanos (Rol 4)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 4) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            pnlMensaje.Visible = False
            CargarTurnos() ' Llenar combo de base de datos
        End If
    End Sub

    Private Sub CargarTurnos()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_TURNOS_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cur As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cur.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cur)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlTurno.DataSource = reader
                        ddlTurno.DataTextField = "NOMBRE_TURNO"
                        ddlTurno.DataValueField = "ID_TURNO"
                        ddlTurno.DataBind()
                    End Using
                End Using
            End Using
            ddlTurno.Items.Insert(0, New ListItem("-- Selecciona un Turno --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar los turnos: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_TRIPULANTE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = txtNombre.Text.Trim()
                    cmd.Parameters.Add("p_puesto", OracleDbType.Varchar2).Value = ddlPuesto.SelectedValue
                    cmd.Parameters.Add("p_id_turno", OracleDbType.Int32).Value = Convert.ToInt32(ddlTurno.SelectedValue)

                    If String.IsNullOrEmpty(txtLicencia.Text.Trim()) Then
                        cmd.Parameters.Add("p_licencia", OracleDbType.Varchar2).Value = DBNull.Value
                    Else
                        cmd.Parameters.Add("p_licencia", OracleDbType.Varchar2).Value = txtLicencia.Text.Trim().ToUpper()
                    End If

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = paramOut.Value.ToString()

                    If resultado = "EXITO" Then
                        MostrarMensaje("✅ Personal y turno registrados correctamente en el sistema.", True)
                        LimpiarCampos()
                    Else
                        MostrarMensaje("⚠️ Error al registrar: " & resultado, False)
                    End If
                End Using
            End Using

        Catch ex As Exception
            MostrarMensaje("❌ Error de sistema: " & ex.Message, False)
        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm")
    End Sub

    Private Sub LimpiarCampos()
        txtNombre.Text = ""
        ddlPuesto.SelectedIndex = 0
        ddlTurno.SelectedIndex = 0
        txtLicencia.Text = ""
    End Sub
End Class