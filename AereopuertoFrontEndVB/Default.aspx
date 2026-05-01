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
        
        /* Estilos del Sidebar y Acordeón */
        .sidebar { background: linear-gradient(180deg, #0d47a1 0%, #1565c0 100%); min-height: 100vh; color: white; padding-top: 2rem; box-shadow: 4px 0 15px rgba(0,0,0,0.1); }
        .sidebar-brand { font-size: 1.6rem; font-weight: 900; padding: 0 1.5rem; margin-bottom: 2rem; display: block; color: white; text-decoration: none; letter-spacing: 1px; }
        .nav-link { color: rgba(255,255,255,0.7); font-weight: 500; padding: 0.8rem 1.5rem; transition: all 0.3s; border-left: 4px solid transparent; display: flex; align-items: center; gap: 10px; cursor: pointer; }
        .nav-link:hover, .nav-link.active { color: white; background-color: rgba(255,255,255,0.15); border-left-color: #64b5f6; }
        .section-title { font-size: 0.75rem; font-weight: bold; letter-spacing: 1.5px; color: #90caf9; margin: 1.5rem 1.5rem 0.5rem 1.5rem; text-transform: uppercase; opacity: 0.8; }
        
        /* Animación de flechas en el menú */
        .toggle-icon { font-size: 0.7rem; transition: transform 0.3s ease; }
        .nav-link:not(.collapsed) .toggle-icon { transform: rotate(180deg); }
        .submenu { background-color: rgba(0,0,0,0.1); margin-bottom: 5px; }
        .submenu .nav-link { padding: 0.5rem 1.5rem 0.5rem 2.5rem; font-size: 0.9rem; border-left: none; }
        .submenu .nav-link:hover { background-color: transparent; color: #64b5f6; padding-left: 2.8rem; }

        /* Estilos Principales */
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
                
                <div class="col-md-2 sidebar d-none d-md-block" style="height: 100vh; overflow-y: auto;">
                    <a href="Default.aspx" class="sidebar-brand">✈️ LA AURORA</a>
                    <ul class="nav flex-column mb-5 pb-5">
                        <li class="section-title">General</li>
                        <li class="nav-item"><a class="nav-link active" href="Default.aspx">📊 Panel Principal</a></li>
                        <li class="nav-item"><a class="nav-link" href="Radar.aspx">🌍 Radar en Vivo</a></li>
                        
                        <asp:Panel ID="pnlCliente" runat="server" Visible="false">
                            <li class="section-title text-light">Módulo Pasajero</li>
                            
                            <li class="nav-item">
                                <a class="nav-link collapsed d-flex justify-content-between" data-bs-toggle="collapse" href="#collapseMiViaje">
                                    <span>✈️ Mi Viaje</span><span class="toggle-icon">▼</span>
                                </a>
                                <div class="collapse submenu" id="collapseMiViaje">
                                    <ul class="nav flex-column py-2">
                                        <li class="nav-item"><a class="nav-link text-warning fw-bold" href="Cliente/Reservas.aspx">🛒 Reservas</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Cliente/MisBoletos.aspx">🎫 Mis Boletos</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Cliente/PaseAbordar.aspx">📱 Pase Abordar</a></li>
                                    </ul>
                                </div>
                            </li>

                            <li class="nav-item">
                                <a class="nav-link collapsed d-flex justify-content-between" data-bs-toggle="collapse" href="#collapseEquipajeCliente">
                                    <span>🧳 Mi Equipaje</span><span class="toggle-icon">▼</span>
                                </a>
                                <div class="collapse submenu" id="collapseEquipajeCliente">
                                    <ul class="nav flex-column py-2">
                                        <li class="nav-item"><a class="nav-link" href="Cliente/Equipaje.aspx">⚖️ Equipaje</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Cliente/ReclamoEquipaje.aspx">🔎 Reclamo Equipaje</a></li>
                                    </ul>
                                </div>
                            </li>

                            <li class="nav-item">
                                <a class="nav-link collapsed d-flex justify-content-between" data-bs-toggle="collapse" href="#collapseFinanzas">
                                    <span>💳 Pagos y Facturas</span><span class="toggle-icon">▼</span>
                                </a>
                                <div class="collapse submenu" id="collapseFinanzas">
                                    <ul class="nav flex-column py-2">
                                        <li class="nav-item"><a class="nav-link text-success fw-bold" href="Cliente/Pagos.aspx">💲 Pagos</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Cliente/MisFacturas.aspx">🧾 Mis Facturas</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Cliente/DetalleFactura.aspx">📄 Detalle Factura</a></li>
                                    </ul>
                                </div>
                            </li>

                            <li class="nav-item">
                                <a class="nav-link" href="Cliente/SoporteTickets.aspx">🎧 Soporte Tickets</a>
                            </li>
                        </asp:Panel>

                        <asp:Panel ID="pnlEmpleado" runat="server" Visible="false">
                            <li class="section-title text-info">Operaciones GUA</li>
                            
                            <li class="nav-item">
                                <a class="nav-link collapsed d-flex justify-content-between" data-bs-toggle="collapse" href="#collapseMostrador">
                                    <span>👥 Mostrador & Gate</span><span class="toggle-icon">▼</span>
                                </a>
                                <div class="collapse submenu" id="collapseMostrador">
                                    <ul class="nav flex-column py-2">
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/Checkin.aspx">✅ Check-In</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/RegistroEquipaje.aspx">⚖️ Registro Equipaje</a></li>
                                    </ul>
                                </div>
                            </li>

                            <li class="nav-item">
                                <a class="nav-link collapsed d-flex justify-content-between" data-bs-toggle="collapse" href="#collapseVuelosRutas">
                                    <span>📡 Vuelos y Rutas</span><span class="toggle-icon">▼</span>
                                </a>
                                <div class="collapse submenu" id="collapseVuelosRutas">
                                    <ul class="nav flex-column py-2">
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/ControlVuelos.aspx">🎛️ Control Vuelo</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/ProgramasVuelo.aspx">📅 Programas Vuelos</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/Escalas.aspx">🛑 Escalas</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/Rutas.aspx">🗺️ Rutas</a></li>
                                    </ul>
                                </div>
                            </li>

                            <li class="nav-item">
                                <a class="nav-link collapsed d-flex justify-content-between" data-bs-toggle="collapse" href="#collapseRampaMantenimiento">
                                    <span>🛠️ Rampa & Mant.</span><span class="toggle-icon">▼</span>
                                </a>
                                <div class="collapse submenu" id="collapseRampaMantenimiento">
                                    <ul class="nav flex-column py-2">
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/AsignarPuertas.aspx">🚪 Asignar Puertas</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/EscanerRampa.aspx">📟 Escanear Rampa</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/MantenimientoAeronaves.aspx">🔧 Mantenimiento</a></li>
                                    </ul>
                                </div>
                            </li>

                            <li class="nav-item">
                                <a class="nav-link collapsed d-flex justify-content-between" data-bs-toggle="collapse" href="#collapseReservasSeguridad">
                                    <span>⚠️ Alertas & Reservas</span><span class="toggle-icon">▼</span>
                                </a>
                                <div class="collapse submenu" id="collapseReservasSeguridad">
                                    <ul class="nav flex-column py-2">
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/GestionReservas.aspx">📂 Gestión Reservas</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/DetalleReservas.aspx">📋 Detalle Reservas</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/ControlSeguridad.aspx">🛂 Control Seguridad</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/EmisionAlertas.aspx">🔔 Emisión Alertas</a></li>
                                    </ul>
                                </div>
                            </li>

                            <li class="nav-item">
                                <a class="nav-link collapsed d-flex justify-content-between" data-bs-toggle="collapse" href="#collapseCatalogosOpe">
                                    <span>🏢 Catálogos Fijos</span><span class="toggle-icon">▼</span>
                                </a>
                                <div class="collapse submenu" id="collapseCatalogosOpe">
                                    <ul class="nav flex-column py-2">
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/Aerolineas.aspx">🏢 Aerolíneas</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/Aeronaves.aspx">🛩️ Aeronaves</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Operaciones/Aereopuertos.aspx">🛫 Aeropuertos</a></li>
                                    </ul>
                                </div>
                            </li>
                        </asp:Panel>

                        <asp:Panel ID="pnlRRHH" runat="server" Visible="false">
                            <li class="section-title" style="color: #ce93d8;">Recursos Humanos</li>
                            <li class="nav-item">
                                <a class="nav-link collapsed d-flex justify-content-between" data-bs-toggle="collapse" href="#collapseRRHH">
                                    <span>🧑‍✈️ Gestión Personal</span><span class="toggle-icon">▼</span>
                                </a>
                                <div class="collapse submenu" id="collapseRRHH">
                                    <ul class="nav flex-column py-2">
                                        <li class="nav-item"><a class="nav-link" href="RRHH/RegistroTripulacion.aspx">📝 Registro Tripulación</a></li>
                                        <li class="nav-item"><a class="nav-link" href="RRHH/Tripulacion.aspx">👨‍✈️ Tripulación</a></li>
                                        <li class="nav-item"><a class="nav-link" href="RRHH/GestionTurnos.aspx">⏱️ Gestión Turnos</a></li>
                                        <li class="nav-item"><a class="nav-link" href="RRHH/EvaluacionPersonal.aspx">⭐ Evaluación Personal</a></li>
                                        <li class="nav-item"><a class="nav-link" href="RRHH/GestionAsistencia.aspx">♿ Gestión Asistencia</a></li>
                                    </ul>
                                </div>
                            </li>
                        </asp:Panel>
<!-- PANEL EXCLUSIVO PARA SEGURIDAD -->
                        <asp:Panel ID="pnlSeguridad" runat="server" Visible="false">
                            <li class="section-title text-danger">Seguridad Aeroportuaria</li>
                            <li class="nav-item"><a class="nav-link" href="Seguridad/Arrestos.aspx">🚔 Arrestos</a></li>
                        </asp:Panel>

                        <!-- PANEL EXCLUSIVO PARA SERVICIO AL CLIENTE / SOPORTE -->
                        <asp:Panel ID="pnlSoporte" runat="server" Visible="false">
                            <li class="section-title" style="color: #4db6ac;">Atención y Soporte</li>
                            <li class="nav-item">
                                <a class="nav-link collapsed d-flex justify-content-between" data-bs-toggle="collapse" href="#collapseSoporte">
                                    <span>💻 Soporte Gral</span><span class="toggle-icon">▼</span>
                                </a>
                                <div class="collapse submenu" id="collapseSoporte">
                                    <ul class="nav flex-column py-2">
                                        <li class="nav-item"><a class="nav-link" href="Soporte/BandejaTickets.aspx">📥 Bandeja Tickets</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Soporte/ObjetosPerdidos.aspx">🎒 Objetos Perdidos</a></li>
                                    </ul>
                                </div>
                            </li>
                        </asp:Panel>

                        <asp:Panel ID="pnlAdmin" runat="server" Visible="false">
                            <li class="section-title text-warning" title="Conectado a Servidor Standby">Admin y Auditoría</li>
                            
                            <li class="nav-item">
                                <a class="nav-link collapsed d-flex justify-content-between" data-bs-toggle="collapse" href="#collapseReportes">
                                    <span>📊 Reportería</span><span class="toggle-icon">▼</span>
                                </a>
                                <div class="collapse submenu" id="collapseReportes">
                                    <ul class="nav flex-column py-2">
                                        <li class="nav-item"><a class="nav-link" href="Admin/AdminPuertas.aspx">🚪 Admin Puertas</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/Auditoria.aspx">🔍 Auditoría</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/AuditoriaVuelos.aspx">✈️ Aud. Vuelos</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/BitacoraSesiones.aspx">🔑 Bitácora Sesiones</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/BitacoraSistemas.aspx">⚙️ Bitácora Sistemas</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/DetalleReportes.aspx">📄 Detalle Reportes</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/GestionPromociones.aspx">🏷️ Promociones</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/HistorialReportes.aspx">📚 Historial Rep.</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/PlanillaPago.aspx">💵 Plantilla Pago</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/ReporteAerolineas.aspx">🏢 Rep. Aerolíneas</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/ReporteArrestos.aspx">🚔 Rep. Arrestos</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/ReporteObjetos.aspx">🎒 Rep. Objetos</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/ReportePasajeros.aspx">👥 Rep. Pasajeros</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/ReporteVuelosAire.aspx">🌍 Vuelos Aire</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/ReporteVuelosGral.aspx">📋 Vuelos Gral.</a></li>
                                        <li class="nav-item"><a class="nav-link" href="Admin/AuditoriaPagos.aspx">💳 Aud. Pagos</a></li>
                                     <li class="nav-item"><a class="nav-link" href="Admin/GestionPlanilla.aspx">📋 Gestión Planilla</a></li>

                                    </ul>
                                </div>
                            </li>

                          
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

                    <!-- 🔥 AQUÍ ESTÁ LA NUEVA ESTRUCTURA (col-md-3) -->
                    <div class="row mb-5">
                        <div class="col-md-3 mb-3">
                            <div class="card stat-card shadow-sm text-white h-100" style="background-color: #2c3e50;">
                                <div class="card-body p-4 d-flex justify-content-between align-items-center">
                                    <div><h6 class="text-uppercase mb-2 text-white-50 fw-bold letter-spacing-1">Vuelos Activos</h6><h1 class="fw-bold mb-0 display-5"><asp:Label ID="lblVuelosActivos" runat="server" Text="0"></asp:Label></h1></div>
                                    <h1 class="opacity-25 m-0 display-3">📡</h1>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-3 mb-3">
                            <div class="card stat-card shadow-sm text-white h-100" style="background-color: #27ae60;">
                                <div class="card-body p-4 d-flex justify-content-between align-items-center">
                                    <div><h6 class="text-uppercase mb-2 text-white-50 fw-bold letter-spacing-1">Llegadas Hoy</h6><h1 class="fw-bold mb-0 display-5"><asp:Label ID="lblLlegadas" runat="server" Text="0"></asp:Label></h1></div>
                                    <h1 class="opacity-25 m-0 display-3">🛬</h1>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-3 mb-3">
                            <div class="card stat-card shadow-sm text-dark h-100" style="background-color: #f1c40f;">
                                <div class="card-body p-4 d-flex justify-content-between align-items-center">
                                    <div><h6 class="text-uppercase mb-2 text-black-50 fw-bold letter-spacing-1">Salidas Hoy</h6><h1 class="fw-bold mb-0 display-5"><asp:Label ID="lblSalidas" runat="server" Text="0"></asp:Label></h1></div>
                                    <h1 class="opacity-25 m-0 display-3">🛫</h1>
                                </div>
                            </div>
                        </div>
                        
                        <!-- 🌤️ NUEVO WIDGET DEL CLIMA EN VIVO -->
                        <div class="col-md-3 mb-3">
                            <div class="card stat-card shadow-sm text-white h-100" style="background: linear-gradient(135deg, #00c6ff 0%, #0072ff 100%);">
                                <div class="card-body p-4 d-flex justify-content-between align-items-center">
                                    <div>
                                        <h6 class="text-uppercase mb-2 text-white-50 fw-bold letter-spacing-1">Clima GUA</h6>
                                        <h1 class="fw-bold mb-0 display-6"><asp:Label ID="lblClimaTemp" runat="server" Text="--"></asp:Label>°C</h1>
                                        <small class="text-capitalize fw-bold opacity-75"><asp:Label ID="lblClimaCondicion" runat="server" Text="Cargando..."></asp:Label></small>
                                    </div>
                                    <h1 class="m-0 display-4">
                                        <asp:Literal ID="litClimaIcono" runat="server">🌤️</asp:Literal>
                                    </h1>
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

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    
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