<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Usuarios.aspx.vb" Inherits="AereopuertoFrontEndVB.Usuarios" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Gestión de Usuarios - Admin</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background-color: #f8f9fa; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .top-bar { background-color: #212529; color: white; padding: 15px 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .admin-card { background: white; border-radius: 12px; box-shadow: 0 5px 20px rgba(0,0,0,0.05); padding: 30px; border-top: 5px solid #0d6efd; margin-top: 30px; }
        .badge-admin { background-color: #dc3545; color: white; }
        .badge-empleado { background-color: #0d6efd; color: white; }
        .badge-cliente { background-color: #198754; color: white; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top-bar d-flex justify-content-between align-items-center">
            <h4 class="m-0 fw-bold">⚙️ Panel de Control - Administración</h4>
            <a href="../Default.aspx" class="btn btn-outline-light btn-sm fw-bold rounded-pill px-4">← Volver al Portal</a>
        </div>

        <div class="container-fluid py-4 px-md-5">
            <div class="row">
                <div class="col-lg-8">
                    <div class="admin-card">
                        <h4 class="fw-bold mb-4 text-dark">Directorio de Usuarios</h4>
                        
                        <asp:Panel ID="pnlExito" runat="server" Visible="false" CssClass="alert alert-success fw-bold">
                            <asp:Label ID="lblExito" runat="server"></asp:Label>
                        </asp:Panel>
                        
                        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger fw-bold">
                            <asp:Label ID="lblError" runat="server"></asp:Label>
                        </asp:Panel>

                        <div class="table-responsive">
                           <table class="table table-hover align-middle">
    <thead class="table-dark">
        <tr>
            <th>ID</th>
            <th>Nombre Completo</th>
            <th>Correo (Usuario)</th>
            <th>Rol</th>
            <th>Estado</th> <th>Acciones</th>
        </tr>
    </thead>
    <tbody>
        <asp:Repeater ID="rptUsuarios" runat="server">
            <ItemTemplate>
                <tr>
                    <td class="fw-bold text-secondary">#<%# Eval("id_usuario") %></td>
                    <td class="fw-bold"><%# Eval("nombre_completo") %></td>
                    <td><%# Eval("correo") %></td>
                    <td>
                        <span class='badge <%# If(Eval("nombre_rol").ToString() = "Administrador", "badge-admin", If(Eval("nombre_rol").ToString() = "Empleado", "badge-empleado", "badge-cliente")) %> px-3 py-2'>
                            <%# Eval("nombre_rol") %>
                        </span>
                    </td>
                    
                    <td>
                        <span class='badge <%# If(Eval("estado").ToString().ToUpper() = "ACTIVO", "bg-success", "bg-danger") %>'>
                            <%# Eval("estado") %>
                        </span>
                    </td>
                    
                    <td>
                        <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn btn-sm btn-outline-primary fw-bold mb-1" 
                            CommandName="EditarRol" 
                            CommandArgument='<%# Eval("id_usuario") & "|" & Eval("nombre_completo") & "|" & Eval("id_rol") %>'>
                            ✏️ Rol
                        </asp:LinkButton>
                        
                        <asp:LinkButton ID="btnToggleEstado" runat="server" 
                            CssClass='<%# If(Eval("estado").ToString().ToUpper() = "ACTIVO", "btn btn-sm btn-outline-danger fw-bold mb-1", "btn btn-sm btn-outline-success fw-bold mb-1") %>' 
                            CommandName="ToggleEstado" 
                            CommandArgument='<%# Eval("id_usuario") & "|" & Eval("estado") %>'
                            OnClientClick='<%# "return confirm(""¿Estás seguro que deseas " & If(Eval("estado").ToString().ToUpper() = "ACTIVO", "DESACTIVAR", "ACTIVAR") & " a este usuario?"");" %>'>
                            <%# If(Eval("estado").ToString().ToUpper() = "ACTIVO", "🚫 Desactivar", "✅ Activar") %>
                        </asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>
</table>
                        </div>
                    </div>
                </div>

                <div class="col-lg-4 mt-4 mt-lg-0">
                    <asp:Panel ID="pnlEditarRol" runat="server" Visible="false" CssClass="admin-card border-top-0" style="border-top: 5px solid #ffc107 !important;">
                        <h5 class="fw-bold text-warning text-darken mb-3">Actualizar Permisos</h5>
                        <p class="text-muted small">Estás modificando los accesos de:</p>
                        <h4 class="fw-bold text-primary mb-4"><asp:Label ID="lblNombreEdicion" runat="server"></asp:Label></h4>

                        <asp:HiddenField ID="hfUsuarioEditando" runat="server" />

                        <div class="mb-4">
                            <label class="form-label fw-bold">Asignar Nuevo Rol</label>
                            <asp:DropDownList ID="ddlRoles" runat="server" CssClass="form-select form-select-lg shadow-sm"></asp:DropDownList>
                        </div>

                        <div class="d-grid gap-2">
                            <asp:Button ID="btnGuardarCambios" runat="server" Text="💾 Guardar Cambios" CssClass="btn btn-warning fw-bold text-dark btn-lg shadow-sm" />
                            <asp:Button ID="btnCancelarEdicion" runat="server" Text="Cancelar" CssClass="btn btn-light fw-bold text-secondary" />
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </form>
</body>
</html>