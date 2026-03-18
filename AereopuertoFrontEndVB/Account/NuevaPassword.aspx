<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="NuevaPassword.aspx.vb" Inherits="AereopuertoFrontEndVB.NuevaPassword" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Nueva Contraseña - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background: #f4f7f6; height: 100vh; display: flex; align-items: center; justify-content: center; margin: 0; }
        .recover-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); width: 100%; max-width: 450px; padding: 40px; border-top: 5px solid #2e7d32; }
        .form-control { height: 50px; border-radius: 10px; }
        .btn-success { background-color: #2e7d32; height: 50px; border-radius: 10px; font-weight: bold; border: none; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="recover-card">
            <div class="text-center mb-4">
                <h3 class="fw-bold text-success">🔒 Nueva Contraseña</h3>
                <p class="text-muted small">Crea una nueva contraseña para tu cuenta.</p>
            </div>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert text-center rounded-3 mb-4">
                <asp:Label ID="lblMensaje" runat="server" CssClass="fw-bold"></asp:Label>
            </asp:Panel>

            <asp:Panel ID="pnlFormulario" runat="server">
                <div class="mb-3">
                    <label class="form-label fw-bold text-secondary">Nueva Contraseña</label>
                    <asp:TextBox ID="txtPass1" runat="server" CssClass="form-control" TextMode="Password" required="true"></asp:TextBox>
                </div>

                <div class="mb-4">
                    <label class="form-label fw-bold text-secondary">Confirmar Nueva Contraseña</label>
                    <asp:TextBox ID="txtPass2" runat="server" CssClass="form-control" TextMode="Password" required="true"></asp:TextBox>
                </div>

                <asp:Button ID="btnActualizar" runat="server" Text="Actualizar Contraseña" CssClass="btn btn-success w-100 mb-3 shadow-sm" />
            </asp:Panel>

            <div class="text-center mt-3 border-top pt-3">
                <a href="Login.aspx" class="text-secondary text-decoration-none small">Ir al Login</a>
            </div>
        </div>
    </form>
</body>
</html>