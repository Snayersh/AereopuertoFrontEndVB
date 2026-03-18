<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ProgramasVuelo.aspx.vb" Inherits="AereopuertoFrontEndVB.ProgramasVuelo" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Programación de Vuelos</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .admin-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 30px; border: none; margin-top: 30px; margin-bottom: 30px; }
        .form-control, .form-select { height: 45px; border-radius: 8px; }
        .btn-success { background-color: #2e7d32; border: none; height: 45px; font-weight: bold; border-radius: 8px; transition: 0.3s; }
        .btn-success:hover { background-color: #1b5e20; transform: translateY(-2px); }
        .section-title { color: #0d47a1; font-weight: bold; font-size: 1.1rem; border-bottom: 2px solid #e3f2fd; padding-bottom: 5px; margin-bottom: 15px; }
        .readonly-field { background-color: #e9ecef !important; cursor: not-allowed; color: #6c757d; font-weight: bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">⚙️ Administración de Operaciones</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Dashboard</a>
        </div>

        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-10">
                    <div class="admin-card">
                        <div class="text-center mb-4">
                            <h3 class="fw-bold" style="color: #0d47a1;">📅 Creación de Vuelo</h3>
                            <p class="text-muted m-0">Define las rutas, horarios y aeronaves para la operación.</p>
                        </div>

                        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info text-center rounded-3 mb-4">
                            <asp:Label ID="lblMensaje" runat="server" CssClass="fw-bold"></asp:Label>
                        </asp:Panel>

                        <div class="row g-4">
                            <div class="col-md-6">
                                <div class="section-title">Datos Operativos</div>
                                
                                <div class="mb-3">
                                    <label class="form-label fw-bold text-secondary">Código de Vuelo</label>
                                    <asp:TextBox ID="txtCodigoVuelo" runat="server" CssClass="form-control text-uppercase" placeholder="Ej. IB6341" required="true"></asp:TextBox>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label fw-bold text-secondary">Aerolínea Responsable</label>
                                    <asp:DropDownList ID="ddlAerolinea" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlAerolinea_SelectedIndexChanged" required="true"></asp:DropDownList>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label fw-bold text-secondary">Aeronave Asignada</label>
                                    <asp:DropDownList ID="ddlAeronave" runat="server" CssClass="form-select" required="true">
                                        <asp:ListItem Text="-- Primero seleccione aerolínea --" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label fw-bold text-secondary">Estado Inicial</label>
                                    <input type="text" class="form-control readonly-field text-primary" value="Programado" readonly />
                                </div>
                            </div>

                            <div class="col-md-6">
                                <div class="section-title">Ruta y Horarios</div>

                                <div class="mb-3">
                                    <label class="form-label fw-bold text-secondary">Aeropuerto de Origen</label>
                                    <input type="text" class="form-control readonly-field" value="Aurora (GUA)" readonly />
                                </div>

                                <div class="mb-3">
                                    <label class="form-label fw-bold text-secondary">Aeropuerto de Destino</label>
                                    <asp:DropDownList ID="ddlDestino" runat="server" CssClass="form-select" required="true"></asp:DropDownList>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label fw-bold text-secondary">Fecha y Hora de Salida</label>
                                    <asp:TextBox ID="txtSalida" runat="server" CssClass="form-control" TextMode="DateTimeLocal" required="true"></asp:TextBox>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label fw-bold text-secondary">Fecha y Hora de Llegada (Automática)</label>
                                    <asp:TextBox ID="txtLlegada" runat="server" CssClass="form-control readonly-field" TextMode="DateTimeLocal" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="d-flex justify-content-end border-top pt-4 mt-2">
                            <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-light border me-2 fw-bold px-4" CausesValidation="false" />
                            <asp:Button ID="btnGuardar" runat="server" Text="🛫 Programar Vuelo" CssClass="btn btn-success px-5 shadow-sm" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <script>
        function calcularLlegada() {
            let txtSalida = document.getElementById('<%= txtSalida.ClientID %>');
            let ddlDestino = document.getElementById('<%= ddlDestino.ClientID %>');
            let txtLlegada = document.getElementById('<%= txtLlegada.ClientID %>');

            if (txtSalida.value && ddlDestino.value) {
                let opcionSeleccionada = ddlDestino.options[ddlDestino.selectedIndex];
                let minutosViaje = parseInt(opcionSeleccionada.getAttribute('data-minutos')) || 120;

                let fechaSalida = new Date(txtSalida.value);
                fechaSalida.setMinutes(fechaSalida.getMinutes() + minutosViaje);

                let tzoffset = (new Date()).getTimezoneOffset() * 60000;
                let localISOTime = (new Date(fechaSalida - tzoffset)).toISOString().slice(0, 16);
                
                txtLlegada.value = localISOTime;
            } else {
                txtLlegada.value = '';
            }
        }

        document.getElementById('<%= ddlDestino.ClientID %>').addEventListener('change', calcularLlegada);
        document.getElementById('<%= txtSalida.ClientID %>').addEventListener('input', calcularLlegada);
    </script>
</body>
</html>