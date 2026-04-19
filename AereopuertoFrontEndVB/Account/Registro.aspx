<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Registro.aspx.vb" Inherits="AereopuertoFrontEndVB.Registro" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Crear Cuenta - La Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background: #f4f7f6; padding: 40px 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .registro-card { background: white; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.08); padding: 40px; width: 100%; max-width: 900px; margin: 0 auto; border: none; }
        .form-control { height: 45px; border-radius: 8px; border: 1px solid #dee2e6; }
        .btn-primary { background-color: #0d47a1; height: 50px; border-radius: 10px; font-weight: bold; border: none; transition: 0.3s; }
        .btn-primary:hover { background-color: #1976d2; transform: translateY(-2px); }
        .section-title { color: #0d47a1; font-weight: bold; border-bottom: 2px solid #e3f2fd; padding-bottom: 5px; margin-bottom: 20px; margin-top: 20px; }
        
        /* Clase para cuando el botón está deshabilitado */
        .btn-disabled { opacity: 0.7; cursor: not-allowed; pointer-events: none; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="registro-card">
                <div class="text-center mb-4">
                    <h2 class="fw-bold" style="color: #0d47a1;">✈️ Registro de Pasajero</h2>
                    <p class="text-muted">Completa tu perfil para acceder a los servicios de La Aurora</p>
                </div>

                <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info text-center rounded-3 mb-4 shadow-sm">
                    <asp:Label ID="lblMensaje" runat="server" CssClass="fw-bold"></asp:Label>
                </asp:Panel>

                <h5 class="section-title">Datos Personales</h5>
                <div class="row">
                    <div class="col-md-4 mb-3">
                        <label class="form-label text-secondary small fw-bold">Primer Nombre *</label>
                        <asp:TextBox ID="txtPrimerNombre" runat="server" CssClass="form-control" required="true"></asp:TextBox>
                    </div>
                    <div class="col-md-4 mb-3">
                        <label class="form-label text-secondary small fw-bold">Segundo Nombre</label>
                        <asp:TextBox ID="txtSegundoNombre" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-4 mb-3">
                        <label class="form-label text-secondary small fw-bold">Tercer Nombre</label>
                        <asp:TextBox ID="txtTercerNombre" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4 mb-3">
                        <label class="form-label text-secondary small fw-bold">Primer Apellido *</label>
                        <asp:TextBox ID="txtPrimerApellido" runat="server" CssClass="form-control" required="true"></asp:TextBox>
                    </div>
                    <div class="col-md-4 mb-3">
                        <label class="form-label text-secondary small fw-bold">Segundo Apellido</label>
                        <asp:TextBox ID="txtSegundoApellido" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-4 mb-3">
                        <label class="form-label text-secondary small fw-bold">Apellido de Casada</label>
                        <asp:TextBox ID="txtApellidoCasada" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6 mb-3">
                        <label class="form-label text-secondary small fw-bold">Fecha de Nacimiento *</label>
                        <asp:TextBox ID="txtFechaNac" runat="server" CssClass="form-control" TextMode="Date" required="true"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-label text-secondary small fw-bold">Teléfono *</label>
                        <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" required="true"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-label text-secondary small fw-bold">No. Pasaporte *</label>
                        <asp:TextBox ID="txtPasaporte" runat="server" CssClass="form-control" placeholder="Ej: P-123456" required="true"></asp:TextBox>
                    </div>
                </div>

                <h5 class="section-title">Dirección de Residencia</h5>
                <div class="row">
                    <div class="col-md-4 mb-3">
                        <label class="form-label text-secondary small fw-bold">País</label>
                        <asp:TextBox ID="txtPais" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-4 mb-3">
                        <label class="form-label text-secondary small fw-bold">Departamento / Estado</label>
                        <asp:TextBox ID="txtDepartamento" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-4 mb-3">
                        <label class="form-label text-secondary small fw-bold">Municipio / Ciudad</label>
                        <asp:TextBox ID="txtMunicipio" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-3 mb-3">
                        <label class="form-label text-secondary small fw-bold">Zona</label>
                        <asp:TextBox ID="txtZona" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-label text-secondary small fw-bold">Colonia</label>
                        <asp:TextBox ID="txtColonia" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-label text-secondary small fw-bold">Calle / Avenida</label>
                        <asp:TextBox ID="txtCalleAvenida" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-label text-secondary small fw-bold">No. Casa</label>
                        <asp:TextBox ID="txtNumCasa" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>

                <h5 class="section-title">Datos de Acceso</h5>
                <div class="row">
                    <div class="col-md-6 mb-3">
                        <label class="form-label text-secondary small fw-bold">Correo Electrónico *</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" required="true"></asp:TextBox>
                    </div>
                    <div class="col-md-6 mb-4">
                        <label class="form-label text-secondary small fw-bold">Contraseña *</label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" required="true"></asp:TextBox>
                    </div>
                </div>

                <asp:Button ID="btnRegistrar" runat="server" Text="Crear Cuenta" 
                    CssClass="btn btn-primary w-100 mb-3 shadow-sm" 
                    UseSubmitBehavior="false" 
                    OnClientClick="bloquearBoton(this);" />

                <div class="text-center mt-3">
                    <span class="text-muted">¿Ya tienes cuenta?</span>
                    <a href="Login.aspx" class="text-primary fw-bold text-decoration-none ms-1">Inicia sesión aquí</a>
                </div>
            </div>
        </div>
    </form>

    <script>
        function bloquearBoton(btn) {
            // Solo lo bloquea si el formulario es válido (HTML5 required check)
            if (typeof (Page_ClientValidate) == 'function') {
                if (Page_ClientValidate() == false) { return false; }
            }
            if (document.getElementById('form1').checkValidity()) {
                btn.value = 'Procesando registro, por favor espere... ⏳';
                btn.classList.add('btn-disabled');
                return true;
            }
            return false;
        }

        document.addEventListener("DOMContentLoaded", function () {
            var hoy = new Date();
            var anioMaximo = hoy.getFullYear() - 18;
            var mes = ("0" + (hoy.getMonth() + 1)).slice(-2);
            var dia = ("0" + hoy.getDate()).slice(-2);
            var fechaMax = anioMaximo + "-" + mes + "-" + dia;

            // Asignamos la fecha máxima al input de Fecha de Nacimiento
            document.getElementById('<%= txtFechaNac.ClientID %>').setAttribute("max", fechaMax);
        });
    </script>
</body>
</html>