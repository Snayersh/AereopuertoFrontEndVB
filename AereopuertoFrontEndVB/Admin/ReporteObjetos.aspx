<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReporteObjetos.aspx.vb" Inherits="AereopuertoFrontEndVB.ReporteObjetos" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Reporte de Objetos Perdidos</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.18.5/xlsx.full.min.js"></script>
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', sans-serif; padding-top: 30px; }
        .report-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); padding: 30px; margin-bottom: 30px; border-top: 5px solid #1976d2; }
        .btn-export { font-weight: bold; padding: 8px 20px; border-radius: 8px; margin-left: 10px; }
        @media print {
            .no-print { display: none !important; }
            body { background-color: white; padding: 0; }
            .report-card { box-shadow: none; border: none; padding: 0; }
            .chart-container { max-height: 300px !important; }
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
                    <h2 class="fw-bold text-primary m-0">🎒 Reporte de Objetos Perdidos y Encontrados</h2>
                    <p class="text-muted">Fuente de Datos: <span class="badge bg-warning text-dark">Servidor Réplica (Read-Only)</span></p>
                </div>

                <div class="row align-items-center mb-5">
                    <div class="col-md-5 chart-container" style="position: relative; height:30vh;">
                        <canvas id="graficaObjetos"></canvas>
                    </div>
                    <div class="col-md-7">
                        <h5 class="fw-bold text-dark">Inventario Actual</h5>
                        <p class="text-muted">Proporción de objetos resguardados en bodega contra objetos que ya han sido reclamados por sus dueños. Este reporte excluye objetos perecederos o destruidos por protocolo de bioseguridad.</p>
                    </div>
                </div>

                <h5 class="fw-bold border-bottom pb-2 mb-3">Registro de Artículos</h5>
                <div class="table-responsive">
                    <table class="table table-striped table-hover border" id="tablaReporteObjetos">
                        <thead class="table-dark">
                            <tr>
                                <th>Descripción del Objeto</th>
                                <th>Lugar Encontrado</th>
                                <th>Fecha y Hora</th>
                                <th>Estado</th>
                                <th>Entregado a (Dueño)</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptDatos" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="fw-bold text-dark"><%# Eval("descripcion") %></td>
                                        <td><%# Eval("lugar_encontrado") %></td>
                                        <td><%# Eval("fecha") %></td>
                                        <td><span class="badge bg-primary"><%# Eval("estado_reclamo") %></span></td>
                                        <td class="text-success fw-bold"><%# If(IsDBNull(Eval("entregado_a")), "---", Eval("entregado_a")) %></td>
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
                const ctx = document.getElementById('graficaObjetos').getContext('2d');
                new Chart(ctx, {
                    type: 'pie',
                    data: {
                        labels: JSON.parse(lblString),
                        datasets: [{
                            data: JSON.parse(dataString),
                            backgroundColor: ['#1976d2', '#388e3c', '#f57c00', '#7b1fa2'],
                            borderWidth: 1
                        }]
                    },
                    options: { responsive: true, maintainAspectRatio: false }
                });
            }
        };

        function exportarExcel() {
            var wb = XLSX.utils.table_to_book(document.getElementById("tablaReporteObjetos"), { sheet: "Objetos_Perdidos" });
            XLSX.writeFile(wb, "Reporte_Objetos_LaAurora.xlsx");
        }
    </script>
</body>
</html>