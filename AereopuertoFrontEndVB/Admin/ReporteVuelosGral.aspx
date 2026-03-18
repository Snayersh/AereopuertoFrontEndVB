<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReporteVuelosGral.aspx.vb" Inherits="AereopuertoFrontEndVB.ReporteVuelosGral" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Reporte General de Vuelos - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.18.5/xlsx.full.min.js"></script>
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', sans-serif; padding-top: 30px; }
        .report-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); padding: 30px; margin-bottom: 30px; border-top: 5px solid #00bcd4; }
        .btn-export { font-weight: bold; padding: 8px 20px; border-radius: 8px; margin-left: 10px; }
        @media print {
            .no-print { display: none !important; }
            body { background-color: white; padding: 0; }
            .report-card { box-shadow: none; border: none; padding: 0; }
            .chart-container { max-height: 350px !important; }
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
                    <button type="button" class="btn btn-success btn-export shadow-sm" onclick="exportarExcel()">📊 Exportar a Excel</button>
                    <button type="button" class="btn btn-danger btn-export shadow-sm" onclick="window.print()">🖨️ Imprimir PDF</button>
                </div>
            </div>

            <div class="report-card">
                <div class="text-center mb-4">
                    <h2 class="fw-bold text-info m-0" style="color: #00838f !important;">📋 Reporte General de Estados de Vuelo</h2>
                    <p class="text-muted">Fuente de Datos: <span class="badge bg-warning text-dark shadow-sm">Servidor Réplica (Read-Only)</span></p>
                </div>

                <div class="row mb-5 align-items-center">
                    <div class="col-md-6 chart-container" style="position: relative; height:35vh;">
                        <canvas id="graficaEstados"></canvas>
                    </div>
                    <div class="col-md-6">
                        <h5 class="fw-bold border-bottom pb-2">Métricas del Sistema</h5>
                        <p class="text-muted">Proporción histórica de vuelos según su estado operativo. Ideal para auditar índices de cancelación frente a vuelos exitosos (aterrizados) o programados.</p>
                        
                        <div class="table-responsive mt-3">
                            <table class="table table-sm table-bordered table-hover" id="tablaReporteEstados">
                                <thead class="table-light">
                                    <tr>
                                        <th>Estado Operativo</th>
                                        <th class="text-center">Total Vuelos</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptDatos" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="fw-bold text-secondary"><%# Eval("estado_vuelo") %></td>
                                                <td class="text-center fw-bold fs-6"><%# Eval("total_vuelos") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <script>
        window.onload = function () {
            let lblString = document.getElementById('<%= hfLabels.ClientID %>').value;
            let dataString = document.getElementById('<%= hfDatos.ClientID %>').value;

            if (lblString && dataString) {
                const ctx = document.getElementById('graficaEstados').getContext('2d');
                new Chart(ctx, {
                    type: 'doughnut',
                    data: {
                        labels: JSON.parse(lblString),
                        datasets: [{
                            data: JSON.parse(dataString),
                            // Colores para Programado, Abordando, En Vuelo, Aterrizado, Cancelado, Retrasado
                            backgroundColor: ['#90caf9', '#ffb74d', '#4fc3f7', '#81c784', '#e57373', '#ff9800'],
                            borderWidth: 2,
                            borderColor: '#ffffff'
                        }]
                    },
                    options: { 
                        responsive: true, 
                        maintainAspectRatio: false,
                        plugins: {
                            legend: { position: 'right' }
                        }
                    }
                });
            }
        };

        function exportarExcel() {
            var wb = XLSX.utils.table_to_book(document.getElementById("tablaReporteEstados"), { sheet: "Estados_Vuelo" });
            XLSX.writeFile(wb, "Reporte_General_Vuelos.xlsx");
        }
    </script>
</body>
</html>