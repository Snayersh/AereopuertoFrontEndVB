<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Pagos.aspx.vb" Inherits="AereopuertoFrontEndVB.Pagos" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Pago de Reserva - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .payment-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 40px; border-top: 5px solid #1565c0; margin-top: 40px; margin-bottom: 40px; }
        .form-control { height: 50px; border-radius: 8px; font-size: 1.1rem; }
        
        /* Botón Estandarizado al Azul Oficial */
        .btn-custom { background-color: #0d47a1; color: white; border: none; height: 50px; font-weight: bold; border-radius: 8px; font-size: 1.1rem; transition: 0.3s; }
        .btn-custom:hover { background-color: #1565c0; color: white; transform: translateY(-2px); box-shadow: 0 5px 15px rgba(13, 71, 161, 0.3); }
        
        .cc-box { background: linear-gradient(135deg, #0d47a1 0%, #1976d2 100%); color: white; border-radius: 15px; padding: 25px; margin-bottom: 25px; box-shadow: 0 8px 20px rgba(0,0,0,0.15); }
        .cc-chip { width: 50px; height: 35px; background: #ffd54f; border-radius: 5px; margin-bottom: 15px; opacity: 0.9; box-shadow: inset 0 0 5px rgba(0,0,0,0.2); }
        .section-title { font-size: 0.9rem; font-weight: bold; color: #1565c0; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 15px; }
        .receipt-box { background-color: #f8f9fc; border: 2px dashed #bbdefb; border-radius: 10px; padding: 20px; margin-top: 20px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">💳 Pasarela de Pagos Segura</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Inicio</a>
        </div>

        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-8 col-lg-6">
                    <div class="payment-card">
                        <div class="text-center mb-4">
                            <h2 class="fw-bold" style="color: #0d47a1;">Completa tu Compra</h2>
                            <p class="text-muted">Ingresa tu código de reserva y los datos de pago.</p>
                        </div>

                        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger text-center rounded-3 mb-4 shadow-sm">
                            <asp:Label ID="lblError" runat="server" CssClass="fw-bold"></asp:Label>
                        </asp:Panel>

                        <asp:Panel ID="pnlExito" runat="server" Visible="false" CssClass="text-center mb-4">
                            <div class="alert alert-success rounded-3 p-4 border-2 border-success shadow-sm">
                                <h3 class="fw-bold text-success mb-2">¡Pago Procesado con Éxito! ✅</h3>
                                <p class="fs-5 mb-1">Tus boletos ahora están confirmados.</p>
                                
                                <div class="receipt-box text-start mt-4 mb-4">
                                    <div class="d-flex justify-content-between border-bottom pb-2 mb-2 border-primary border-opacity-25">
                                        <span class="text-secondary fw-bold">Localizador:</span>
                                        <span class="fw-bold fs-5 text-primary"><asp:Label ID="lblLocalizadorExito" runat="server"></asp:Label></span>
                                    </div>
                                    <div class="d-flex justify-content-between">
                                        <span class="text-secondary fw-bold">Factura Generada:</span>
                                        <span class="fw-bold text-dark"><asp:Label ID="lblFactura" runat="server"></asp:Label></span>
                                    </div>
                                </div>

                                <a href="MisBoletos.aspx" class="btn btn-custom px-5 py-3 rounded-pill shadow">Ir a Imprimir Pase de Abordar 🎫</a>
                            </div>
                        </asp:Panel>

                        <asp:Panel ID="pnlFormulario" runat="server">
                            <div class="section-title">1. Datos de la Reserva</div>
                            <div class="mb-4">
                                <label class="form-label fw-bold text-secondary">Código de Reserva (Localizador)</label>
                                <asp:TextBox ID="txtCodigoReserva" runat="server" CssClass="form-control text-uppercase border-primary border-opacity-25" placeholder="Ej: B-A1B2C" required="true"></asp:TextBox>
                                <div class="form-text">El código que obtuviste al seleccionar tus asientos.</div>
                            </div>

                            <div class="section-title mt-5">2. Método de Pago</div>
                            
                            <div class="cc-box">
                                <div class="cc-chip"></div>
                                <div class="mb-3">
                                    <label class="form-label small text-white-50 mb-1">Número de Tarjeta</label>
                                    <asp:TextBox ID="txtTarjeta" runat="server" CssClass="form-control bg-light border-0 fw-bold text-dark" placeholder="0000 0000 0000 0000" MaxLength="19" required="true"></asp:TextBox>
                                </div>
                                <div class="row">
                                    <div class="col-6">
                                        <label class="form-label small text-white-50 mb-1">Vencimiento</label>
                                        <asp:TextBox ID="txtVencimiento" runat="server" CssClass="form-control bg-light border-0 fw-bold text-dark" placeholder="MM/YY" MaxLength="5" required="true"></asp:TextBox>
                                    </div>
                                    <div class="col-6">
                                        <label class="form-label small text-white-50 mb-1">CVV</label>
                                        <asp:TextBox ID="txtCVV" runat="server" CssClass="form-control bg-light border-0 fw-bold text-dark" placeholder="123" MaxLength="4" TextMode="Password" required="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="mt-3">
                                    <label class="form-label small text-white-50 mb-1">Nombre en la tarjeta</label>
                                    <asp:TextBox ID="txtNombreTitular" runat="server" CssClass="form-control bg-light border-0 text-uppercase fw-bold text-dark" placeholder="JUAN PEREZ" required="true"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mt-4">
                                <asp:Button ID="btnPagar" runat="server" Text="Procesar Pago Seguro 🔒" CssClass="btn btn-custom w-100 shadow-sm" OnClientClick="return simularValidacion(this);" UseSubmitBehavior="false" />
                            </div>
                        </asp:Panel>

                    </div>
                </div>
            </div>
        </div>
    </form>

    <script>
        // Lógica de formateo visual de la tarjeta
        document.getElementById('<%= txtTarjeta.ClientID %>').addEventListener('input', function (e) {
            var target = e.target;
            var position = target.selectionEnd;
            var length = target.value.length;
            target.value = target.value.replace(/[^\d]/g, '').replace(/(.{4})/g, '$1 ').trim();
            target.selectionEnd = position += ((target.value.charAt(position - 1) === ' ' && target.value.charAt(length - 1) === ' ' && length !== target.value.length) ? 1 : 0);
        });

        document.getElementById('<%= txtVencimiento.ClientID %>').addEventListener('input', function (e) {
            var target = e.target;
            target.value = target.value.replace(/[^\d]/g, '').replace(/^(\d{2})(\d{1,2})/, '$1/$2').trim();
        });

        document.getElementById('<%= txtCVV.ClientID %>').addEventListener('input', function (e) {
            this.value = this.value.replace(/[^\d]/g, '');
        });

        // Animación de Validación Segura (Ajustada a colores oficiales)
        function simularValidacion(boton) {
            var form = document.getElementById('form1');
            if (!form.checkValidity()) {
                form.reportValidity();
                return false;
            }

            Swal.fire({
                title: 'Conectando con el Banco',
                html: 'Enviando transacción cifrada...<br><br>',
                allowOutsideClick: false,
                showConfirmButton: false,
                didOpen: () => {
                    Swal.showLoading();
                    
                    setTimeout(() => {
                        const content = Swal.getHtmlContainer();
                        if (content) {
                            // Cambiado al azul corporativo #0d47a1
                            content.innerHTML = '<span style="color:#0d47a1; font-weight:bold;">Validando fondos y seguridad de la tarjeta...</span><br><br>';
                        }
                    }, 1500);

                    setTimeout(() => {
                        Swal.close();
                        __doPostBack('<%= btnPagar.UniqueID %>', '');
                    }, 3000);
                }
            });

            return false;
        }
    </script>
</body>
</html>