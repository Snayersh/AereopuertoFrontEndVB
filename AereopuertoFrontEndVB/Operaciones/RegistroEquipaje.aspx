<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RegistroEquipaje.aspx.vb" Inherits="AereopuertoFrontEndVB.RegistroEquipaje" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Registro de Equipaje - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .luggage-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 40px; margin-top: 40px; border-top: 5px solid #f39c12; }
        .form-control { height: 50px; border-radius: 8px; font-size: 1.1rem; }
        .weight-input { font-size: 2rem; text-align: right; font-weight: bold; color: #d35400; height: 70px; }
        .input-group-text { font-size: 1.5rem; font-weight: bold; background-color: #f8f9fa; border-radius: 0 8px 8px 0; }
        .btn-warning { background-color: #f39c12; border: none; color: white; height: 55px; font-weight: bold; font-size: 1.2rem; transition: 0.3s; }
        .btn-warning:hover { background-color: #e67e22; transform: translateY(-2px); box-shadow: 0 5px 15px rgba(243, 156, 18, 0.3); color: white; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">⚖️ Control de Equipaje</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Panel</a>
        </div>

        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-8 col-lg-6">
                    <div class="luggage-card">
                        <div class="text-center mb-4">
                            <h2 class="fw-bold text-dark">Registrar Maleta</h2>
                            <p class="text-muted">Ingresa el localizador del pasajero y el peso marcado en la báscula.</p>
                        </div>

                        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4">
                            <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                        </asp:Panel>

                        <div class="mb-4">
                            <label class="form-label fw-bold text-secondary">Localizador de Reserva</label>
                            <asp:TextBox ID="txtCodigo" runat="server" CssClass="form-control text-uppercase fw-bold text-center fs-4" placeholder="Ej: B-A1B2C" required="true"></asp:TextBox>
                        </div>

                        <div class="mb-4">
                            <label class="form-label fw-bold text-secondary">Peso de la Maleta</label>
                            <div class="input-group">
                                <asp:TextBox ID="txtPeso" runat="server" CssClass="form-control weight-input" placeholder="0.00" TextMode="Number" step="0.01" required="true"></asp:TextBox>
                                <span class="input-group-text">KG</span>
                            </div>
                        </div>

                        <div class="mb-4">
                            <label class="form-label fw-bold text-secondary">Descripción / Observaciones</label>
                            <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" placeholder="Ej: Maleta de tela color azul, con candado" required="true"></asp:TextBox>
                        </div>

                        <asp:Button ID="btnRegistrar" runat="server" Text="Imprimir Etiqueta y Guardar 🖨️" CssClass="btn btn-warning w-100 rounded-pill shadow-sm mt-2" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>