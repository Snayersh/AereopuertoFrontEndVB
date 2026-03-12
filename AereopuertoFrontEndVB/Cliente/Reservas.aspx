<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Reservas.aspx.vb" Inherits="AereopuertoFrontEndVB.Reservas" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Reserva de Vuelos - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .booking-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 35px; border: none; margin-top: 30px; border-top: 5px solid #1976d2; margin-bottom: 40px; }
        .form-control, .form-select { height: 50px; border-radius: 8px; font-size: 1.1rem; }
        .btn-primary { background-color: #0d47a1; border: none; height: 50px; font-weight: bold; border-radius: 8px; font-size: 1.1rem; transition: 0.3s; }
        .btn-primary:hover { background-color: #1565c0; transform: translateY(-2px); box-shadow: 0 5px 15px rgba(13, 71, 161, 0.3); }
        
        /* ESTILOS DEL MAPA DE ASIENTOS */
        .plane-fuselage { background: #eef2f5; border-radius: 40px 40px 10px 10px; padding: 30px 20px; border: 2px solid #cfd8dc; max-width: 350px; margin: 0 auto; }
        .seat-row { display: flex; justify-content: center; margin-bottom: 12px; }
        .seat { width: 40px; height: 40px; border-radius: 8px; margin: 0 5px; display: flex; align-items: center; justify-content: center; font-size: 0.8rem; font-weight: bold; cursor: pointer; transition: 0.2s; border: 2px solid transparent; }
        .aisle { width: 30px; } /* El pasillo */
        
        /* Estados de los asientos */
        .seat.available { background-color: #ffffff; border-color: #90caf9; color: #1976d2; }
        .seat.available:hover { background-color: #bbdefb; }
        .seat.selected { background-color: #0d47a1; color: white; border-color: #0d47a1; transform: scale(1.1); box-shadow: 0 4px 8px rgba(13,71,161,0.3); }
        .seat.occupied { background-color: #e0e0e0; color: #9e9e9e; cursor: not-allowed; border-color: #bdbdbd; text-decoration: line-through; }
        
        .ticket-result { background-color: #e8f5e9; border: 2px dashed #4caf50; border-radius: 10px; padding: 20px; margin-top: 20px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">🎫 Portal de Pasajeros</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Inicio</a>
        </div>

        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-10">
                    <div class="booking-card">
                        <div class="text-center mb-4">
                            <h2 class="fw-bold" style="color: #0d47a1;">Encuentra tu próximo destino</h2>
                            <p class="text-muted fs-5">Reserva tu boleto y elige tu asiento ideal.</p>
                        </div>

                        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger text-center rounded-3 mb-4">
                            <asp:Label ID="lblError" runat="server" CssClass="fw-bold"></asp:Label>
                        </asp:Panel>

                     <asp:Panel ID="pnlExito" runat="server" Visible="false" CssClass="ticket-result text-center mb-4">
    <h4 class="text-success fw-bold mb-2">¡Reserva Confirmada! 🎉</h4>
    <p class="m-0 text-secondary">Tu código de localización es:</p>
    <h1 class="fw-bold text-dark my-2 tracking-widest"><asp:Label ID="lblCodigoBoleto" runat="server"></asp:Label></h1>
    <p class="fs-5 m-0 text-primary fw-bold">Asiento: <asp:Label ID="lblAsientoConfirmado" runat="server"></asp:Label></p>
    
    <div class="mt-4 mb-2">
        <asp:HyperLink ID="hlPagarAhora" runat="server" CssClass="btn btn-success fw-bold px-4 py-2 shadow" style="border-radius: 8px; font-size: 1.1rem;">
            💳 Pagar Ahora
        </asp:HyperLink>
    </div>
    
    <p class="small text-muted m-0 mt-2">O si lo prefieres, puedes pagarlo más tarde desde "Mis Boletos".</p>
</asp:Panel>

                        <div class="row">
                            <div class="col-md-6 border-end pe-md-4">
                                <h5 class="fw-bold text-primary mb-3">1. Datos del Viaje</h5>
                                
                                <div class="mb-4">
                                    <label class="form-label fw-bold text-secondary">Vuelos Disponibles</label>
                                    <asp:DropDownList ID="ddlVuelos" runat="server" CssClass="form-select" required="true" AutoPostBack="true" OnSelectedIndexChanged="ddlVuelos_SelectedIndexChanged"></asp:DropDownList>
                                </div>

                                <div class="mb-4">
                                    <label class="form-label fw-bold text-secondary">Clase de Cabina</label>
                                    <asp:DropDownList ID="ddlClase" runat="server" CssClass="form-select" required="true"></asp:DropDownList>
                                </div>

                                <asp:HiddenField ID="hfAsientoSeleccionado" runat="server" />
                                
                                <div class="mt-5">
                                    <asp:Button ID="btnReservar" runat="server" Text="Confirmar Reserva" CssClass="btn btn-primary w-100 shadow-sm" OnClientClick="return ValidarAsiento();" />
                                </div>
                            </div>

                            <div class="col-md-6 ps-md-4">
                                <h5 class="fw-bold text-primary mb-3 text-center">2. Selección de Asiento</h5>
                                
                                <div class="d-flex justify-content-center mb-3 small fw-bold">
                                    <div class="me-3"><span style="display:inline-block; width:15px; height:15px; background:#fff; border:2px solid #90caf9; border-radius:3px;"></span> Libre</div>
                                    <div class="me-3"><span style="display:inline-block; width:15px; height:15px; background:#e0e0e0; border:2px solid #bdbdbd; border-radius:3px;"></span> Ocupado</div>
                                    <div><span style="display:inline-block; width:15px; height:15px; background:#0d47a1; border-radius:3px;"></span> Tu Selección</div>
                                </div>

                                <div class="plane-fuselage" id="panelAvion" runat="server" visible="false">
                                    <asp:Literal ID="litMapaAsientos" runat="server"></asp:Literal>
                                </div>
                                
                                <div class="text-center mt-3 fw-bold text-secondary">
                                    Asiento elegido: <span id="lblAsientoMostrado" class="text-primary fs-4">Ninguno</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>

   <script>
       // Array para guardar todos los asientos que vayas tocando
       let asientosSeleccionados = [];

       function seleccionarAsiento(elemento, numeroAsiento) {
           // Si el asiento ya estaba seleccionado, lo quitamos
           if (elemento.classList.contains('selected')) {
               elemento.classList.remove('selected');
               elemento.classList.add('available');

               // Lo borramos de nuestra lista
               asientosSeleccionados = asientosSeleccionados.filter(a => a !== numeroAsiento);
           }
           // Si estaba libre, lo seleccionamos
           else {
               elemento.classList.remove('available');
               elemento.classList.add('selected');

               // Lo agregamos a la lista
               asientosSeleccionados.push(numeroAsiento);
           }

           // Unimos los asientos con comas (Ej: "1A,1B,2C") y los guardamos para VB
           document.getElementById('<%= hfAsientoSeleccionado.ClientID %>').value = asientosSeleccionados.join(',');
            
            // Mostramos la lista en pantalla
            let textoMostrar = asientosSeleccionados.length > 0 ? asientosSeleccionados.join(', ') : 'Ninguno';
            document.getElementById('lblAsientoMostrado').innerText = textoMostrar;
        }

        function ValidarAsiento() {
            var asiento = document.getElementById('<%= hfAsientoSeleccionado.ClientID %>').value;
           if (asiento === "") {
               alert("Por favor, selecciona al menos un asiento en el mapa del avión.");
               return false;
           }
           return true;
       }
   </script>
</body>
    </html>