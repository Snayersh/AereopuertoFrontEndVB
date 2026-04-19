<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Equipaje.aspx.vb" Inherits="AereopuertoFrontEndVB.Equipaje" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Gestión de Equipaje - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .main-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 35px; border-top: 5px solid #ff9800; margin-top: 40px; }
        .form-control, .form-select { height: 50px; border-radius: 8px; }
        .baggage-item { background: #fff8e1; border: 1px solid #ffe082; border-left: 5px solid #ffb300; border-radius: 8px; padding: 15px; margin-bottom: 15px; display: flex; align-items: center; }
        .baggage-icon { font-size: 2rem; margin-right: 15px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">🧳 Portal de Equipaje</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Inicio</a>
        </div>

        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-10">
                    <div class="main-card">
                        <div class="text-center mb-4">
                            <h2 class="fw-bold" style="color: #ff9800;">Documentación de Equipaje</h2>
                            <p class="text-muted">Registra tus maletas antes de llegar al aeropuerto para agilizar tu abordaje.</p>
                        </div>

                        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info text-center rounded-3 mb-4 fw-bold">
                            <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                        </asp:Panel>

                        <div class="mb-5 bg-light p-4 rounded-3 border">
                            <label class="form-label fw-bold text-secondary">1. Selecciona tu Reserva</label>
                            <asp:DropDownList ID="ddlBoletos" runat="server" CssClass="form-select fw-bold text-primary" AutoPostBack="true" OnSelectedIndexChanged="ddlBoletos_SelectedIndexChanged"></asp:DropDownList>
                        </div>

                        <asp:Panel ID="pnlGestionEquipaje" runat="server" Visible="false">
                            <div class="row">
                                <div class="col-md-5 border-end pe-md-4">
                                    <h5 class="fw-bold text-primary mb-3">2. Agregar Nueva Maleta</h5>
                                    
                                    <div class="mb-3">
                                        <label class="form-label fw-bold text-secondary">Tipo de Equipaje *</label>
                                        <asp:DropDownList ID="ddlTipoEquipaje" runat="server" CssClass="form-select" required="true">
                                            <asp:ListItem Text="-- Seleccionar --" Value=""></asp:ListItem>
                                            <asp:ListItem Text="Maleta de Bodega (Documentada)" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Equipaje de Mano (Cabina)" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Artículo Personal (Mochila)" Value="3"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>

                                    <div class="mb-3">
                                        <label class="form-label fw-bold text-secondary">Peso Aproximado (Libras) *</label>
                                        <asp:TextBox ID="txtPeso" runat="server" CssClass="form-control" TextMode="Number" step="0.1" required="true" placeholder="Ej: 45.5"></asp:TextBox>
                                    </div>

                                    <div class="mb-4">
                                        <label class="form-label fw-bold text-secondary">Descripción (Color, marca, tamaño)</label>
                                        <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" required="true" placeholder="Ej: Maleta roja grande marca Samsonite"></asp:TextBox>
                                    </div>

                                    <asp:Button ID="btnRegistrarEquipaje" runat="server" Text="Registrar Maleta ➕" CssClass="btn btn-warning w-100 fw-bold shadow-sm" style="height: 50px; font-size: 1.1rem; color: #424242;" />
                                </div>

                                <div class="col-md-7 ps-md-4 mt-4 mt-md-0">
                                    <h5 class="fw-bold text-primary mb-3">3. Tu Equipaje Documentado</h5>
                                    
                                    <asp:Panel ID="pnlVacio" runat="server" Visible="true" CssClass="text-center py-4">
                                        <h1 class="text-muted opacity-50">👜</h1>
                                        <p class="text-muted">Aún no has registrado equipaje para este vuelo.</p>
                                    </asp:Panel>

                                    <asp:Repeater ID="rptEquipaje" runat="server">
                                        <ItemTemplate>
                                            <div class="baggage-item shadow-sm">
                                                <div class="baggage-icon">🧳</div>
                                                <div>
                                                    <h6 class="fw-bold m-0 text-dark"><%# Eval("descripcion") %></h6>
                                                    <small class="text-muted fw-bold">Peso: <%# Eval("peso") %> Libras</small>
                                                    <br />
                                                    <span class="badge bg-secondary mt-1"><%# Eval("tipo_equipaje") %></span>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </asp:Panel>

                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>