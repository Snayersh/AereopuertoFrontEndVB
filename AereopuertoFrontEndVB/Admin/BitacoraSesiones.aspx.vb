Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class BitacoraSesiones
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Solo Administradores (Asumimos Rol 1 y Rol 4 según tus configuraciones previas)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 1 AndAlso idRol <> 4) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarBitacora()
        End If
    End Sub

    ' Botón para refrescar la tabla manualmente
    Protected Sub btnActualizar_Click(sender As Object, e As EventArgs) Handles btnActualizar.Click
        CargarBitacora()
    End Sub

    ' =======================================================
    ' CARGAR LA TABLA DE AUDITORÍA
    ' =======================================================
    Private Sub CargarBitacora()
        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_BITACORA_SESIONES", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptSesiones.DataSource = dt
                            rptSesiones.DataBind()
                            rptSesiones.Visible = True
                            pnlVacio.Visible = False
                        Else
                            rptSesiones.Visible = False
                            pnlVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar la bitácora: " & ex.Message)
        End Try
    End Sub

    ' =======================================================
    ' FORMATEO DE FECHAS Y ESTADOS (SIN ERRORES HTML)
    ' =======================================================
    Protected Sub rptSesiones_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptSesiones.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then

            ' 1. Formatear Fecha Inicio
            Dim fechaInicioObj = DataBinder.Eval(e.Item.DataItem, "FECHA_INICIO")
            Dim lblFechaInicio As Label = CType(e.Item.FindControl("lblFechaInicio"), Label)
            If Not IsDBNull(fechaInicioObj) Then
                lblFechaInicio.Text = Convert.ToDateTime(fechaInicioObj).ToString("dd MMM yyyy - HH:mm:ss")
            End If

            ' 2. Formatear Fecha Fin y determinar el Estado de la sesión
            Dim fechaFinObj = DataBinder.Eval(e.Item.DataItem, "FECHA_FIN")
            Dim lblFechaFin As Label = CType(e.Item.FindControl("lblFechaFin"), Label)
            Dim lblBadgeEstado As Label = CType(e.Item.FindControl("lblBadgeEstado"), Label)

            ' Si la fecha fin es Nula, significa que el usuario sigue logueado
            If IsDBNull(fechaFinObj) Then
                lblFechaFin.Text = "<span class='text-success fst-italic'>En línea ahora...</span>"

                lblBadgeEstado.Text = "🟢 ACTIVA"
                lblBadgeEstado.CssClass = "badge-activa shadow-sm"
            Else
                ' Si ya tiene fecha fin, la sesión terminó
                lblFechaFin.Text = Convert.ToDateTime(fechaFinObj).ToString("dd MMM yyyy - HH:mm:ss")

                lblBadgeEstado.Text = "⚪ CERRADA"
                lblBadgeEstado.CssClass = "badge-cerrada shadow-sm"
            End If

        End If
    End Sub

    Private Sub MostrarMensaje(mensaje As String)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = "alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm"
    End Sub
End Class