<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="MisFacturas.aspx.vb" Inherits="AereopuertoFrontEndVB.MisFacturas" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Mis Facturas - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .invoice-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 40px; margin-top: 40px; border-top: 5px solid #00796b; }
        .table th { background-color: #f8f9fa; color: #555; font-weight: bold; text-transform: uppercase; font-size: 0.85rem; letter-spacing: 1px; }
        .table td { vertical-align: middle; font-size: 1.1rem; }
        .badge-pagado { background-color: #e8f5e9; color: #2e7d32; padding: 6px 12px; border-radius: 20px; font-size: 0.85rem; font-weight: bold; display: inline-block; }
        .badge-anulado { background-color: #ffebee; color: #c62828; padding: 6px 12px; border-radius: 20px; font-size: 0.85rem; font-weight: bold; display: inline-block; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">🧾 Historial de Facturación</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Inicio</a>
        </div>

        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-10">
                    <div class="invoice-card">
                        <div class="text-center mb-5">
                            <h2 class="fw-bold text-dark">Mis Facturas</h2>
                            <p class="text-muted">Resumen de todos tus pagos y comprobantes generados en el sistema.</p>
                        </div>

                        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger text-center rounded-3 mb-4 fw-bold">
                            <asp:Label ID="lblError" runat="server"></asp:Label>
                        </asp:Panel>

                        <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="text-center py-5">
                            <h1 style="font-size: 4rem; color: #ccc;">🧾</h1>
                            <h4 class="text-secondary fw-bold mt-3">No tienes facturas generadas</h4>
                            <p class="text-muted">Tus facturas aparecerán aquí una vez que completes el pago de una reserva.</p>
                            <a href="Reservas.aspx" class="btn btn-primary mt-3 rounded-pill px-4">Comprar Boletos</a>
                        </asp:Panel>

                        <asp:Panel ID="pnlDatos" runat="server">
                            <div class="table-responsive">
                                <table class="table table-hover border">
                                    <thead>
                                        <tr>
                                            <th>No. Factura</th>
                                            <th>Fecha de Emisión</th>
                                            <th class="text-end">Total Pagado</th>
                                            <th class="text-center">Estado</th>
                                            <th class="text-center">Acción</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptFacturas" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="fw-bold text-primary"><%# Eval("numero_factura") %></td>
                                                    <td><%# Eval("fecha_emision") %></td>
                                                    <td class="text-end fw-bold text-dark">
                                                        <asp:Label ID="lblTotal" runat="server"></asp:Label>
                                                    </td>
                                                    <td class="text-center">
                                                        <asp:Label ID="lblEstado" runat="server"></asp:Label>
                                                    </td>
                                                    <td class="text-center">
                                                        <a href='DetalleFactura.aspx?id=<%# Eval("id_factura") %>' target="_blank" class="btn btn-sm btn-outline-primary rounded-pill fw-bold">📄 Ver Detalle</a>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>