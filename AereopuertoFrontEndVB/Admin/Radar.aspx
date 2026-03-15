<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Radar.aspx.vb" Inherits="AereopuertoFrontEndVB.Radar" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Radar de Vuelos en Vivo</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    
    <link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" />
    <script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>

    <style>
        body, html { margin: 0; padding: 0; height: 100%; background-color: #121212; font-family: 'Segoe UI', sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; z-index: 1000; position: relative; box-shadow: 0 4px 10px rgba(0,0,0,0.5); }
        
        /* El contenedor del mapa ocupará toda la pantalla debajo de la barra */
        #mapaRadar { height: calc(100vh - 65px); width: 100%; z-index: 1; }
        
        /* Estilo para las etiquetas de los aviones */
        .flight-label { background: rgba(0, 0, 0, 0.7); color: #00ff00; border: 1px solid #00ff00; padding: 2px 6px; border-radius: 4px; font-weight: bold; font-size: 11px; white-space: nowrap; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">📡 Radar de Seguimiento de Vuelos (Live)</h4>
            <div>
                <span class="badge bg-success me-3">🟢 Actualización automática cada 5s</span>
                <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Dashboard</a>
            </div>
        </div>

        <div id="mapaRadar"></div>

        <asp:HiddenField ID="hfDatosVuelos" runat="server" />
    </form>

    <script>
        // 1. INICIALIZAR EL MAPA CENTRADO EN GUATEMALA
        var map = L.map('mapaRadar').setView([15.5, -90.25], 6); // Coordenadas centro de GUA, zoom 6

        // 2. AGREGAR LA CAPA DEL MAPA OSCURO (Estilo Radar)
        L.tileLayer('https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png', {
            attribution: '&copy; OpenStreetMap contributors &copy; CARTO'
        }).addTo(map);

        // Ícono personalizado del avión
        var avionIcon = L.divIcon({
            html: '<div style="font-size: 24px; color: #00ff00; transform: rotate(-45deg);">✈️</div>',
            className: 'dummy',
            iconSize: [24, 24],
            iconAnchor: [12, 12]
        });

        var marcadoresAviones = {}; // Guardaremos los aviones aquí para moverlos

        function actualizarRadar() {
            // Leer los datos que nos mandó VB.NET
            var rawData = document.getElementById('<%= hfDatosVuelos.ClientID %>').value;
            if (!rawData) return;
            
            var vuelos = JSON.parse(rawData);
            var ahora = new Date().getTime();

            vuelos.forEach(function(v) {
                var tSalida = new Date(v.fecha_salida).getTime();
                var tLlegada = new Date(v.fecha_llegada).getTime();
                
                var latActual, lngActual;
                
                // CÁLCULO MATEMÁTICO DEL PROGRESO
                if (ahora < tSalida || v.id_estado_vuelo == 1) {
                    // Aún no despega (Está en el origen)
                    latActual = v.orig_lat;
                    lngActual = v.orig_lng;
                } else if (ahora > tLlegada) {
                    // Ya llegó (Está en el destino)
                    latActual = v.dest_lat;
                    lngActual = v.dest_lng;
                } else {
                    // ESTÁ EN EL AIRE: Calculamos el porcentaje del viaje
                    var duracionTotal = tLlegada - tSalida;
                    var tiempoTranscurrido = ahora - tSalida;
                    var porcentaje = tiempoTranscurrido / duracionTotal;

                    // Interpolar coordenadas
                    latActual = v.orig_lat + ((v.dest_lat - v.orig_lat) * porcentaje);
                    lngActual = v.orig_lng + ((v.dest_lng - v.orig_lng) * porcentaje);
                }

                // Dibujar o mover el avión
                if (marcadoresAviones[v.codigo_vuelo]) {
                    // Si ya existe, solo le cambiamos la coordenada
                    marcadoresAviones[v.codigo_vuelo].setLatLng([latActual, lngActual]);
                } else {
                    // Dibujamos una línea punteada que muestra la ruta completa
                    L.polyline([[v.orig_lat, v.orig_lng], [v.dest_lat, v.dest_lng]], {color: '#3388ff', weight: 1, dashArray: '5, 5'}).addTo(map);
                    
                    // Creamos el marcador del avión
                    var marcador = L.marker([latActual, lngActual], {icon: avionIcon}).addTo(map);
                    
                    // Le ponemos su viñeta con el código de vuelo
                    marcador.bindTooltip(v.codigo_vuelo + " (" + v.origen_iata + " ➔ " + v.destino_iata + ")", {
                        permanent: true, direction: 'right', className: 'flight-label', offset: [10, 0]
                    });
                    
                    marcadoresAviones[v.codigo_vuelo] = marcador;
                }
            });
        }

        // Ejecutar por primera vez
        actualizarRadar();

        // MAGIA: El motor se recalcula cada 5 segundos para mover los aviones en vivo
        setInterval(actualizarRadar, 5000);
    </script>
</body>
</html>