<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AsignarPuertas.aspx.vb" Inherits="AereopuertoFrontEndVB.AsignarPuertas" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Torre de Control: Asignación - La Aurora</title>
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
        .gate-badge { background-color: #e3f2fd; color: #0d47a1; font-weight: bold; padding: 6px 12px; border-radius: 8px; font-size: 0.95rem; border: 1px solid #bbdefb; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center mb-5">
            <h4 class="m-0 fw-bold">📡 Torre de Control: Asignación de Muelles</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Panel de Operaciones</a>
        </div>

        <div class="container" style="max-width: 1100px;">
            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="row">
                <div class="col-md-4">
                    <div class="aur-card">
                        <h5 class="section-title">Enlazar Vuelo a Puerta</h5>
                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">Vuelo Programado *</label>
                            <asp:DropDownList ID="ddlVuelos" runat="server" CssClass="form-select bg-light border-secondary" required="true"></asp:DropDownList>
                        </div>
                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">Puerta Física *</label>
                            <asp:DropDownList ID="ddlPuertas" runat="server" CssClass="form-select bg-light border-secondary" required="true"></asp:DropDownList>
                        </div>
                        <div class="mb-4">
                            <label class="form-label small fw-bold text-secondary">Hora de Abordaje Estimada *</label>
                            <asp:TextBox ID="txtHora" runat="server" CssClass="form-control fw-bold text-primary" TextMode="DateTimeLocal" required="true"></asp:TextBox>
                        </div>
                        <asp:Button ID="btnAsignar" runat="server" Text="Confirmar Asignación ✈️" CssClass="btn btn-custom w-100 py-3 shadow-sm" />
                    </div>
                </div>

                <div class="col-md-8">
                    <div class="aur-card" style="border-top-color: #2e7d32;">
                        <h5 class="section-title" style="color: #2e7d32;">Monitor de Tráfico Terrestre</h5>
                        <div class="table-responsive">
                            <table class="table table-hover align-middle">
                                <thead>
                                    <tr>
                                        <th>IDENTIFICADOR VUELO</th>
                                        <th class="text-center">PUERTA ASIGNADA</th>
                                        <th>HORA ABORDAJE</th>
                                        <th>ESTADO</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptAsignaciones" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="fw-bold fs-5 text-dark"><%# Eval("codigo_vuelo") %></td>
                                                <td class="text-center">
                                                    <span class="gate-badge shadow-sm"><%# Eval("nombre_puerta") %></span>
                                                </td>
                                                <td class="fw-bold text-secondary">
                                                    ⏰ <%# Eval("hora", "{0:HH:mm}") %> hrs
                                                </td>
                                                <td>
                                                    <span class="badge bg-success shadow-sm px-2 py-1">Operando</span>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                            
                            <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="text-center py-4">
                                <p class="text-muted fw-bold">No hay asignaciones activas en este momento.</p>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>