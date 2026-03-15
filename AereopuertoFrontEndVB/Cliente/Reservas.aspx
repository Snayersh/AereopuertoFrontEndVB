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
  
        /* Colores por clase de cabina */
        .seat.primera { border-color: #ffd700; color: #b8860b; background-color: #fffde7; }
        .seat.ejecutiva { border-color: #ab47bc; color: #6a1b9a; background-color: #f3e5f5; }
        .seat.economica { border-color: #90caf9; color: #1976d2; background-color: #e3f2fd; }
        
        /* Efecto de apagado para asientos que no son de la clase seleccionada */
        .seat.dimmed { 
            opacity: 0.2; 
            pointer-events: none; /* Evita que le den clic */
            background-color: #f5f5f5 !important; 
            border-color: #e0e0e0 !important; 
            color: #bdbdbd !important; 
        }

        /* Caja de precio total */
        .price-box { background: #e8f5e9; border: 2px solid #4caf50; border-radius: 10px; padding: 15px; margin-top: 20px; text-align: center; }
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
                <div class="col-md-11">
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
                            <div class="col-lg-5 border-end pe-lg-4">
                                <h5 class="fw-bold text-primary mb-4">1. Datos del Viaje</h5>
                                
                                <div class="mb-4">
                                    <label class="form-label fw-bold text-secondary">Vuelos Disponibles</label>
                                    <asp:DropDownList ID="ddlVuelos" runat="server" CssClass="form-select shadow-sm" required="true" AutoPostBack="true" OnSelectedIndexChanged="ddlVuelos_SelectedIndexChanged"></asp:DropDownList>
                                </div>

                                <div class="mb-4">
                                    <label class="form-label fw-bold text-secondary">Clase de Cabina</label>
<asp:DropDownList ID="ddlClase" runat="server" CssClass="form-select shadow-sm"></asp:DropDownList>                                </div>

                                <asp:HiddenField ID="hfAsientoSeleccionado" runat="server" />
                                
                                <div class="mt-5 pt-3">
                                    <asp:Button ID="btnReservar" runat="server" Text="Confirmar Reserva" CssClass="btn btn-primary w-100 shadow" OnClientClick="return ValidarAsiento();" />
                                </div>
                            </div>

                            <div class="col-lg-7 ps-lg-5 mt-4 mt-lg-0">
                                <h5 class="fw-bold text-primary mb-4 text-center">2. Selección de Asiento</h5>
                                
                              <div class="d-flex justify-content-center flex-wrap mb-4 small fw-bold gap-3">
    <div><span style="display:inline-block; width:15px; height:15px; background:#fffde7; border:2px solid #ffd700; border-radius:3px; vertical-align: middle;"></span> Primera</div>
    <div><span style="display:inline-block; width:15px; height:15px; background:#f3e5f5; border:2px solid #ab47bc; border-radius:3px; vertical-align: middle;"></span> Ejecutiva</div>
    <div><span style="display:inline-block; width:15px; height:15px; background:#e3f2fd; border:2px solid #90caf9; border-radius:3px; vertical-align: middle;"></span> Económica</div>
    <div><span style="display:inline-block; width:15px; height:15px; background:#e0e0e0; border:2px solid #bdbdbd; border-radius:3px; vertical-align: middle;"></span> Ocupado</div>
    <div><span style="display:inline-block; width:15px; height:15px; background:#0d47a1; border-radius:3px; vertical-align: middle;"></span> Tu Selección</div>
</div>

                                <div class="plane-fuselage shadow-sm" id="panelAvion" runat="server" visible="false">
                                    <asp:Literal ID="litMapaAsientos" runat="server"></asp:Literal>
                                </div>
                                
                                <div class="text-center mt-4 fw-bold text-secondary">
                                    Asientos elegidos: <span id="lblAsientoMostrado" class="text-primary fs-5">Ninguno</span>
                                </div>

                                <div class="price-box shadow-sm mx-auto" id="cajaPrecio" style="display: none; max-width: 350px;">
                                    <h5 class="mb-0 text-dark">Total Estimado: <span id="lblTotalPrecio" class="text-success fw-bold fs-3">Q 0.00</span></h5>
                                    <small class="text-muted fw-bold" id="lblDetallePrecio">0 asientos x Q 0.00</small>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>

 <script>
     // Array para guardar los asientos elegidos en formato "1A:3" (Asiento : IdClase)
     let asientosSeleccionados = [];

     function siExisteFuncion(nombre) {
         if (typeof window[nombre] === "function") window[nombre]();
     }

     // 1. Efecto Visual de Filtrado (Ya no borra los asientos seleccionados)
     function actualizarFiltroYPrecio() {
         let ddl = document.getElementById('<%= ddlClase.ClientID %>');
         if (!ddl) return;

         let valor = ddl.value;
         let claseActualCSS = "";
         if (valor === "1") claseActualCSS = "economica";
         else if (valor === "2") claseActualCSS = "ejecutiva";
         else if (valor === "3") claseActualCSS = "primera";

         let todosLosAsientos = document.querySelectorAll('.seat');
         todosLosAsientos.forEach(asiento => {
             if (asiento.classList.contains('occupied')) return;

             // Si no hay filtro, o el asiento coincide con el filtro, lo encendemos
             if (claseActualCSS === "" || asiento.classList.contains(claseActualCSS)) {
                 asiento.classList.remove('dimmed');
             } else {
                 // Si no coincide, lo opacamos
                 asiento.classList.add('dimmed');
             }
         });
     }

     // 2. Selección Multi-Clase
     function seleccionarAsiento(elemento, numeroAsiento) {
         if (elemento.classList.contains('dimmed') || elemento.classList.contains('occupied')) return;

         // Obtenemos el ID de la clase de este asiento en particular
         let idClaseAsiento = elemento.getAttribute('data-idclase');
         let comboDatos = numeroAsiento + ":" + idClaseAsiento; // Ej: "1A:3"

         if (elemento.classList.contains('selected')) {
             // Deseleccionar
             elemento.classList.remove('selected');
             asientosSeleccionados = asientosSeleccionados.filter(a => a !== comboDatos);
             elemento.innerText = numeroAsiento;
         } else {
             // Seleccionar
             elemento.classList.add('selected');
             asientosSeleccionados.push(comboDatos);
             elemento.innerText = '✖';
         }

         // Guardamos en el HiddenField para mandarlo a Visual Basic
         document.getElementById('<%= hfAsientoSeleccionado.ClientID %>').value = asientosSeleccionados.join(',');
            
            // Extraemos solo los números para mostrarlos en pantalla (limpiando el ":1")
            let soloNumeros = asientosSeleccionados.map(a => a.split(':')[0]);
            document.getElementById('lblAsientoMostrado').innerText = soloNumeros.length > 0 ? soloNumeros.join(', ') : 'Ninguno';

            calcularTotal();
        }

        // 3. Calculadora Inteligente
        function calcularTotal() {
            let total = 0;
            let cantidad = 0;
            
            // Buscamos todos los asientos que tengan la clase "selected" y sumamos sus precios
            document.querySelectorAll('.seat.selected').forEach(asiento => {
                total += parseFloat(asiento.getAttribute('data-precio'));
                cantidad++;
            });

            let cajaPrecio = document.getElementById('cajaPrecio');

            if (cantidad > 0) {
                document.getElementById('lblTotalPrecio').innerText = "Q " + total.toFixed(2);
                document.getElementById('lblDetallePrecio').innerText = cantidad + " asientos seleccionados";
                cajaPrecio.style.display = "block";
            } else {
                cajaPrecio.style.display = "none";
            }
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