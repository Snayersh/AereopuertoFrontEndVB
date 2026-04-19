<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GestionAsistencia.aspx.vb" Inherits="AereopuertoFrontEndVB.GestionAsistencia" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Control de Asistencia - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f0f2f5; font-family: 'Segoe UI', sans-serif; }
        .attendance-card { background: white; border-radius: 12px; box-shadow: 0 5px 15px rgba(0,0,0,0.05); padding: 25px; border-top: 5px solid #2e7d32; }
        .status-present { color: #2e7d32; font-weight: bold; }
        .status-absent { color: #d32f2f; font-weight: bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container py-5">
            <div class="mb-4">
                <a href="../Default.aspx" class="btn btn-sm btn-link text-dark fw-bold">← Regresar al Tablero</a>
            </div>

            <h3 class="fw-bold mb-4 text-dark">⏱️ Registro de Asistencia de Personal</h3>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="row">
                <div class="col-md-5 mb-4">
                    <div class="attendance-card">
                        <h5 class="fw-bold mb-4">Marcar Asistencia</h5>
                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">Seleccionar Empleado *</label>
                            <asp:DropDownList ID="ddlEmpleados" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">Fecha de Registro *</label>
                            <asp:TextBox ID="txtFecha" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">Estado de Asistencia *</label>
                            <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                                <asp:ListItem Text="-- Seleccionar --" Value=""></asp:ListItem>
                                <asp:ListItem Text="Presente" Value="Presente"></asp:ListItem>
                                <asp:ListItem Text="Ausente" Value="Ausente"></asp:ListItem>
                                <asp:ListItem Text="Tardanza" Value="Tardanza"></asp:ListItem>
                                <asp:ListItem Text="Permiso/Vacaciones" Value="Permiso"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <asp:Button ID="btnRegistrarAsistencia" runat="server" Text="Confirmar Marcación" CssClass="btn btn-success w-100 fw-bold py-2 mt-3" />
                    </div>
                </div>

                <div class="col-md-7">
                    <div class="bg-white p-4 rounded-3 shadow-sm border">
                        <h5 class="fw-bold mb-3 text-secondary">Marcaciones Recientes</h5>
                        <div class="table-responsive">
                            <table class="table align-middle">
                                <thead class="table-light">
                                    <tr class="small">
                                        <th>Empleado</th>
                                        <th>Fecha</th>
                                        <th>Estado</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptAsistencia" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="fw-bold"><%# Eval("nombre_empleado") %></td>
                                                <td><%# Eval("fecha", "{0:dd/MM/yyyy}") %></td>
                                                <td>
                                                    <span class='<%# If(Eval("estado").ToString() = "Presente", "status-present", "status-absent") %>'>
                                                        <%# Eval("estado") %>
                                                    </span>
                                                </td>
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