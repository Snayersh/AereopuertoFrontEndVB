<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="BitacoraSistemas.aspx.vb" Inherits="AereopuertoFrontEndVB.BitacoraSistemas" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Bitácora de Sistema - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .log-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 30px; border-top: 5px solid #1565c0; margin-bottom: 30px; }
        .form-control, .form-select { border-radius: 8px; }
        .btn-custom { background-color: #0d47a1; color: white; font-weight: bold; border-radius: 8px; transition: 0.3s; }
        .btn-custom:hover { background-color: #1565c0; color: white; transform: translateY(-2px); box-shadow: 0 5px 15px rgba(13, 71, 161, 0.2); }
        
        .table th { background-color: #f8f9fc; color: #555; text-transform: uppercase; font-size: 0.85rem; letter-spacing: 1px; border-bottom: 2px solid #e2e8f0; }
        .table td { vertical-align: middle; }
        
        /* Badges estandarizados */
        .badge-info { background-color: #e3f2fd; color: #0d47a1; font-weight: bold; border-radius: 8px; padding: 6px 12px; font-size: 0.85rem; }
        .badge-error { background-color: #ffebee; color: #c62828; font-weight: bold; border-radius: 8px; padding: 6px 12px; font-size: 0.85rem; }
        .badge-warning { background-color: #fff8e1; color: #f57f17; font-weight: bold; border-radius: 8px; padding: 6px 12px; font-size: 0.85rem; }
        .badge-time { background-color: #f1f3f5; color: #495057; border: 1px solid #ced4da; font-weight: bold; border-radius: 6px; padding: 4px 8px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">📋 Auditoría del Sistema</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Panel Principal</a>
        </div>

        <div class="container py-5">
            <div class="text-center mb-4">
                <h2 class="fw-bold text-dark">Bitácora de Eventos</h2>
                <p class="text-muted">Rastreo de actividades, errores y accesos al sistema central.</p>
            </div>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="log-card">
                <h6 class="fw-bold text-secondary mb-3 text-uppercase" style="letter-spacing: 1px;">🔍 Filtros de Búsqueda</h6>
                <div class="row g-3">
                    <div class="col-md-3">
                        <label class="form-label small fw-bold text-secondary">Palabra Clave</label>
                        <asp:TextBox ID="txtPalabraClave" runat="server" CssClass="form-control" placeholder="Ej: cancelado, error..."></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label small fw-bold text-secondary">Usuario</label>
                        <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Correo del usuario..."></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label small fw-bold text-secondary">Tipo de Evento</label>
                        <asp:DropDownList ID="ddlTipoEvento" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Todos los Eventos" Value=""></asp:ListItem>
                            <asp:ListItem Text="LOGIN / ACCESO" Value="LOGIN"></asp:ListItem>
                            <asp:ListItem Text="OPERACIÓN (Normal)" Value="OPERACION"></asp:ListItem>
                            <asp:ListItem Text="ERROR (Fallos)" Value="ERROR"></asp:ListItem>
                            <asp:ListItem Text="SEGURIDAD (Alertas)" Value="SEGURIDAD"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3 d-flex align-items-end">
                        <asp:Button ID="btnBuscarLogs" runat="server" Text="Buscar Eventos" CssClass="btn btn-custom w-100 py-2" />
                    </div>
                </div>
            </div>

            <div class="log-card">
                <div class="table-responsive">
                    <table class="table table-hover align-middle border-0">
                        <thead>
                            <tr>
                                <th>Fecha y Hora</th>
                                <th>Usuario</th>
                                <th>Tipo de Acción</th>
                                <th>Módulo Afectado</th>
                                <th>Descripción Detallada</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptBitacora" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="text-nowrap"><span class="badge-time shadow-sm"><%# Eval("fecha", "{0:dd/MM/yyyy HH:mm}") %></span></td>
                                        <td class="fw-bold text-dark"><%# Eval("usuario") %></td>
                                        <td>
                                            <span class='<%# ObtenerBadgeAccion(Eval("accion").ToString()) %> shadow-sm'>
                                                <%# Eval("accion") %>
                                            </span>
                                        </td>
                                        <td class="text-muted small fw-bold"><%# Eval("tabla_afectada") %></td>
                                        <td class="text-secondary"><%# Eval("descripcion") %></td>
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