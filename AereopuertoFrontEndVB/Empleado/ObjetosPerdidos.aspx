<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ObjetosPerdidos.aspx.vb" Inherits="AereopuertoFrontEndVB.ObjetosPerdidos" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Registro y Búsqueda de Objetos</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; padding-top: 40px; padding-bottom: 50px; }
        .form-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); padding: 30px; border-top: 6px solid #1976d2; margin-bottom: 30px; }
        .section-title { font-size: 1.2rem; font-weight: 800; color: #1976d2; border-bottom: 2px solid #bbdefb; padding-bottom: 10px; margin-bottom: 20px; }
        .btn-primary-custom { background-color: #1976d2; color: white; border: none; font-weight: bold; }
        .btn-primary-custom:hover { background-color: #1565c0; transform: translateY(-2px); color: white; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container" style="max-width: 900px;">
            <div class="mb-4 text-center">
                <a href="../Default.aspx" class="btn btn-outline-secondary fw-bold rounded-pill px-4 shadow-sm">← Volver al Tablero Principal</a>
            </div>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="form-card">
                <h5 class="section-title">🎒 Registrar Nuevo Objeto Hallado</h5>
                <div class="row">
                    <div class="col-md-7 mb-3">
                        <label class="form-label text-secondary fw-bold small">Descripción del Artículo *</label>
                        <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" required="true" placeholder="Ej. Mochila negra..."></asp:TextBox>
                    </div>
                    <div class="col-md-5 mb-3">
                        <label class="form-label text-secondary fw-bold small">Lugar Encontrado *</label>
                        <asp:DropDownList ID="ddlLugar" runat="server" CssClass="form-select" required="true">
                            <asp:ListItem Text="-- Seleccionar --" Value=""></asp:ListItem>
                            <asp:ListItem Text="Baños Públicos" Value="Baños Públicos"></asp:ListItem>
                            <asp:ListItem Text="Salas de Abordaje" Value="Salas de Abordaje"></asp:ListItem>
                            <asp:ListItem Text="Control Rayos X" Value="Control Rayos X"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                
                <asp:Button ID="btnGuardarObjeto" runat="server" Text="📥 Registrar en Bodega" CssClass="btn btn-primary-custom w-100 shadow-sm py-2 mt-2" />
            </div>

            <div class="form-card" style="border-top-color: #388e3c;">
                <h5 class="section-title" style="color: #388e3c; border-bottom-color: #c8e6c9;">🔍 Buscador y Entrega de Objetos</h5>
                
                <div class="input-group mb-4 shadow-sm">
                    <span class="input-group-text bg-light fw-bold text-secondary">Filtro:</span>
                    <asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="form-select" style="max-width: 180px;">
                        <asp:ListItem Text="📦 En Bodega" Value="EN BODEGA" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="✅ Reclamados" Value="RECLAMADO"></asp:ListItem>
                        <asp:ListItem Text="📋 Mostrar Todos" Value="TODOS"></asp:ListItem>
                    </asp:DropDownList>

                    <asp:TextBox ID="txtBusqueda" runat="server" CssClass="form-control" placeholder="Buscar por descripción o lugar..."></asp:TextBox>
                    
                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-success fw-bold px-4" 
                        formnovalidate="formnovalidate" CausesValidation="false" />
                </div>

                <div class="table-responsive">
                    <table class="table table-hover align-middle border">
                        <thead class="table-light">
                            <tr>
                                <th>Descripción</th>
                                <th>Lugar</th>
                                <th>Estado</th>
                                <th>Acción / Entregar a:</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptObjetos" runat="server" OnItemCommand="rptObjetos_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td class="fw-bold text-dark"><%# Eval("descripcion") %></td>
                                        <td class="text-muted small"><%# Eval("lugar_encontrado") %></td>
                                        <td>
                                            <span class='badge <%# If(Eval("estado_reclamo").ToString() = "EN BODEGA", "bg-warning text-dark", "bg-success") %>'>
                                                <%# Eval("estado_reclamo") %>
                                            </span>
                                        </td>
                                        <td>
                                            <asp:Panel ID="pnlEntregar" runat="server" Visible='<%# Eval("estado_reclamo").ToString() = "EN BODEGA" %>'>
                                                <div class="input-group input-group-sm">
                                                    <asp:TextBox ID="txtEntregarA" runat="server" CssClass="form-control" placeholder="Nombre del dueño..."></asp:TextBox>
                                                    <asp:Button ID="btnEntregar" runat="server" Text="Entregar" CommandName="Entregar" CommandArgument='<%# Eval("id_objeto") %>' CssClass="btn btn-primary fw-bold" formnovalidate="formnovalidate" CausesValidation="false" />
                                                </div>
                                            </asp:Panel>

                                            <asp:Panel ID="pnlEntregado" runat="server" Visible='<%# Eval("estado_reclamo").ToString() = "RECLAMADO" %>'>
                                                <span class="text-success fw-bold">Entregado a: <%# Eval("entregado_a") %></span>
                                            </asp:Panel>
                                        </td>
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