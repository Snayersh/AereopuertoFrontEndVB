<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GestionReservas.aspx.vb" Inherits="AereopuertoFrontEndVB.GestionReservas" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Gestión de Reservas - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .admin-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 30px; border: none; margin-top: 30px; }
        
        .badge-reservado { background-color: #fff3e0; color: #e65100; padding: 6px 12px; border-radius: 20px; font-weight: bold; }
        .badge-pagado { background-color: #e8f5e9; color: #2e7d32; padding: 6px 12px; border-radius: 20px; font-weight: bold; }
        .badge-cancelado { background-color: #ffebee; color: #c62828; padding: 6px 12px; border-radius: 20px; font-weight: bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">⚙️ Administración de Operaciones</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Dashboard</a>
        </div>

        <div class="container-fluid px-4">
            <div class="row justify-content-center">
                <div class="col-md-11">
                    <div class="admin-card">
                        <div class="border-bottom pb-3 mb-4 d-flex justify-content-between align-items-center">
                            <div>
                                <h3 class="fw-bold" style="color: #0d47a1;">📋 Registro Global de Pasajeros</h3>
                                <p class="text-muted m-0">Monitorea todos los boletos emitidos en el aeropuerto.</p>
                            </div>
                            <button type="button" class="btn btn-outline-primary fw-bold" onclick="window.print()">🖨️ Imprimir Reporte</button>
                        </div>

                        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info text-center rounded-3 mb-4">
                            <asp:Label ID="lblMensaje" runat="server" CssClass="fw-bold"></asp:Label>
                        </asp:Panel>

                        <div class="table-responsive">
                            <table class="table table-hover align-middle">
                                <thead class="table-dark">
                                    <tr>
                                        <th>LOCALIZADOR</th>
                                        <th>PASAJERO</th>
                                        <th>VUELO</th>
                                        <th>RUTA</th>
                                        <th>FECHA SALIDA</th>
                                        <th>PRECIO</th>
                                        <th>ESTADO</th>
                                        <th class="text-center">ACCIONES</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptReservas" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td><h5 class="m-0 fw-bold text-dark"><%# Eval("Localizador") %></h5></td>
                                                <td><span class="fw-bold text-secondary"><%# Eval("Pasajero") %></span></td>
                                                <td><span class="text-primary fw-bold"><%# Eval("Vuelo") %></span></td>
                                                <td><strong><%# Eval("Ruta") %></strong></td>
                                                <td><%# Eval("FechaSalida") %></td>
                                                <td>$<%# Convert.ToDecimal(Eval("Precio")).ToString("0.00") %></td>
                                                <td>
                                                    <span class='<%# ObtenerClaseEstado(Eval("Estado").ToString()) %>'>
                                                        <%# Eval("Estado") %>
                                                    </span>
                                                </td>
                                                <td class="text-center">
                                                    <a href="#" class="btn btn-sm btn-light border fw-bold text-primary">Gestionar</a>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                        
                        <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="text-center py-5">
                            <h2 class="text-muted">Aún no hay reservas registradas en el sistema.</h2>
                        </asp:Panel>

                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>