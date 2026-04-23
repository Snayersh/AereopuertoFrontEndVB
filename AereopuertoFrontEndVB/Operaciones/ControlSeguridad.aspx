<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ControlSeguridad.aspx.vb" Inherits="AereopuertoFrontEndVB.ControlSeguridad" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Control de Seguridad - La Aurora</title>
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
        
        /* Badges de Seguridad */
        .badge-aprobado { background-color: #e8f5e9; color: #2e7d32; font-weight: bold; border-radius: 6px; padding: 6px 12px; font-size: 0.85rem; }
        .badge-alerta { background-color: #ffebee; color: #c62828; font-weight: bold; border-radius: 6px; padding: 6px 12px; font-size: 0.85rem; }
        .badge-revision { background-color: #fff3e0; color: #e65100; font-weight: bold; border-radius: 6px; padding: 6px 12px; font-size: 0.85rem; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center mb-5">
            <h4 class="m-0 fw-bold">🛂 Punto de Control y Seguridad</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Panel</a>
        </div>

        <div class="container" style="max-width: 1100px;">
            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="row">
                <div class="col-md-4">
                    <div class="aur-card">
                        <h5 class="section-title">Nueva Inspección</h5>

                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">ID de la Persona (Pasajero/Empleado) *</label>
                            <asp:TextBox ID="txtIdPersona" runat="server" CssClass="form-control bg-light text-center fw-bold" TextMode="Number" required="true" placeholder="Ej: 450"></asp:TextBox>
                        </div>

                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">Punto / Tipo de Revisión *</label>
                            <asp:DropDownList ID="ddlTipoRevision" runat="server" CssClass="form-select border-secondary" required="true"></asp:DropDownList>
                        </div>

                        <div class="mb-4">
                            <label class="form-label small fw-bold text-secondary">Veredicto de Seguridad *</label>
                            <asp:DropDownList ID="ddlResultado" runat="server" CssClass="form-select border-secondary fw-bold" required="true">
                                <asp:ListItem Text="-- Seleccione Veredicto --" Value=""></asp:ListItem>
                                <asp:ListItem Text="🟢 APROBADO - Sin Novedades" Value="APROBADO"></asp:ListItem>
                                <asp:ListItem Text="🟠 RETENIDO - Inspección Manual" Value="INSPECCION MANUAL ADICIONAL"></asp:ListItem>
                                <asp:ListItem Text="🔴 ALERTA - Objeto Prohibido" Value="ALERTA DE SEGURIDAD"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        
                        <asp:Button ID="btnGuardar" runat="server" Text="Registrar Veredicto 🔒" CssClass="btn btn-custom w-100 py-3 shadow-sm" />
                    </div>
                </div>

                <div class="col-md-8">
                    <div class="aur-card" style="border-top-color: #424242;">
                        <h5 class="section-title" style="color: #424242;">Monitor de Tránsito Seguro</h5>
                        <div class="table-responsive">
                            <table class="table table-hover align-middle">
                                <thead class="table-light">
                                    <tr>
                                        <th>ID INSPECCIÓN</th>
                                        <th>PUNTO DE CONTROL</th>
                                        <th>PERSONA (ID)</th>
                                        <th>FECHA Y HORA</th>
                                        <th class="text-center">RESULTADO</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptRevisiones" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="text-muted small fw-bold">CHK-<%# Eval("ID_REVISION") %></td>
                                                <td class="fw-bold text-dark"><%# Eval("TIPO_REVISION") %></td>
                                                <td class="text-secondary fw-bold">👤 <%# Eval("ID_PERSONA") %></td>
                                                <td class="text-secondary small">
                                                    <asp:Label ID="lblFecha" runat="server"></asp:Label>
                                                </td>
                                                <td class="text-center">
                                                    <asp:Label ID="lblBadgeResultado" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>

                            <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="text-center py-4">
                                <p class="text-muted fw-bold">No hay registros de seguridad recientes.</p>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>