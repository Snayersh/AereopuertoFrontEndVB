<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PaseAbordar.aspx.vb" Inherits="AereopuertoFrontEndVB.PaseAbordar" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Pase de Abordar - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #e9ecef; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; padding: 40px 0; }
        .boarding-pass { max-width: 800px; margin: 0 auto 40px auto; background: white; border-radius: 15px; box-shadow: 0 15px 35px rgba(0,0,0,0.1); display: flex; overflow: hidden; position: relative; }
        .bp-left { flex: 2; padding: 30px; border-right: 2px dashed #ccc; }
        .bp-right { flex: 1; padding: 30px; background-color: #f8f9fa; display: flex; flex-direction: column; align-items: center; justify-content: center; }
        
        .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 25px; border-bottom: 3px solid #0d47a1; padding-bottom: 10px; }
        .header h3 { color: #0d47a1; margin: 0; font-weight: 800; text-transform: uppercase; letter-spacing: 1px; }
        
        .lbl { font-size: 0.75rem; color: #777; text-transform: uppercase; font-weight: bold; margin-bottom: 2px; }
        .val { font-size: 1.2rem; font-weight: bold; color: #222; margin-bottom: 15px; }
        
        .iata-code { font-size: 3rem; font-weight: 900; color: #0d47a1; line-height: 1; }
        .city-name { font-size: 0.9rem; color: #555; }
        
        /* Simulación de código de barras */
        .barcode { font-family: 'Libre Barcode 39', cursive, sans-serif; font-size: 4rem; margin-top: 15px; color: #333; letter-spacing: 2px; transform: scaleY(1.5); }
        
        /* Reglas estrictas para Imprimir (1 boleto por hoja) */
        @media print {
            body { background: white; padding: 0; }
            .no-print { display: none !important; }
            .boarding-pass { 
                box-shadow: none; 
                border: 1px solid #000; 
                margin-top: 0; 
                margin-bottom: 0 !important;
                page-break-after: always; /* ¡La magia para que salgan en hojas separadas! */
            }
        }
    </style>
    <link href="https://fonts.googleapis.com/css2?family=Libre+Barcode+39&display=swap" rel="stylesheet">
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="text-center mb-4 no-print">
            <button type="button" class="btn btn-primary btn-lg fw-bold px-5 rounded-pill shadow" onclick="window.print();">
                🖨️ Imprimir Pases de Abordar
            </button>
            <p class="mt-2 text-muted">Asegúrate de llevar estos documentos el día de tu vuelo.</p>
        </div>

        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger text-center mx-auto" style="max-width: 800px;">
            <asp:Label ID="lblError" runat="server" CssClass="fw-bold"></asp:Label>
        </asp:Panel>

        <asp:Repeater ID="rptPases" runat="server">
            <ItemTemplate>
                <div class="boarding-pass">
                    <div class="bp-left">
                        <div class="header">
                            <h3>✈️ La Aurora Airlines</h3>
                            <span class="badge bg-dark px-3 py-2 fs-6">PASE DE ABORDAR</span>
                        </div>
                        
                        <div class="row">
                            <div class="col-8">
                                <div class="lbl">Nombre del Pasajero</div>
                                <div class="val text-uppercase"><%# Eval("pasajero") %></div>
                            </div>
                            <div class="col-4">
                                <div class="lbl">Vuelo</div>
                                <div class="val text-primary"><%# Eval("codigo_vuelo") %></div>
                            </div>
                        </div>

                        <div class="row align-items-center my-3">
                            <div class="col-5 text-center">
                                <div class="iata-code"><%# Eval("origen_iata") %></div>
                                <div class="city-name"><%# Eval("origen_ciudad") %></div>
                            </div>
                            <div class="col-2 text-center fs-2 text-muted">✈️</div>
                            <div class="col-5 text-center">
                                <div class="iata-code"><%# Eval("destino_iata") %></div>
                                <div class="city-name"><%# Eval("destino_ciudad") %></div>
                            </div>
                        </div>

                        <div class="row mt-4 border-top pt-3">
                            <div class="col-3">
                                <div class="lbl">Fecha</div>
                                <div class="val fs-6"><%# Eval("fecha") %></div>
                            </div>
                            <div class="col-3">
                                <div class="lbl">Hora de Salida</div>
                                <div class="val fs-6"><%# Eval("hora_salida") %></div>
                            </div>
                            <div class="col-3">
                                <div class="lbl">Clase</div>
                                <div class="val fs-6 text-primary"><%# Eval("clase_cabina") %></div>
                            </div>
                            <div class="col-3">
                                <div class="lbl">Asiento</div>
                                <div class="val fs-3 text-danger"><%# Eval("asiento") %></div>
                            </div>
                        </div>
                    </div>

                    <div class="bp-right border-start">
                        <h5 class="text-primary fw-bold text-uppercase mb-3">Tarjeta de Embarque</h5>
                        
                        <div class="w-100 mb-2">
                            <div class="lbl">Localizador</div>
                            <div class="val fs-4"><%# Eval("codigo_boleto") %></div>
                        </div>
                        
                        <div class="w-100 mb-2">
                            <div class="lbl">Asiento</div>
                            <div class="val fs-3 text-danger"><%# Eval("asiento") %></div>
                        </div>

                        <div class="barcode text-center w-100 mt-auto">
                            *<%# Eval("codigo_boleto") %>*
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

    </form>
</body>
</html>