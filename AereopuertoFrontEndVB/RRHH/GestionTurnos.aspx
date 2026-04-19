<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GestionTurnos.aspx.vb" Inherits="AereopuertoFrontEndVB.GestionTurnos" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Gestión de Turnos - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', sans-serif; }
        .card-custom { border: none; border-radius: 15px; box-shadow: 0 8px 20px rgba(0,0,0,0.06); }
        .header-accent { background: #1a237e; color: white; border-radius: 15px 15px 0 0; padding: 20px; }
        .btn-assign { background: #1a237e; color: white; font-weight: bold; border-radius: 8px; }
        .btn-assign:hover { background: #0d47a1; color: white; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container py-5">
            <div class="mb-4">
                <a href="../Default.aspx" class="text-decoration-none fw-bold text-secondary">← Volver al Panel de Control</a>
            </div>

            <h2 class="fw-bold mb-4">🗓️ Asignación de Turnos de Personal</h2>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="row">
                <div class="col-lg-4 mb-4">
                    <div class="card card-custom">
                        <div class="header-accent">
                            <h5 class="mb-0">Programar Turno</h5>
                        </div>
                        <div class="card-body p-4">
                            <div class="mb-3">
                                <label class="form-label small fw-bold text-muted">Empleado *</label>
                                <asp:DropDownList ID="ddlEmpleados" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="mb-3">
                                <label class="form-label small fw-bold text-muted">Tipo de Turno *</label>
                                <asp:DropDownList ID="ddlTiposTurno" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="mb-3">
                                <label class="form-label small fw-bold text-muted">Fecha *</label>
                                <asp:TextBox ID="txtFecha" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="mb-3">
                                <label class="form-label small fw-bold text-muted">Observaciones</label>
                                <asp:TextBox ID="txtObservacion" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="Notas adicionales..."></asp:TextBox>
                            </div>
                            <asp:Button ID="btnGuardarAsignacion" runat="server" Text="Registrar Turno" CssClass="btn btn-assign w-100 py-2 mt-2" />
                        </div>
                    </div>
                </div>

                <div class="col-lg-8">
                    <div class="card card-custom p-4">
                        <h5 class="fw-bold mb-4 text-dark">Agenda de Turnos Registrados</h5>
                        <div class="table-responsive">
                            <table class="table table-hover align-middle">
                                <thead class="table-light">
                                    <tr class="small text-uppercase">
                                        <th>Empleado</th>
                                        <th>Turno</th>
                                        <th>Fecha</th>
                                        <th>Notas</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptTurnos" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="fw-bold text-primary"><%# Eval("nombre_empleado") %></td>
                                                <td><span class="badge bg-info text-dark"><%# Eval("nombre_turno") %></span></td>
                                                <td><%# Eval("fecha", "{0:dd/MM/yyyy}") %></td>
                                                <td class="text-muted small"><%# Eval("observacion") %></td>
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