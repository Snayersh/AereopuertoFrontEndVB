<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="BitacoraSistemas.aspx.vb" Inherits="AereopuertoFrontEndVB.BitacoraSistemas" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Bitácora de Sistema - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f0f2f5; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .log-card { background: white; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.05); padding: 25px; border-top: 4px solid #1976d2; }
        .badge-info { background-color: #e3f2fd; color: #1976d2; font-weight: bold; border-radius: 6px; padding: 4px 8px; }
        .badge-error { background-color: #ffebee; color: #c62828; font-weight: bold; border-radius: 6px; padding: 4px 8px; }
        .badge-warning { background-color: #fff8e1; color: #f57f17; font-weight: bold; border-radius: 6px; padding: 4px 8px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container py-5">
            <div class="mb-4">
                <a href="../Default.aspx" class="text-decoration-none fw-bold text-secondary">← Volver al Tablero Principal</a>
            </div>

            <h2 class="fw-bold mb-4 text-dark">📋 Bitácora de Eventos del Sistema</h2>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="log-card mb-4">
                <div class="row">
                    <div class="col-md-3 mb-3">
                        <label class="form-label small fw-bold text-secondary">Palabra Clave (Descripción)</label>
                        <asp:TextBox ID="txtPalabraClave" runat="server" CssClass="form-control" placeholder="Ej: cancelado, error..."></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-label small fw-bold text-secondary">Usuario</label>
                        <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Correo del usuario..."></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-label small fw-bold text-secondary">Tipo de Evento</label>
                        <asp:DropDownList ID="ddlTipoEvento" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Todos" Value=""></asp:ListItem>
                            <asp:ListItem Text="LOGIN / ACCESO" Value="LOGIN"></asp:ListItem>
                            <asp:ListItem Text="OPERACION (Normal)" Value="OPERACION"></asp:ListItem>
                            <asp:ListItem Text="ERROR (Fallos)" Value="ERROR"></asp:ListItem>
                            <asp:ListItem Text="SEGURIDAD (Alertas)" Value="SEGURIDAD"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3 mb-3 d-flex align-items-end">
                        <asp:Button ID="btnBuscarLogs" runat="server" Text="Buscar Eventos" CssClass="btn btn-primary w-100 fw-bold" />
                    </div>
                </div>
            </div>

            <div class="log-card">
                <div class="table-responsive">
                    <table class="table table-hover align-middle border-top">
                        <thead class="table-light">
                            <tr class="small text-secondary text-uppercase">
                                <th>Fecha y Hora</th>
                                <th>Usuario</th>
                                <th>Tipo (Acción)</th>
                                <th>Módulo/Tabla</th>
                                <th>Descripción del Evento</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptBitacora" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="text-nowrap"><span class="badge bg-secondary"><%# Eval("fecha", "{0:dd/MM/yyyy HH:mm}") %></span></td>
                                        <td class="fw-bold"><%# Eval("usuario") %></td>
                                        <td>
                                            <span class='<%# ObtenerBadgeAccion(Eval("accion").ToString()) %>'>
                                                <%# Eval("accion") %>
                                            </span>
                                        </td>
                                        <td class="text-muted small"><%# Eval("tabla_afectada") %></td>
                                        <td><%# Eval("descripcion") %></td>
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