Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Rutas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Seguridad: Empleados y Administradores tienen acceso. Clientes e invitados NO.
        If Session("UserRole") Is Nothing OrElse (Session("UserRole").ToString() <> "Empleado" AndAlso Session("UserRole").ToString() <> "Admin") Then
            Response.Redirect("~/Default.aspx")
        End If

        If Not IsPostBack Then
            pnlMensaje.Visible = False
            CargarAeropuertos()
        End If
    End Sub

    Private Sub CargarAeropuertos()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                ' Query para traer aeropuertos (Misma lista para origen y destino)
                Dim query As String = "SELECT id_aeropuerto, nombre || ' (' || codigo_iata || ')' AS detalle FROM AUR_AEROPUERTO ORDER BY nombre"

                Using cmd As New OracleCommand(query, conn)
                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ' Llenamos el combo de Origen
                        ddlOrigen.DataSource = reader
                        ddlOrigen.DataTextField = "detalle"
                        ddlOrigen.DataValueField = "id_aeropuerto"
                        ddlOrigen.DataBind()
                    End Using

                    ' Volvemos a leer para llenar el combo de Destino
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlDestino.DataSource = reader
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

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Dim duracion As Integer

        If String.IsNullOrEmpty(ddlOrigen.SelectedValue) OrElse String.IsNullOrEmpty(ddlDestino.SelectedValue) Then
            MostrarMensaje("Debe seleccionar un aeropuerto de Origen y uno de Destino.", False)
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
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center rounded-3 mb-4", "alert alert-danger text-center rounded-3 mb-4")
    End Sub
End Class