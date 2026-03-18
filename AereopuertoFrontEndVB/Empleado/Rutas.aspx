<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Rutas.aspx.vb" Inherits="AereopuertoFrontEndVB.Rutas" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Gestión de Rutas - Operaciones</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #00838f; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .employee-card { background: white; border-radius: 12px; box-shadow: 0 8px 20px rgba(0,0,0,0.05); padding: 30px; border-top: 5px solid #00bcd4; margin-top: 30px; margin-bottom: 30px; }
        .form-control, .form-select { height: 45px; border-radius: 8px; }
        .btn-operativo { background-color: #00838f; border: none; color: white; font-weight: bold; border-radius: 8px; transition: 0.3s; padding: 10px 30px; }
        .btn-operativo:hover { background-color: #006064; color: white; transform: translateY(-2px); }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">🧑‍💼 Operaciones - Tiempos de Vuelo</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Dashboard</a>
        </div>

        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-8">
                    <div class="employee-card">
                        <div class="border-bottom pb-3 mb-4 text-center">
                            <h3 class="fw-bold" style="color: #00838f;">Establecer Rutas y Tiempos</h3>
                            <p class="text-muted m-0">Define la duración en minutos entre dos aeropuertos.</p>
                        </div>

                        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info text-center rounded-3 mb-4">
                            <asp:Label ID="lblMensaje" runat="server" CssClass="fw-bold"></asp:Label>
                        </asp:Panel>

                        <div class="row">
                            <div class="col-md-6 mb-4">
                                <label class="form-label fw-bold text-secondary">🛫 Aeropuerto de Origen</label>
                                <asp:DropDownList ID="ddlOrigen" runat="server" CssClass="form-select" required="true"></asp:DropDownList>
                            </div>
                            <div class="col-md-6 mb-4">
                                <label class="form-label fw-bold text-secondary">🛬 Aeropuerto de Destino</label>
                                <asp:DropDownList ID="ddlDestino" runat="server" CssClass="form-select" required="true"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="row justify-content-center">
                            <div class="col-md-6 mb-4">
                                <label class="form-label fw-bold text-secondary">⏱️ Tiempo Estimado de Vuelo</label>
                                <div class="input-group">
                                    <asp:TextBox ID="txtDuracion" runat="server" CssClass="form-control text-center fs-5" TextMode="Number" placeholder="Ej. 120" required="true"></asp:TextBox>
                                    <span class="input-group-text bg-light fw-bold text-secondary">minutos</span>
                                </div>
                            </div>
                        </div>

                        <div class="d-flex justify-content-center border-top pt-4">
                            <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-light border me-3 fw-bold px-4" CausesValidation="false" />
                            <asp:Button ID="btnGuardar" runat="server" Text="💾 Guardar Ruta" CssClass="btn btn-operativo shadow-sm" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>