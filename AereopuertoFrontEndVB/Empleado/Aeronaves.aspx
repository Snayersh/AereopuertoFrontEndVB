<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Aeronaves.aspx.vb" Inherits="AereopuertoFrontEndVB.Aeronaves" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Catálogo de Aeronaves</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .admin-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 30px; border: none; margin-top: 30px; }
        .form-control, .form-select { height: 45px; border-radius: 8px; }
        .btn-success { background-color: #2e7d32; border: none; height: 45px; font-weight: bold; border-radius: 8px; transition: 0.3s; }
        .btn-success:hover { background-color: #1b5e20; transform: translateY(-2px); }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">⚙️ Administración de Catálogos</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Dashboard</a>
        </div>

        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-8">
                    <div class="admin-card">
                        <div class="border-bottom pb-3 mb-4">
                            <h3 class="fw-bold" style="color: #0d47a1;">Registro de Flota (Aeronaves)</h3>
                            <p class="text-muted m-0">Asigna nuevos aviones a las aerolíneas operativas.</p>
                        </div>

                        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info text-center rounded-3 mb-4">
                            <asp:Label ID="lblMensaje" runat="server" CssClass="fw-bold"></asp:Label>
                        </asp:Panel>

                        <div class="mb-4">
                            <label class="form-label fw-bold text-secondary">Aerolínea Propietaria</label>
                            <asp:DropDownList ID="ddlAerolineas" runat="server" CssClass="form-select" required="true">
                                </asp:DropDownList>
                        </div>

                        <div class="row">
                            <div class="col-md-8 mb-4">
                                <label class="form-label fw-bold text-secondary">Modelo de la Aeronave</label>
                                <asp:TextBox ID="txtModelo" runat="server" CssClass="form-control" placeholder="Ej. Airbus A350-900" required="true"></asp:TextBox>
                            </div>
                            <div class="col-md-4 mb-4">
                                <label class="form-label fw-bold text-secondary">Capacidad (Pasajeros)</label>
                                <asp:TextBox ID="txtCapacidad" runat="server" CssClass="form-control" TextMode="Number" placeholder="Ej. 348" required="true" min="1"></asp:TextBox>
                            </div>
                        </div>

                        <div class="d-flex justify-content-end border-top pt-4">
                            <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-light border me-2 fw-bold px-4" CausesValidation="false" />
                            <asp:Button ID="btnGuardar" runat="server" Text="💾 Guardar Aeronave" CssClass="btn btn-success px-4 shadow-sm" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>