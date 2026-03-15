<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="MiPerfil.aspx.vb" Inherits="AereopuertoFrontEndVB.MiPerfil" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Mi Perfil - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #0d47a1; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .profile-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); padding: 40px; margin-top: 40px; border-top: 5px solid #1976d2; }
        .form-control { height: 45px; border-radius: 8px; }
        .icon-circle { width: 80px; height: 80px; background-color: #e3f2fd; color: #1976d2; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 2.5rem; margin: 0 auto 20px auto; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">👤 Gestión de Cuenta</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Inicio</a>
        </div>

        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-8 col-lg-6">
                    <div class="profile-card">
                        <div class="text-center">
                            <div class="icon-circle">🧑‍💻</div>
                            <h3 class="fw-bold text-dark">Mi Perfil Personal</h3>
                            <p class="text-muted">Actualiza tu información de contacto</p>
                        </div>

                        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center fw-bold">
                            <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                        </asp:Panel>

                        <div class="row mt-4">
                            <div class="col-md-6 mb-3">
                                <label class="form-label fw-bold text-secondary">Primer Nombre</label>
                                <asp:TextBox ID="txtPrimerNombre" runat="server" CssClass="form-control shadow-sm" required="true"></asp:TextBox>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="form-label fw-bold text-secondary">Segundo Nombre</label>
                                <asp:TextBox ID="txtSegundoNombre" runat="server" CssClass="form-control shadow-sm"></asp:TextBox>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="form-label fw-bold text-secondary">Primer Apellido</label>
                                <asp:TextBox ID="txtPrimerApellido" runat="server" CssClass="form-control shadow-sm" required="true"></asp:TextBox>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="form-label fw-bold text-secondary">Segundo Apellido</label>
                                <asp:TextBox ID="txtSegundoApellido" runat="server" CssClass="form-control shadow-sm"></asp:TextBox>
                            </div>
                            <div class="col-md-12 mb-4">
                                <label class="form-label fw-bold text-secondary">Teléfono de Contacto</label>
                                <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control shadow-sm" TextMode="Phone" required="true"></asp:TextBox>
                            </div>
                        </div>

                        <asp:Button ID="btnGuardar" runat="server" Text="💾 Guardar Cambios" CssClass="btn btn-primary w-100 fw-bold py-2 shadow-sm" style="border-radius: 8px;" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>