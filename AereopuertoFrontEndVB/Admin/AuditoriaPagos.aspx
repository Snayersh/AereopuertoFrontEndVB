<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AuditoriaPagos.aspx.vb" Inherits="AereopuertoFrontEndVB.AuditoriaPagos" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Auditoría de Pagos - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f8f9fa; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #37474f; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .audit-card { background: white; border-radius: 12px; box-shadow: 0 5px 20px rgba(0,0,0,0.05); padding: 30px; border-top: 4px solid #ff9800; margin-top: 30px; }
        .table th { background-color: #eceff1; color: #37474f; text-transform: uppercase; font-size: 0.85rem; }
        .badge-estado { font-size: 0.85rem; padding: 6px 10px; border-radius: 6px; font-weight: bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">🔎 Auditoría y Movimientos Financieros</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Panel Principal</a>
        </div>

        <div class="container py-4">
            <div class="audit-card">
                <h5 class="fw-bold text-dark mb-4">Bitácora de Transiciones de Pagos</h5>
                
                <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm">
                    <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                </asp:Panel>

                <div class="table-responsive">
                    <table class="table table-hover align-middle">
                        <thead>
                            <tr>
                                <th># Transacción</th>
                                <th>ID Pago</th>
                                <th>Fecha del Cambio</th>
                                <th>Estado Anterior</th>
                                <th>Estado Nuevo</th>
                                <th>Observaciones</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptHistorial" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="text-muted fw-bold">#<%# Eval("ID_HISTORIAL") %></td>
                                        <td class="fw-bold text-primary"><%# Eval("ID_PAGO") %></td>
                                        <td class="small"><%# Eval("FECHA") %></td>
                                        <td>
                                            <span class="badge bg-secondary badge-estado"><%# If(IsDBNull(Eval("ESTADO_ANTERIOR")), "NINGUNO", Eval("ESTADO_ANTERIOR")) %></span>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblEstadoNuevo" runat="server"></asp:Label>
                                        </td>
                                        <td class="small text-muted fst-italic"><%# Eval("OBSERVACION") %></td>
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