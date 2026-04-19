<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ClimaAeropuerto.aspx.vb" Inherits="AereopuertoFrontEndVB.ClimaAeropuerto" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Radar Meteorológico - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #e0e5ec; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .weather-card { 
            background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%); 
            color: white; 
            border-radius: 20px; 
            padding: 40px; 
            box-shadow: 0 15px 35px rgba(0,0,0,0.2); 
            text-align: center;
        }
        .temp-display { font-size: 5rem; font-weight: 800; line-height: 1; margin: 20px 0; }
        .condition-display { font-size: 1.5rem; text-transform: uppercase; letter-spacing: 2px; color: #bbdefb; }
        .history-card { background: white; border-radius: 15px; padding: 25px; box-shadow: 0 5px 15px rgba(0,0,0,0.05); }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container py-5">
            <div class="mb-4">
                <a href="../Default.aspx" class="text-decoration-none fw-bold text-secondary">← Volver al Centro de Control</a>
            </div>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="row justify-content-center">
                <div class="col-md-5 mb-4">
                    <div class="weather-card">
                        <h4 class="fw-bold m-0">✈️ Aeropuerto La Aurora (GUA)</h4>
                        <p class="text-light opacity-75">Condiciones en Tiempo Real</p>
                        
                        <div class="temp-display">
                            <asp:Label ID="lblTemperatura" runat="server" Text="--"></asp:Label>°C
                        </div>
                        
                        <div class="condition-display fw-bold mb-4">
                            <asp:Label ID="lblCondicion" runat="server" Text="Cargando..."></asp:Label>
                        </div>

                        <asp:Button ID="btnActualizarClima" runat="server" Text="🔄 Sincronizar Radar Satelital" CssClass="btn btn-light text-primary fw-bold rounded-pill px-4 py-2" />
                    </div>
                </div>

                <div class="col-md-7">
                    <div class="history-card">
                        <h5 class="fw-bold mb-3 text-dark">📋 Bitácora Meteorológica (AUR_CLIMA)</h5>
                        <div class="table-responsive">
                            <table class="table table-hover align-middle">
                                <thead class="table-light">
                                    <tr class="small text-secondary">
                                        <th>Fecha y Hora</th>
                                        <th>Temperatura</th>
                                        <th>Condición</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptHistorialClima" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td><span class="badge bg-secondary"><%# Eval("fecha", "{0:dd/MM/yyyy HH:mm}") %></span></td>
                                                <td class="fw-bold text-danger"><%# Eval("temperatura") %> °C</td>
                                                <td class="text-capitalize"><%# Eval("condicion") %></td>
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