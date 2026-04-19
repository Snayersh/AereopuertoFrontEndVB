<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EscanerRampa.aspx.vb" Inherits="AereopuertoFrontEndVB.EscanerRampa" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Escáner de Rampa - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #37474f; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: white; }
        .scanner-card { background: #263238; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.5); padding: 30px; margin-top: 50px; border: 2px solid #546e7a; }
        .scan-input { height: 80px; font-size: 2.5rem; text-align: center; font-weight: bold; background: #eceff1; border: 3px dashed #ff9800; text-transform: uppercase; border-radius: 10px; }
        .scan-input:focus { border-color: #ffb300; outline: none; background: #ffffff; }
        .btn-update { font-size: 1.2rem; font-weight: bold; height: 60px; border-radius: 10px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-8 col-lg-6">
                    <div class="scanner-card">
                        <div class="d-flex justify-content-between align-items-center mb-4">
                            <h3 class="fw-bold m-0 text-warning">🛄 Escáner de Rampa</h3>
                            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold">Salir</a>
                        </div>

                        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 fs-5">
                            <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                        </asp:Panel>

                        <div class="mb-4">
                            <label class="form-label fw-bold text-light">Código de Etiqueta (ID Equipaje)</label>
                            <asp:TextBox ID="txtIdEquipaje" runat="server" CssClass="form-control scan-input" placeholder="Ej: 10542" AutoPostBack="false"></asp:TextBox>
                        </div>

                        <div class="mb-4">
                            <label class="form-label fw-bold text-light">Nuevo Estado de la Maleta</label>
                            <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select form-select-lg fw-bold"></asp:DropDownList>
                        </div>

                        <asp:Button ID="btnActualizar" runat="server" Text="Actualizar Ubicación 📡" CssClass="btn btn-warning w-100 btn-update" />
                        
                        <div class="text-center mt-4">
                            <small class="text-secondary">Aeropuerto Internacional La Aurora - Operaciones de Pista</small>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>