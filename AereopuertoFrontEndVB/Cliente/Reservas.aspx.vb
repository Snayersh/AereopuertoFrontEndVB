Imports System.Web.Services

Public Class Reservas
    Inherits System.Web.UI.Page

    Private CorreoUsuario As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = 0
        If Session("IdRol") IsNot Nothing Then idRol = Convert.ToInt32(Session("IdRol"))

        If Session("UserEmail") Is Nothing OrElse (idRol <> 2) Then
            Response.Redirect("~/Account/Login.aspx")
            Return
        End If

        CorreoUsuario = Session("UserEmail").ToString()

        If Not IsPostBack Then
            pnlError.Visible = False
            pnlExito.Visible = False
            CargarVuelosDisponibles()
            CargarClases()
        End If
    End Sub

    Private Sub CargarVuelosDisponibles()
        Dim respuesta = ClienteReservaService.ObtenerVuelosDisponibles()
        If respuesta.success Then
            ddlVuelos.Items.Clear()
            ddlVuelos.Items.Add(New ListItem("-- Selecciona tu vuelo --", ""))
            For Each item In respuesta.vuelos
                ddlVuelos.Items.Add(New ListItem(item("detalle").ToString(), item("id_vuelo").ToString()))
            Next
        Else
            MostrarError(respuesta.mensaje)
        End If
    End Sub

    Private Sub CargarClases()
        Dim respuesta = ClienteReservaService.ObtenerClases()
        If respuesta.success Then
            ddlClase.Items.Clear()
            ddlClase.Items.Add(New ListItem("-- Selecciona la clase --", ""))
            For Each item In respuesta.clases
                ddlClase.Items.Add(New ListItem(item("nombre").ToString(), item("id_tipo_boleto").ToString()))
            Next
            ddlClase.Attributes.Add("onchange", "actualizarFiltroYPrecio();")
        Else
            MostrarError(respuesta.mensaje)
        End If
    End Sub

    Protected Sub ddlVuelos_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlVuelos.SelectedIndexChanged
        If String.IsNullOrEmpty(ddlVuelos.SelectedValue) Then
            panelAvion.Visible = False
            Return
        End If

        GenerarMapaDeAsientos(Convert.ToInt32(ddlVuelos.SelectedValue))
    End Sub

    Private Sub GenerarMapaDeAsientos(idVuelo As Integer)
        ' 🔥 Llamamos al servicio para obtener la data cruda
        Dim respuesta = ClienteReservaService.ObtenerDatosMapaAsientos(idVuelo)

        If Not respuesta.success Then
            MostrarError(respuesta.mensaje)
            panelAvion.Visible = False
            Return
        End If

        ' Ahora que tenemos la data, armamos el HTML localmente
        Dim capacidad As Integer = respuesta.capacidad
        Dim asientosOcupados As List(Of String) = respuesta.ocupados

        Dim htmlAvion As New StringBuilder()
        Dim letras() As String = {"A", "B", "C", "D"}
        Dim totalFilas As Integer = Math.Ceiling(capacidad / 4.0)
        Dim asientoActual As Integer = 1

        Dim limitePrimera As Integer = Math.Ceiling(totalFilas * 0.1)
        Dim limiteEjecutiva As Integer = Math.Ceiling(totalFilas * 0.3)

        For fila As Integer = 1 To totalFilas
            htmlAvion.Append("<div class='seat-row'>")

            Dim claseCSS As String
            Dim precioAsiento As Decimal
            Dim idClaseAsiento As Integer

            If fila <= limitePrimera Then
                claseCSS = "primera"
                precioAsiento = 950.0
                idClaseAsiento = 3
            ElseIf fila <= limiteEjecutiva Then
                claseCSS = "ejecutiva"
                precioAsiento = 550.0
                idClaseAsiento = 2
            Else
                claseCSS = "economica"
                precioAsiento = 250.0
                idClaseAsiento = 1
            End If

            For col As Integer = 0 To 3
                If asientoActual > capacidad Then Exit For
                Dim codigoAsiento As String = fila.ToString() & letras(col)

                If asientosOcupados.Contains(codigoAsiento) Then
                    htmlAvion.Append($"<div class='seat occupied {claseCSS}' onclick=""alert('Ocupado.');"">{codigoAsiento}</div>")
                Else
                    htmlAvion.Append($"<div class='seat available {claseCSS}' data-precio='{precioAsiento}' data-idclase='{idClaseAsiento}' onclick=""seleccionarAsiento(this, '{codigoAsiento}')"">{codigoAsiento}</div>")
                End If

                If col = 1 Then htmlAvion.Append("<div class='aisle'></div>")
                asientoActual += 1
            Next
            htmlAvion.Append("</div>")
        Next

        litMapaAsientos.Text = htmlAvion.ToString()
        panelAvion.Visible = True
        hfAsientoSeleccionado.Value = ""
        ClientScript.RegisterStartupScript(Me.GetType(), "LimpiarAsiento", "document.getElementById('lblAsientoMostrado').innerText = 'Ninguno'; siExisteFuncion('actualizarFiltroYPrecio');", True)
    End Sub

    Protected Sub btnReservar_Click(sender As Object, e As EventArgs) Handles btnReservar.Click
        Dim asientosElegidos As String = hfAsientoSeleccionado.Value

        If String.IsNullOrEmpty(ddlVuelos.SelectedValue) Then
            MostrarError("Debes seleccionar un vuelo.")
            Return
        End If

        If String.IsNullOrEmpty(asientosElegidos) Then
            MostrarError("Haz clic en el mapa para elegir tus asientos.")
            Return
        End If

        Dim listaAsientos As String() = asientosElegidos.Split(","c)

        ' 🔥 Llamamos al servicio centralizado para reservar
        Dim respuesta = ClienteReservaService.ProcesarReservaMasiva(
            CorreoUsuario,
            Convert.ToInt32(ddlVuelos.SelectedValue),
            listaAsientos
        )

        If respuesta.success Then
            pnlError.Visible = False
            ddlVuelos.Enabled = False
            ddlClase.Enabled = False
            btnReservar.Enabled = False
            panelAvion.Visible = False

            lblCodigoBoleto.Text = respuesta.codigo
            lblAsientoConfirmado.Text = respuesta.asientos
            hlPagarAhora.NavigateUrl = "Pagos.aspx?codigo=" & respuesta.codigo
            pnlExito.Visible = True
        Else
            MostrarError("Problemas al reservar: " & respuesta.mensaje)
        End If
    End Sub

    Private Sub MostrarError(mensaje As String)
        pnlExito.Visible = False
        pnlError.Visible = True
        lblError.Text = mensaje
    End Sub

    ' ====================================================================
    ' 5. SINCRONIZACIÓN SILENCIOSA (SignalR / WebMethods)
    ' ====================================================================
    <WebMethod()>
    Public Shared Function ObtenerAsientosOcupados(idVuelo As Integer) As List(Of String)
        Dim respuesta = ClienteReservaService.ObtenerDatosMapaAsientos(idVuelo)
        If respuesta.success Then
            Return CType(respuesta.ocupados, List(Of String))
        End If
        Return New List(Of String)()
    End Function

End Class