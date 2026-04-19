<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RegistroTripulacion.aspx.vb" Inherits="AereopuertoFrontEndVB.RegistroTripulacion" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Registro de Tripulación - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; padding-top: 40px; }
        .form-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); padding: 40px; border-top: 6px solid #0d47a1; max-width: 700px; margin: 0 auto; }
        .section-title { font-size: 1.2rem; font-weight: 800; color: #0d47a1; border-bottom: 2px solid #e3f2fd; padding-bottom: 10px; margin-bottom: 25px; }
        .btn-primary { background-color: #0d47a1; border: none; font-weight: bold; padding: 12px; transition: 0.3s; }
        .btn-primary:hover { background-color: #1976d2; transform: translateY(-2px); }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="mb-4 text-center">
                <a href="../Default.aspx" class="btn btn-outline-secondary fw-bold rounded-pill px-4 shadow-sm">← Volver al Tablero Principal</a>
            </div>

            <div class="form-card">
                <div class="text-center mb-4">
                    <h2 class="fw-bold text-dark">🧑‍✈️ Alta de Personal de Vuelo</h2>
                    <p class="text-muted">Registra a los pilotos y sobrecargos en la base de datos.</p>
                </div>

                <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                    <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                </asp:Panel>

                <h5 class="section-title">Datos del Empleado</h5>
                
                <div class="mb-3">
                    <label class="form-label text-secondary fw-bold small">Nombre Completo *</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" required="true" placeholder="Ej. Carlos Mendoza"></asp:TextBox>
                </div>

                <div class="row">
                    <div class="col-md-6 mb-3">
                        <label class="form-label text-secondary fw-bold small">Puesto Asignado *</label>
                        <asp:DropDownList ID="ddlPuesto" runat="server" CssClass="form-select" required="true">
                            <asp:ListItem Text="-- Seleccionar --" Value=""></asp:ListItem>
                            <asp:ListItem Text="Piloto Capitán" Value="Piloto"></asp:ListItem>
                            <asp:ListItem Text="Copiloto (Primer Oficial)" Value="Copiloto"></asp:ListItem>
                            <asp:ListItem Text="Jefe de Cabina" Value="Jefe Cabina"></asp:ListItem>
                            <asp:ListItem Text="Sobrecargo / Azafata" Value="Sobrecargo"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-6 mb-4">
                        <label class="form-label text-secondary fw-bold small">No. de Licencia (DGAC)</label>
                        <asp:TextBox ID="txtLicencia" runat="server" CssClass="form-control" placeholder="Ej. L-987654 (Opcional)"></asp:TextBox>
                    </div>
                </div>

                <asp:Button ID="btnGuardar" runat="server" Text="Registrar Tripulante" CssClass="btn btn-primary w-100 shadow-sm" />
            </div>
        </div>
    </form>
</body>
</html>