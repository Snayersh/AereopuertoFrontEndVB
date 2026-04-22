Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class GestionPuertas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        ' Seguridad: Solo Admin (1) o Empleado (3)
        If Session("UserEmail") Is Nothing OrElse (idRol <> 1 AndAlso idRol <> 3) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarAeropuertos()
            CargarPuertas()
        End If
    End Sub

    Private Sub CargarPuertas()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_PUERTAS_ADMIN", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)
                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptPuertas.DataSource = dt
                        rptPuertas.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar puertas: " & ex.Message, False)
        End Try
    End Sub

    ' Lógica para pintar los colores de estado sin errores de protección
    Protected Sub rptPuertas_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPuertas.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim estado As String = DataBinder.Eval(e.Item.DataItem, "ESTADO").ToString().ToUpper()
            Dim lblBadge As Label = CType(e.Item.FindControl("lblBadgeEstado"), Label)

            lblBadge.Text = estado
            Select Case estado
                Case "DISPONIBLE" : lblBadge.CssClass = "badge-disponible"
                Case "OCUPADA" : lblBadge.CssClass = "badge-ocupada"
                Case "MANTENIMIENTO" : lblBadge.CssClass = "badge-mantenimiento"
                Case Else : lblBadge.CssClass = "badge bg-secondary text-white"
            End Select
        End If
    End Sub

    ' ... Métodos de CargarAeropuertos y btnGuardar_Click (Siguiendo el mismo patrón de las otras páginas) ...

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success fw-bold rounded-3 mb-4", "alert alert-danger fw-bold rounded-3 mb-4")
    End Sub
End Class