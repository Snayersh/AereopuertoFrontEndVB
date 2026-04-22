<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GestionPlanilla.aspx.vb" Inherits="AereopuertoFrontEndVB.GestionPlanilla" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Gestión de Planilla - La Aurora</title>
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
        .money-badge { background-color: #e8f5e9; color: #2e7d32; font-weight: bold; border-radius: 6px; padding: 6px 12px; font-size: 0.95rem; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center mb-5">
            <h4 class="m-0 fw-bold">💰 Finanzas y Nómina</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Centro de Control</a>
        </div>

        <div class="container" style="max-width: 900px;">
            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="row">
                <div class="col-md-4">
                    <div class="aur-card">
                        <h5 class="section-title">Cierre Mensual</h5>
                        
                        <div class="mb-3">
                            <label class="form-label text-secondary fw-bold small">Mes *</label>
                            <asp:DropDownList ID="ddlMes" runat="server" CssClass="form-select bg-light border-secondary" required="true">
                                <asp:ListItem Text="Enero" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Febrero" Value="2"></asp:ListItem>
                                <asp:ListItem Text="Marzo" Value="3"></asp:ListItem>
                                <asp:ListItem Text="Abril" Value="4"></asp:ListItem>
                                <asp:ListItem Text="Mayo" Value="5"></asp:ListItem>
                                <asp:ListItem Text="Junio" Value="6"></asp:ListItem>
                                <asp:ListItem Text="Julio" Value="7"></asp:ListItem>
                                <asp:ListItem Text="Agosto" Value="8"></asp:ListItem>
                                <asp:ListItem Text="Septiembre" Value="9"></asp:ListItem>
                                <asp:ListItem Text="Octubre" Value="10"></asp:ListItem>
                                <asp:ListItem Text="Noviembre" Value="11"></asp:ListItem>
                                <asp:ListItem Text="Diciembre" Value="12"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="mb-3">
                            <label class="form-label text-secondary fw-bold small">Año *</label>
                            <asp:TextBox ID="txtAnio" runat="server" CssClass="form-control" TextMode="Number" required="true"></asp:TextBox>
                        </div>

                        <div class="mb-4">
                            <label class="form-label text-secondary fw-bold small">Total Pagado (GTQ) *</label>
                            <div class="input-group">
                                <span class="input-group-text fw-bold text-success">Q</span>
                                <asp:TextBox ID="txtTotal" runat="server" CssClass="form-control fw-bold" TextMode="Number" step="0.01" required="true" placeholder="0.00"></asp:TextBox>
                            </div>
                        </div>
                        
                        <asp:Button ID="btnGuardar" runat="server" Text="Registrar Planilla 💾" CssClass="btn btn-custom w-100 py-3 mt-2 shadow-sm" />
                    </div>
                </div>

                <div class="col-md-8">
                    <div class="aur-card">
                        <h5 class="section-title" style="color: #424242;">Historial de Planillas</h5>
                        
                        <div class="table-responsive">
                            <table class="table table-hover align-middle">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>Período</th>
                                        <th class="text-end">Total Liquidado</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptPlanillas" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="text-muted small">#<%# Eval("ID_PLANILLA") %></td>
                                                <td class="fw-bold text-dark">
                                                    <asp:Label ID="lblPeriodo" runat="server"></asp:Label>
                                                </td>
                                                <td class="text-end">
                                                    <span class="money-badge shadow-sm">
                                                        <%# Convert.ToDecimal(Eval("TOTAL")).ToString("C2", New System.Globalization.CultureInfo("es-GT")) %>
                                                    </span>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                            
                            <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="text-center py-4">
                                <p class="text-muted fw-bold">No hay planillas registradas en el sistema.</p>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>