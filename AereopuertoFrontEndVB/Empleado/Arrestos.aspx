<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Arrestos.aspx.vb" Inherits="AereopuertoFrontEndVB.Arrestos" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Infracciones y Antecedentes</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; padding-top: 40px; padding-bottom: 50px; }
        .form-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); padding: 30px; border-top: 6px solid #d32f2f; margin-bottom: 30px; }
        .section-title { font-size: 1.2rem; font-weight: 800; color: #d32f2f; border-bottom: 2px solid #ffcdd2; padding-bottom: 10px; margin-bottom: 20px; }
        .btn-danger-custom { background-color: #d32f2f; color: white; border: none; font-weight: bold; }
        .btn-danger-custom:hover { background-color: #b71c1c; color: white; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container" style="max-width: 900px;">
            <div class="mb-4 text-center">
                <a href="../Default.aspx" class="btn btn-outline-secondary fw-bold rounded-pill px-4 shadow-sm">← Volver al Tablero Principal</a>
            </div>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="form-card" style="border-top-color: #424242;">
                <h5 class="section-title" style="color: #424242; border-bottom-color: #bdbdbd;">🔍 Verificación de Pasajeros (Antecedentes)</h5>
                
                <div class="input-group mb-4 shadow-sm">
                    <asp:TextBox ID="txtBusqueda" runat="server" CssClass="form-control" placeholder="Buscar por Nombre o Pasaporte..."></asp:TextBox>
                    
                    <asp:Button ID="btnBuscar" runat="server" Text="Verificar Historial" CssClass="btn btn-dark fw-bold px-4" 
                        formnovalidate="formnovalidate" CausesValidation="false" />
                </div>

                <div class="table-responsive">
                    <table class="table table-hover align-middle border">
                        <thead class="table-light">
                            <tr>
                                <th>Infractor</th>
                                <th>Pasaporte</th>
                                <th>Motivo / Alerta</th>
                                <th>Fecha</th>
                                <th>Autoridad</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptArrestos" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="fw-bold text-danger"><%# Eval("nombre_infractor") %></td>
                                        <td><%# Eval("pasaporte") %></td>
                                        <td class="text-muted small"><%# Eval("motivo") %></td>
                                        <td><span class="badge bg-secondary"><%# Eval("fecha_arresto", "{0:dd/MM/yyyy}") %></span></td>
                                        <td class="fw-bold"><%# Eval("autoridad_cargo") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>

            <div class="form-card">
                <h5 class="section-title">🚨 Registrar Nueva Infracción</h5>
                <div class="row">
                    <div class="col-md-6 mb-3">
                        <label class="form-label text-secondary fw-bold small">Nombre del Infractor *</label>
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" required="true"></asp:TextBox>
                    </div>
                    <div class="col-md-6 mb-3">
                        <label class="form-label text-secondary fw-bold small">Pasaporte / DPI</label>
                        <asp:TextBox ID="txtPasaporte" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="mb-3">
                    <label class="form-label text-secondary fw-bold small">Motivo *</label>
                    <asp:TextBox ID="txtMotivo" runat="server" CssClass="form-control" required="true"></asp:TextBox>
                </div>
                <div class="mb-3">
                    <label class="form-label text-secondary fw-bold small">Autoridad a Cargo *</label>
                    <asp:DropDownList ID="ddlAutoridad" runat="server" CssClass="form-select" required="true">
                        <asp:ListItem Text="-- Seleccionar --" Value=""></asp:ListItem>
                        <asp:ListItem Text="PNC" Value="PNC"></asp:ListItem>
                        <asp:ListItem Text="Interpol" Value="Interpol"></asp:ListItem>
                        <asp:ListItem Text="Seguridad Interna" Value="Seguridad Interna"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                
                <asp:Button ID="btnGuardarArresto" runat="server" Text="Registrar Incidente Oficial" CssClass="btn btn-danger-custom w-100 shadow-sm mt-2" />
            </div>
        </div>
    </form>
</body>
</html>