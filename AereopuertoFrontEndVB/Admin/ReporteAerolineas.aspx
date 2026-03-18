<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReporteAerolineas.aspx.vb" Inherits="AereopuertoFrontEndVB.ReporteAerolineas" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Reporte de Aerolíneas - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.18.5/xlsx.full.min.js"></script>
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', sans-serif; padding-top: 30px; }
        .report-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); padding: 30px; margin-bottom: 30px; border-top: 5px solid #8e24aa; }
        .btn-export { font-weight: bold; padding: 8px 20px; border-radius: 8px; margin-left: 10px; }
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
                    <h2 class="fw-bold m-0" style="color: #6a1b9a;">✈️ Aerolíneas Activas y Carga de Vuelos</h2>
                    <p class="text-muted">Fuente de Datos: <span class="badge bg-warning text-dark">Servidor Réplica (Read-Only)</span></p>
                </div>

                <div class="mb-5" style="position: relative; height:35vh; width:100%">
                    <canvas id="graficaAerolineas"></canvas>
                </div>

                <h5 class="fw-bold border-bottom pb-2 mb-3">Detalle Operativo por Aerolínea</h5>
                <div class="table-responsive">
                    <table class="table table-striped table-hover border" id="tablaReporteAerolineas">
                        <thead class="table-dark" style="background-color: #4a148c;">
                            <tr>
                                <th>Nombre de Aerolínea</th>
                                <th>País de Origen</th>
                                <th class="text-center">Total Vuelos Asignados</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptDatos" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="fw-bold text-dark"><%# Eval("aerolinea") %></td>
                                        <td><%# Eval("pais_origen") %></td>
                                        <td class="text-center fw-bold fs-5 text-primary"><%# Eval("total_vuelos_asignados") %></td>
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
                const ctx = document.getElementById('graficaAerolineas').getContext('2d');
                new Chart(ctx, {
                    type: 'bar', // Gráfica de barras
                    data: {
                        labels: JSON.parse(lblString),
                        datasets: [{
                            label: 'Vuelos Asignados',
                            data: JSON.parse(dataString),
                            backgroundColor: 'rgba(142, 36, 170, 0.7)',
                            borderColor: 'rgba(106, 27, 154, 1)',
                            borderWidth: 2,
                            borderRadius: 4
                        }]
                    },
                    options: { 
                        responsive: true, 
                        maintainAspectRatio: false,
                        indexAxis: 'y', // La vuelve horizontal para que se lean mejor los nombres largos
                        plugins: { legend: { display: false } }
                    }
                });
            }
        };

        function exportarExcel() {
            var wb = XLSX.utils.table_to_book(document.getElementById("tablaReporteAerolineas"), { sheet: "Aerolineas" });
            XLSX.writeFile(wb, "Reporte_Aerolineas_Activas.xlsx");
        }
    </script>
</body>
</html>