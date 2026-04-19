<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PlanillaPago.aspx.vb" Inherits="AereopuertoFrontEndVB.PlanillaPago" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Nómina y Pagos - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f8f9fa; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .finance-card { background: white; border-radius: 12px; box-shadow: 0 5px 20px rgba(0,0,0,0.05); padding: 25px; border-top: 5px solid #2e7d32; }
        .total-box { background-color: #e8f5e9; color: #2e7d32; border-radius: 8px; padding: 15px; text-align: center; border: 1px solid #c8e6c9; }
        .currency-input { font-family: monospace; font-size: 1.1rem; font-weight: bold; color: #1b5e20; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container py-5">
            <div class="mb-4 d-flex justify-content-between align-items-center">
                <h3 class="fw-bold m-0 text-dark">💸 Gestión de Nómina y Salarios</h3>
                <a href="../Default.aspx" class="btn btn-outline-secondary fw-bold rounded-pill px-4 shadow-sm">← Volver al Panel</a>
            </div>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="row">
                <div class="col-lg-4 mb-4">
                    <div class="finance-card">
                        <h5 class="fw-bold mb-4">Registrar Pago a Empleado</h5>
                        
                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">Periodo de Planilla *</label>
                            <asp:DropDownList ID="ddlPlanilla" runat="server" CssClass="form-select" AutoPostBack="true"></asp:DropDownList>
                        </div>
                        
                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">Empleado *</label>
                            <asp:DropDownList ID="ddlEmpleado" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>

                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">Salario Base (GTQ) *</label>
                            <div class="input-group">
                                <span class="input-group-text bg-light fw-bold">Q</span>
                                <asp:TextBox ID="txtSalario" runat="server" CssClass="form-control currency-input" TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                            </div>
                        </div>

                        <div class="mb-4">
                            <label class="form-label small fw-bold text-secondary">Bonificación / Horas Extras (GTQ)</label>
                            <div class="input-group">
                                <span class="input-group-text bg-light fw-bold">Q</span>
                                <asp:TextBox ID="txtBonificacion" runat="server" CssClass="form-control currency-input" TextMode="Number" step="0.01" Text="0.00"></asp:TextBox>
                            </div>
                        </div>

                        <asp:Button ID="btnGuardarPago" runat="server" Text="💾 Procesar Pago" CssClass="btn btn-success w-100 fw-bold py-2 fs-5 shadow-sm" />
                    </div>
                </div>

                <div class="col-lg-8">
                    <div class="bg-white p-4 rounded-3 shadow-sm border">
                        <div class="d-flex justify-content-between align-items-center mb-4">
                            <h5 class="fw-bold text-dark m-0">Detalle de la Planilla Seleccionada</h5>
                            <div class="total-box py-2 px-4">
                                <span class="small fw-bold text-uppercase d-block">Total Desembolso</span>
                                <h4 class="m-0 fw-bold">Q <asp:Label ID="lblTotalPlanilla" runat="server" Text="0.00"></asp:Label></h4>
                            </div>
                        </div>
                        
                        <div class="table-responsive">
                            <table class="table table-hover align-middle">
                                <thead class="table-light">
                                    <tr class="small text-secondary text-uppercase">
                                        <th>Colaborador</th>
                                        <th class="text-end">Salario Base</th>
                                        <th class="text-end">Bonificación</th>
                                        <th class="text-end fw-bold text-dark">Total a Pagar</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptDetallePlanilla" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="fw-bold text-primary"><%# Eval("nombre_empleado") %></td>
                                                <td class="text-end text-muted">Q <%# Convert.ToDecimal(Eval("salario")).ToString("N2") %></td>
                                                <td class="text-end text-success">+ Q <%# Convert.ToDecimal(Eval("bonificacion")).ToString("N2") %></td>
                                                <td class="text-end fw-bold fs-6">Q <%# Convert.ToDecimal(Eval("total")).ToString("N2") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>