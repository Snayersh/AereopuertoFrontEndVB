<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReporteVuelosAire.aspx.vb" Inherits="AereopuertoFrontEndVB.ReporteVuelosAire" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Reporte Vuelos en el Aire - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.18.5/xlsx.full.min.js"></script>
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', sans-serif; padding-top: 30px; }
        .report-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); padding: 30px; margin-bottom: 30px; border-top: 5px solid #0288d1; }
        .btn-export { font-weight: bold; padding: 8px 20px; border-radius: 8px; margin-left: 10px; }
        .live-dot { height: 12px; width: 12px; background-color: #e53935; border-radius: 50%; display: inline-block; animation: pulse 1.5s infinite; }
        @keyframes pulse { 0% { box-shadow: 0 0 0 0 rgba(229, 57, 53, 0.7); } 70% { box-shadow: 0 0 0 10px rgba(229, 57, 53, 0); } 100% { box-shadow: 0 0 0 0 rgba(229, 57, 53, 0); } }
        @media print {
            .no-print { display: none !important; }
            body { background-color: white; padding: 0; }
            .report-card { box-shadow: none; border: none; padding: 0; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hfLabels" runat="server" />
        <asp:HiddenField ID="hfDatos" runat="server" />

        <div class="container">
            <div class="d-flex justify-content-between align-items-center mb-4 no-print">
                <a href="../Default.aspx" class="btn btn-outline-secondary fw-bold rounded-pill px-4">← Volver al Dashboard</a>
                <div>
                    <button type="button" class="btn btn-success btn-export shadow-sm" onclick="exportarExcel()">📊 Exportar Excel</button>
                    <button type="button" class="btn btn-danger btn-export shadow-sm" onclick="window.print()">🖨️ Imprimir PDF</button>
                </div>
            </div>

            <div class="report-card">
                <div class="text-center mb-4">
                    <h2 class="fw-bold text-primary m-0"><span class="live-dot me-2"></span>Vuelos Actualmente en el Aire</h2>
                    <p class="text-muted">Fuente de Datos: <span class="badge bg-warning text-dark">Servidor Réplica (Read-Only)</span></p>
                </div>

                <div class="mb-5" style="position: relative; height:30vh; width:100%">
                    <canvas id="graficaVuelosAire"></canvas>
                </div>

                <h5 class="fw-bold border-bottom pb-2 mb-3">Tráfico Aéreo Actual</h5>
                <div class="table-responsive">
                    <table class="table table-striped table-hover border" id="tablaReporteVuelosAire">
                        <thead class="table-dark" style="background-color: #0277bd;">
                            <tr>
                                <th>Vuelo</th>
                                <th>Aerolínea</th>
                                <th>Origen</th>
                                <th>Destino</th>
                                <th>Hora de Salida</th>
                                <th>Estado Actual</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptDatos" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="fw-bold text-dark"><%# Eval("codigo_vuelo") %></td>
                                        <td class="text-primary fw-bold"><%# Eval("aerolinea") %></td>
                                        <td><%# Eval("origen") %></td>
                                        <td><%# Eval("destino") %></td>
                                        <td><h6 class="m-0 fw-bold"><%# Eval("hora_salida") %></h6></td>
                                        <td><span class="badge bg-success shadow-sm">☁️ <%# Eval("estado_actual") %></span></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </form>

    <script>
        window.onload = function () {
            let lblString = document.getElementById('<%= hfLabels.ClientID %>').value;
            let dataString = document.getElementById('<%= hfDatos.ClientID %>').value;

            if (lblString && dataString) {
                const ctx = document.getElementById('graficaVuelosAire').getContext('2d');
                new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: JSON.parse(lblString),
                        datasets: [{
                            label: 'Vuelos volando ahora',
                            data: JSON.parse(dataString),
                            backgroundColor: 'rgba(3, 169, 244, 0.7)',
                            borderColor: 'rgba(2, 119, 189, 1)',
                            borderWidth: 2,
                            borderRadius: 4
                        }]
                    },
                    options: { 
                        responsive: true, 
                        maintainAspectRatio: false,
                        plugins: { legend: { display: false } },
                        scales: { y: { beginAtZero: true, ticks: { stepSize: 1 } } }
                    }
                });
            }
        };

        function exportarExcel() {
            var wb = XLSX.utils.table_to_book(document.getElementById("tablaReporteVuelosAire"), { sheet: "Vuelos_En_Aire" });
            XLSX.writeFile(wb, "Reporte_Vuelos_En_Aire.xlsx");
        }
    </script>
</body>
</html>