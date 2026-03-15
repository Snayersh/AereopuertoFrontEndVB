<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ControlVuelos.aspx.vb" Inherits="AereopuertoFrontEndVB.ControlVuelos" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Torre de Control - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #121212; color: #e0e0e0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #1f1f1f; padding: 15px 20px; border-bottom: 2px solid #0d47a1; }
        .control-panel { background: #1e1e1e; border-radius: 12px; padding: 25px; box-shadow: 0 10px 30px rgba(0,0,0,0.5); margin-top: 30px; }
        
        .table { color: #fff; }
        .table-dark th { background-color: #272727; color: #90caf9; border-color: #444; }
        .table-hover tbody tr:hover { background-color: #2a2a2a; }
        .table td { border-color: #444; vertical-align: middle; }

        .estado-1 { color: #90caf9; font-weight: bold; } /* Programado */
        .estado-2 { color: #a5d6a7; font-weight: bold; } /* En Vuelo / Abordando */
        .estado-3 { color: #81c784; font-weight: bold; } /* Aterrizado */
        .estado-4 { color: #ffb74d; font-weight: bold; } /* Retrasado */
        .estado-5 { color: #e57373; font-weight: bold; text-decoration: line-through; } /* Cancelado */
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold text-primary">📡 Torre de Control ATC</h4>
            <a href="../Default.aspx" class="btn btn-outline-primary btn-sm fw-bold rounded-pill px-4">← Panel Principal</a>
        </div>

        <div class="container-fluid px-4">
            <div class="control-panel">
                <h3 class="fw-bold mb-4 text-white">Monitoreo de Vuelos</h3>
                
                <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info fw-bold text-center">
                    <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                </asp:Panel>

                <div class="table-responsive">
                    <table class="table table-hover table-dark">
                        <thead>
                            <tr>
                                <th>VUELO</th>
                                <th>ORIGEN</th>
                                <th>DESTINO</th>
                                <th>FECHA</th>
                                <th>HORA</th>
                                <th>ESTADO ACTUAL</th>
                                <th>CAMBIAR ESTADO A:</th>
                                <th>ACCIÓN</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptControlVuelos" runat="server" OnItemCommand="rptControlVuelos_ItemCommand" OnItemDataBound="rptControlVuelos_ItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td class="fs-5 fw-bold text-warning"><%# Eval("codigo_vuelo") %></td>
                                        <td><h2>🛫</h2> <%# Eval("origen") %></td>
                                        <td><h2>🛬</h2> <%# Eval("destino") %></td>
                                        <td><%# Eval("fecha") %></td>
                                        <td class="fs-5"><%# Eval("hora_salida") %></td>
                                        <td>
                                            <span class='estado-<%# Eval("id_estado_vuelo") %>'>
                                                <%# Eval("estado_actual") %>
                                            </span>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlNuevoEstado" runat="server" CssClass="form-select form-select-sm bg-dark text-white border-secondary"></asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnActualizar" runat="server" Text="Actualizar" CssClass="btn btn-primary btn-sm fw-bold" 
                                                CommandName="ActualizarEstado" CommandArgument='<%# Eval("id_vuelo") %>' />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </form>
</body>
</html>