<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReportePasajeros.aspx.vb" Inherits="AereopuertoFrontEndVB.ReportePasajeros" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Reporte: Pasajeros por Vuelo</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.18.5/xlsx.full.min.js"></script>

    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', sans-serif; padding-top: 30px; }
        .report-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); padding: 30px; margin-bottom: 30px; border-top: 5px solid #ff9800; }
        .btn-export { font-weight: bold; padding: 8px 20px; border-radius: 8px; margin-left: 10px; }
        
        /* Ocultar botones y menús al momento de imprimir o guardar como PDF */
        @media print {
            .no-print { display: none !important; }
            body { background-color: white; padding: 0; }
            .report-card { box-shadow: none; border: none; padding: 0; }
            canvas { max-height: 400px !important; }
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
                    <button type="button" class="btn btn-success btn-export shadow-sm" onclick="exportarExcel()">📊 Exportar Excel (Power BI)</button>
                    <button type="button" class="btn btn-danger btn-export shadow-sm" onclick="window.print()">🖨️ Imprimir / PDF</button>
                </div>
            </div>

            <div class="report-card">
                <div class="text-center mb-4">
                    <h2 class="fw-bold text-dark m-0">Reporte de Ocupación por Vuelo</h2>
                    <p class="text-muted">Fuente de Datos: <span class="badge bg-warning text-dark">Servidor Réplica (Read-Only)</span></p>
                </div>

                <div class="mb-5" style="position: relative; height:40vh; width:100%">
                    <canvas id="graficaPasajeros"></canvas>
                </div>

                <h5 class="fw-bold border-bottom pb-2 mb-3">Detalle de Datos</h5>
                <div class="table-responsive">
                    <table class="table table-striped table-hover border" id="tablaReporte">
                        <thead class="table-dark">
                            <tr>
                                <th>Código de Vuelo</th>
                                <th>Aerolínea</th>
                                <th>Origen</th>
                                <th>Destino</th>
                                <th>Total Pasajeros Registrados</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptDatos" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="fw-bold text-primary"><%# Eval("codigo_vuelo") %></td>
                                        <td><%# Eval("aerolinea") %></td>
                                        <td><%# Eval("origen") %></td>
                                        <td><%# Eval("destino") %></td>
                                        <td class="fs-5 fw-bold"><%# Eval("total_pasajeros") %></td>
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
        // ==========================================
        // 1. INICIALIZAR LA GRÁFICA (CHART.JS)
        // ==========================================
        window.onload = function () {
            let lblString = document.getElementById('<%= hfLabels.ClientID %>').value;
            let dataString = document.getElementById('<%= hfDatos.ClientID %>').value;

            if (lblString && dataString) {
                let etiquetas = JSON.parse(lblString);
                let valores = JSON.parse(dataString);

                const ctx = document.getElementById('graficaPasajeros').getContext('2d');
                new Chart(ctx, {
                    type: 'bar', // Puede ser 'bar', 'pie', 'line'
                    data: {
                        labels: etiquetas,
                        datasets: [{
                            label: 'Cantidad de Pasajeros',
                            data: valores,
                            backgroundColor: 'rgba(13, 71, 161, 0.7)', // Azul La Aurora
                            borderColor: 'rgba(13, 71, 161, 1)',
                            borderWidth: 2,
                            borderRadius: 5, // Bordes redondeados en las barras
                            hoverBackgroundColor: 'rgba(255, 152, 0, 0.8)' // Naranja al pasar el mouse
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: { display: false } // Oculta la leyenda superior
                        },
                        scales: {
                            y: { beginAtZero: true, title: { display: true, text: 'Número de Personas' } }
                        }
                    }
                });
            }
        };

        // ==========================================
        // 2. FUNCIÓN PARA EXPORTAR A EXCEL (SHEETJS)
        // ==========================================
        function exportarExcel() {
            // Obtener la tabla HTML
            var tabla = document.getElementById("tablaReporte");
            
            // Convertir la tabla a una hoja de cálculo
            var wb = XLSX.utils.table_to_book(tabla, { sheet: "Ocupacion_Vuelos" });
            
            // Descargar el archivo Excel (este archivo entra nítido a Power BI)
            XLSX.writeFile(wb, "Reporte_Pasajeros_LaAurora.xlsx");
        }
    </script>
</body>
</html>