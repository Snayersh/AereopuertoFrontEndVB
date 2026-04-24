<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="BitacoraSesiones.aspx.vb" Inherits="AereopuertoFrontEndVB.BitacoraSesiones" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Auditoría de Accesos - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .aur-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 30px; border-top: 5px solid #1565c0; margin-bottom: 30px; }
        .section-title { font-size: 1.1rem; font-weight: 800; color: #0d47a1; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 20px; }
        .table th { background-color: #f8f9fc; color: #555; text-transform: uppercase; font-size: 0.85rem; letter-spacing: 1px; border-bottom: 2px solid #e2e8f0; }
        .table td { vertical-align: middle; }
        
        /* Badges de Auditoría */
        .badge-activa { background-color: #e8f5e9; color: #2e7d32; font-weight: bold; border-radius: 6px; padding: 6px 12px; }
        .badge-cerrada { background-color: #f5f5f5; color: #616161; font-weight: bold; border-radius: 6px; padding: 6px 12px; }
        .text-fecha { font-size: 0.9rem; font-weight: bold; color: #424242; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center mb-5">
            <h4 class="m-0 fw-bold">👁️‍🗨️ Auditoría del Sistema: Registro de Accesos</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Panel de Control</a>
        </div>

        <div class="container" style="max-width: 1200px;">
            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="aur-card" style="border-top-color: #424242;">
                <div class="d-flex justify-content-between align-items-center mb-4">
                    <h5 class="section-title mb-0" style="color: #424242;">Bitácora Global de Conexiones</h5>
                    <asp:Button ID="btnActualizar" runat="server" Text="↻ Refrescar Monitor" CssClass="btn btn-outline-secondary btn-sm fw-bold shadow-sm" />
                </div>
                
                <p class="text-muted small mb-4">Este panel es de solo lectura y registra todas las entradas y salidas de los usuarios dentro de la plataforma La Aurora.</p>

                <div class="table-responsive">
                    <table class="table table-hover align-middle">
                        <thead class="table-light">
                            <tr>
                                <th>ID SESIÓN</th>
                                <th>USUARIO AUTENTICADO</th>
                                <th>FECHA / HORA INGRESO</th>
                                <th>FECHA / HORA SALIDA</th>
                                <th class="text-center">ESTADO ACTUAL</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptSesiones" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="text-muted small fw-bold">AUTH-<%# Eval("ID_SESION") %></td>
                                        <td>
                                            <div class="fw-bold text-dark"><%# Eval("NOMBRE_USUARIO") %></div>
                                            <div class="text-secondary small"><%# Eval("CORREO") %></div>
                                        </td>
                                        <td>
                                            <span class="text-fecha"><asp:Label ID="lblFechaInicio" runat="server"></asp:Label></span>
                                        </td>
                                        <td>
                                            <span class="text-fecha"><asp:Label ID="lblFechaFin" runat="server"></asp:Label></span>
                                        </td>
                                        <td class="text-center">
                                            <asp:Label ID="lblBadgeEstado" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>

                    <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="text-center py-5">
                        <p class="text-muted fw-bold">No hay registros de sesiones en el sistema.</p>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </form>
</body>
</html>