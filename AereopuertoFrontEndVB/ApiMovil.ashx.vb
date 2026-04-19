Imports System.Web
Imports System.Web.Script.Serialization
Imports System.IO
Imports System.Collections.Generic

Public Class ApiMovil
    Implements IHttpHandler

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/json"
        context.Response.AddHeader("Access-Control-Allow-Origin", "*")
        context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS")
        context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type")

        If context.Request.HttpMethod = "OPTIONS" Then
            context.Response.End()
            Return
        End If

        Dim js As New JavaScriptSerializer()
        Dim respuesta As Object = Nothing

        Try
            Dim jsonString As String = New StreamReader(context.Request.InputStream).ReadToEnd()
            Dim datosReq As Dictionary(Of String, Object) = Nothing

            If Not String.IsNullOrEmpty(jsonString) Then
                datosReq = js.Deserialize(Of Dictionary(Of String, Object))(jsonString)
            End If

            Dim ObtenerParam = Function(llave As String) As String
                                   If datosReq IsNot Nothing AndAlso datosReq.ContainsKey(llave) Then
                                       Return datosReq(llave).ToString()
                                   End If
                                   Return context.Request(llave)
                               End Function

            Dim accion As String = ObtenerParam("accion")

            ' 🔥 1. CAPTURAMOS EL TOKEN QUE VENGA DE REACT NATIVE
            Dim tokenRecibido As String = ObtenerParam("token")

            ' 🔥 2. LISTA DE ACCIONES PÚBLICAS (No necesitan token)
            Dim accionesPublicas As New List(Of String)({"login", "registrar_usuario", "recuperar_password", "radar_vivo", "radar_mapa", "radar_estadisticas"})

            ' 🔥 3. EL GUARDIA DE SEGURIDAD
            If Not accionesPublicas.Contains(accion) Then
                ' Buscamos el correo (algunas veces lo mandas como correo y otras como correo_usuario)
                Dim emailSeguridad As String = ObtenerParam("correo")
                If String.IsNullOrEmpty(emailSeguridad) Then emailSeguridad = ObtenerParam("correo_usuario")

                ' Si no mandó correo o token, no lo dejamos pasar y enviamos el chisme
                If String.IsNullOrEmpty(emailSeguridad) OrElse String.IsNullOrEmpty(tokenRecibido) Then
                    context.Response.Write(js.Serialize(New With {
                        .success = False,
                        .mensaje = "SESION_EXPIRADA",
                        .debug_info = "Falta dato. Correo recibido: '" & emailSeguridad & "' | Token recibido: '" & tokenRecibido & "'"
                    }))
                    Return
                End If

                ' Validamos en la Base de Datos y si falla, mandamos el chisme
                Dim validacion As String = AuthService.ValidarToken(emailSeguridad, tokenRecibido)
                If validacion <> "EXITO" Then
                    context.Response.Write(js.Serialize(New With {
                        .success = False,
                        .mensaje = "SESION_EXPIRADA",
                        .debug_info = "Oracle dijo: '" & validacion & "' | Token de la app: '" & tokenRecibido & "'"
                    }))
                    Return
                End If
            End If

            ' Si pasamos el guardia, ejecutamos el semáforo normalmente
            Select Case accion
                Case "login"
                    respuesta = AuthService.ValidarUsuario(ObtenerParam("correo"), ObtenerParam("password"))
                Case "perfil_obtener"
                    respuesta = ClienteService.ObtenerPerfil(ObtenerParam("correo"))
                Case "perfil_actualizar"
                    respuesta = ClienteService.ActualizarPerfil(ObtenerParam("correo"), ObtenerParam("primer_nombre"), ObtenerParam("primer_apellido"), ObtenerParam("telefono"))
                Case "vuelos_disponibles"
                    respuesta = VueloService.ListarVuelosActivos()
                Case "clases_disponibles"
                    respuesta = VueloService.ListarClases()
                Case "mapa_asientos"
                    respuesta = VueloService.ObtenerMapaAsientos(ObtenerParam("id_vuelo"))
                Case "crear_reserva"
                    Dim correo = ObtenerParam("correo")
                    Dim idVuelo = ObtenerParam("id_vuelo")
                    Dim asientosArray = datosReq("asientos")
                    respuesta = ReservaService.CrearReservaMasiva(correo, idVuelo, asientosArray)
                Case "procesar_pago"
                    respuesta = PagoService.ProcesarPago(ObtenerParam("codigo_reserva"), ObtenerParam("correo_usuario"))
                Case "mis_facturas"
                    respuesta = PagoService.ListarFacturas(ObtenerParam("correo"))
                Case "detalle_factura"
                    respuesta = PagoService.ObtenerDetalleFactura(ObtenerParam("id_factura"))
                Case "equipaje_boletos"
                    respuesta = EquipajeService.ObtenerBoletosPagados(ObtenerParam("correo"))
                Case "equipaje_registrar"
                    respuesta = EquipajeService.RegistrarMaleta(ObtenerParam("codigo_boleto"), ObtenerParam("peso"), ObtenerParam("descripcion"), ObtenerParam("tipoEquipaje"))
                Case "equipaje_lista"
                    respuesta = EquipajeService.ListarMaletasPorBoleto(ObtenerParam("codigo_boleto"))
                Case "radar_estadisticas"
                    respuesta = RadarService.ObtenerEstadisticasDashboard()
                Case "detalle_vuelo"
                    respuesta = RadarService.ObtenerDetalleVueloEspecifico(ObtenerParam("id"))
                Case "recuperar_password"
                    respuesta = AuthService.RecuperarPassword(ObtenerParam("correo"))
                Case "registrar_usuario"
                    respuesta = AuthService.RegistrarUsuario(ObtenerParam("primer_nombre"), ObtenerParam("primer_apellido"), ObtenerParam("pasaporte"), ObtenerParam("telefono"), ObtenerParam("pais"), ObtenerParam("correo"), ObtenerParam("password"), ObtenerParam("fecha_nacimiento"))
                Case "obtener_pase_abordar"
                    respuesta = OperacionesService.ObtenerPaseAbordar(ObtenerParam("codigo"), ObtenerParam("correo"))
                Case "mis_boletos"
                    respuesta = ReservaService.ObtenerMisBoletos(ObtenerParam("correo"), ObtenerParam("estado"))
                Case "cancelar_reserva"
                    respuesta = ReservaService.CancelarReserva(ObtenerParam("id_boleto"), ObtenerParam("correo"))

                Case "radar_vivo"
                    respuesta = RadarService.ObtenerVuelosEnVivo()
                Case "radar_mapa"
                    respuesta = RadarService.ObtenerVuelosMapa()

                Case Else
                    respuesta = New With {.success = False, .mensaje = "Acción no reconocida por el servidor."}
            End Select

        Catch ex As Exception
            respuesta = New With {.success = False, .mensaje = "Error interno del servidor: " & ex.Message}
        End Try

        context.Response.Write(js.Serialize(respuesta))
    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class