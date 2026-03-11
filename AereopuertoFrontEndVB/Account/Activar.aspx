<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Activar.aspx.vb" Inherits="AereopuertoFrontEndVB.Activar" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Activación de Cuenta - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background: #f4f7f6; height: 100vh; display: flex; align-items: center; justify-content: center; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .activation-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); padding: 40px; width: 100%; max-width: 500px; text-align: center; border: none; }
        .icon-circle { width: 80px; height: 80px; border-radius: 50%; display: flex; align-items: center; justify-content: center; margin: 0 auto 20px auto; font-size: 2rem; }
        .success-icon { background-color: #e8f5e9; color: #2e7d32; }
        .error-icon { background-color: #ffebee; color: #c62828; }
        .btn-primary { background-color: #0d47a1; height: 45px; border-radius: 8px; font-weight: bold; border: none; transition: 0.3s; padding: 10px 30px; }
        .btn-primary:hover { background-color: #1976d2; transform: translateY(-2px); }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="activation-card">
            
            <asp:Panel ID="pnlExito" runat="server" Visible="false">
                <div class="icon-circle success-icon">✓</div>
                <h2 class="fw-bold" style="color: #2e7d32;">¡Cuenta Activada!</h2>
                <p class="text-muted mt-3 mb-4">Tu dirección de correo ha sido verificada correctamente. Ya puedes acceder a todos los servicios del aeropuerto La Aurora.</p>
                <a href="Login.aspx" class="btn btn-primary w-100 shadow-sm">Ir a Iniciar Sesión</a>
            </asp:Panel>

            <asp:Panel ID="pnlError" runat="server" Visible="false">
                <div class="icon-circle error-icon">✕</div>
                <h2 class="fw-bold" style="color: #c62828;">Enlace Inválido</h2>
                <p class="text-muted mt-3 mb-4">
                    <asp:Label ID="lblMensajeError" runat="server" Text="El enlace de activación ha expirado, es incorrecto o la cuenta ya fue activada previamente."></asp:Label>
                </p>
                <a href="Registro.aspx" class="btn btn-outline-secondary fw-bold w-100">Volver al Registro</a>
            </asp:Panel>

        </div>
    </form>
</body>
</html>