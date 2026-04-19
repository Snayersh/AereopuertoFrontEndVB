<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Auditoria.aspx.vb" Inherits="AereopuertoFrontEndVB.Auditoria" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Bitácora de Auditoría - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f8f9fa; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .audit-card { background: white; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.05); padding: 25px; border-top: 4px solid #424242; }
        .action-insert { color: #2e7d32; font-weight: bold; background: #e8f5e9; padding: 3px 8px; border-radius: 4px; }
        .action-update { color: #f57c00; font-weight: bold; background: #fff3e0; padding: 3px 8px; border-radius: 4px; }
        .action-delete { color: #c62828; font-weight: bold; background: #ffebee; padding: 3px 8px; border-radius: 4px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container py-5">
            <div class="mb-4">
                <a href="../Default.aspx" class="text-decoration-none fw-bold text-secondary">← Volver al Tablero Principal</a>
            </div>

            <h2 class="fw-bold mb-4 text-dark">🛡️ Bitácora de Auditoría del Sistema</h2>
            <p class="text-muted">Registro inmutable de transacciones en la base de datos.</p>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="audit-card mb-4">
                <div class="row align-items-end">
                    <div class="col-md-3">
                        <label class="form-label small fw-bold text-secondary">Tabla Afectada</label>
                        <asp:TextBox ID="txtTabla" runat="server" CssClass="form-control" placeholder="Ej: AUR_VUELO"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label small fw-bold text-secondary">Usuario</label>
                        <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Correo del usuario..."></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label small fw-bold text-secondary">Acción</label>
                        <asp:DropDownList ID="ddlAccion" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Todas las Acciones" Value=""></asp:ListItem>
                            <asp:ListItem Text="INSERT (Creación)" Value="INSERT"></asp:ListItem>
                            <asp:ListItem Text="UPDATE (Modificación)" Value="UPDATE"></asp:ListItem>
                            <asp:ListItem Text="DELETE (Eliminación)" Value="DELETE"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar Registros" CssClass="btn btn-dark w-100 fw-bold" />
                    </div>
                </div>
            </div>

            <div class="audit-card">
                <div class="table-responsive">
                    <table class="table table-hover align-middle">
                        <thead class="table-light">
                            <tr class="small text-secondary text-uppercase">
                                <th>ID</th>
                                <th>Tabla Modificada</th>
                                <th>Acción</th>
                                <th>Fecha y Hora</th>
                                <th>Usuario / Sistema</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptAuditoria" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="text-muted">#<%# Eval("id_auditoria") %></td>
                                        <td class="fw-bold"><%# Eval("tabla") %></td>
                                        <td>
                                            <span class='<%#ObtenerClaseAccion(Eval("accion").ToString()) %>'>
                                                <%# Eval("accion") %>
                                            </span>
                                        </td>
                                        <td><%# Eval("fecha", "{0:dd/MM/yyyy HH:mm:ss}") %></td>
                                        <td class="text-primary fw-bold"><%# Eval("usuario") %></td>
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