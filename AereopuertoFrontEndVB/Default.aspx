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
        
        /* Estilos del Menú Lateral */
        .sidebar { background-color: #0d47a1; min-height: 100vh; color: white; padding-top: 2rem; box-shadow: 4px 0 15px rgba(0,0,0,0.1); }
        .sidebar-brand { font-size: 1.5rem; font-weight: bold; padding: 0 1.5rem; margin-bottom: 2rem; display: block; color: white; text-decoration: none; }
        .nav-link { color: rgba(255,255,255,0.7); font-weight: 500; padding: 0.8rem 1.5rem; transition: all 0.3s; border-left: 4px solid transparent; }
        .nav-link:hover, .nav-link.active { color: white; background-color: rgba(255,255,255,0.1); border-left-color: #64b5f6; }
        .section-title { font-size: 0.75rem; font-weight: bold; letter-spacing: 1px; color: #90caf9; margin: 1.5rem 1.5rem 0.5rem 1.5rem; text-transform: uppercase; }
        
        /* Estilos del Contenido Principal */
        .main-content { padding: 2rem; height: 100vh; overflow-y: auto; }
        .dashboard-header { background: linear-gradient(135deg, #1976d2 0%, #1565c0 100%); color: white; border-radius: 15px; padding: 2rem; margin-bottom: 2rem; box-shadow: 0 8px 20px rgba(25, 118, 210, 0.2); }
        .stat-card { border-radius: 15px; border: none; transition: transform 0.3s ease; }
        .stat-card:hover { transform: translateY(-5px); }
        .table-container { border-radius: 15px; background: white; padding: 1.5rem; box-shadow: 0 5px 20px rgba(0,0,0,0.05); }
        
        /* Badges de Estado */
        .badge-llegada { background-color: #e3f2fd; color: #0d47a1; padding: 6px 12px; border-radius: 20px; font-weight: bold; }
        .badge-salida { background-color: #fff3e0; color: #e65100; padding: 6px 12px; border-radius: 20px; font-weight: bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid p-0">
            <div class="row g-0">
                
                <div class="col-md-2 sidebar d-none d-md-block">
                    <a href="Default.aspx" class="sidebar-brand">✈️ La Aurora</a>
                    
                    <ul class="nav flex-column mb-5">
                        <li class="section-title">General</li>
                        <li class="nav-item"><a class="nav-link active" href="Default.aspx">📊 Panel Principal</a></li>
                        
                        <asp:Panel ID="pnlCliente" runat="server" Visible="false">
                            <li class="section-title text-light">Mi Cuenta</li>
                                                    <li class="nav-item"><a class="nav-link" href="Account/MiPerfil.aspx">👤 Mi Perfil</a></li>
                            <li class="nav-item"><a class="nav-link text-warning fw-bold" href="Cliente/Reservas.aspx">🛒 Reservar Vuelo</a></li>
                            <li class="nav-item"><a class="nav-link" href="Cliente/MisBoletos.aspx">🎫 Mis Boletos</a></li>
                            <li class="nav-item"><a class="nav-link text-success fw-bold" href="Cliente/Pagos.aspx">💳 Pagar Reserva</a></li>
                            <li class="nav-item"><a class="nav-link" href="Cliente/Equipaje.aspx">🧳 Mi Equipaje</a></li>
                        </asp:Panel>

                        <asp:Panel ID="pnlEmpleado" runat="server" Visible="false">
                            <li class="section-title text-info">Operativa</li>
                            <li class="nav-item"><a class="nav-link" href="Admin/ControlVuelos.aspx">📡 Torre de Control</a></li>
                            <li class="nav-item"><a class="nav-link" href="Empleado/RegistroEquipaje.aspx">⚖️ Registro Equipaje</a></li>
                            <li class="nav-item"><a class="nav-link" href="Empleado/CheckIn.aspx">✅ Validar Abordaje</a></li>
                        </asp:Panel>

                        <asp:Panel ID="pnlAdmin" runat="server" Visible="false">
                            <li class="section-title text-warning">Logística de Vuelos</li>
                            <li class="nav-item"><a class="nav-link" href="Admin/ProgramasVuelo.aspx">📅 Crear Vuelo</a></li>
                            <li class="nav-item"><a class="nav-link" href="Admin/Tripulacion.aspx">👨‍✈️ Asignar Tripulación</a></li>
                            <li class="nav-item"><a class="nav-link" href="Admin/Escalas.aspx">🛑 Gestión de Escalas</a></li>
                            <li class="nav-item"><a class="nav-link" href="Admin/ControlVuelos.aspx">📡 Torre de Control</a></li>
                            
                            <li class="section-title text-danger">Catálogos Fijos</li>
                            <li class="nav-item"><a class="nav-link" href="Admin/Aerolineas.aspx">🏢 Aerolíneas</a></li>
                            <li class="nav-item"><a class="nav-link" href="Admin/Aeronaves.aspx">🛩️ Aeronaves</a></li>
                            <li class="nav-item"><a class="nav-link" href="Admin/Aereopuertos.aspx">🛫 Aeropuertos</a></li>
                            <li class="nav-item"><a class="nav-link" href="Admin/Radar.aspx">🛫 Radar</a></li>

                            
                            <li class="section-title text-success">Recursos Humanos</li>
                            <li class="nav-item"><a class="nav-link" href="Admin/Empleados.aspx">🧑‍💼 Personal (Empleados)</a></li>
                            <li class="nav-item"><a class="nav-link" href="Admin/Usuarios.aspx">👥 Usuarios y Accesos</a></li>

                            <li class="section-title text-secondary">Reportes y Auditoría</li>
                            <li class="nav-item"><a class="nav-link" href="Admin/ReportePagos.aspx">💵 Historial de Pagos</a></li>
                            <li class="nav-item"><a class="nav-link" href="Admin/Bitacora.aspx">📜 Bitácora del Sistema</a></li>
                        </asp:Panel>
                    </ul>
                </div>
                <div class="col-md-10 main-content">
                    <div class="dashboard-header d-flex justify-content-between align-items-center flex-wrap">
                        <div>
                            <h2 class="fw-bold mb-1 text-white">Centro de Control GUA</h2>
                            <p class="mb-0 text-white-50 fs-5">
                                <asp:Label ID="lblSaludo" runat="server" Text="Bienvenido, Invitado"></asp:Label>
                            </p>
                        </div>
                        <div>
                            <asp:Panel ID="pnlBotonesAcceso" runat="server">
                                <a href="Account/Login.aspx" class="btn btn-light text-primary fw-bold rounded-pill px-4 me-2">Iniciar Sesión</a>
                                <a href="Account/Registro.aspx" class="btn btn-outline-light fw-bold rounded-pill px-4">Registrarse</a>
                            </asp:Panel>
                            
                            <asp:Panel ID="pnlBotonSalir" runat="server" Visible="false">
                                <asp:Button ID="btnLogout" runat="server" Text="Cerrar Sesión" CssClass="btn btn-danger fw-bold rounded-pill px-4 shadow-sm" OnClick="btnLogout_Click" />
                            </asp:Panel>
                        </div>
                    </div>

                    <div class="row mb-4">
                        <div class="col-md-4 mb-3">
                            <div class="card stat-card shadow-sm text-white" style="background-color: #2c3e50;">
                                <div class="card-body p-4 d-flex justify-content-between align-items-center">
                                    <div><h6 class="text-uppercase mb-1 text-white-50">Vuelos Activos</h6>
                                        <h2 class="fw-bold mb-0"><asp:Label ID="lblVuelosActivos" runat="server" Text="0"></asp:Label></h2>
                                    </div>
                                    <h1 class="opacity-50 m-0">📡</h1>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 mb-3">
                            <div class="card stat-card shadow-sm text-white" style="background-color: #27ae60;">
                                <div class="card-body p-4 d-flex justify-content-between align-items-center">
                                    <div><h6 class="text-uppercase mb-1 text-white-50">Llegadas Hoy</h6>
                                        <h2 class="fw-bold mb-0"><asp:Label ID="lblLlegadas" runat="server" Text="0"></asp:Label></h2>
                                    </div>
                                    <h1 class="opacity-50 m-0">🛬</h1>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 mb-3">
                            <div class="card stat-card shadow-sm text-dark" style="background-color: #f1c40f;">
                                <div class="card-body p-4 d-flex justify-content-between align-items-center">
                                    <div><h6 class="text-uppercase mb-1 text-black-50">Salidas Hoy</h6>
                                        <h2 class="fw-bold mb-0"><asp:Label ID="lblSalidas" runat="server" Text="0"></asp:Label></h2>
                                    </div>
                                    <h1 class="opacity-50 m-0">🛫</h1>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="table-container">
                        <h4 class="fw-bold mb-4">Monitoreo en Tiempo Real</h4>
                        <div class="table-responsive">
                            <table class="table table-hover align-middle">
                                <thead class="table-dark">
                                    <tr>
                                        <th>VUELO</th><th>AEROLÍNEA</th><th>TIPO</th><th>RUTA</th><th>HORA</th><th>ESTADO</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptVuelos" runat="server">
                                        <ItemTemplate>
                                            <tr onclick="window.location.href='DetalleVuelo.aspx?id=<%# Eval("IdVuelo") %>';" style="cursor: pointer;" title="Haz clic en la fila para ver detalles">
                                                <td><strong class="fs-5 text-primary"><%# Eval("NumeroVuelo") %></strong></td>
                                                <td><span class='<%# If(Eval("Aerolinea").ToString() = "Iberia", "text-danger fw-bold", "text-secondary fw-bold") %>'><%# Eval("Aerolinea") %></span></td>
                                                <td><span class='<%# If(Convert.ToBoolean(Eval("EsLlegada")), "badge-llegada", "badge-salida") %>'><%# If(Convert.ToBoolean(Eval("EsLlegada")), "🛬 Llegada", "🛫 Salida") %></span></td>
                                                <td><strong><%# Eval("OrigenDestino") %></strong></td>
                                                <td><h5 class="m-0 fw-bold"><%# Convert.ToDateTime(Eval("HoraProgramada")).ToString("HH:mm") %></h5></td>
                                                <td><span class='<%# ObtenerClaseEstado(Eval("Estado").ToString()) %>'><%# ObtenerIconoEstado(Eval("Estado").ToString()) %> <%# Eval("Estado") %></span></td>
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