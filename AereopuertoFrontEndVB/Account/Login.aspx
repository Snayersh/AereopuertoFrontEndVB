<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="AereopuertoFrontEndVB.Login" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Acceso al Sistema - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { 
            background: linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5)), url('https://images.unsplash.com/photo-1436491865332-7a61a109c0f3?auto=format&fit=crop&w=1350&q=80');
            background-size: cover; background-position: center; height: 100vh; display: flex; align-items: center; justify-content: center; margin: 0; 
        }
        .login-card { 
            background: rgba(255, 255, 255, 0.95); border-radius: 20px; box-shadow: 0 15px 35px rgba(0,0,0,0.2); 
            width: 100%; max-width: 450px; padding: 40px; border: none; 
        }
        .form-control { height: 50px; border-radius: 10px; }
        .btn-primary { background-color: #0d47a1; height: 50px; border-radius: 10px; font-weight: bold; border: none; transition: 0.3s; }
        .btn-primary:hover { background-color: #1976d2; transform: translateY(-2px); }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-card">
            <div class="text-center mb-4">
                <h2 class="fw-bold" style="color: #0d47a1;">✈️ Portal de Acceso</h2>
                <p class="text-muted">Aeropuerto Internacional La Aurora</p>
            </div>

            <asp:Panel ID="pnlExito" runat="server" Visible="false" CssClass="alert alert-success text-center shadow-sm rounded-3 mb-4" style="border-left: 5px solid #2e7d32;">
                <h6 class="fw-bold mb-1" style="color: #2e7d32;">¡Registro Exitoso! 🎉</h6>
                <small class="text-dark">Tu cuenta ha sido creada. Por favor, <b>revisa tu correo electrónico</b> para activarla antes de iniciar sesión.</small>
            </asp:Panel>

            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger text-center rounded-3 mb-4">
                <asp:Label ID="lblError" runat="server" CssClass="fw-bold"></asp:Label>
            </asp:Panel>

            <div class="mb-3">
                <label class="form-label fw-bold text-secondary">Correo Electrónico</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" required="true" placeholder="usuario@correo.com"></asp:TextBox>
            </div>

            <div class="mb-4">
                <label class="form-label fw-bold text-secondary">Contraseña</label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" required="true"></asp:TextBox>
                
                <div class="mt-2 form-check">
                    <input type="checkbox" class="form-check-input" id="chkShowPass" onclick="togglePassword()">
                    <label class="form-check-label text-muted small" for="chkShowPass" style="cursor: pointer;">Mostrar contraseña</label>
                </div>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Ingresar al Sistema" CssClass="btn btn-primary w-100 mb-3 shadow-sm" />

            <div class="text-center mt-3 border-top pt-3">
                <span class="text-muted small">¿No tienes cuenta?</span>
                <a href="Registro.aspx" class="text-primary fw-bold text-decoration-none small ms-1">Regístrate aquí</a>
            </div>
            
            <div class="text-center mt-2">
                <a href="../Default.aspx" class="text-secondary text-decoration-none small">← Volver al inicio</a>
            </div>
        </div>
    </form>

    <script>
        // Script para el botón de mostrar contraseña
        function togglePassword() {
            var x = document.getElementById('<%= txtPassword.ClientID %>');
            if (x.type === "password") { x.type = "text"; } else { x.type = "password"; }
        }
    </script>
</body>
</html>