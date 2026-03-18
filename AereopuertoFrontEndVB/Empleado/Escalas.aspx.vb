Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Escalas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("UserRole") Is Nothing OrElse (Session("UserRole").ToString() <> "Empleado" AndAlso Session("UserRole").ToString() <> "Admin") Then
            Response.Redirect("~/Default.aspx")
        End If

        If Not IsPostBack Then
            pnlMensaje.Visible = False
        End If
    End Sub



    Private Sub CargarListas()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                conn.Open()

                ' Llenar DropDown de Vuelos
                Using cmdVuelos As New OracleCommand("SP_CARGAR_VUELOS_ESCALA", conn)
                    cmdVuelos.CommandType = CommandType.StoredProcedure
                    Dim curVuelos As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    curVuelos.Direction = ParameterDirection.Output
                    cmdVuelos.Parameters.Add(curVuelos)

                    Using readerVuelos As OracleDataReader = cmdVuelos.ExecuteReader()
                        ddlVuelo.DataSource = readerVuelos
                        ddlVuelo.DataTextField = "vuelo_desc"
                        ddlVuelo.DataValueField = "id_vuelo"
                        ddlVuelo.DataBind()
                    End Using
                End Using
                ddlVuelo.Items.Insert(0, New ListItem("-- Seleccione el Vuelo --", ""))

                ' Llenar DropDown de Aeropuertos
                Using cmdPto As New OracleCommand("SP_CARGAR_AEROPUERTOS_ESCALA", conn)
                    cmdPto.CommandType = CommandType.StoredProcedure
                    Dim curPto As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    curPto.Direction = ParameterDirection.Output
                    cmdPto.Parameters.Add(curPto)

                    Using readerPto As OracleDataReader = cmdPto.ExecuteReader()
                        ddlAeropuerto.DataSource = readerPto
                        ddlAeropuerto.DataTextField = "aeropuerto_desc"
                        ddlAeropuerto.DataValueField = "id_aeropuerto"
                        ddlAeropuerto.DataBind()
                    End Using
                End Using
                ddlAeropuerto.Items.Insert(0, New ListItem("-- Seleccione el Aeropuerto --", ""))

            End Using
        Catch ex As Exception
            MostrarAlerta("Error al cargar los catálogos: " & ex.Message, "alert-danger")
        End Try
    End Sub

    Protected Sub btnGuardarEscala_Click(sender As Object, e As EventArgs) Handles btnGuardarEscala.Click
        ' 1. Validaciones iniciales
        If String.IsNullOrEmpty(ddlVuelo.SelectedValue) OrElse String.IsNullOrEmpty(ddlAeropuerto.SelectedValue) Then
            MostrarAlerta("Por favor, seleccione un Vuelo y un Aeropuerto.", "alert-warning")
            Return
        End If

        Dim orden As Integer = Convert.ToInt32(txtOrden.Text)
        Dim horaLlegada As DateTime = Convert.ToDateTime(txtHoraLlegada.Text)
        Dim horaSalida As DateTime = Convert.ToDateTime(txtHoraSalida.Text)

        ' Regla de negocio: La salida debe ser después de la llegada
        If horaSalida <= horaLlegada Then
            MostrarAlerta("Error: La hora de salida debe ser mayor a la hora de llegada.", "alert-danger")
            Return
        End If

        ' 2. Guardar en Oracle
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CREAR_ESCALA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_orden", OracleDbType.Int32).Value = orden
                    cmd.Parameters.Add("p_hora_llegada", OracleDbType.TimeStamp).Value = horaLlegada
                    cmd.Parameters.Add("p_hora_salida", OracleDbType.TimeStamp).Value = horaSalida
                    cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = Convert.ToInt32(ddlVuelo.SelectedValue)
                    cmd.Parameters.Add("p_id_aeropuerto", OracleDbType.Int32).Value = Convert.ToInt32(ddlAeropuerto.SelectedValue)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim res As String = outResultado.Value.ToString()

                    If res = "EXITO" Then
                        MostrarAlerta("¡Escala registrada correctamente en el sistema logístico!", "alert-success")
                        ' Limpiar formulario
                        txtOrden.Text = ""
                        txtHoraLlegada.Text = ""
                        txtHoraSalida.Text = ""
                        ddlVuelo.SelectedIndex = 0
                        ddlAeropuerto.SelectedIndex = 0
                    Else
                        MostrarAlerta("No se pudo registrar la escala: " & res, "alert-danger")
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarAlerta("Error del sistema: " & ex.Message, "alert-danger")
        End Try
    End Sub

    Private Sub MostrarAlerta(mensaje As String, claseCss As String)
        pnlMensaje.Visible = True
        pnlMensaje.CssClass = "alert text-center fw-bold rounded-3 mb-4 " & claseCss
        lblMensaje.Text = mensaje
    End Sub
End Class