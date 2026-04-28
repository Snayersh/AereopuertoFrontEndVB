Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class AuditoriaPagos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Solo Administradores (1) o Finanzas/RRHH (4) pueden ver auditorías
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 1 AndAlso idRol <> 4) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarHistorial()
        End If
    End Sub

    Private Sub CargarHistorial()
        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_HISTORIAL_PAGOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptHistorial.DataSource = dt
                        rptHistorial.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            pnlMensaje.Visible = True
            lblMensaje.Text = "Error al cargar la bitácora: " & ex.Message
        End Try
    End Sub

    ' =================================================================
    ' EVENTO: Colorear el nuevo estado dinámicamente
    ' =================================================================
    Protected Sub rptHistorial_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptHistorial.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then

            Dim lblEstadoNuevo As Label = CType(e.Item.FindControl("lblEstadoNuevo"), Label)
            Dim estadoNuevo As String = DataBinder.Eval(e.Item.DataItem, "ESTADO_NUEVO").ToString().ToUpper()

            lblEstadoNuevo.Text = estadoNuevo

            ' Lógica de colores para auditoría
            If estadoNuevo = "PAGADO" OrElse estadoNuevo = "APROBADO" Then
                lblEstadoNuevo.CssClass = "badge bg-success badge-estado"
            ElseIf estadoNuevo = "ANULADO" OrElse estadoNuevo = "RECHAZADO" Then
                lblEstadoNuevo.CssClass = "badge bg-danger badge-estado"
            Else
                lblEstadoNuevo.CssClass = "badge bg-warning text-dark badge-estado"
            End If

        End If
    End Sub

End Class