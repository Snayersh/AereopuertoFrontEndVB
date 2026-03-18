<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DetalleFactura.aspx.vb" Inherits="AereopuertoFrontEndVB.DetalleFactura" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Factura Detallada - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #e9ecef; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; padding: 40px 0; }
        .invoice-box { max-width: 800px; margin: auto; padding: 40px; border: 1px solid #eee; box-shadow: 0 10px 30px rgba(0,0,0,0.1); background: #fff; border-radius: 10px; }
        .invoice-header { border-bottom: 2px solid #0d47a1; padding-bottom: 20px; margin-bottom: 20px; }
        .invoice-title { color: #0d47a1; font-weight: 900; letter-spacing: 1px; }
        .info-label { font-size: 0.85rem; color: #777; font-weight: bold; text-transform: uppercase; margin-bottom: 2px; }
        .info-value { font-size: 1.1rem; font-weight: bold; color: #333; margin-bottom: 15px; }
        .table th { background-color: #f8f9fa; text-transform: uppercase; font-size: 0.85rem; color: #555; }
        .total-row { font-size: 1.5rem; font-weight: bold; color: #0d47a1; background-color: #f8f9fa; }
        @media print {
            body { background: white; padding: 0; }
            .no-print { display: none !important; }
            .invoice-box { box-shadow: none; border: none; padding: 0; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="text-center mb-4 no-print">
            <button type="button" class="btn btn-primary btn-lg fw-bold px-5 rounded-pill shadow" onclick="window.print();">
                🖨️ Imprimir Factura
            </button>
            <button type="button" class="btn btn-secondary btn-lg fw-bold px-4 rounded-pill shadow ms-2" onclick="window.close();">
                Cerrar
            </button>
        </div>

        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger text-center mx-auto no-print" style="max-width: 800px;">
            <asp:Label ID="lblError" runat="server" CssClass="fw-bold"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlFactura" runat="server" CssClass="invoice-box">
            <div class="invoice-header d-flex justify-content-between align-items-center">
                <div>
                    <h2 class="invoice-title m-0">✈️ LA AURORA AIRLINES</h2>
                    <p class="text-muted m-0">Aeropuerto Internacional La Aurora, Guatemala</p>
                </div>
                <div class="text-end">
                    <h4 class="fw-bold text-dark m-0"><asp:Label ID="lblNumeroFactura" runat="server"></asp:Label></h4>
                    <span class="badge bg-success fs-6 mt-1">PAGADO</span>
                </div>
            </div>

            <div class="row mb-4">
                <div class="col-sm-6">
                    <div class="info-label">Facturado a:</div>
                    <div class="info-value"><asp:Label ID="lblCliente" runat="server"></asp:Label></div>
                    <div class="info-label">Correo:</div>
                    <div class="info-value"><asp:Label ID="lblCorreo" runat="server"></asp:Label></div>
                </div>
                <div class="col-sm-6 text-end">
                    <div class="info-label">Fecha de Emisión:</div>
                    <div class="info-value"><asp:Label ID="lblFecha" runat="server"></asp:Label></div>
                    <div class="info-label">Moneda:</div>
                    <div class="info-value">Quetzales (GTQ)</div>
                </div>
            </div>

            <table class="table table-bordered mb-4">
                <thead>
                    <tr>
                        <th style="width: 50%;">Descripción</th>
                        <th class="text-center">Cant.</th>
                        <th class="text-end">Precio Unitario</th>
                        <th class="text-end">Subtotal</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptDetalles" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("descripcion") %></td>
                                <td class="text-center"><%# Eval("cantidad") %></td>
                                <td class="text-end">Q <%# Convert.ToDecimal(Eval("precio_unitario")).ToString("N2") %></td>
                                <td class="text-end fw-bold">Q <%# Convert.ToDecimal(Eval("subtotal")).ToString("N2") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <tr>
                        <td colspan="3" class="text-end fw-bold align-middle fs-5">TOTAL PAGADO:</td>
                        <td class="text-end total-row">Q <asp:Label ID="lblTotal" runat="server"></asp:Label></td>
                    </tr>
                </tbody>
            </table>

            <div class="text-center mt-5 text-muted small">
                <p class="mb-1"><strong>¡Gracias por volar con nosotros!</strong></p>
                <p>Este documento es un comprobante de pago válido. Consérvelo para cualquier aclaración.</p>
            </div>
        </asp:Panel>
    </form>
</body>
</html>