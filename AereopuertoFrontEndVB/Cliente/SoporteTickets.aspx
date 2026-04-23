<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="SoporteTickets.aspx.vb" Inherits="AereopuertoFrontEndVB.SoporteTickets" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Centro de Soporte - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .aur-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 30px; border-top: 5px solid #1565c0; margin-bottom: 30px; }
        .section-title { font-size: 1.1rem; font-weight: 800; color: #0d47a1; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 20px; }
        .btn-custom { background-color: #0d47a1; color: white; border: none; font-weight: bold; border-radius: 8px; transition: 0.3s; }
        .btn-custom:hover { background-color: #1565c0; transform: translateY(-2px); box-shadow: 0 5px 15px rgba(13, 71, 161, 0.2); color: white;}
        .table th { background-color: #f8f9fc; color: #555; text-transform: uppercase; font-size: 0.85rem; letter-spacing: 1px; border-bottom: 2px solid #e2e8f0; }
        .table td { vertical-align: middle; }
        
        /* Badges de Estado */
        .badge-abierto { background-color: #e3f2fd; color: #0d47a1; font-weight: bold; border-radius: 6px; padding: 6px 12px; }
        .badge-cerrado { background-color: #f5f5f5; color: #616161; font-weight: bold; border-radius: 6px; padding: 6px 12px; }
        .badge-proceso { background-color: #fff3e0; color: #e65100; font-weight: bold; border-radius: 6px; padding: 6px 12px; }
        
        /* Burbuja de Chat */
        .chat-bubble { background-color: #f8f9fa; border-left: 4px solid #1565c0; padding: 15px; border-radius: 8px; margin-bottom: 15px; }
        .chat-date { font-size: 0.8rem; color: #888; font-weight: bold; margin-bottom: 5px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center mb-5">
            <h4 class="m-0 fw-bold">🎧 Centro de Ayuda y Soporte Técnico</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver a Mi Perfil</a>
        </div>

        <div class="container" style="max-width: 1100px;">
            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <asp:Panel ID="pnlListado" runat="server">
                <div class="row">
                    <div class="col-md-4">
                        <div class="aur-card">
                            <h5 class="section-title">Abrir Nuevo Caso</h5>
                            <p class="text-muted small mb-4">¿Tienes dudas con tu reserva o equipaje? Levanta un ticket y te asistiremos.</p>

                            <div class="mb-3">
                                <label class="form-label small fw-bold text-secondary">Categoría de Ayuda *</label>
                                <asp:DropDownList ID="ddlTipoTicket" runat="server" CssClass="form-select bg-light border-secondary" required="true"></asp:DropDownList>
                            </div>

                            <div class="mb-4">
                                <label class="form-label small fw-bold text-secondary">Asunto / Resumen breve *</label>
                                <asp:TextBox ID="txtAsunto" runat="server" CssClass="form-control" required="true" placeholder="Ej: Error al imprimir mi pase..." MaxLength="150"></asp:TextBox>
                            </div>
                            
                            <asp:Button ID="btnGuardarTicket" runat="server" Text="Enviar Ticket 📨" CssClass="btn btn-custom w-100 py-3 shadow-sm" />
                        </div>
                    </div>

                    <div class="col-md-8">
                        <div class="aur-card" style="border-top-color: #424242;">
                            <h5 class="section-title" style="color: #424242;">Historial de mis Casos</h5>
                            <div class="table-responsive">
                                <table class="table table-hover align-middle">
                                    <thead class="table-light">
                                        <tr>
                                            <th>NO. CASO</th>
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
                                                    <td class="fw-bold text-dark"><%# Eval("ASUNTO") %></td>
                                                    <td class="text-center">
                                                        <asp:Label ID="lblBadgeEstado" runat="server"></asp:Label>
                                                    </td>
                                                    <td class="text-center">
                                                        <a href="SoporteTickets.aspx?id=<%# Eval("ID_TICKET") %>" class="btn btn-sm btn-outline-primary fw-bold shadow-sm">Ver Hilo 💬</a>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>

                                <asp:Panel ID="pnlVacioTickets" runat="server" Visible="false" CssClass="text-center py-4">
                                    <p class="text-muted fw-bold">No tienes tickets de soporte abiertos.</p>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlConversacion" runat="server" Visible="false">
                <div class="aur-card mb-3">
                    <div class="d-flex justify-content-between align-items-center mb-4 border-bottom pb-3">
                        <h5 class="section-title mb-0">HILO DE CONVERSACIÓN</h5>
                        <a href="SoporteTickets.aspx" class="btn btn-sm btn-secondary fw-bold">Cerrar Hilo ✖️</a>
                    </div>

                    <asp:Repeater ID="rptRespuestas" runat="server">
                        <ItemTemplate>
                            <div class="chat-bubble shadow-sm">
                                <div class="chat-date">
                                    <asp:Label ID="lblFechaMensaje" runat="server"></asp:Label>
                                </div>
                                <div class="text-dark">
                                    <%# Eval("MENSAJE") %>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:Panel ID="pnlVacioRespuestas" runat="server" Visible="false" CssClass="text-center py-4">
                        <p class="text-muted fw-bold">Aún no hay mensajes en este ticket. Sé el primero en escribir los detalles.</p>
                    </asp:Panel>
                </div>

                <div class="aur-card" style="border-top-color: #2e7d32;">
                    <h5 class="section-title" style="color: #2e7d32;">Añadir Respuesta / Detalle</h5>
                    <div class="mb-3">
                        <asp:TextBox ID="txtNuevaRespuesta" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" required="true" placeholder="Escribe tu mensaje aquí..."></asp:TextBox>
                    </div>
                    <div class="text-end">
                        <asp:Button ID="btnEnviarRespuesta" runat="server" Text="Enviar Mensaje 🚀" CssClass="btn btn-success fw-bold px-5 py-2 shadow-sm" />
                    </div>
                </div>
            </asp:Panel>

        </div>
    </form>
</body>
</html>