<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Aeropuertos.aspx.vb" Inherits="AereopuertoFrontEndVB.Aeropuertos" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Catálogo de Aeropuertos</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .admin-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 30px; border: none; margin-top: 30px; margin-bottom: 30px; }
        .form-control { height: 45px; border-radius: 8px; }
        .btn-success { background-color: #2e7d32; border: none; height: 45px; font-weight: bold; border-radius: 8px; transition: 0.3s; }
        .btn-success:hover { background-color: #1b5e20; transform: translateY(-2px); }
        .section-title { color: #0d47a1; font-weight: bold; font-size: 1.1rem; border-bottom: 2px solid #e3f2fd; padding-bottom: 5px; margin-bottom: 15px; margin-top: 20px;}
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
                            <h3 class="fw-bold" style="color: #0d47a1;">Registro de Aeropuertos</h3>
                            <p class="text-muted m-0">Agrega nuevos destinos, incluyendo su ubicación geográfica para el sistema de Radar en vivo.</p>
                        </div>

                        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info text-center rounded-3 mb-4">
                            <asp:Label ID="lblMensaje" runat="server" CssClass="fw-bold"></asp:Label>
                        </asp:Panel>

                        <div class="section-title">Datos Generales</div>
                        <div class="row">
                            <div class="col-md-9 mb-4">
                                <label class="form-label fw-bold text-secondary">Nombre del Aeropuerto</label>
                                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" placeholder="Ej. Aeropuerto Internacional La Aurora" required="true"></asp:TextBox>
                            </div>
                            <div class="col-md-3 mb-4">
                                <label class="form-label fw-bold text-secondary">Código IATA</label>
                                <asp:TextBox ID="txtIata" runat="server" CssClass="form-control text-uppercase" placeholder="Ej. GUA" MaxLength="3" required="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6 mb-4">
                                <label class="form-label fw-bold text-secondary">País</label>
                                <asp:TextBox ID="txtPais" runat="server" CssClass="form-control" placeholder="Ej. Guatemala" required="true"></asp:TextBox>
                            </div>
                            <div class="col-md-6 mb-4">
                                <label class="form-label fw-bold text-secondary">Ciudad</label>
                                <asp:TextBox ID="txtCiudad" runat="server" CssClass="form-control" placeholder="Ej. Ciudad de Guatemala" required="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="section-title">Coordenadas para el Radar</div>
                        <div class="row">
                            <div class="col-md-6 mb-4">
                                <label class="form-label fw-bold text-secondary">Latitud (Y)</label>
                                <asp:TextBox ID="txtLatitud" runat="server" CssClass="form-control" placeholder="Ej. 14.5833" required="true"></asp:TextBox>
                            </div>
                            <div class="col-md-6 mb-4">
                                <label class="form-label fw-bold text-secondary">Longitud (X)</label>
                                <asp:TextBox ID="txtLongitud" runat="server" CssClass="form-control" placeholder="Ej. -90.5275" required="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="d-flex justify-content-end border-top pt-4">
                            <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-light border me-2 fw-bold px-4" CausesValidation="false" />
                            <asp:Button ID="btnGuardar" runat="server" Text="💾 Guardar Aeropuerto" CssClass="btn btn-success px-4 shadow-sm" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>