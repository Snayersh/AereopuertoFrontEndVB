<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="BandejaTickets.aspx.vb" Inherits="AereopuertoFrontEndVB.BandejaTickets" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Bandeja de Soporte - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #212529; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .aur-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 30px; border-top: 5px solid #ff9800; margin-bottom: 30px; }
        .section-title { font-size: 1.1rem; font-weight: 800; color: #212529; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 20px; }
        .table th { background-color: #f8f9fc; color: #555; text-transform: uppercase; font-size: 0.85rem; letter-spacing: 1px; border-bottom: 2px solid #e2e8f0; }
        .table td { vertical-align: middle; }
        
        /* Badges de Estado */
        .badge-abierto { background-color: #e3f2fd; color: #0d47a1; font-weight: bold; border-radius: 6px; padding: 6px 12px; }
        .badge-cerrado { background-color: #f5f5f5; color: #616161; font-weight: bold; border-radius: 6px; padding: 6px 12px; }
        
        /* Burbuja de Chat del Agente */
        .chat-bubble-agent { background-color: #fff3e0; border-right: 4px solid #ff9800; padding: 15px; border-radius: 8px; margin-bottom: 15px; text-align: right; }
        .chat-bubble-client { background-color: #f8f9fa; border-left: 4px solid #0d47a1; padding: 15px; border-radius: 8px; margin-bottom: 15px; }
        .chat-date { font-size: 0.8rem; color: #888; font-weight: bold; margin-bottom: 5px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center mb-5 border-bottom border-warning border-3">
            <h4 class="m-0 fw-bold">🎧 Torre de Control: Atención al Pasajero</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Panel</a>
        </div>

        <div class="container-fluid px-4" style="max-width: 1400px;">
            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <asp:Panel ID="pnlListado" runat="server">
                <div class="aur-card">
                    <h5 class="section-title">Bandeja de Entrada Global</h5>
                    <p class="text-muted small mb-4">Revisa y responde las solicitudes y reclamos de los pasajeros.</p>

                    <div class="table-responsive">
                        <table class="table table-hover align-middle">
                            <thead class="table-light">
                                <tr>
                                    <th>NO. CASO</th>
                                    <th>PASAJERO (CORREO)</th>
                                    <th>TIPO</th>
                                    <th>ASUNTO</th>
                                    <th class="text-center">ESTADO</th>
                                    <th class="text-center">ACCIÓN</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptTickets" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="text-muted small fw-bold">TCK-<%# Eval("ID_TICKET") %></td>
                                            <td class="fw-bold text-dark">👤 <%# Eval("CLIENTE") %></td>
                                            <td class="small fw-bold text-secondary"><%# Eval("TIPO_TICKET") %></td>
                                            <td class="text-dark"><%# Eval("ASUNTO") %></td>
                                            <td class="text-center">
                                                <asp:Label ID="lblBadgeEstado" runat="server"></asp:Label>
                                            </td>
                                            <td class="text-center">
                                                <a href="BandejaTickets.aspx?id=<%# Eval("ID_TICKET") %>" class="btn btn-sm btn-outline-primary fw-bold shadow-sm">Atender 💬</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                        <asp:Panel ID="pnlVacioTickets" runat="server" Visible="false" CssClass="text-center py-5">
                            <h1 class="opacity-50">📭</h1>
                            <p class="text-muted fw-bold">Excelente trabajo. La bandeja de entrada está vacía.</p>
                        </asp:Panel>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlConversacion" runat="server" Visible="false">
                <div class="row">
                    <div class="col-lg-8">
                        <div class="aur-card mb-3">
                            <div class="d-flex justify-content-between align-items-center mb-4 border-bottom pb-3">
                                <div>
                                    <h5 class="section-title mb-0">HILO DE CONVERSACIÓN</h5>
                                    <small class="text-muted fw-bold">Caso: TCK-<asp:Label ID="lblTicketIdVisor" runat="server"></asp:Label></small>
                                </div>
                                <a href="BandejaTickets.aspx" class="btn btn-sm btn-secondary fw-bold">Cerrar Visor ✖️</a>
                            </div>

                            <div style="max-height: 500px; overflow-y: auto; padding-right: 15px;">
                                <asp:Repeater ID="rptRespuestas" runat="server">
                                    <ItemTemplate>
                                        <div class='<%# If(Eval("MENSAJE").ToString().StartsWith("🎧"), "chat-bubble-agent shadow-sm", "chat-bubble-client shadow-sm") %>'>
                                            <div class="chat-date">
                                                <asp:Label ID="lblFechaMensaje" runat="server"></asp:Label>
                                            </div>
                                            <div class="text-dark fw-bold">
                                                <%# Eval("MENSAJE") %>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>

                                <asp:Panel ID="pnlVacioRespuestas" runat="server" Visible="false" CssClass="text-center py-4">
                                    <p class="text-muted fw-bold">El pasajero aún no ha enviado los detalles. Esperando información...</p>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-4">
                        <div class="aur-card" style="border-top-color: #212529;">
                            <h5 class="section-title" style="color: #212529;">Panel de Respuesta</h5>
                            <p class="small text-muted">Redacta tu respuesta. Se etiquetará automáticamente como Soporte Técnico.</p>
                            
                            <div class="mb-3">
                                <asp:TextBox ID="txtNuevaRespuesta" runat="server" CssClass="form-control bg-light" TextMode="MultiLine" Rows="5" placeholder="Escribe tu respuesta oficial aquí..."></asp:TextBox>
                            </div>
                            
                            <div class="d-grid gap-2">
                                <asp:Button ID="btnEnviarRespuesta" runat="server" Text="Enviar Mensaje 🚀" CssClass="btn btn-primary fw-bold py-2 shadow-sm" />
                                <hr />
                                <p class="small text-muted text-center m-0">Si el problema fue resuelto, cierra el caso.</p>
                                <asp:Button ID="btnCerrarTicket" runat="server" Text="Finalizar y Cerrar Caso 🔒" CssClass="btn btn-danger fw-bold py-2 shadow-sm" OnClientClick="return confirm('¿Estás seguro de cerrar este ticket definitivamente?');" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

        </div>
    </form>
</body>
</html>