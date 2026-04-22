<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="adminPuertas.aspx.vb" Inherits="AereopuertoFrontEndVB.GestionPuertas" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Gestión de Puertas - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .aur-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 30px; border-top: 5px solid #1565c0; margin-bottom: 30px; }
        .section-title { font-size: 1.1rem; font-weight: 800; color: #0d47a1; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 20px; }
        .btn-custom { background-color: #0d47a1; color: white; font-weight: bold; border-radius: 8px; transition: 0.3s; }
        .btn-custom:hover { background-color: #1565c0; transform: translateY(-2px); box-shadow: 0 5px 15px rgba(13, 71, 161, 0.2); color: white;}
        
        /* Estados de Puerta */
        .badge-disponible { background-color: #e8f5e9; color: #2e7d32; font-weight: bold; border-radius: 8px; padding: 6px 12px; }
        .badge-ocupada { background-color: #ffebee; color: #c62828; font-weight: bold; border-radius: 8px; padding: 6px 12px; }
        .badge-mantenimiento { background-color: #fff3e0; color: #e65100; font-weight: bold; border-radius: 8px; padding: 6px 12px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center mb-5">
            <h4 class="m-0 fw-bold">🚪 Control de Puertas de Embarque</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Panel</a>
        </div>

        <div class="container">
            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="row">
                <div class="col-md-4">
                    <div class="aur-card">
                        <h5 class="section-title">Nueva Puerta</h5>
                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">Código de Puerta (Ej: A1, B12)</label>
                            <asp:TextBox ID="txtCodigo" runat="server" CssClass="form-control text-uppercase" required="true" MaxLength="10"></asp:TextBox>
                        </div>
                        <div class="mb-4">
                            <label class="form-label small fw-bold text-secondary">Aeropuerto Destino</label>
                            <asp:DropDownList ID="ddlAeropuerto" runat="server" CssClass="form-select bg-light border-secondary" required="true"></asp:DropDownList>
                        </div>
                        <asp:Button ID="btnGuardar" runat="server" Text="Registrar Puerta 📥" CssClass="btn btn-custom w-100 py-3 shadow-sm" />
                    </div>
                </div>

                <div class="col-md-8">
                    <div class="aur-card">
                        <h5 class="section-title">Monitor de Puertas en Tiempo Real</h5>
                        <div class="table-responsive">
                            <table class="table table-hover align-middle">
                                <thead class="table-light">
                                    <tr class="small text-secondary">
                                        <th>CÓDIGO</th>
                                        <th>AEROPUERTO</th>
                                        <th>ESTADO ACTUAL</th>
                                        <th class="text-center">ACCIONES</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptPuertas" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="fw-bold fs-5 text-primary"><%# Eval("CODIGO") %></td>
                                                <td class="text-secondary"><%# Eval("NOMBRE_AEROPUERTO") %></td>
                                                <td>
                                                    <asp:Label ID="lblBadgeEstado" runat="server"></asp:Label>
                                                </td>
                                                <td class="text-center">
                                                    <asp:Button ID="btnMantenimiento" runat="server" Text="🔧" CssClass="btn btn-sm btn-outline-warning" ToolTip="Poner en Mantenimiento" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>