<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DetalleReportes.aspx.vb" Inherits="AereopuertoFrontEndVB.DetalleReportes" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Detalles de Reportes - La Aurora</title>
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center mb-5">
            <h4 class="m-0 fw-bold">📑 Análisis y Detalle de Reportes</h4>
            <a href="HistorialReportes.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver a Historial</a>
        </div>

        <div class="container" style="max-width: 1000px;">
            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="aur-card">
                <h5 class="section-title">1. Seleccionar Archivo de Bitácora</h5>
                <div class="row align-items-end">
                    <div class="col-md-9 mb-3 mb-md-0">
                        <label class="form-label small fw-bold text-secondary">Documento Base *</label>
                        <asp:DropDownList ID="ddlReportes" runat="server" CssClass="form-select bg-light border-secondary"></asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <asp:Button ID="btnVerDetalles" runat="server" Text="Cargar Contenido 🔍" CssClass="btn btn-success w-100 fw-bold shadow-sm" />
                    </div>
                </div>
            </div>

            <asp:Panel ID="pnlGestionDetalles" runat="server" Visible="false">
                <div class="row">
                    <div class="col-md-4">
                        <div class="aur-card" style="border-top-color: #2e7d32;">
                            <h5 class="section-title" style="color: #2e7d32;">Agregar Registro</h5>
                            <div class="mb-4">
                                <label class="form-label small fw-bold text-secondary">Contenido de la línea *</label>
                                <asp:TextBox ID="txtContenido" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" required="true" placeholder="Escriba los datos a anexar al reporte..."></asp:TextBox>
                            </div>
                            <asp:Button ID="btnGuardarDetalle" runat="server" Text="Anexar Línea ➕" CssClass="btn btn-custom w-100 py-3 shadow-sm" />
                        </div>
                    </div>

                    <div class="col-md-8">
                        <div class="aur-card" style="border-top-color: #424242;">
                            <h5 class="section-title" style="color: #424242;">Contenido Actual del Reporte</h5>
                            <div class="table-responsive">
                                <table class="table table-hover align-middle">
                                    <thead class="table-light">
                                        <tr>
                                            <th style="width: 15%;">LÍNEA #</th>
                                            <th>CONTENIDO ANEXADO</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptDetalles" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="text-muted small fw-bold"><%# Eval("ID_DETALLE") %></td>
                                                    <td class="text-dark"><%# Eval("CONTENIDO") %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>

                                <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="text-center py-4">
                                    <p class="text-muted fw-bold">Este reporte está vacío. No tiene líneas de detalle registradas.</p>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>