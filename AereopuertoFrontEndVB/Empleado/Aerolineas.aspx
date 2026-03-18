<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Aerolineas.aspx.vb" Inherits="AereopuertoFrontEndVB.Aerolineas" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Gestión de Aerolíneas</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .admin-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 30px; border: none; margin-top: 30px; }
        .form-control { height: 45px; border-radius: 8px; }
        .btn-success { background-color: #2e7d32; border: none; height: 45px; font-weight: bold; border-radius: 8px; }
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
                            <h3 class="fw-bold" style="color: #0d47a1;">Registro de Aerolínea</h3>
                            <p class="text-muted m-0">Agrega nuevas aerolíneas operativas al sistema.</p>
                        </div>

                        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info text-center rounded-3 mb-4">
                            <asp:Label ID="lblMensaje" runat="server" CssClass="fw-bold"></asp:Label>
                        </asp:Panel>

                        <div class="row">
                            <div class="col-md-8 mb-3">
                                <label class="form-label fw-bold text-secondary">Nombre de la Aerolínea</label>
                                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" placeholder="Ej. Iberia L.A.E." required="true"></asp:TextBox>
                            </div>
                            <div class="col-md-4 mb-3">
                                <label class="form-label fw-bold text-secondary">Código IATA</label>
                                <asp:TextBox ID="txtIata" runat="server" CssClass="form-control text-uppercase" placeholder="Ej. IB" MaxLength="3" required="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="mb-4">
                            <label class="form-label fw-bold text-secondary">País de Origen</label>
                            <asp:TextBox ID="txtPais" runat="server" CssClass="form-control" placeholder="Ej. España" required="true"></asp:TextBox>
                        </div>

                        <div class="d-flex justify-content-end mt-2">
                            <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-light border me-2 fw-bold px-4" CausesValidation="false" />
                            <asp:Button ID="btnGuardar" runat="server" Text="💾 Guardar Aerolínea" CssClass="btn btn-success px-4" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>