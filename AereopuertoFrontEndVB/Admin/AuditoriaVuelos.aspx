<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AuditoriaVuelos.aspx.vb" Inherits="AereopuertoFrontEndVB.AuditoriaVuelos" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Radar de Vuelos - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f0f4f8; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0f4c75; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .radar-card { background: white; border-radius: 12px; box-shadow: 0 5px 20px rgba(0,0,0,0.05); padding: 30px; border-top: 4px solid #3282b8; margin-top: 30px; }
        .table th { background-color: #e0ece4; color: #1b262c; text-transform: uppercase; font-size: 0.85rem; letter-spacing: 1px; }
        .badge-vuelo { font-size: 0.85rem; padding: 6px 12px; border-radius: 6px; font-weight: bold; letter-spacing: 0.5px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">📡 Bitácora de Tráfico Aéreo</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Centro de Mando</a>
        </div>

        <div class="container py-4">
            <div class="radar-card">
                <h5 class="fw-bold text-dark mb-4">Registro de Transiciones de Estado</h5>
                
                <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm">
                    <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                </asp:Panel>

                <div class="table-responsive">
                    <table class="table table-hover align-middle">
                        <thead>
                            <tr>
                                <th># Reg.</th>
                                <th>Código de Vuelo</th>
                                <th>Fecha y Hora</th>
                                <th class="text-center">Estado Anterior</th>
                                <th class="text-center">Estado Nuevo</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptHistorialVuelos" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="text-muted fw-bold">#<%# Eval("ID_HISTORIAL") %></td>
                                        <td class="fw-bold text-primary">✈️ <%# Eval("CODIGO_VUELO") %></td>
                                        <td class="small"><%# Eval("FECHA_CAMBIO") %></td>
                                        <td class="text-center">
                                            <span class="badge bg-secondary badge-vuelo"><%# If(IsDBNull(Eval("ESTADO_ANTERIOR")), "INICIO", Eval("ESTADO_ANTERIOR")) %></span>
                                        </td>
                                        <td class="text-center">
                                            <asp:Label ID="lblEstadoNuevo" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </form>
</body>
</html>