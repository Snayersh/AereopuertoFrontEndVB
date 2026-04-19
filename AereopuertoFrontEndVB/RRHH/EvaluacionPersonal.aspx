<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EvaluacionPersonal.aspx.vb" Inherits="AereopuertoFrontEndVB.EvaluacionPersonal" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Evaluación de Personal - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #2e7d32; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .hr-card { background: white; border-radius: 12px; box-shadow: 0 5px 20px rgba(0,0,0,0.05); padding: 30px; border-top: 4px solid #4caf50; margin-bottom: 30px; }
        .score-input { font-size: 2rem; font-weight: bold; text-align: center; color: #2e7d32; height: 70px; border: 2px dashed #81c784; border-radius: 10px; }
        .score-input:focus { border-color: #4caf50; box-shadow: 0 0 10px rgba(76, 175, 80, 0.2); outline: none; }
        .table-custom th { background-color: #e8f5e9; color: #2e7d32; text-transform: uppercase; font-size: 0.85rem; }
        .badge-score { font-size: 1.1rem; padding: 8px 12px; border-radius: 8px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">👥 Gestión de Talento y Desempeño</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Panel Principal</a>
        </div>

        <div class="container py-5">
            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold rounded-3 mb-4 shadow-sm">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="row">
                <div class="col-lg-4">
                    <div class="hr-card">
                        <h5 class="fw-bold text-success mb-4">📝 Nueva Evaluación</h5>
                        
                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">Colaborador a Evaluar *</label>
                            <asp:DropDownList ID="ddlEmpleado" runat="server" CssClass="form-select bg-light border-secondary" required="true"></asp:DropDownList>
                        </div>
                        
                        <div class="mb-3">
                            <label class="form-label small fw-bold text-secondary">Puntuación Global (1 al 100) *</label>
                            <asp:TextBox ID="txtPuntaje" runat="server" CssClass="form-control score-input" TextMode="Number" step="0.01" min="1" max="100" placeholder="Ej: 95.50" required="true"></asp:TextBox>
                            <div class="form-text text-center">Menor a 60 requiere plan de mejora.</div>
                        </div>

                        <div class="mb-4">
                            <label class="form-label small fw-bold text-secondary">Comentarios / Feedback del Supervisor *</label>
                            <asp:TextBox ID="txtComentario" runat="server" CssClass="form-control bg-light" TextMode="MultiLine" Rows="4" placeholder="Detalle las fortalezas y áreas de oportunidad del colaborador..." required="true" MaxLength="250"></asp:TextBox>
                        </div>

                        <asp:Button ID="btnGuardarEvaluacion" runat="server" Text="Registrar Desempeño 📊" CssClass="btn btn-success w-100 py-3 fw-bold fs-6 shadow-sm" />
                    </div>
                </div>

                <div class="col-lg-8">
                    <div class="hr-card">
                        <h5 class="fw-bold text-success mb-4">📋 Historial de Revisiones Recientes</h5>
                        
                        <div class="table-responsive">
                            <table class="table table-hover align-middle">
                                <thead class="table-custom">
                                    <tr>
                                        <th>Colaborador</th>
                                        <th>Fecha</th>
                                        <th class="text-center">Puntuación</th>
                                        <th>Feedback</th>
                                    </tr>
                                </thead>
                                <tbody>
                                 <asp:Repeater ID="rptEvaluaciones" runat="server">
    <ItemTemplate>
        <tr>
            <td class="fw-bold text-dark"><%# Eval("nombre_empleado") %></td>
            <td class="text-muted small"><%# Eval("fecha_evaluacion") %></td>
            <td class="text-center">
                <asp:Label ID="lblBadgePuntaje" runat="server"></asp:Label>
            </td>
            <td class="small text-secondary fst-italic">"<%# Eval("comentario") %>"</td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
                                </tbody>
                            </table>
                            
                            <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="text-center py-4">
                                <p class="text-muted">No hay evaluaciones registradas en el sistema.</p>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>