Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class AuditoriaVuelos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Solo Administradores (1) y Operaciones/Torre de Control (3)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 1 AndAlso idRol <> 3) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarHistorialVuelos()
        End If
    End Sub

    Private Sub CargarHistorialVuelos()
        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_HISTORIAL_VUELOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptHistorialVuelos.DataSource = dt
                        rptHistorialVuelos.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            pnlMensaje.Visible = True
            lblMensaje.Text = "Error al conectar con el radar: " & ex.Message
        End Try
    End Sub

    ' =================================================================
    ' EVENTO: Colorear el estado del vuelo según la fase operativa
    ' =================================================================
    Protected Sub rptHistorialVuelos_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptHistorialVuelos.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then

            Dim lblEstadoNuevo As Label = CType(e.Item.FindControl("lblEstadoNuevo"), Label)
            Dim estadoNuevo As String = DataBinder.Eval(e.Item.DataItem, "ESTADO_NUEVO").ToString().ToUpper()

            lblEstadoNuevo.Text = estadoNuevo

            ' Lógica aeronáutica para los colores
            Select Case estadoNuevo
                Case "ATERRIZÓ", "COMPLETADO"
                    lblEstadoNuevo.CssClass = "badge bg-success badge-vuelo"
                Case "EN VUELO", "VOLANDO", "DESPEGÓ"
                    lblEstadoNuevo.CssClass = "badge bg-primary badge-vuelo"
                Case "ABORDANDO", "EN PUERTA"
                    lblEstadoNuevo.CssClass = "badge bg-info text-dark badge-vuelo"
                Case "RETRASADO", "EN ESPERA"
                    lblEstadoNuevo.CssClass = "badge bg-warning text-dark badge-vuelo"
                Case "CANCELADO", "DESVIADO"
                    lblEstadoNuevo.CssClass = "badge bg-danger badge-vuelo"
                Case Else
                    lblEstadoNuevo.CssClass = "badge bg-dark badge-vuelo" ' Por defecto
            End Select

        End If
    End Sub

End Class