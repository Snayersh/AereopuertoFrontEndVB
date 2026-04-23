<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="HistorialReportes.aspx.vb" Inherits="AereopuertoFrontEndVB.HistorialReportes" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Historial de Reportes - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .aur-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 30px; border-top: 5px solid #1565c0; margin-bottom: 30px; }
        .section-title { font-size: 1.1rem; font-weight: 800; color: #0d47a1; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 20px; }
        .btn-custom { background-color: #0d47a1; color: white; border: none; font-weight: bold; border-radius: 8px; transition: 0.3s; }
        .btn-custom:hover { background-color: #1565c0; transform: translateY(-2px); box-shadow: 0 5px 15px rgba(13, 71, 161, 0.2); color: white;}
        .table th { background-color: #f8f9fc; color: #555; text-transform: uppercase; font-size: 0.85rem; letter-spacing: 1px; border-bottom: 2px solid #e2e8f0; }
        .table td { vertical-align: middle; }
        .badge-date { background-color: #e3f2fd; color: #0d47a1; font-weight: bold; border-radius: 6px; padding: 6px 12px; font-size: 0.85rem; border: 1px solid #bbdefb; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center mb-5">
            <h4 class="m-0 fw-bold">📊 Gestor e Historial de Reportes</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Centro de Control</a>
        </div>

        <div class="container" style="max-width: 1000px;">
            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="row">
                <div class="col-md-4">
                    <div class="aur-card">
                        <h5 class="section-title">Generar Extracción</h5>
                        <p class="text-muted small mb-4">Ingresa el título del reporte a exportar para que quede registrado en la bitácora de auditoría.</p>

                        <div class="mb-4">
                            <label class="form-label small fw-bold text-secondary">Nombre del Documento *</label>
                            <asp:TextBox ID="txtNombreReporte" runat="server" CssClass="form-control bg-light" required="true" placeholder="Ej: Pasajeros_Abril_2026"></asp:TextBox>
                        </div>
                        
                        <asp:Button ID="btnGuardar" runat="server" Text="Registrar y Extraer 📄" CssClass="btn btn-custom w-100 py-3 shadow-sm" />
                    </div>
                </div>

                <div class="col-md-8">
                    <div class="aur-card" style="border-top-color: #424242;">
                        <h5 class="section-title" style="color: #424242;">Bitácora de Documentos</h5>
                        <div class="table-responsive">
                            <table class="table table-hover align-middle">
                                <thead class="table-light">
                                    <tr>
                                        <th>CÓDIGO</th>
                                        <th>TÍTULO DEL REPORTE</th>
                                        <th class="text-end">FECHA DE EXTRACCIÓN</th>
                                        <th class="text-center">ACCIONES</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptReportes" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="text-muted small fw-bold">DOC-<%# Eval("ID_REPORTE") %></td>
                                                <td class="fw-bold text-dark"><%# Eval("NOMBRE") %></td>
                                                <td class="text-end">
                                                    <asp:Label ID="lblBadgeFecha" runat="server"></asp:Label>
                                                </td>
                                                <td class="text-center">
                                                    <a href="DetalleReportes.aspx?id=<%# Eval("ID_REPORTE") %>" class="btn btn-sm btn-outline-primary fw-bold shadow-sm">Ver Detalles 🔍</a>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>

                            <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="text-center py-5">
                                <p class="text-muted fw-bold">No hay reportes registrados en la bitácora.</p>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>