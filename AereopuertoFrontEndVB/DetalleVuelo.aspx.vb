Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class DetalleVuelo
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim idVueloString As String = Request.QueryString("id")
            Dim idVuelo As Integer

            If Not String.IsNullOrEmpty(idVueloString) AndAlso Integer.TryParse(idVueloString, idVuelo) Then
                CargarDatosDelVuelo(idVuelo)
            Else
                pnlDetalle.Visible = False
                pnlError.Visible = True
            End If
        End If
    End Sub

    Private Sub CargarDatosDelVuelo(id As Integer)
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_DETALLE_VUELO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = id

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()

                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            Dim row As DataRow = dt.Rows(0)

                            ' Cabecera
                            lblCodigoVuelo.Text = row("codigo_vuelo").ToString().ToUpper()
                            lblAerolineaHead.Text = row("aerolinea").ToString()
                            lblAerolinea.Text = row("aerolinea").ToString()

                            Dim estadoActual As String = row("estado_vuelo").ToString()
                            lblEstado.Text = estadoActual
                            ConfigurarColorEstado(estadoActual)

                            ' Origen
                            lblOrigenCiudad.Text = row("origen_ciudad").ToString()
                            lblOrigenIata.Text = row("origen_iata").ToString()
                            lblOrigenAero.Text = row("origen_aeropuerto").ToString()
                            lblOrigenPais.Text = row("origen_pais").ToString()

                            ' Destino
                            lblDestinoCiudad.Text = row("destino_ciudad").ToString()
                            lblDestinoIata.Text = row("destino_iata").ToString()
                            lblDestinoAero.Text = row("destino_aeropuerto").ToString()
                            lblDestinoPais.Text = row("destino_pais").ToString()

                            ' Fechas y Tiempos
                            Dim fechaSalida As DateTime = Convert.ToDateTime(row("fecha_salida"))
                            Dim fechaLlegada As DateTime = Convert.ToDateTime(row("fecha_llegada"))

                            lblFechaSalida.Text = fechaSalida.ToString("dd MMM yyyy - HH:mm")
                            lblFechaLlegada.Text = fechaLlegada.ToString("dd MMM yyyy - HH:mm")

                            lblAeronaveModelo.Text = row("aeronave_modelo").ToString()
                            lblCapacidad.Text = row("aeronave_capacidad").ToString()

                            Dim duracion As TimeSpan = fechaLlegada - fechaSalida
                            lblDuracion.Text = $"{duracion.Hours}h {duracion.Minutes}m"

                            ' MANDAMOS DATOS A JAVASCRIPT PARA EL CRONÓMETRO EN VIVO
                            hfSalidaISO.Value = fechaSalida.ToString("yyyy-MM-ddTHH:mm:ss")
                            hfLlegadaISO.Value = fechaLlegada.ToString("yyyy-MM-ddTHH:mm:ss")
                            hfEstadoActual.Value = estadoActual.ToUpper()

                            pnlDetalle.Visible = True
                            pnlError.Visible = False
                        Else
                            pnlDetalle.Visible = False
                            pnlError.Visible = True
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            pnlDetalle.Visible = False
            pnlError.Visible = True
        End Try
    End Sub

    Private Sub ConfigurarColorEstado(estado As String)
        estado = estado.ToUpper()
        If estado = "PROGRAMADO" Then
            lblEstado.CssClass = "badge-estado bg-programado"
        ElseIf estado = "ATERRIZÓ" Or estado = "FINALIZADO" Or estado = "ATERRIZADO" Then
            lblEstado.CssClass = "badge-estado bg-finalizado"
        ElseIf estado = "RETRASADO" Then
            lblEstado.CssClass = "badge-estado bg-retrasado"
        ElseIf estado = "CANCELADO" Then
            lblEstado.CssClass = "badge-estado bg-cancelado"
        Else
            lblEstado.CssClass = "badge-estado bg-activo"
        End If
    End Sub
End Class