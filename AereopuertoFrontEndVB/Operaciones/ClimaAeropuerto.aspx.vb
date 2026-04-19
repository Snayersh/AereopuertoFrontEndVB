Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Net
Imports System.Web.Script.Serialization ' Usado para leer el JSON de la API

Public Class ClimaAeropuerto
    Inherits System.Web.UI.Page

    Dim API_KEY As String = Config.API_KEY
    Private Const CIUDAD_AEROPUERTO As String = "Guatemala City"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 3) Then
            Response.Redirect("~/Account/Login.aspx")
        End If
    End Sub

    Protected Sub btnActualizarClima_Click(sender As Object, e As EventArgs) Handles btnActualizarClima.Click
        ObtenerClimaEnTiempoReal()
    End Sub

    Private Sub ObtenerClimaEnTiempoReal()
        Try
            ' 1. Llamar a la API pública de Clima (En español y en grados Celsius)
            Dim url As String = $"https://api.openweathermap.org/data/2.5/weather?q={CIUDAD_AEROPUERTO}&units=metric&appid={API_KEY}&lang=es"

            Using client As New WebClient()
                client.Encoding = System.Text.Encoding.UTF8
                Dim jsonRespuesta As String = client.DownloadString(url)

                ' 2. Convertir el JSON a objetos de VB.NET
                Dim js As New JavaScriptSerializer()
                Dim datosClima As Dictionary(Of String, Object) = js.Deserialize(Of Dictionary(Of String, Object))(jsonRespuesta)

                ' 3. Extraer Temperatura y Condición
                Dim mainData As Dictionary(Of String, Object) = DirectCast(datosClima("main"), Dictionary(Of String, Object))
                Dim weatherArray As ArrayList = DirectCast(datosClima("weather"), ArrayList)
                Dim weatherData As Dictionary(Of String, Object) = DirectCast(weatherArray(0), Dictionary(Of String, Object))

                Dim temp As Decimal = Convert.ToDecimal(mainData("temp"))
                Dim condicion As String = weatherData("description").ToString()

                ' 4. Mostrar en pantalla
                lblTemperatura.Text = Math.Round(temp, 1).ToString()
                lblCondicion.Text = condicion

                ' 5. Guardar en nuestra Base de Datos (AUR_CLIMA)
                GuardarClimaBD(temp, condicion, 1) ' Enviamos ID_AEROPUERTO = 1

                MostrarMensaje("✅ Clima sincronizado satelitalmente y guardado en la bitácora.", True)
                CargarHistorialDB("1")
            End Using

        Catch ex As WebException
            MostrarMensaje("⚠️ Error de conexión con el satélite (API Key inválida o sin internet).", False)
        Catch ex As Exception
            MostrarMensaje("❌ Error interno: " & ex.Message, False)
        End Try
    End Sub

    Private Sub GuardarClimaBD(temp As Decimal, condicion As String, idAeropuerto As Integer)
        Dim db As New ConexionDB()
        Using conn As OracleConnection = db.ObtenerConexion()
            Using cmd As New OracleCommand("SP_REGISTRAR_CLIMA", conn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.Add("p_temperatura", OracleDbType.Decimal).Value = temp
                cmd.Parameters.Add("p_condicion", OracleDbType.Varchar2).Value = condicion
                cmd.Parameters.Add("p_id_aeropuerto", OracleDbType.Int32).Value = idAeropuerto

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub CargarHistorialDB(idAeropuerto As String)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                ' Un simple query para traer los últimos 10 registros
                Dim sql As String = "SELECT FECHA, TEMPERATURA, CONDICION FROM AUR_CLIMA WHERE ID_AEROPUERTO = :idAero ORDER BY FECHA DESC FETCH NEXT 10 ROWS ONLY"
                Using cmd As New OracleCommand(sql, conn)
                    cmd.Parameters.Add(New OracleParameter("idAero", idAeropuerto))
                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptHistorialClima.DataSource = dt
                        rptHistorialClima.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Silencioso para la carga inicial
        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success fw-bold text-center mb-4", "alert alert-danger fw-bold text-center mb-4")
    End Sub
End Class