Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Arrestos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 5) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarHistorial("")
        End If
    End Sub

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

    Protected Sub btnGuardarArresto_Click(sender As Object, e As EventArgs) Handles btnGuardarArresto.Click
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_ARRESTO", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' Parámetros de texto
                    cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = txtNombre.Text.Trim()
                    cmd.Parameters.Add("p_pasaporte", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(txtPasaporte.Text), DBNull.Value, txtPasaporte.Text.Trim().ToUpper())
                    cmd.Parameters.Add("p_motivo", OracleDbType.Varchar2).Value = txtMotivo.Text.Trim()
                    cmd.Parameters.Add("p_autoridad", OracleDbType.Varchar2).Value = ddlAutoridad.SelectedValue

                    ' Parámetros numéricos (Opcionales)
                    cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = If(String.IsNullOrEmpty(txtIdVuelo.Text), DBNull.Value, Convert.ToInt32(txtIdVuelo.Text))
                    cmd.Parameters.Add("p_id_aeropuerto", OracleDbType.Int32).Value = If(String.IsNullOrEmpty(txtIdAeropuerto.Text), DBNull.Value, Convert.ToInt32(txtIdAeropuerto.Text))

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If paramOut.Value.ToString() = "EXITO" Then
                        MostrarMensaje("🚨 Incidente registrado en el sistema. Alerta activa.", True)

                        ' Limpiar formulario
                        txtNombre.Text = ""
                        txtPasaporte.Text = ""
                        txtIdVuelo.Text = ""
                        txtIdAeropuerto.Text = ""
                        txtMotivo.Text = ""
                        ddlAutoridad.SelectedIndex = 0

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