<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="AereopuertoFrontEndVB._Default" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Centro de Control - La Aurora GUA</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; overflow-x: hidden; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .sidebar { background: linear-gradient(180deg, #0d47a1 0%, #1565c0 100%); min-height: 100vh; color: white; padding-top: 2rem; box-shadow: 4px 0 15px rgba(0,0,0,0.1); }
        .sidebar-brand { font-size: 1.6rem; font-weight: 900; padding: 0 1.5rem; margin-bottom: 2rem; display: block; color: white; text-decoration: none; letter-spacing: 1px; }
        .nav-link { color: rgba(255,255,255,0.7); font-weight: 500; padding: 0.8rem 1.5rem; transition: all 0.3s; border-left: 4px solid transparent; display: flex; align-items: center; gap: 10px; }
        .nav-link:hover, .nav-link.active { color: white; background-color: rgba(255,255,255,0.15); border-left-color: #64b5f6; }
        .section-title { font-size: 0.75rem; font-weight: bold; letter-spacing: 1.5px; color: #90caf9; margin: 1.5rem 1.5rem 0.5rem 1.5rem; text-transform: uppercase; opacity: 0.8; }
        .main-content { padding: 2.5rem; height: 100vh; overflow-y: auto; background-color: #f8f9fc; }
        .dashboard-header { background: linear-gradient(135deg, #1976d2 0%, #0d47a1 100%); color: white; border-radius: 15px; padding: 2rem; margin-bottom: 2.5rem; box-shadow: 0 10px 25px rgba(13, 71, 161, 0.2); }
        .stat-card { border-radius: 15px; border: none; transition: all 0.3s ease; overflow: hidden; position: relative; }
        .stat-card:hover { transform: translateY(-5px); box-shadow: 0 15px 30px rgba(0,0,0,0.1) !important; }
        
        .table-container { border-radius: 15px; background: white; padding: 2rem; box-shadow: 0 5px 20px rgba(0,0,0,0.03); border: 1px solid #edf2f9; }
        .table-custom tbody tr { transition: all 0.2s ease; border-bottom: 1px solid #f1f3f5; cursor: pointer; }
        .table-custom tbody tr:hover { background-color: #ffffff; transform: scale(1.01); box-shadow: 0 5px 15px rgba(0,0,0,0.08); z-index: 10; position: relative; border-radius: 8px; }
        .table-custom td { padding: 1rem 0.5rem; }
        
        .live-indicator { display: inline-block; width: 12px; height: 12px; background-color: #e74c3c; border-radius: 50%; margin-right: 10px; animation: pulse 1.5s infinite; }
        @keyframes pulse { 0% { box-shadow: 0 0 0 0 rgba(231, 76, 60, 0.7); } 70% { box-shadow: 0 0 0 10px rgba(231, 76, 60, 0); } 100% { box-shadow: 0 0 0 0 rgba(231, 76, 60, 0); } }
        
        .badge-llegada { background-color: #e3f2fd; color: #0d47a1; padding: 8px 12px; border-radius: 20px; font-weight: bold; font-size: 0.85rem; }
        .badge-salida { background-color: #fff3e0; color: #e65100; padding: 8px 12px; border-radius: 20px; font-weight: bold; font-size: 0.85rem; }

        /* Estilo para los filtros */
        .filter-section { background-color: #f8f9fa; padding: 15px; border-radius: 8px; border-left: 4px solid #0d47a1; margin-bottom: 20px; }
        .btn-check:checked + .btn-outline-custom { background-color: #0d47a1; color: white; border-color: #0d47a1; }
        .btn-outline-custom { color: #0d47a1; border-color: #0d47a1; font-weight: bold; }
        .btn-outline-custom:hover { background-color: rgba(13, 71, 161, 0.1); color: #0d47a1; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid p-0">
            <div class="row g-0">
                <div class="col-md-2 sidebar d-none d-md-block">
                    <a href="Default.aspx" class="sidebar-brand">✈️ LA AURORA</a>
                    <ul class="nav flex-column mb-5">
                        <li class="section-title">General</li>
                        <li class="nav-item"><a class="nav-link active" href="Default.aspx">📊 Panel Principal</a></li>
                        <li class="nav-item"><a class="nav-link" href="Radar.aspx">🌍 Radar en Vivo</a></li>
                        
                        <asp:Panel ID="pnlCliente" runat="server" Visible="false">
                            <li class="section-title text-light">Mi Cuenta</li>
                            <li class="nav-item"><a class="nav-link" href="Account/MiPerfil.aspx">👤 Mi Perfil</a></li>
                            <li class="nav-item"><a class="nav-link text-warning fw-bold" href="Cliente/Reservas.aspx">🛒 Reservar Vuelo</a></li>
                            <li class="nav-item"><a class="nav-link" href="Cliente/MisBoletos.aspx">🎫 Mis Boletos</a></li>
                            <li class="nav-item"><a class="nav-link text-success fw-bold" href="Cliente/Pagos.aspx">💳 Pagar Reserva</a></li>
                            <li class="nav-item"><a class="nav-link" href="Cliente/Equipaje.aspx">🧳 Mi Equipaje</a></li>
                            <li class="nav-item"><a class="nav-link" href="Cliente/MisFacturas.aspx">🧾 Mis Facturas</a></li>
                        </asp:Panel>

                        <asp:Panel ID="pnlEmpleado" runat="server" Visible="false">
                            <li class="section-title text-info">Operativa de Pasajeros</li>
                            <li class="nav-item"><a class="nav-link" href="Empleado/CheckIn.aspx">✅ Validar Abordaje</a></li>
                            <li class="nav-item"><a class="nav-link" href="Empleado/RegistroEquipaje.aspx">⚖️ Registro Equipaje</a></li>
                            
                            <li class="section-title text-danger">Seguridad e Incidencias</li>
                            <li class="nav-item"><a class="nav-link" href="Empleado/ObjetosPerdidos.aspx">🎒 Objetos Perdidos</a></li>
                            <li class="nav-item"><a class="nav-link" href="Empleado/Arrestos.aspx">🚔 Arrestos (Infracciones)</a></li>

                            <li class="section-title text-warning">Logística de Vuelos</li>
                            <li class="nav-item"><a class="nav-link" href="Empleado/ControlVuelos.aspx">📡 Torre de Control</a></li>
                            <li class="nav-item"><a class="nav-link" href="Empleado/ProgramasVuelo.aspx">📅 Crear Vuelo</a></li>
                            <li class="nav-item"><a class="nav-link" href="Empleado/Tripulacion.aspx">👨‍✈️ Asignar Tripulación</a></li>
                            <li class="nav-item"><a class="nav-link" href="Empleado/RegistroTripulacion.aspx">🧑‍✈️ Registrar Personal</a></li>
                            <li class="nav-item"><a class="nav-link" href="Empleado/Escalas.aspx">🛑 Gestión Escalas</a></li>

                            <li class="section-title text-secondary">Catálogos Fijos</li>
                            <li class="nav-item"><a class="nav-link" href="Empleado/Aerolineas.aspx">🏢 Aerolíneas</a></li>
                            <li class="nav-item"><a class="nav-link" href="Empleado/Aeronaves.aspx">🛩️ Aeronaves</a></li>
                            <li class="nav-item"><a class="nav-link" href="Empleado/Aereopuertos.aspx">🛫 Aeropuertos</a></li>
                        </asp:Panel>

                        <asp:Panel ID="pnlAdmin" runat="server" Visible="false">
                            <li class="section-title text-warning" title="Conectado a Servidor Standby">📊 Reportería (Réplica)</li>
                            <li class="nav-item"><a class="nav-link" href="Admin/ReportePasajeros.aspx">📄 Pasajeros por Vuelo</a></li>
                            <li class="nav-item"><a class="nav-link" href="Admin/ReporteVuelosGral.aspx">📋 Vuelos Prog / Canc</a></li>
                            <li class="nav-item"><a class="nav-link" href="Admin/ReporteAerolineas.aspx">✈️ Aerolíneas Activas</a></li>
                            <li class="nav-item"><a class="nav-link" href="Admin/ReporteVuelosAire.aspx">🌍 Vuelos en el Aire</a></li>
                            <li class="nav-item"><a class="nav-link" href="Admin/Usuarios.aspx">👥 Usuarios y Roles</a></li>

                        </asp:Panel>
                    </ul>
                </div>

                <div class="col-md-10 main-content">
                    <div class="dashboard-header d-flex justify-content-between align-items-center flex-wrap">
                        <div>
                            <h2 class="fw-bold mb-1 text-white">Centro de Control GUA</h2>
                            <p class="mb-0 text-white-50 fs-5"><asp:Label ID="lblSaludo" runat="server" Text="Bienvenido al sistema"></asp:Label></p>
                        </div>
                        <div>
                            <asp:Panel ID="pnlBotonesAcceso" runat="server">
                                <a href="Account/Login.aspx" class="btn btn-light text-primary fw-bold rounded-pill px-4 me-2 shadow-sm">Iniciar Sesión</a>
                                <a href="Account/Registro.aspx" class="btn btn-outline-light fw-bold rounded-pill px-4">Registrarse</a>
                            </asp:Panel>
                            <asp:Panel ID="pnlBotonSalir" runat="server" Visible="false">
                                <asp:Button ID="btnLogout" runat="server" Text="Cerrar Sesión" CssClass="btn btn-danger fw-bold rounded-pill px-4 shadow" OnClick="btnLogout_Click" />
                            </asp:Panel>
                        </div>
                    </div>

                    <div class="row mb-5">
                        <div class="col-md-4 mb-3">
                            <div class="card stat-card shadow-sm text-white h-100" style="background-color: #2c3e50;">
                                <div class="card-body p-4 d-flex justify-content-between align-items-center">
                                    <div><h6 class="text-uppercase mb-2 text-white-50 fw-bold letter-spacing-1">Vuelos Activos</h6><h1 class="fw-bold mb-0 display-5"><asp:Label ID="lblVuelosActivos" runat="server" Text="0"></asp:Label></h1></div>
                                    <h1 class="opacity-25 m-0 display-3">📡</h1>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 mb-3">
                            <div class="card stat-card shadow-sm text-white h-100" style="background-color: #27ae60;">
                                <div class="card-body p-4 d-flex justify-content-between align-items-center">
                                    <div><h6 class="text-uppercase mb-2 text-white-50 fw-bold letter-spacing-1">Llegadas Hoy</h6><h1 class="fw-bold mb-0 display-5"><asp:Label ID="lblLlegadas" runat="server" Text="0"></asp:Label></h1></div>
                                    <h1 class="opacity-25 m-0 display-3">🛬</h1>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 mb-3">
                            <div class="card stat-card shadow-sm text-dark h-100" style="background-color: #f1c40f;">
                                <div class="card-body p-4 d-flex justify-content-between align-items-center">
                                    <div><h6 class="text-uppercase mb-2 text-black-50 fw-bold letter-spacing-1">Salidas Hoy</h6><h1 class="fw-bold mb-0 display-5"><asp:Label ID="lblSalidas" runat="server" Text="0"></asp:Label></h1></div>
                                    <h1 class="opacity-25 m-0 display-3">🛫</h1>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="table-container">
                        <div class="d-flex justify-content-between align-items-center flex-wrap mb-3">
                            <div class="d-flex align-items-center">
                                <div class="live-indicator"></div>
                                <h4 class="fw-bold m-0 text-dark">Monitoreo en Tiempo Real</h4>
                            </div>
                            
                            <div class="btn-group shadow-sm" role="group">
                                <input type="checkbox" class="btn-check chk-filtro-default" id="chkLlegadas" value="Llegada" checked autocomplete="off">
                                <label class="btn btn-outline-primary fw-bold" for="chkLlegadas">🛬 Llegadas</label>

                                <input type="checkbox" class="btn-check chk-filtro-default" id="chkSalidas" value="Salida" checked autocomplete="off">
                                <label class="btn btn-outline-warning fw-bold text-dark border-warning" for="chkSalidas">🛫 Salidas</label>
                            </div>
                        </div>

                        <div class="filter-section d-flex flex-wrap gap-2 align-items-center shadow-sm">
                            <span class="text-secondary fw-bold me-2">ESTADOS:</span>
                            
                            <input type="checkbox" class="btn-check chk-filtro-default" id="chkProg" value="PROGRAMADO" checked autocomplete="off">
                            <label class="btn btn-outline-custom btn-sm" for="chkProg">Programado</label>

                            <input type="checkbox" class="btn-check chk-filtro-default" id="chkAbordVuelo" value="VUELO_ABORDANDO" checked autocomplete="off">
                            <label class="btn btn-outline-custom btn-sm" for="chkAbordVuelo">En Vuelo / Abordando</label>

                            <input type="checkbox" class="btn-check chk-filtro-default" id="chkAterr" value="ATERRIZADO" checked autocomplete="off">
                            <label class="btn btn-outline-custom btn-sm" for="chkAterr">Aterrizado</label>

                            <input type="checkbox" class="btn-check chk-filtro-default" id="chkRetr" value="RETRASADO" checked autocomplete="off">
                            <label class="btn btn-outline-warning btn-sm fw-bold" for="chkRetr">Retrasado</label>

                            <input type="checkbox" class="btn-check chk-filtro-default" id="chkCanc" value="CANCELADO" checked autocomplete="off">
                            <label class="btn btn-outline-danger btn-sm fw-bold" for="chkCanc">Cancelado</label>
                        </div>
                        
                        <div class="table-responsive">
                            <table class="table table-custom align-middle" id="tablaVuelosDefault">
                                <thead class="table-light text-secondary">
                                    <tr>
                                        <th class="border-0">VUELO</th>
                                        <th class="border-0">AEROLÍNEA</th>
                                        <th class="border-0">TIPO</th>
                                        <th class="border-0">RUTA</th>
                                        <th class="border-0">HORA</th>
                                        <th class="border-0">ESTADO</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptVuelos" runat="server">
                                        <ItemTemplate>
                                            <tr class="vuelo-row" onclick="window.location.href='DetalleVuelo.aspx?id=<%# Eval("IdVuelo") %>';" title="Haz clic para ver detalles">
                                                <td><strong class="fs-5 text-primary"><%# Eval("NumeroVuelo") %></strong></td>
                                                <td><span class='<%# If(Eval("Aerolinea").ToString() = "Iberia", "text-danger fw-bold", "text-secondary fw-bold") %>'><%# Eval("Aerolinea") %></span></td>
                                                <td class="col-tipo"><span class='<%# If(Convert.ToBoolean(Eval("EsLlegada")), "badge-llegada", "badge-salida") %>'><%# If(Convert.ToBoolean(Eval("EsLlegada")), "Llegada", "Salida") %></span></td>
                                                <td><strong class="text-dark"><%# Eval("OrigenDestino") %></strong></td>
                                                <td><h5 class="m-0 fw-bold text-dark"><%# Convert.ToDateTime(Eval("HoraProgramada")).ToString("HH:mm") %></h5></td>
                                                <td class="col-estado">
                                                    <span class='<%# ObtenerClaseEstado(Eval("Estado").ToString()) %> shadow-sm' data-estado='<%# Eval("Estado").ToString().ToUpper() %>'>
                                                        <%# ObtenerIconoEstado(Eval("Estado").ToString()) %> <%# Eval("Estado") %>
                                                    </span>
                                                </td>
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

    <script>
        function aplicarFiltrosTablero() {
            let showLlegadas = document.getElementById('chkLlegadas').checked;
            let showSalidas = document.getElementById('chkSalidas').checked;

            let chkProg = document.getElementById('chkProg').checked;
            let chkAbordVuelo = document.getElementById('chkAbordVuelo').checked;
            let chkAterr = document.getElementById('chkAterr').checked;
            let chkRetr = document.getElementById('chkRetr').checked;
            let chkCanc = document.getElementById('chkCanc').checked;

            let rows = document.querySelectorAll('#tablaVuelosDefault tbody .vuelo-row');

            rows.forEach(row => {
                let tipoText = row.querySelector('.col-tipo').innerText.trim();
                let isLlegada = tipoText.includes('Llegada');
                let isSalida = tipoText.includes('Salida');

                let spanEstado = row.querySelector('.col-estado span');
                let estadoActual = spanEstado ? spanEstado.getAttribute('data-estado') : "";

                let mostrarPorTipo = false;
                let mostrarPorEstado = false;

                if ((isLlegada && showLlegadas) || (isSalida && showSalidas)) {
                    mostrarPorTipo = true;
                }

                if (estadoActual === "PROGRAMADO" && chkProg) mostrarPorEstado = true;
                if ((estadoActual === "ABORDANDO" || estadoActual === "EN VUELO") && chkAbordVuelo) mostrarPorEstado = true;
                if ((estadoActual === "ATERRIZADO" || estadoActual === "ATERRIZÓ" || estadoActual === "FINALIZADO") && chkAterr) mostrarPorEstado = true;
                if (estadoActual === "RETRASADO" && chkRetr) mostrarPorEstado = true;
                if (estadoActual === "CANCELADO" && chkCanc) mostrarPorEstado = true;

                if (mostrarPorTipo && mostrarPorEstado) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            });
        }

        document.querySelectorAll('.chk-filtro-default').forEach(chk => {
            chk.addEventListener('change', aplicarFiltrosTablero);
        });

        window.onload = aplicarFiltrosTablero;
    </script>
</body>
</html>