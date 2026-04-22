<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EmisionAlertas.aspx.vb" Inherits="AereopuertoFrontEndVB.EmisionAlertas" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Centro de Alertas - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .aur-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 30px; border-top: 5px solid #1565c0; margin-bottom: 30px; }
        .form-control, .form-select { border-radius: 8px; }
        .btn-custom { background-color: #0d47a1; color: white; font-weight: bold; border-radius: 8px; transition: 0.3s; }
        .btn-custom:hover { background-color: #1565c0; color: white; transform: translateY(-2px); box-shadow: 0 5px 15px rgba(13, 71, 161, 0.2); }
        
        .table th { background-color: #f8f9fc; color: #555; text-transform: uppercase; font-size: 0.85rem; letter-spacing: 1px; border-bottom: 2px solid #e2e8f0; }
        .table td { vertical-align: middle; }
        .badge-time { background-color: #f1f3f5; color: #495057; border: 1px solid #ced4da; font-weight: bold; border-radius: 6px; padding: 4px 8px; font-size: 0.8rem; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">📢 Centro de Emisión de Alertas</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Centro de Control</a>
        </div>

        <div class="container py-5">
            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="row">
                <div class="col-lg-4">
                    <div class="aur-card">
                        <h5 class="fw-bold text-dark mb-4">Redactar Notificación</h5>
                        
                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">Destinatario *</label>
                            <asp:DropDownList ID="ddlUsuarioDestino" runat="server" CssClass="form-select bg-light border-secondary" required="true"></asp:DropDownList>
                            <div class="form-text">Seleccione el pasajero o empleado.</div>
                        </div>

                        <div class="mb-4">
                            <label class="form-label small fw-bold text-secondary">Mensaje de Alerta *</label>
                            <asp:TextBox ID="txtMensaje" runat="server" CssClass="form-control bg-light" TextMode="MultiLine" Rows="5" placeholder="Ej: Estimado pasajero, su vuelo ha cambiado a la puerta 5..." required="true" MaxLength="250"></asp:TextBox>
                            <div class="form-text text-end"><small>Máx. 250 caracteres</small></div>
                        </div>

                        <asp:Button ID="btnEnviar" runat="server" Text="Emitir Alerta 🔔" CssClass="btn btn-custom w-100 py-3 shadow-sm" />
                    </div>
                </div>

                <div class="col-lg-8">
                    <div class="aur-card">
                        <h5 class="fw-bold text-dark mb-4">Historial de Comunicados Recientes</h5>
                        
                        <div class="table-responsive">
                            <table class="table table-hover align-middle">
                                <thead>
                                    <tr>
                                        <th>Fecha de Emisión</th>
                                        <th>Destinatario</th>
                                        <th>Contenido del Mensaje</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptAlertas" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="text-nowrap"><span class="badge-time"><%# Eval("FECHA_ENVIO") %></span></td>
                                                <td class="fw-bold text-primary">👤 <%# Eval("DESTINATARIO") %></td>
                                                <td class="small text-secondary fst-italic">"<%# Eval("MENSAJE") %>"</td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                            
                            <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="text-center py-4">
                                <p class="text-muted fw-bold">No hay alertas recientes emitidas en el sistema.</p>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>