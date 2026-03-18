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

        /* Colores sincronizados con tu Base de Datos Real */
        .estado-1 { color: #90caf9; font-weight: bold; } /* 1: Programado */
        .estado-2 { color: #ffb74d; font-weight: bold; } /* 2: Abordando */
        .estado-3 { color: #a5d6a7; font-weight: bold; } /* 3: En Vuelo */
        .estado-4 { color: #81c784; font-weight: bold; } /* 4: Aterrizado */
        .estado-5 { color: #e57373; font-weight: bold; text-decoration: line-through; } /* 5: Cancelado */
        .estado-6 { color: #ffb74d; font-weight: bold; } /* 6: Retrasado */

        /* Estilo para los filtros */
        .filter-section { background-color: #272727; padding: 15px; border-radius: 8px; border-left: 4px solid #0d47a1; }
        .btn-check:checked + .btn-outline-custom { background-color: #0d47a1; color: white; border-color: #0d47a1; }
        .btn-outline-custom { color: #90caf9; border-color: #90caf9; font-weight: bold; }
        .btn-outline-custom:hover { background-color: rgba(13, 71, 161, 0.3); color: white; }
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
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <h3 class="fw-bold m-0 text-white">Monitoreo de Vuelos</h3>
                </div>

                <div class="filter-section mb-4 d-flex flex-wrap gap-2 align-items-center shadow-sm">
                    <span class="text-secondary fw-bold me-2">FILTRAR POR ESTADO:</span>
                    
                    <input type="checkbox" class="btn-check filtro-torre" id="chkProg" checked autocomplete="off">
                    <label class="btn btn-outline-custom btn-sm" for="chkProg">Programado</label>

                    <input type="checkbox" class="btn-check filtro-torre" id="chkAbordVuelo" autocomplete="off">
                    <label class="btn btn-outline-custom btn-sm" for="chkAbordVuelo">En Vuelo / Abordando</label>

                    <input type="checkbox" class="btn-check filtro-torre" id="chkAterr" autocomplete="off">
                    <label class="btn btn-outline-custom btn-sm" for="chkAterr">Aterrizado</label>

                    <input type="checkbox" class="btn-check filtro-torre" id="chkRetr" autocomplete="off">
                    <label class="btn btn-outline-warning btn-sm fw-bold" for="chkRetr">Retrasado</label>

                    <input type="checkbox" class="btn-check filtro-torre" id="chkCanc" autocomplete="off">
                    <label class="btn btn-outline-danger btn-sm fw-bold" for="chkCanc">Cancelado</label>
                </div>
                
                <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info fw-bold text-center">
                    <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                </asp:Panel>

                <div class="table-responsive">
                    <table class="table table-hover table-dark" id="tablaTorre">
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
                                    <tr class="vuelo-row">
                                        <td class="fs-5 fw-bold text-warning"><%# Eval("codigo_vuelo") %></td>
                                        <td><h2>🛫</h2> <%# Eval("origen") %></td>
                                        <td><h2>🛬</h2> <%# Eval("destino") %></td>
                                        <td><%# Eval("fecha") %></td>
                                        <td class="fs-5"><%# Eval("hora_salida") %></td>
                                        <td class="col-estado">
                                            <span class='estado-<%# Eval("id_estado_vuelo") %> span-estado' data-id='<%# Eval("id_estado_vuelo") %>'>
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

    <script>
        function aplicarFiltrosTorre() {
            let chkProg = document.getElementById('chkProg').checked;
            let chkAbordVuelo = document.getElementById('chkAbordVuelo').checked;
            let chkAterr = document.getElementById('chkAterr').checked;
            let chkRetr = document.getElementById('chkRetr').checked;
            let chkCanc = document.getElementById('chkCanc').checked;

            let rows = document.querySelectorAll('#tablaTorre tbody .vuelo-row');

            rows.forEach(row => {
                let span = row.querySelector('.span-estado');
                if (span) {
                    let estadoId = span.getAttribute('data-id');
                    let mostrar = false;

                    // Lógica de coincidencia exacta con tus IDs de Base de Datos
                    if (estadoId == 1 && chkProg) mostrar = true;
                    if ((estadoId == 2 || estadoId == 3) && chkAbordVuelo) mostrar = true;
                    if (estadoId == 4 && chkAterr) mostrar = true;
                    if (estadoId == 5 && chkCanc) mostrar = true;
                    if (estadoId == 6 && chkRetr) mostrar = true;

                    row.style.display = mostrar ? '' : 'none';
                }
            });
        }

        // Le pegamos el evento de 'change' a todos los checks
        document.querySelectorAll('.filtro-torre').forEach(chk => {
            chk.addEventListener('change', aplicarFiltrosTorre);
        });

        // Ejecutar el filtro en cuanto cargue la página
        window.onload = aplicarFiltrosTorre;
    </script>
</body>
</html>