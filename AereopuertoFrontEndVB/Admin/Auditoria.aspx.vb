Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Auditoria
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Roles permitidos: 1 (Admin) o 4 (Recursos Humanos)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse idRol <> 1 Then
            Response.Redirect("~/Account/Login.aspx")
        End If
        If Not IsPostBack Then
            CargarBitacora("", "", "")
        End If
    End Sub

    Protected Sub btnFiltrar_Click(sender As Object, e As EventArgs) Handles btnFiltrar.Click
        CargarBitacora(txtTabla.Text.Trim(), txtUsuario.Text.Trim(), ddlAccion.SelectedValue)
    End Sub

    Private Sub CargarBitacora(tabla As String, usuario As String, accion As String)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CONSULTAR_AUDITORIA", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' Enviamos parámetros de filtro, si están vacíos la BD devuelve todo
                    cmd.Parameters.Add("p_tabla", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(tabla), DBNull.Value, tabla.ToUpper())
                    cmd.Parameters.Add("p_usuario", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(usuario), DBNull.Value, usuario.ToLower())
                    cmd.Parameters.Add("p_accion", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(accion), DBNull.Value, accion.ToUpper())

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptAuditoria.DataSource = dt
                        rptAuditoria.DataBind()

                        If dt.Rows.Count = 0 Then
                            MostrarMensaje("No se encontraron registros de auditoría con esos filtros.", False)
                        Else
                            pnlMensaje.Visible = False
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error de conexión: " & ex.Message, False)
        End Try
    End Sub

    ' Método auxiliar para darle color a la acción en el Frontend
    Protected Function ObtenerClaseAccion(accion As String) As String
        Select Case accion.ToUpper()
            Case "INSERT"
                Return "action-insert"
            Case "UPDATE"
                Return "action-update"
            Case "DELETE"
                Return "action-delete"
            Case Else
                Return "text-secondary fw-bold"
        End Select
    End Function

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success fw-bold text-center mb-4", "alert alert-warning fw-bold text-center mb-4")
    End Sub
End Class