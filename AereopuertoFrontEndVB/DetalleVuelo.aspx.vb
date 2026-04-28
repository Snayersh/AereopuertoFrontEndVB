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
        ' 🔥 Llamamos al nuevo servicio
        Dim respuesta = DashboardService.ObtenerDetalleVuelo(id)

        If respuesta.success Then
            Dim row = respuesta.datos ' Diccionario con llaves en minúscula

            ' Cabecera y Estado
            lblCodigoVuelo.Text = row("codigo_vuelo").ToString().ToUpper()
            lblAerolineaHead.Text = row("aerolinea").ToString()
            lblAerolinea.Text = row("aerolinea").ToString()

            Dim estadoActual As String = row("estado_vuelo").ToString()
            lblEstado.Text = estadoActual
            ConfigurarColorEstado(estadoActual)

            ' Origen y Destino
            lblOrigenCiudad.Text = row("origen_ciudad").ToString()
            lblOrigenIata.Text = row("origen_iata").ToString()
            lblOrigenAero.Text = row("origen_aeropuerto").ToString()
            lblOrigenPais.Text = row("origen_pais").ToString()

            lblDestinoCiudad.Text = row("destino_ciudad").ToString()
            lblDestinoIata.Text = row("destino_iata").ToString()
            lblDestinoAero.Text = row("destino_aeropuerto").ToString()
            lblDestinoPais.Text = row("destino_pais").ToString()

            ' Tiempos y Aeronave
            Dim fechaSalida As DateTime = Convert.ToDateTime(row("fecha_salida"))
            Dim fechaLlegada As DateTime = Convert.ToDateTime(row("fecha_llegada"))
            lblFechaSalida.Text = fechaSalida.ToString("dd MMM yyyy - HH:mm")
            lblFechaLlegada.Text = fechaLlegada.ToString("dd MMM yyyy - HH:mm")

            lblAeronaveModelo.Text = row("aeronave_modelo").ToString()
            lblCapacidad.Text = row("aeronave_capacidad").ToString()

            Dim duracion As TimeSpan = fechaLlegada - fechaSalida
            lblDuracion.Text = $"{duracion.Hours}h {duracion.Minutes}m"

            ' Datos para JS
            hfSalidaISO.Value = fechaSalida.ToString("yyyy-MM-ddTHH:mm:ss")
            hfLlegadaISO.Value = fechaLlegada.ToString("yyyy-MM-ddTHH:mm:ss")
            hfEstadoActual.Value = estadoActual.ToUpper()

            ' ========================================================
            ' LÓGICA DE ESCALAS (Refactorizada)
            ' ========================================================
            If row("escala_iata") IsNot Nothing Then
                lblEscalaIataMap.Text = row("escala_iata").ToString()
                lblEscalaCiudadMap.Text = row("escala_ciudad").ToString()
                lblEscalaAero.Text = row("escala_aeropuerto").ToString()
                lblEscalaPais.Text = row("escala_pais").ToString()

                lblEscalaHoraLlegada.Text = Convert.ToDateTime(row("escala_llegada")).ToString("HH:mm")
                lblEscalaHoraSalida.Text = Convert.ToDateTime(row("escala_salida")).ToString("HH:mm")

                pnlEscalaMap.Visible = True
                pnlEscalaDetalle.Visible = True
            Else
                pnlEscalaMap.Visible = False
                pnlEscalaDetalle.Visible = False
            End If

            pnlDetalle.Visible = True
            pnlError.Visible = False
        Else
            pnlDetalle.Visible = False
            pnlError.Visible = True
        End If
    End Sub

    Private Sub ConfigurarColorEstado(estado As String)
        estado = estado.ToUpper()
        ' Mantienes tu lógica de CSS aquí ya que es puramente visual de la web
        Select Case estado
            Case "PROGRAMADO" : lblEstado.CssClass = "badge-estado bg-programado"
            Case "ATERRIZÓ", "FINALIZADO", "ATERRIZADO" : lblEstado.CssClass = "badge-estado bg-finalizado"
            Case "RETRASADO" : lblEstado.CssClass = "badge-estado bg-retrasado"
            Case "CANCELADO" : lblEstado.CssClass = "badge-estado bg-cancelado"
            Case Else : lblEstado.CssClass = "badge-estado bg-activo"
        End Select
    End Sub
End Class