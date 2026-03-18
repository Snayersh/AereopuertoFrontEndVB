Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Arrestos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("UserRole") Is Nothing OrElse (Session("UserRole").ToString() <> "Empleado" AndAlso Session("UserRole").ToString() <> "Admin") Then
            Response.Redirect("~/Default.aspx")
        End If

        If Not IsPostBack Then
            ' ¡Aquí está la magia! Ya sin la comilla, cargará todos al entrar.
            CargarHistorial("")
        End If
    End Sub

    ' --- Buscar Historial ---
    Protected Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        CargarHistorial(txtBusqueda.Text.Trim())
    End Sub

    Private Sub CargarHistorial(busqueda As String)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_BUSCAR_ARRESTOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_busqueda", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(busqueda), DBNull.Value, busqueda)

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptArrestos.DataSource = dt
                        rptArrestos.DataBind()

                        If dt.Rows.Count = 0 AndAlso Not String.IsNullOrEmpty(busqueda) Then
                            MostrarMensaje("ℹ️ Pasajero limpio. No se encontraron antecedentes.", True)
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error en búsqueda: " & ex.Message, False)
        End Try
    End Sub

    ' --- Guardar Nuevo Arresto ---
    Protected Sub btnGuardarArresto_Click(sender As Object, e As EventArgs) Handles btnGuardarArresto.Click
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_ARRESTO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = txtNombre.Text.Trim()
                    cmd.Parameters.Add("p_pasaporte", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(txtPasaporte.Text), DBNull.Value, txtPasaporte.Text.Trim().ToUpper())
                    cmd.Parameters.Add("p_motivo", OracleDbType.Varchar2).Value = txtMotivo.Text.Trim()
                    cmd.Parameters.Add("p_autoridad", OracleDbType.Varchar2).Value = ddlAutoridad.SelectedValue

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If paramOut.Value.ToString() = "EXITO" Then
                        MostrarMensaje("🚨 Incidente registrado en el sistema. Alerta activa.", True)
                        txtNombre.Text = ""
                        txtPasaporte.Text = ""
                        txtMotivo.Text = ""
                        ddlAutoridad.SelectedIndex = 0
                        ' Cargamos la búsqueda con el nombre que acabamos de meter para que lo vea en pantalla
                        CargarHistorial(txtNombre.Text.Trim())
                    Else
                        MostrarMensaje("⚠️ Error: " & paramOut.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error: " & ex.Message, False)
        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center fw-bold rounded-3 mb-4", "alert alert-danger text-center fw-bold rounded-3 mb-4")
    End Sub
End Class