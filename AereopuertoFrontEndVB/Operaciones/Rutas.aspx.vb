Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Rutas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        ' Rol 3 (Operaciones) es el correcto para gestionar las rutas aéreas.
        If Session("UserEmail") Is Nothing OrElse (idRol <> 3) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            pnlMensaje.Visible = False
            CargarAeropuertos()
        End If
    End Sub

    ' -------------------------------------------------------------
    ' 1. MÉTODO PARA LLENAR DESPLEGABLES (100% SP + OPTIMIZADO)
    ' -------------------------------------------------------------
    Private Sub CargarAeropuertos()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_AEROPUERTOS_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()

                    ' 🔥 OPTIMIZACIÓN: Llenamos un DataTable una sola vez
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        ' Llenamos el combo de Origen con la data en memoria
                        ddlOrigen.DataSource = dt
                        ddlOrigen.DataTextField = "detalle"
                        ddlOrigen.DataValueField = "id_aeropuerto"
                        ddlOrigen.DataBind()

                        ' Llenamos el combo de Destino reusando la misma data (¡Sin volver a consultar a la BD!)
                        ddlDestino.DataSource = dt
                        ddlDestino.DataTextField = "detalle"
                        ddlDestino.DataValueField = "id_aeropuerto"
                        ddlDestino.DataBind()
                    End Using
                End Using
            End Using

            ddlOrigen.Items.Insert(0, New ListItem("-- Seleccione Origen --", ""))
            ddlDestino.Items.Insert(0, New ListItem("-- Seleccione Destino --", ""))

        Catch ex As Exception
            MostrarMensaje("Error al cargar los aeropuertos: " & ex.Message, False)
        End Try
    End Sub

    ' -------------------------------------------------------------
    ' 2. GUARDAR RUTA
    ' -------------------------------------------------------------
    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Dim duracion As Integer

        If String.IsNullOrEmpty(ddlOrigen.SelectedValue) OrElse String.IsNullOrEmpty(ddlDestino.SelectedValue) Then
            MostrarMensaje("Debe seleccionar un aeropuerto de Origen y uno de Destino.", False)
            Return
        End If

        ' Validar que no elijan el mismo aeropuerto
        If ddlOrigen.SelectedValue = ddlDestino.SelectedValue Then
            MostrarMensaje("El aeropuerto de origen no puede ser el mismo que el de destino.", False)
            Return
        End If

        If Not Integer.TryParse(txtDuracion.Text.Trim(), duracion) OrElse duracion <= 0 Then
            MostrarMensaje("Ingrese una duración válida en minutos (mayor a 0).", False)
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_INSERTAR_RUTA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_origen", OracleDbType.Int32).Value = Convert.ToInt32(ddlOrigen.SelectedValue)
                    cmd.Parameters.Add("p_id_destino", OracleDbType.Int32).Value = Convert.ToInt32(ddlDestino.SelectedValue)
                    cmd.Parameters.Add("p_duracion_minutos", OracleDbType.Int32).Value = duracion

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        MostrarMensaje("¡Ruta operativa registrada con éxito!", True)
                        LimpiarCampos()
                    Else
                        MostrarMensaje(resultado, False)
                    End If
                End Using
            End Using

        Catch ex As Exception
            MostrarMensaje("Error de conexión: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        LimpiarCampos()
        pnlMensaje.Visible = False
    End Sub

    Private Sub LimpiarCampos()
        ddlOrigen.SelectedIndex = 0
        ddlDestino.SelectedIndex = 0
        txtDuracion.Text = ""
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center rounded-3 mb-4 fw-bold", "alert alert-danger text-center rounded-3 mb-4 fw-bold")
    End Sub
End Class