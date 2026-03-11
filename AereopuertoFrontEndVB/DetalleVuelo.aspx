<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DetalleVuelo.aspx.vb" Inherits="AereopuertoFrontEndVB.DetalleVuelo" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Detalles del Vuelo - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; padding-top: 30px; }
        .detail-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); padding: 40px; border-top: 6px solid #0d47a1; margin-bottom: 30px; }
        .section-title { font-size: 0.85rem; font-weight: bold; letter-spacing: 1.5px; color: #90caf9; text-transform: uppercase; border-bottom: 2px solid #f0f0f0; padding-bottom: 10px; margin-bottom: 20px; }
        .data-label { font-size: 0.85rem; color: #6c757d; font-weight: 600; text-transform: uppercase; margin-bottom: 5px; }
        .data-value { font-size: 1.2rem; color: #2c3e50; font-weight: bold; margin-bottom: 20px; }
        .flight-code { font-size: 2.5rem; font-weight: 900; color: #0d47a1; letter-spacing: 2px; }
        
        /* Badges dinámicos */
        .badge-estado { font-size: 1rem; padding: 8px 15px; border-radius: 20px; font-weight: bold; }
        .bg-programado { background-color: #fff3e0; color: #e65100; }
        .bg-activo { background-color: #e3f2fd; color: #0d47a1; }
        .bg-finalizado { background-color: #e8f5e9; color: #2e7d32; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="mb-4">
                <a href="Default.aspx" class="btn btn-outline-secondary fw-bold rounded-pill px-4">← Volver al Tablero Principal</a>
            </div>

            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger text-center rounded-3 shadow-sm py-4">
                <h4 class="fw-bold">⚠️ Vuelo no encontrado</h4>
                <p class="m-0">No pudimos cargar la información de este vuelo. Es posible que el ID sea incorrecto o el vuelo haya sido eliminado.</p>
            </asp:Panel>

            <asp:Panel ID="pnlDetalle" runat="server">
                <div class="detail-card">
                    <div class="d-flex justify-content-between align-items-center border-bottom pb-4 mb-4 flex-wrap">
                        <div>
                            <div class="data-label">Vuelo Operado por <asp:Label ID="lblAerolineaHead" runat="server"></asp:Label></div>
                            <div class="flight-code"><asp:Label ID="lblCodigoVuelo" runat="server">AUR-XXX</asp:Label></div>
                        </div>
                        <div class="text-end mt-3 mt-md-0">
                            <div class="data-label mb-2">Estado Actual</div>
                            <asp:Label ID="lblEstado" runat="server" CssClass="badge-estado bg-programado">Programado</asp:Label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-6 pe-md-5">
                            <div class="section-title text-primary">Información de la Ruta</div>
                            
                            <div class="data-label">🛫 Origen</div>
                            <div class="data-value">
                                <asp:Label ID="lblOrigenCiudad" runat="server"></asp:Label> (<asp:Label ID="lblOrigenIata" runat="server"></asp:Label>)<br />
                                <small class="text-muted fw-normal"><asp:Label ID="lblOrigenAero" runat="server"></asp:Label> - <asp:Label ID="lblOrigenPais" runat="server"></asp:Label></small>
                            </div>

                            <div class="data-label">🛬 Destino</div>
                            <div class="data-value">
                                <asp:Label ID="lblDestinoCiudad" runat="server"></asp:Label> (<asp:Label ID="lblDestinoIata" runat="server"></asp:Label>)<br />
                                <small class="text-muted fw-normal"><asp:Label ID="lblDestinoAero" runat="server"></asp:Label> - <asp:Label ID="lblDestinoPais" runat="server"></asp:Label></small>
                            </div>

                            <div class="row mt-4">
                                <div class="col-6">
                                    <div class="data-label">Salida Programada</div>
                                    <div class="data-value fs-5"><asp:Label ID="lblFechaSalida" runat="server"></asp:Label></div>
                                </div>
                                <div class="col-6">
                                    <div class="data-label">Llegada Estimada</div>
                                    <div class="data-value fs-5"><asp:Label ID="lblFechaLlegada" runat="server"></asp:Label></div>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-6 border-start ps-md-5 mt-4 mt-md-0">
                            <div class="section-title text-primary">Detalles Operativos</div>

                            <div class="data-label">✈️ Aerolínea</div>
                            <div class="data-value"><asp:Label ID="lblAerolinea" runat="server"></asp:Label></div>

                            <div class="data-label">🛩️ Equipo (Aeronave)</div>
                            <div class="data-value"><asp:Label ID="lblAeronaveModelo" runat="server"></asp:Label></div>

                            <div class="data-label">👥 Capacidad Máxima</div>
                            <div class="data-value"><asp:Label ID="lblCapacidad" runat="server"></asp:Label> Pasajeros</div>
                            
                            <div class="data-label">⏱️ Tiempo de Vuelo Estimado</div>
                            <div class="data-value text-success"><asp:Label ID="lblDuracion" runat="server"></asp:Label></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>