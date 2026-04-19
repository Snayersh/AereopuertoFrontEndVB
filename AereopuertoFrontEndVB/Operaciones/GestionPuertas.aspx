<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GestionPuertas.aspx.vb" Inherits="AereopuertoFrontEndVB.GestionPuertas" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Asignación de Puertas - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f8f9fa; font-family: 'Segoe UI', sans-serif; }
        .admin-card { background: white; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.05); padding: 25px; border-left: 5px solid #0d47a1; }
        .gate-badge { background-color: #e3f2fd; color: #0d47a1; font-weight: bold; padding: 5px 12px; border-radius: 6px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-5">
          <h3 class="fw-bold mb-4">🚪 Control de Puertas de Embarque</h3>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>
            <div class="row">
            <div class="row">
                <div class="col-md-4">
                    <div class="admin-card mb-4">
                        <h5 class="fw-bold mb-3">Nueva Asignación</h5>
                        <div class="mb-3">
                            <label class="form-label small fw-bold">Vuelo *</label>
                            <asp:DropDownList ID="ddlVuelos" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="mb-3">
                            <label class="form-label small fw-bold">Puerta *</label>
                            <asp:DropDownList ID="ddlPuertas" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="mb-3">
                            <label class="form-label small fw-bold">Hora de Abordaje *</label>
                            <asp:TextBox ID="txtHora" runat="server" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
                        </div>
                        <asp:Button ID="btnAsignar" runat="server" Text="Confirmar Puerta" CssClass="btn btn-primary w-100 fw-bold" />
                    </div>
                </div>

                <div class="col-md-8">
                    <div class="bg-white p-4 rounded shadow-sm">
                        <h5 class="fw-bold mb-3">Vuelos Próximos</h5>
                        <div class="table-responsive">
                            <table class="table align-middle">
                                <thead>
                                    <tr class="text-secondary small">
                                        <th>VUELO</th>
                                        <th>PUERTA</th>
                                        <th>HORA ABORDAJE</th>
                                        <th>ESTADO</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptAsignaciones" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="fw-bold"><%# Eval("codigo_vuelo") %></td>
                                                <td><span class="gate-badge"><%# Eval("nombre_puerta") %></span></td>
                                                <td><%# Eval("hora", "{0:HH:mm}") %> hrs</td>
                                                <td><span class="badge bg-success">Activo</span></td>
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