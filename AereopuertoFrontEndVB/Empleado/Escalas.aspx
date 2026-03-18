<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Escalas.aspx.vb" Inherits="AereopuertoFrontEndVB.Escalas" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Gestión de Escalas - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .admin-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 40px; margin-top: 40px; border-top: 5px solid #d32f2f; }
        .form-control, .form-select { height: 50px; border-radius: 8px; font-weight: 500; }
        .btn-danger { background-color: #d32f2f; border: none; font-weight: bold; border-radius: 8px; font-size: 1.1rem; transition: 0.3s; }
        .btn-danger:hover { background-color: #b71c1c; transform: translateY(-2px); box-shadow: 0 5px 15px rgba(211, 47, 47, 0.3); }
        .section-title { font-size: 0.9rem; font-weight: bold; color: #d32f2f; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 15px; border-bottom: 2px solid #ffebee; padding-bottom: 5px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">🛑 Logística de Escalas</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Panel</a>
        </div>

        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-10 col-lg-8">
                    <div class="admin-card">
                        <div class="text-center mb-4">
                            <h2 class="fw-bold text-dark">Programar Nueva Escala</h2>
                            <p class="text-muted">Añade paradas intermedias a un vuelo existente.</p>
                        </div>

                        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4">
                            <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                        </asp:Panel>

                        <div class="section-title">1. Selección de Ruta</div>
                        <div class="row mb-4">
                            <div class="col-md-12 mb-3">
                                <label class="form-label fw-bold text-secondary">Vuelo Principal</label>
                                <asp:DropDownList ID="ddlVuelo" runat="server" CssClass="form-select text-primary fw-bold" required="true"></asp:DropDownList>
                            </div>
                            <div class="col-md-12 mb-3">
                                <label class="form-label fw-bold text-secondary">Aeropuerto de Parada (Escala)</label>
                                <asp:DropDownList ID="ddlAeropuerto" runat="server" CssClass="form-select" required="true"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="section-title">2. Detalles de Tiempos</div>
                        <div class="row mb-4">
                            <div class="col-md-4 mb-3">
                                <label class="form-label fw-bold text-secondary">Orden de Parada</label>
                                <asp:TextBox ID="txtOrden" runat="server" CssClass="form-control text-center" TextMode="Number" min="1" placeholder="Ej: 1" required="true"></asp:TextBox>
                                <div class="form-text small">1 = Primera parada.</div>
                            </div>
                            <div class="col-md-4 mb-3">
                                <label class="form-label fw-bold text-secondary">Llegada a Escala</label>
                                <asp:TextBox ID="txtHoraLlegada" runat="server" CssClass="form-control" TextMode="DateTimeLocal" required="true"></asp:TextBox>
                            </div>
                            <div class="col-md-4 mb-3">
                                <label class="form-label fw-bold text-secondary">Salida de Escala</label>
                                <asp:TextBox ID="txtHoraSalida" runat="server" CssClass="form-control" TextMode="DateTimeLocal" required="true"></asp:TextBox>
                            </div>
                        </div>

                        <asp:Button ID="btnGuardarEscala" runat="server" Text="Guardar Escala 💾" CssClass="btn btn-danger w-100 py-3 shadow-sm mt-2" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>