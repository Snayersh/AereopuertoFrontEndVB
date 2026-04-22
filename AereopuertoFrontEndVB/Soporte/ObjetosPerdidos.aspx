<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ObjetosPerdidos.aspx.vb" Inherits="AereopuertoFrontEndVB.ObjetosPerdidos" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Registro y Búsqueda de Objetos</title>
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center mb-5">
            <h4 class="m-0 fw-bold">🎒 Gestión de Objetos Perdidos</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Centro de Control</a>
        </div>

        <div class="container" style="max-width: 1000px;">
            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="aur-card">
                <h5 class="section-title">📥 Registrar Nuevo Hallazgo</h5>
                <div class="row g-3">
                    <div class="col-md-7">
                        <label class="form-label text-secondary fw-bold small">Descripción Detallada *</label>
                        <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" required="true" placeholder="Ej. Mochila negra marca Targus con laptop adentro..."></asp:TextBox>
                    </div>
                    <div class="col-md-5">
                        <label class="form-label text-secondary fw-bold small">Lugar Encontrado *</label>
                        <asp:DropDownList ID="ddlLugar" runat="server" CssClass="form-select bg-light border-secondary" required="true">
                            <asp:ListItem Text="-- Seleccionar --" Value=""></asp:ListItem>
                            <asp:ListItem Text="Baños Públicos" Value="Baños Públicos"></asp:ListItem>
                            <asp:ListItem Text="Salas de Abordaje" Value="Salas de Abordaje"></asp:ListItem>
                            <asp:ListItem Text="Control Rayos X" Value="Control Rayos X"></asp:ListItem>
                            <asp:ListItem Text="Mostrador de Aerolínea" Value="Mostrador de Aerolínea"></asp:ListItem>
                            <asp:ListItem Text="Zona de Parqueo" Value="Zona de Parqueo"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                
                <asp:Button ID="btnGuardarObjeto" runat="server" Text="Registrar en Bodega 📦" CssClass="btn btn-custom w-100 py-3 mt-4 shadow-sm" />
            </div>

            <div class="aur-card" style="border-top-color: #2e7d32;">
                <h5 class="section-title" style="color: #2e7d32;">🔍 Búsqueda y Entrega de Objetos</h5>
                
                <div class="input-group mb-4 shadow-sm" style="border-radius: 8px; overflow: hidden;">
                    <span class="input-group-text bg-light fw-bold text-secondary border-0">Filtrar:</span>
                    <asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="form-select border-0 bg-light fw-bold" style="max-width: 200px;">
                        <asp:ListItem Text="📦 En Bodega (Pendientes)" Value="EN BODEGA" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="✅ Reclamados (Entregados)" Value="RECLAMADO"></asp:ListItem>
                        <asp:ListItem Text="📋 Mostrar Todos el Historial" Value="TODOS"></asp:ListItem>
                    </asp:DropDownList>

                    <asp:TextBox ID="txtBusqueda" runat="server" CssClass="form-control border-0 px-3" placeholder="Buscar por palabra clave (mochila, pasaporte...)..."></asp:TextBox>
                    
                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-success fw-bold px-5" formnovalidate="formnovalidate" CausesValidation="false" />
                </div>

                <div class="table-responsive">
                    <table class="table table-hover align-middle">
                        <thead>
                            <tr>
                                <th>Artículo</th>
                                <th>Ubicación de Hallazgo</th>
                                <th>Estado</th>
                                <th>Gestión de Entrega</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptObjetos" runat="server" OnItemCommand="rptObjetos_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td class="fw-bold text-dark"><%# Eval("descripcion") %></td>
                                        <td class="text-muted small"><%# Eval("lugar_encontrado") %></td>
                                        <td>
                                            <asp:Label ID="lblBadgeEstado" runat="server"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Panel ID="pnlEntregar" runat="server">
                                                <div class="input-group input-group-sm">
                                                    <asp:TextBox ID="txtEntregarA" runat="server" CssClass="form-control" placeholder="Nombre completo del dueño..."></asp:TextBox>
                                                    <asp:Button ID="btnEntregar" runat="server" Text="Entregar" CommandName="Entregar" CommandArgument='<%# Eval("id_objeto") %>' CssClass="btn btn-primary fw-bold" formnovalidate="formnovalidate" CausesValidation="false" />
                                                </div>
                                            </asp:Panel>

                                            <asp:Panel ID="pnlEntregado" runat="server">
                                                <span class="text-success fw-bold small"> Entregado a: <br/> <%# Eval("entregado_a") %></span>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    
                    <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="text-center py-4">
                        <p class="text-muted fw-bold">No se encontraron objetos con los criterios actuales.</p>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </form>
</body>
</html>