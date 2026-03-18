<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CheckIn.aspx.vb" Inherits="AereopuertoFrontEndVB.CheckIn" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Validación de Abordaje - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .scan-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 40px; margin-top: 40px; }
        .scan-input { height: 70px; font-size: 2rem; text-align: center; letter-spacing: 3px; font-weight: bold; border: 3px dashed #0d47a1; border-radius: 15px; text-transform: uppercase; }
        .scan-input:focus { border-color: #1976d2; box-shadow: 0 0 15px rgba(25, 118, 210, 0.3); outline: none; }
        .info-box { background-color: #f8f9fa; border-radius: 10px; padding: 20px; border-left: 5px solid #0d47a1; margin-top: 20px; }
        .info-label { font-size: 0.8rem; color: #777; text-transform: uppercase; font-weight: bold; margin-bottom: 2px; }
        .info-value { font-size: 1.3rem; font-weight: bold; color: #333; margin-bottom: 15px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">✅ Control de Abordaje</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Panel</a>
        </div>

        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-8 col-lg-6">
                    <div class="scan-card">
                        <div class="text-center mb-4">
                            <h2 class="fw-bold text-dark">Escanear Pase de Abordar</h2>
                            <p class="text-muted">Utiliza el lector de código de barras o ingresa el localizador manualmente.</p>
                        </div>

                        <div class="mb-4">
                            <asp:TextBox ID="txtCodigo" runat="server" CssClass="form-control scan-input" placeholder="Ej: B-12345" AutoPostBack="false"></asp:TextBox>
                        </div>
                        
                        <asp:Button ID="btnBuscar" runat="server" Text="🔍 Verificar Pase" CssClass="btn btn-primary w-100 py-3 fw-bold fs-5 rounded-pill shadow-sm mb-4" />

                        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold fs-5 rounded-3 mb-4">
                            <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                        </asp:Panel>

                        <asp:Panel ID="pnlResultado" runat="server" Visible="false">
                            <div class="info-box shadow-sm">
                                <div class="row">
                                    <div class="col-12 text-center mb-3">
                                        <div class="info-label">Pasajero</div>
                                        <div class="info-value text-primary fs-3"><asp:Label ID="lblPasajero" runat="server"></asp:Label></div>
                                    </div>
                                    <div class="col-6 text-center">
                                        <div class="info-label">Vuelo</div>
                                        <div class="info-value"><asp:Label ID="lblVuelo" runat="server"></asp:Label></div>
                                    </div>
                                    <div class="col-6 text-center">
                                        <div class="info-label">Asiento</div>
                                        <div class="info-value text-danger fs-2"><asp:Label ID="lblAsiento" runat="server"></asp:Label></div>
                                    </div>
                                    <div class="col-12 text-center mt-3">
                                        <div class="info-label">Ruta</div>
                                        <div class="info-value"><asp:Label ID="lblRuta" runat="server"></asp:Label></div>
                                    </div>
                                </div>
                            </div>

                            <asp:Button ID="btnConfirmar" runat="server" Text="Autorizar Ingreso al Avión ✈️" CssClass="btn btn-success w-100 py-3 mt-4 fw-bold fs-5 rounded-pill shadow" Visible="false" />
                        </asp:Panel>

                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>