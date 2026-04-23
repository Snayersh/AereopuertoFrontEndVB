<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReclamoEquipaje.aspx.vb" Inherits="AereopuertoFrontEndVB.ReclamoEquipaje" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Reclamo de Equipaje - La Aurora</title>
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
        
        /* Badges de Estado para Reclamos */
        .badge-pendiente { background-color: #fff3e0; color: #e65100; font-weight: bold; border-radius: 8px; padding: 6px 12px; }
        .badge-investigacion { background-color: #e3f2fd; color: #0d47a1; font-weight: bold; border-radius: 8px; padding: 6px 12px; }
        .badge-resuelto { background-color: #e8f5e9; color: #2e7d32; font-weight: bold; border-radius: 8px; padding: 6px 12px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center mb-5">
            <h4 class="m-0 fw-bold">🧳 Centro de Asistencia de Equipaje</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver a Mi Perfil</a>
        </div>

        <div class="container" style="max-width: 1100px;">
            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="row">
                <div class="col-md-4">
                    <div class="aur-card">
                        <h5 class="section-title text-danger">Generar Nuevo Reporte</h5>
                        <p class="text-muted small mb-4">Lamentamos los inconvenientes. Por favor, indícanos qué sucedió con tu equipaje.</p>

                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">Seleccione la Maleta Afectada *</label>
                            <asp:DropDownList ID="ddlEquipaje" runat="server" CssClass="form-select bg-light border-secondary" required="true"></asp:DropDownList>
                            <div class="form-text"><small>Solo se muestran los equipajes registrados a tu nombre.</small></div>
                        </div>

                        <div class="mb-4">
                            <label class="form-label small fw-bold text-secondary">Descripción del Problema *</label>
                            <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" required="true" placeholder="Ej: Mi maleta no llegó en la banda de la puerta 4, o mi maleta llegó con el zipper roto..."></asp:TextBox>
                        </div>
                        
                        <asp:Button ID="btnGuardar" runat="server" Text="Enviar Reclamo 📨" CssClass="btn btn-custom w-100 py-3 shadow-sm" />
                    </div>
                </div>

                <div class="col-md-8">
                    <div class="aur-card">
                        <h5 class="section-title">Mis Casos Abiertos y Resueltos</h5>
                        <div class="table-responsive">
                            <table class="table table-hover align-middle">
                                <thead class="table-light">
                                    <tr class="small text-secondary">
                                        <th>NO. CASO</th>
                                        <th>EQUIPAJE ASOCIADO</th>
                                        <th>DETALLE DEL REPORTE</th>
                                        <th class="text-center">ESTADO</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptReclamos" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="fw-bold text-muted">#<%# Eval("ID_RECLAMO") %></td>
                                                <td class="fw-bold text-dark"><%# Eval("INFO_EQUIPAJE") %></td>
                                                <td class="text-secondary small fst-italic">"<%# Eval("DESCRIPCION") %>"</td>
                                                <td class="text-center">
                                                    <asp:Label ID="lblBadgeEstado" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>

                            <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="text-center py-5">
                                <h1 class="opacity-25 mb-3">✅</h1>
                                <p class="text-muted fw-bold">No tienes reclamos de equipaje registrados.</p>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>