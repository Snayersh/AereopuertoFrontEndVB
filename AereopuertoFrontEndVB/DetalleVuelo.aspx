<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DetalleVuelo.aspx.vb" Inherits="AereopuertoFrontEndVB.DetalleVuelo" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Detalles del Vuelo - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; padding-top: 30px; }
        .detail-card { background: white; border-radius: 20px; box-shadow: 0 15px 35px rgba(0,0,0,0.05); padding: 40px; border-top: 8px solid #0d47a1; margin-bottom: 30px; }
        
        .section-title { font-size: 0.9rem; font-weight: 800; letter-spacing: 1.5px; color: #1976d2; text-transform: uppercase; border-bottom: 2px solid #e3f2fd; padding-bottom: 10px; margin-bottom: 25px; }
        .data-label { font-size: 0.8rem; color: #90a4ae; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 3px; }
        .data-value { font-size: 1.1rem; color: #263238; font-weight: 700; margin-bottom: 20px; }
        
        .flight-code { font-size: 3rem; font-weight: 900; color: #0d47a1; letter-spacing: 2px; line-height: 1; }
        
        /* Badges dinámicos */
        .badge-estado { font-size: 1.1rem; padding: 10px 20px; border-radius: 30px; font-weight: 800; text-transform: uppercase; letter-spacing: 1px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .bg-programado { background-color: #fff3e0; color: #e65100; border: 1px solid #ffe0b2; }
        .bg-activo { background-color: #e3f2fd; color: #0d47a1; border: 1px solid #bbdefb; animation: pulse 2s infinite; }
        .bg-finalizado { background-color: #e8f5e9; color: #2e7d32; border: 1px solid #c8e6c9; }
        .bg-retrasado { background-color: #ffe0b2; color: #f57c00; border: 1px solid #ffcc80; }
        .bg-cancelado { background-color: #ffcdd2; color: #c62828; text-decoration: line-through; }

        @keyframes pulse { 0% { box-shadow: 0 0 0 0 rgba(13, 71, 161, 0.4); } 70% { box-shadow: 0 0 0 10px rgba(13, 71, 161, 0); } 100% { box-shadow: 0 0 0 0 rgba(13, 71, 161, 0); } }

        /* Estilos de la ruta del vuelo */
        .route-container { background: #f8f9fa; border-radius: 15px; padding: 30px; margin-bottom: 30px; border: 1px dashed #cfd8dc; }
        .iata-code { font-size: 2.5rem; font-weight: 900; color: #2c3e50; }
        .progress-container { position: relative; height: 12px; background-color: #e9ecef; border-radius: 10px; margin: 20px 0; overflow: visible; }
        .progress-bar-custom { background: linear-gradient(90deg, #1976d2, #4fc3f7); height: 100%; border-radius: 10px; position: relative; transition: width 1s linear; }
        .airplane-icon { position: absolute; right: -15px; top: -12px; font-size: 24px; text-shadow: 2px 2px 4px rgba(0,0,0,0.2); }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hfSalidaISO" runat="server" />
        <asp:HiddenField ID="hfLlegadaISO" runat="server" />
        <asp:HiddenField ID="hfEstadoActual" runat="server" />

        <div class="container">
            <div class="mb-4">
                <a href="Default.aspx" class="btn btn-outline-secondary fw-bold rounded-pill px-4 shadow-sm">← Volver al Radar / Tablero</a>
            </div>

            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger text-center rounded-3 shadow py-5">
                <h1 style="font-size: 4rem;">⚠️</h1>
                <h3 class="fw-bold mt-3">Vuelo no encontrado</h3>
                <p class="text-muted">No pudimos cargar la información de este vuelo en la base de datos.</p>
            </asp:Panel>

            <asp:Panel ID="pnlDetalle" runat="server">
                <div class="detail-card">
                    <div class="d-flex justify-content-between align-items-center flex-wrap mb-4">
                        <div>
                            <div class="data-label text-primary">Vuelo Operado por <asp:Label ID="lblAerolineaHead" runat="server" CssClass="fw-bold text-dark"></asp:Label></div>
                            <div class="flight-code"><asp:Label ID="lblCodigoVuelo" runat="server"></asp:Label></div>
                        </div>
                        <div class="text-end mt-3 mt-md-0">
                            <div class="data-label mb-2">Estado Actual</div>
                            <asp:Label ID="lblEstado" runat="server"></asp:Label>
                        </div>
                    </div>

                    <div class="route-container">
                        <div class="d-flex justify-content-between align-items-end mb-2">
                            <div class="text-start">
                                <div class="iata-code"><asp:Label ID="lblOrigenIata" runat="server"></asp:Label></div>
                                <div class="fw-bold text-secondary"><asp:Label ID="lblOrigenCiudad" runat="server"></asp:Label></div>
                            </div>
                            <div class="text-center flex-grow-1 px-4">
                                <span class="badge bg-primary rounded-pill px-3 mb-2"><asp:Label ID="lblDuracion" runat="server"></asp:Label> Total</span>
                            </div>
                            <div class="text-end">
                                <div class="iata-code"><asp:Label ID="lblDestinoIata" runat="server"></asp:Label></div>
                                <div class="fw-bold text-secondary"><asp:Label ID="lblDestinoCiudad" runat="server"></asp:Label></div>
                            </div>
                        </div>

                        <div class="progress-container">
                            <div id="flightProgressBar" class="progress-bar-custom" style="width: 0%;">
                                <div class="airplane-icon">✈️</div>
                            </div>
                        </div>

                        <div class="d-flex justify-content-between mt-2">
                            <small id="txtTiempoTranscurrido" class="fw-bold text-success">Calculando...</small>
                            <small id="txtTiempoRestante" class="fw-bold text-danger">Calculando...</small>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-6 pe-md-5">
                            <div class="section-title">🕒 Itinerario de Vuelo</div>
                            
                            <div class="row">
                                <div class="col-6">
                                    <div class="data-label">Salida</div>
                                    <div class="data-value fs-5"><asp:Label ID="lblFechaSalida" runat="server"></asp:Label></div>
                                </div>
                                <div class="col-6 text-end">
                                    <div class="data-label">Llegada Estimada</div>
                                    <div class="data-value fs-5 text-primary"><asp:Label ID="lblFechaLlegada" runat="server"></asp:Label></div>
                                </div>
                            </div>

                            <hr class="text-muted opacity-25" />

                            <div class="data-label mt-3">📍 Terminal de Origen</div>
                            <div class="data-value fs-6 text-muted"><asp:Label ID="lblOrigenAero" runat="server"></asp:Label>, <asp:Label ID="lblOrigenPais" runat="server"></asp:Label></div>

                            <div class="data-label">📍 Terminal de Destino</div>
                            <div class="data-value fs-6 text-muted mb-0"><asp:Label ID="lblDestinoAero" runat="server"></asp:Label>, <asp:Label ID="lblDestinoPais" runat="server"></asp:Label></div>
                        </div>

                        <div class="col-md-6 border-start ps-md-5 mt-4 mt-md-0">
                            <div class="section-title">⚙️ Datos Operativos</div>

                            <div class="data-label">🏢 Aerolínea Responsable</div>
                            <div class="data-value"><asp:Label ID="lblAerolinea" runat="server"></asp:Label></div>

                            <div class="data-label">🛩️ Modelo de Aeronave</div>
                            <div class="data-value"><asp:Label ID="lblAeronaveModelo" runat="server"></asp:Label></div>

                            <div class="data-label">👥 Pasajeros Permitidos</div>
                            <div class="data-value mb-0"><asp:Label ID="lblCapacidad" runat="server"></asp:Label> asientos máximo</div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </form>

    <script>
        function actualizarProgreso() {
            let salidaStr = document.getElementById('<%= hfSalidaISO.ClientID %>').value;
            let llegadaStr = document.getElementById('<%= hfLlegadaISO.ClientID %>').value;
            let estado = document.getElementById('<%= hfEstadoActual.ClientID %>').value;

            if (!salidaStr || !llegadaStr) return;

            let tSalida = new Date(salidaStr).getTime();
            let tLlegada = new Date(llegadaStr).getTime();
            let ahora = new Date().getTime();

            let bar = document.getElementById('flightProgressBar');
            let txtTranscurrido = document.getElementById('txtTiempoTranscurrido');
            let txtRestante = document.getElementById('txtTiempoRestante');

            // Lógica si el vuelo fue cancelado
            if (estado === "CANCELADO") {
                bar.style.width = '0%';
                bar.style.background = '#e53935';
                txtTranscurrido.innerText = "Vuelo Cancelado";
                txtRestante.innerText = "--";
                return;
            }

            // Lógica de cálculo de tiempo
            if (ahora < tSalida || estado === "PROGRAMADO") {
                bar.style.width = '0%';
                txtTranscurrido.innerText = "Aún no despega";

                let diff = tSalida - ahora;
                if (diff > 0) {
                    txtRestante.innerText = "Despega en: " + formatMsToHM(diff);
                } else {
                    txtRestante.innerText = "Retrasado en tierra";
                }

            } else if (ahora > tLlegada || estado === "ATERRIZÓ" || estado === "ATERRIZADO" || estado === "FINALIZADO") {
                bar.style.width = '100%';
                bar.style.background = '#43a047'; // Verde
                txtTranscurrido.innerText = "Vuelo Finalizado";
                txtRestante.innerText = "¡Aterrizó con éxito!";

            } else {
                // EL VUELO ESTÁ EN EL AIRE
                let duracionTotal = tLlegada - tSalida;
                let tiempoPasado = ahora - tSalida;
                let tiempoFalta = tLlegada - ahora;

                let porcentaje = (tiempoPasado / duracionTotal) * 100;

                bar.style.width = porcentaje + '%';
                txtTranscurrido.innerText = "En el aire: " + formatMsToHM(tiempoPasado);
                txtRestante.innerText = "Aterriza en: " + formatMsToHM(tiempoFalta);
            }
        }

        // Función para convertir milisegundos a "2h 15m"
        function formatMsToHM(ms) {
            let totalMinutos = Math.floor(ms / 60000);
            let horas = Math.floor(totalMinutos / 60);
            let minutos = totalMinutos % 60;
            return horas + "h " + minutos + "m";
        }

        // Ejecutar inmediatamente y luego cada 30 segundos
        actualizarProgreso();
        setInterval(actualizarProgreso, 30000);
    </script>
</body>
</html>