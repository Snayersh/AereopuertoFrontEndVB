Imports System.Web
Imports System.Web.Script.Serialization

Public Class ApiMovil
    Implements IHttpHandler

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/json"
        context.Response.ContentEncoding = System.Text.Encoding.UTF8
        context.Response.AppendHeader("Access-Control-Allow-Origin", "*")

        Dim jsSerializer As New JavaScriptSerializer()
        Dim jsonResponse As String = ""

        Try
            Dim action As String = context.Request("action")

            If String.IsNullOrEmpty(action) Then
                Throw New Exception("Acción no especificada.")
            End If

            ' 🚦 EL GRAN ENRUTADOR DE LA API
            Select Case action.ToLower()

                ' ==========================================
                ' 0. MÓDULO PÚBLICO (DASHBOARD / RADAR)
                ' ==========================================
                Case "radar_vuelos"
                    ' Este endpoint devuelve las stats y la tabla de vuelos en vivo
                    jsonResponse = jsSerializer.Serialize(DashboardService.ObtenerRadarEnVivo())

                ' ==========================================
                ' 1. MÓDULO ACCOUNT (SEGURIDAD)
                ' ==========================================
                Case "login"
                    Dim email As String = context.Request("email")
                    Dim pass As String = context.Request("password")
                    jsonResponse = jsSerializer.Serialize(AccountLoginService.IniciarSesion(email, pass))

                Case "validar_sesion"
                    Dim email As String = context.Request("email")
                    Dim token As String = context.Request("token")
                    jsonResponse = jsSerializer.Serialize(AccountValidacionService.ValidarTokenSesion(email, token))

                Case "registro"
                    ' Convertimos la fecha de string a DateTime
                    Dim fechaNac As DateTime = Convert.ToDateTime(context.Request("fechaNacimiento"))
                    jsonResponse = jsSerializer.Serialize(AccountRegistroService.RegistrarNuevoCliente(
                        context.Request("pNombre"), context.Request("sNombre"), context.Request("tNombre"),
                        context.Request("pApellido"), context.Request("sApellido"), context.Request("aCasada"),
                        fechaNac, context.Request("telefono"), context.Request("email"),
                        context.Request("pais"), context.Request("departamento"), context.Request("municipio"),
                        context.Request("zona"), context.Request("colonia"), context.Request("calle"),
                        context.Request("avenida"), context.Request("numCasa"), context.Request("pasaporte"),
                        context.Request("password")
                    ))

                Case "activar_cuenta"
                    Dim token As String = context.Request("token")
                    jsonResponse = jsSerializer.Serialize(AccountActivationService.ActivarCuentaUsuario(token))

                Case "solicitar_recuperacion"
                    Dim email As String = context.Request("email")
                    jsonResponse = jsSerializer.Serialize(AccountPasswordService.SolicitarRecuperacion(email))

                Case "actualizar_password"
                    Dim token As String = context.Request("token")
                    Dim nuevaPass As String = context.Request("password")
                    jsonResponse = jsSerializer.Serialize(AccountPasswordService.ActualizarPasswordConToken(token, nuevaPass))

                ' ==========================================
                ' 2. MÓDULO PERFIL DE USUARIO
                ' ==========================================
                Case "obtener_perfil"
                    Dim email As String = context.Request("email")
                    jsonResponse = jsSerializer.Serialize(ClientePerfilService.ObtenerPerfil(email))

                Case "actualizar_perfil"
                    jsonResponse = jsSerializer.Serialize(ClientePerfilService.ActualizarPerfil(
                        context.Request("email"), context.Request("pNombre"), context.Request("sNombre"),
                        context.Request("pApellido"), context.Request("sApellido"), context.Request("telefono")
                    ))

                ' ==========================================
                ' 3. MÓDULO VUELOS Y RESERVAS
                ' ==========================================
                Case "vuelos_disponibles"
                    jsonResponse = jsSerializer.Serialize(ClienteReservaService.ObtenerVuelosDisponibles())

                Case "clases_disponibles"
                    jsonResponse = jsSerializer.Serialize(ClienteReservaService.ObtenerClases())

                Case "mapa_asientos"
                    Dim idVuelo As Integer = Convert.ToInt32(context.Request("idVuelo"))
                    jsonResponse = jsSerializer.Serialize(ClienteReservaService.ObtenerDatosMapaAsientos(idVuelo))

                Case "procesar_reserva"
                    Dim email As String = context.Request("email")
                    Dim idVuelo As Integer = Convert.ToInt32(context.Request("idVuelo"))
                    ' React Native mandará los asientos separados por coma, ej: "1A:3,12B:1"
                    Dim asientosArray As String() = context.Request("asientos").Split(","c)
                    jsonResponse = jsSerializer.Serialize(ClienteReservaService.ProcesarReservaMasiva(email, idVuelo, asientosArray))

                ' ==========================================
                ' 4. MÓDULO DE PAGOS Y FACTURACIÓN
                ' ==========================================
                Case "procesar_pago"
                    Dim email As String = context.Request("email")
                    Dim codigoReserva As String = context.Request("codigoReserva")
                    Dim idMetodoPago As Integer = Convert.ToInt32(context.Request("idMetodoPago"))
                    jsonResponse = jsSerializer.Serialize(ClientePagoService.ProcesarPago(codigoReserva, email, idMetodoPago))

                Case "mis_facturas"
                    Dim email As String = context.Request("email")
                    jsonResponse = jsSerializer.Serialize(ClienteFacturaService.ObtenerMisFacturas(email))

                Case "detalle_factura"
                    Dim idFactura As Integer = Convert.ToInt32(context.Request("idFactura"))
                    jsonResponse = jsSerializer.Serialize(ClienteFacturaService.ObtenerDetalleFactura(idFactura))

                ' ==========================================
                ' 5. MÓDULO MIS BOLETOS Y PASE DE ABORDAR
                ' ==========================================
                Case "mis_boletos"
                    Dim email As String = context.Request("email")
                    Dim estadoFiltro As Integer = Convert.ToInt32(context.Request("filtro"))
                    jsonResponse = jsSerializer.Serialize(ClienteBoletoService.ObtenerMisBoletos(email, estadoFiltro))

                Case "cancelar_reserva"
                    Dim email As String = context.Request("email")
                    Dim codigoReserva As String = context.Request("codigoReserva")
                    jsonResponse = jsSerializer.Serialize(ClienteBoletoService.CancelarReserva(codigoReserva, email))

                Case "pase_abordar"
                    Dim email As String = context.Request("email")
                    Dim codigoReserva As String = context.Request("codigoReserva")
                    jsonResponse = jsSerializer.Serialize(ClientePaseAbordarService.ObtenerPasesDeAbordar(codigoReserva, email))

                ' ==========================================
                ' 6. MÓDULO DE EQUIPAJE
                ' ==========================================
                Case "tipos_equipaje"
                    jsonResponse = jsSerializer.Serialize(ClienteEquipajeService.ObtenerTiposEquipaje())

                Case "boletos_equipaje"
                    Dim email As String = context.Request("email")
                    jsonResponse = jsSerializer.Serialize(ClienteEquipajeService.ObtenerBoletosDisponibles(email))

                Case "listar_equipaje"
                    Dim codigoBoleto As String = context.Request("codigoBoleto")
                    jsonResponse = jsSerializer.Serialize(ClienteEquipajeService.ObtenerEquipajeRegistrado(codigoBoleto))

                Case "registrar_equipaje"
                    Dim codigoBoleto As String = context.Request("codigoBoleto")
                    Dim peso As Decimal = Convert.ToDecimal(context.Request("peso"))
                    Dim desc As String = context.Request("descripcion")
                    Dim idTipo As Integer = Convert.ToInt32(context.Request("idTipo"))
                    jsonResponse = jsSerializer.Serialize(ClienteEquipajeService.RegistrarNuevaMaleta(codigoBoleto, peso, desc, idTipo))

                Case "rastrear_equipaje"
                    Dim idEquipaje As Integer = Convert.ToInt32(context.Request("idEquipaje"))
                    jsonResponse = jsSerializer.Serialize(ClienteEquipajeService.RastrearEquipaje(idEquipaje))

                ' ==========================================
                ' 7. MÓDULO DE RECLAMOS
                ' ==========================================
                Case "equipaje_cliente_reclamos"
                    Dim email As String = context.Request("email")
                    jsonResponse = jsSerializer.Serialize(ClienteReclamoService.ObtenerEquipajeCliente(email))

                Case "registrar_reclamo"
                    Dim idEquipaje As Integer = Convert.ToInt32(context.Request("idEquipaje"))
                    Dim descripcion As String = context.Request("descripcion")
                    jsonResponse = jsSerializer.Serialize(ClienteReclamoService.RegistrarReclamo(idEquipaje, descripcion))

                Case "historial_reclamos"
                    Dim email As String = context.Request("email")
                    jsonResponse = jsSerializer.Serialize(ClienteReclamoService.ObtenerHistorialReclamos(email))

                ' ==========================================
                ' 8. MÓDULO DE SOPORTE Y TICKETS
                ' ==========================================
                Case "tipos_ticket"
                    jsonResponse = jsSerializer.Serialize(ClienteTicketService.ObtenerTiposTicket())

                Case "listar_tickets"
                    Dim email As String = context.Request("email")
                    jsonResponse = jsSerializer.Serialize(ClienteTicketService.ObtenerTicketsCliente(email))

                Case "crear_ticket"
                    Dim email As String = context.Request("email")
                    Dim asunto As String = context.Request("asunto")
                    Dim idTipo As Integer = Convert.ToInt32(context.Request("idTipo"))
                    jsonResponse = jsSerializer.Serialize(ClienteTicketService.CrearTicket(asunto, email, idTipo))

                Case "hilo_respuestas_ticket"
                    Dim idTicket As Integer = Convert.ToInt32(context.Request("idTicket"))
                    jsonResponse = jsSerializer.Serialize(ClienteTicketService.ObtenerHiloRespuestas(idTicket))

                Case "responder_ticket"
                    Dim idTicket As Integer = Convert.ToInt32(context.Request("idTicket"))
                    Dim mensaje As String = context.Request("mensaje")
                    jsonResponse = jsSerializer.Serialize(ClienteTicketService.AgregarRespuesta(idTicket, mensaje))


                Case "detalle_vuelo"
                    Dim idVuelo As Integer = Convert.ToInt32(context.Request("id"))
                    jsonResponse = jsSerializer.Serialize(DashboardService.ObtenerDetalleVuelo(idVuelo))


                Case "mapa_coordenadas_radar"
                    jsonResponse = jsSerializer.Serialize(DashboardService.ObtenerVuelosParaRadar())
                    ' ==========================================
                    ' SI LA ACCIÓN NO EXISTE
                    ' ==========================================
                Case Else
                    Dim errorObj = New With {.success = False, .mensaje = "Acción '" & action & "' desconocida o no programada."}
                    jsonResponse = jsSerializer.Serialize(errorObj)
            End Select

        Catch ex As Exception
            ' Si falla una conversión (ej. mandan letras en vez de un ID), lo atrapamos aquí
            Dim errorObj = New With {.success = False, .mensaje = "Error del servidor: " & ex.Message}
            jsonResponse = jsSerializer.Serialize(errorObj)
        End Try

        ' Disparamos la respuesta de vuelta al celular (React Native)
        context.Response.Write(jsonResponse)
    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class