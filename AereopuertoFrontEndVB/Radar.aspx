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
        body, html { margin: 0; padding: 0; height: 100%; background-color: #121212; font-family: 'Segoe UI', sans-serif; overflow: hidden; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; z-index: 1000; position: relative; box-shadow: 0 4px 10px rgba(0,0,0,0.5); }
        
        #mapaRadar { height: calc(100vh - 65px); width: 100%; z-index: 1; }
        
        .flight-label { background: rgba(0, 0, 0, 0.85); color: #00ff00; border: 1px solid #00ff00; padding: 4px 8px; border-radius: 6px; font-weight: bold; font-size: 11px; white-space: nowrap; cursor: pointer; transition: 0.2s; }
        .flight-label:hover { background: rgba(0, 255, 0, 0.2); border-color: #fff; color: #fff; transform: scale(1.05); }
        
        .clic-texto { color: #90caf9; font-size: 9px; display: block; margin-top: 2px; }

        /* =========================================
           👇 BLOQUE DE BROMA 1: CSS (BORRAR MAÑANA) 👇
           ========================================= */
        #mapaRadar { cursor: crosshair !important; }
        @keyframes boom {
            0% { transform: scale(0.5); opacity: 1; }
            50% { transform: scale(3); opacity: 1; text-shadow: 0 0 30px red, 0 0 50px yellow; }
            100% { transform: scale(5); opacity: 0; }
        }
        /* =========================================
           👆 FIN BLOQUE DE BROMA 1 👆
           ========================================= */
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">📡 Radar de Seguimiento de Vuelos (Live)</h4>
            <div>
                <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Dashboard</a>
            </div>
        </div>

        <div id="mapaRadar"></div>

        <asp:HiddenField ID="hfDatosVuelos" runat="server" />
    </form>

    <script>
        var limitesMundo = [
            [-90, -180],
            [90, 180]
        ];

        var map = L.map('mapaRadar', {
            maxBounds: limitesMundo,
            maxBoundsViscosity: 1.0,
            minZoom: 3
        }).setView([15.5, -90.25], 6);

        L.tileLayer('https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png', {
            attribution: '&copy; OpenStreetMap contributors &copy; CARTO',
            noWrap: true,
            bounds: limitesMundo
        }).addTo(map);

        // =========================================
        // 👇 BLOQUE DE BROMA 2: EXPLOSIÓN GLOBAL (BORRAR MAÑANA) 👇
        // =========================================
        map.on('click', function (e) {
            var explosionIcon = L.divIcon({
                html: '<div style="font-size: 50px; animation: boom 1s forwards; margin-top: -25px; margin-left: -25px;">💥</div>',
                className: 'dummy',
                iconSize: [50, 50]
            });
            var tempBoom = L.marker(e.latlng, { icon: explosionIcon }).addTo(map);
            setTimeout(function () { map.removeLayer(tempBoom); }, 1000);
        });
        // =========================================
        // 👆 FIN BLOQUE DE BROMA 2 👆
        // =========================================

        var avionIcon = L.divIcon({
            html: '<div style="font-size: 26px; color: #4fc3f7; transform: rotate(-45deg); cursor: pointer; text-shadow: 0 0 10px rgba(79,195,247,0.8);">✈️</div>',
            className: 'dummy',
            iconSize: [26, 26],
            iconAnchor: [13, 13]
        });

        var marcadoresAviones = {};

        function actualizarRadar() {
            var rawData = document.getElementById('<%= hfDatosVuelos.ClientID %>').value;
            if (!rawData) return;

            var vuelos = JSON.parse(rawData);
            var ahora = new Date().getTime();

            vuelos.forEach(function (v) {
                var tSalida = new Date(v.fecha_salida).getTime();
                var tLlegada = new Date(v.fecha_llegada).getTime();

                var latActual, lngActual;

                if (ahora < tSalida || v.id_estado_vuelo == 1) {
                    latActual = v.orig_lat;
                    lngActual = v.orig_lng;
                } else if (ahora > tLlegada) {
                    latActual = v.dest_lat;
                    lngActual = v.dest_lng;
                } else {
                    var duracionTotal = tLlegada - tSalida;
                    var tiempoTranscurrido = ahora - tSalida;
                    var porcentaje = tiempoTranscurrido / duracionTotal;

                    latActual = v.orig_lat + ((v.dest_lat - v.orig_lat) * porcentaje);
                    lngActual = v.orig_lng + ((v.dest_lng - v.orig_lng) * porcentaje);
                }

                if (marcadoresAviones[v.codigo_vuelo]) {
                    marcadoresAviones[v.codigo_vuelo].setLatLng([latActual, lngActual]);
                } else {
                    L.polyline([[v.orig_lat, v.orig_lng], [v.dest_lat, v.dest_lng]], { color: '#1e88e5', weight: 1, dashArray: '5, 5', opacity: 0.5 }).addTo(map);

                    var marcador = L.marker([latActual, lngActual], { icon: avionIcon }).addTo(map);

                    // --- CÓDIGO NORMAL: MAÑANA QUÍTALE LAS BARRAS (//) A ESTAS 3 LÍNEAS ---
                    // marcador.on('click', function () {
                    //     window.location.href = '../DetalleVuelo.aspx?id=' + v.id_vuelo;
                    // });

                    // =========================================
                    // 👇 BLOQUE DE BROMA 3: EXPLOSIÓN DE AVIÓN (BORRAR MAÑANA) 👇
                    // =========================================
                    marcador.on('click', function () {
                        var explosionIcon = L.divIcon({
                            html: '<div style="font-size: 50px; animation: boom 1s forwards; margin-top: -25px; margin-left: -25px;">💥</div>',
                            className: 'dummy',
                            iconSize: [50, 50],
                            iconAnchor: [25, 25]
                        });
                        marcador.setIcon(explosionIcon);
                        setTimeout(function () {
                            window.location.href = '../DetalleVuelo.aspx?id=' + v.id_vuelo;
                        }, 1000);
                    });
                    // =========================================
                    // 👆 FIN BLOQUE DE BROMA 3 👆
                    // =========================================

                    var textoTooltip = v.codigo_vuelo + " (" + v.origen_iata + " ➔ " + v.destino_iata + ")<br><span class='clic-texto'>👉 Clic para ver detalles</span>";

                    marcador.bindTooltip(textoTooltip, {
                        permanent: true, direction: 'right', className: 'flight-label', offset: [15, 0]
                    });

                    marcadoresAviones[v.codigo_vuelo] = marcador;
                }
            });
        }

        actualizarRadar();
        setInterval(actualizarRadar, 500);
    </script>
</body>
</html>