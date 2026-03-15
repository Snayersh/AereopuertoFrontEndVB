<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Tripulacion.aspx.vb" Inherits="AereopuertoFrontEndVB.Tripulacion" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Asignación de Tripulación - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .card-custom { background: white; border-radius: 12px; box-shadow: 0 8px 20px rgba(0,0,0,0.05); padding: 30px; margin-bottom: 25px; border-top: 5px solid #1976d2; }
        .form-select { height: 45px; border-radius: 8px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">👨‍✈️ Gestión de Tripulación</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Inicio</a>
        </div>

        <div class="container mt-5">
            <div class="row justify-content-center">
                <div class="col-lg-8">
                    
                    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold shadow-sm">
                        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                    </asp:Panel>

                    <div class="card-custom">
                        <h5 class="fw-bold text-primary mb-3">1. Seleccionar Vuelo Programado</h5>
                        <asp:DropDownList ID="ddlVuelos" runat="server" CssClass="form-select shadow-sm"></asp:DropDownList>
                    </div>

                    <div class="card-custom">
                        <h5 class="fw-bold text-primary mb-4">2. Asignar Personal al Vuelo</h5>
                        
                        <div class="row">
                            <div class="col-md-6 mb-4">
                                <label class="form-label fw-bold text-secondary">Piloto Principal (Capitán)</label>
                                <asp:DropDownList ID="ddlPiloto" runat="server" CssClass="form-select shadow-sm"></asp:DropDownList>
                            </div>
                            
                            <div class="col-md-6 mb-4">
                                <label class="form-label fw-bold text-secondary">Copiloto (Primer Oficial)</label>
                                <asp:DropDownList ID="ddlCopiloto" runat="server" CssClass="form-select shadow-sm"></asp:DropDownList>
                            </div>

                            <div class="col-md-12 mb-4">
                                <label class="form-label fw-bold text-secondary">Sobrecargo / Tripulante de Cabina</label>
                                <asp:DropDownList ID="ddlSobrecargo" runat="server" CssClass="form-select shadow-sm"></asp:DropDownList>
                                <small class="text-muted">Nota: Puedes agregar múltiples sobrecargos guardando uno por uno.</small>
                            </div>
                        </div>

                        <div class="text-end mt-2">
                            <asp:Button ID="btnAsignar" runat="server" Text="➕ Guardar Asignación de Tripulación" CssClass="btn btn-primary fw-bold px-4 py-2 rounded-pill shadow-sm" />
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </form>
</body>
</html>