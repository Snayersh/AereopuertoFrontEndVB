<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RecuperarPassword.aspx.vb" Inherits="AereopuertoFrontEndVB.RecuperarPassword" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Recuperar Contraseña - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background: #f4f7f6; height: 100vh; display: flex; align-items: center; justify-content: center; margin: 0; }
        .recover-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); width: 100%; max-width: 450px; padding: 40px; border-top: 5px solid #0d47a1; }
        .form-control { height: 50px; border-radius: 10px; }
        .btn-primary { background-color: #0d47a1; height: 50px; border-radius: 10px; font-weight: bold; border: none; }
        .btn-primary:hover { background-color: #1976d2; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="recover-card">
            <div class="text-center mb-4">
                <h3 class="fw-bold text-primary">🔐 Recuperar Contraseña</h3>
                <p class="text-muted small">Ingresa el correo con el que te registraste y te enviaremos un enlace seguro para crear una nueva contraseña.</p>
            </div>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info text-center rounded-3 mb-4">
                <asp:Label ID="lblMensaje" runat="server" CssClass="fw-bold"></asp:Label>
            </asp:Panel>

            <div class="mb-4">
                <label class="form-label fw-bold text-secondary">Correo Electrónico</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" required="true" placeholder="usuario@correo.com"></asp:TextBox>
            </div>

            <asp:Button ID="btnEnviar" runat="server" Text="Enviar Enlace de Recuperación" CssClass="btn btn-primary w-100 mb-3 shadow-sm" />

            <div class="text-center mt-3 border-top pt-3">
                <a href="Login.aspx" class="text-secondary text-decoration-none small">← Volver al inicio de sesión</a>
            </div>
        </div>
    </form>
</body>
</html>