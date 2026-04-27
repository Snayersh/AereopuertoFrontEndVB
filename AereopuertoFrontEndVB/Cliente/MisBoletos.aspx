<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="MisBoletos.aspx.vb" Inherits="AereopuertoFrontEndVB.MisBoletos" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Mis Boletos - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .ticket-container { max-width: 800px; margin: 40px auto; }
        
        /* Estilos del Boleto */
        .ticket-card { background: white; border-radius: 12px; box-shadow: 0 8px 20px rgba(0,0,0,0.08); margin-bottom: 25px; display: flex; overflow: hidden; border-left: 8px solid #0d47a1; transition: transform 0.2s; }
        .ticket-card:hover { transform: scale(1.02); }
        .ticket-main { padding: 25px; flex-grow: 1; }
        .ticket-side { background-color: #f8f9fa; padding: 25px; border-left: 2px dashed #dee2e6; display: flex; flex-direction: column; justify-content: center; align-items: center; min-width: 200px; }
        
        .route-text { font-size: 1.4rem; font-weight: bold; color: #333; }
        .flight-icon { font-size: 1.5rem; color: #1976d2; margin: 0 15px; }
        .ticket-label { font-size: 0.75rem; text-transform: uppercase; color: #888; font-weight: bold; margin-bottom: 2px; }
        .ticket-value { font-size: 1.1rem; font-weight: bold; color: #2c3e50; margin-bottom: 15px; }
        
        .badge-reservado { background-color: #fff3e0; color: #e65100; padding: 5px 10px; border-radius: 20px; font-size: 0.85rem; }
        .badge-pagado { background-color: #e8f5e9; color: #2e7d32; padding: 5px 10px; border-radius: 20px; font-size: 0.85rem; }
        .badge-cancelado { background-color: #ffebee; color: #c62828; padding: 5px 10px; border-radius: 20px; font-size: 0.85rem; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">🎫 Portal de Pasajeros</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Inicio</a>
        </div>

        <div class="container ticket-container">
            <div class="text-center mb-5">
                <h2 class="fw-bold text-dark">Mis Viajes</h2>
                <p class="text-muted">Historial de reservas y pases de abordar</p>
            </div>
            
            <div class="card mb-4 shadow-sm">
                <div class="card-body d-flex align-middle justify-content-between">
                    <div class="d-flex align-items-center">
                        <i class="bi bi-filter-left fs-4 me-2"></i>
                        <span class="fw-bold me-3">Filtrar por:</span>
                        <asp:DropDownList ID="ddlFiltroEstado" runat="server" AutoPostBack="true" 
                            OnSelectedIndexChanged="ddlFiltroEstado_SelectedIndexChanged" 
                            CssClass="form-select shadow-sm">
                            <asp:ListItem Text="Pendiente de Pago" Value="1" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Pagados" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Cancelados" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Todos los boletos" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div>
                        <span class="text-muted small">Mostrando tus boletos con estado seleccionado.</span>
                    </div>
                </div>
            </div>

            <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="text-center py-5">
                <h1 style="font-size: 4rem; color: #ccc;">🧳</h1>
                <h4 class="text-secondary fw-bold mt-3">Aún no tienes vuelos programados</h4>
                <p class="text-muted">¿Qué esperas para planear tu próxima aventura?</p>
                <a href="Reservas.aspx" class="btn btn-primary mt-2 rounded-pill px-4">Comprar Boleto</a>
            </asp:Panel>

            <asp:Repeater ID="rptBoletos" runat="server">
                <ItemTemplate>
                    <div class="ticket-card">
                        
                        <div class="ticket-main">
                            <div class="d-flex align-items-center mb-4">
                                <span class="route-text"><%# Eval("origen") %></span>
                                <span class="flight-icon">✈️</span>
                                <span class="route-text"><%# Eval("destino") %></span>
                            </div>
                            
                            <div class="row">
                                <div class="col-4">
                                    <div class="ticket-label">FECHA</div>
                                    <div class="ticket-value"><%# Eval("fecha_salida") %></div>
                                </div>
                                <div class="col-4">
                                    <div class="ticket-label">HORA SALIDA</div>
                                    <div class="ticket-value"><%# Eval("hora_salida") %></div>
                                </div>
                                <div class="col-4">
                                    <div class="ticket-label">CABINA</div>
                                    <div class="ticket-value text-primary"><%# Eval("clase_cabina") %></div>
                                </div>
                                <div class="col-4 mt-3">
                                    <div class="ticket-label">ASIENTO</div>
                                    <div class="ticket-value text-danger"><%# Eval("asiento_asignado") %></div>
                                </div>
                            </div>
                        </div> <div class="ticket-side">
                            <div class="ticket-label text-center">LOCALIZADOR</div>
                            <h3 class="fw-bold text-dark mb-3"><%# Eval("codigo_reserva") %></h3>
                            
                            <div class="ticket-label text-center">ESTADO</div>
                            <span class='<%# If(Eval("estado_boleto").ToString().ToUpper() = "PAGADO", "badge-pagado", If(Eval("estado_boleto").ToString().ToUpper() = "CANCELADO", "badge-cancelado", "badge-reservado")) %> fw-bold mb-3'>
                                <%# Eval("estado_boleto") %>
                            </span>

                            <asp:Panel ID="pnlAcciones" runat="server" Visible='<%# Eval("estado_boleto").ToString().ToUpper() = "RESERVADO" %>'>
                                <a href='Pagos.aspx?codigo=<%# Eval("codigo_reserva") %>' class="btn btn-sm btn-success w-100 mb-2 fw-bold shadow-sm">💳 Pagar Ahora</a>
                                
                                <asp:LinkButton ID="btnCancelar" runat="server" CssClass="btn btn-sm btn-outline-danger w-100 fw-bold" 
                                    CommandName="CancelarReserva" CommandArgument='<%# Eval("codigo_reserva") %>' 
                                    OnClientClick="return confirm('¿Estás seguro que deseas cancelar esta reserva? El asiento será liberado para otros pasajeros.');">
                                    ❌ Cancelar
                                </asp:LinkButton>
                            </asp:Panel>

                            <asp:Panel ID="pnlAccionesPagado" runat="server" Visible='<%# Eval("estado_boleto").ToString().ToUpper() = "PAGADO" %>'>
                                <a href='PaseAbordar.aspx?codigo=<%# Eval("codigo_reserva") %>' target="_blank" class="btn btn-sm btn-primary w-100 fw-bold shadow-sm">
                                    🖨️ Imprimir Pase
                                </a>
                                <div class="small text-center mt-2 text-success fw-bold">¡Listo para volar!</div>
                            </asp:Panel>
                        </div>

                    </div>
                </ItemTemplate>
            </asp:Repeater>
            
            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger text-center">
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </asp:Panel>
        </div>
    </form>
</body>
</html>