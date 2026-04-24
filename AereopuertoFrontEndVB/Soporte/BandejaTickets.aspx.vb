Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class BandejaTickets
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse idRol <> 6 Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            ' 1. Cargar datos iniciales
            CargarBandejaGlobal()

            ' 2. ¡MAGIA URL! Si el agente le da clic a "Atender"
            If Request.QueryString("id") IsNot Nothing Then
                pnlListado.Visible = False
                pnlConversacion.Visible = True

                Dim idTicket As String = Request.QueryString("id").ToString()
                lblTicketIdVisor.Text = idTicket
                CargarHiloRespuestas(idTicket)
            End If
        End If
    End Sub

    ' =======================================================
    ' CARGAR LA BANDEJA PRINCIPAL DE TICKETS
    ' =======================================================
    Private Sub CargarBandejaGlobal()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_TICKETS_GLOBAL", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptTickets.DataSource = dt
                            rptTickets.DataBind()
                            rptTickets.Visible = True
                            pnlVacioTickets.Visible = False
                        Else
                            rptTickets.Visible = False
                            pnlVacioTickets.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar la bandeja: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub rptTickets_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptTickets.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim estado As String = DataBinder.Eval(e.Item.DataItem, "ESTADO").ToString().ToUpper()
            Dim lblBadge As Label = CType(e.Item.FindControl("lblBadgeEstado"), Label)

            lblBadge.Text = estado
            If estado = "ABIERTO" Then
                lblBadge.CssClass = "badge-abierto shadow-sm"
            Else
                lblBadge.CssClass = "badge-cerrado shadow-sm"
            End If
        End If
    End Sub

    ' =======================================================
    ' VISUALIZADOR DE CONVERSACIÓN (HILO)
    ' =======================================================
    Private Sub CargarHiloRespuestas(idTicket As String)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                ' Reutilizamos el mismo SP del cliente
                Using cmd As New OracleCommand("SP_LISTAR_RESPUESTAS_TICKET", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True
                    cmd.Parameters.Add("p_id_ticket", OracleDbType.Int32).Value = Convert.ToInt32(idTicket)

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptRespuestas.DataSource = dt
                            rptRespuestas.DataBind()
                            rptRespuestas.Visible = True
                            pnlVacioRespuestas.Visible = False
                        Else
                            rptRespuestas.Visible = False
                            pnlVacioRespuestas.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar la conversación: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub rptRespuestas_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptRespuestas.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim fechaObj = DataBinder.Eval(e.Item.DataItem, "FECHA")
            Dim lblFecha As Label = CType(e.Item.FindControl("lblFechaMensaje"), Label)

            If Not IsDBNull(fechaObj) Then
                lblFecha.Text = Convert.ToDateTime(fechaObj).ToString("dd MMM yyyy, HH:mm") & " hrs"
            End If
        End If
    End Sub

    ' =======================================================
    ' ACCIONES DEL AGENTE (RESPONDER Y CERRAR)
    ' =======================================================
    Protected Sub btnEnviarRespuesta_Click(sender As Object, e As EventArgs) Handles btnEnviarRespuesta.Click
        If String.IsNullOrEmpty(txtNuevaRespuesta.Text) Then
            MostrarMensaje("⚠️ No puedes enviar un mensaje vacío.", False)
            Return
        End If

        Dim idTicket As String = Request.QueryString("id").ToString()

        ' 🔥 EL TRUCO MAGISTRAL: Le pegamos el prefijo al texto para diferenciarlo del cliente
        Dim mensajeOficial As String = "🎧 Soporte Técnico: " & txtNuevaRespuesta.Text.Trim()

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                ' Reutilizamos el SP de inserción
                Using cmd As New OracleCommand("SP_AGREGAR_RESPUESTA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_ticket", OracleDbType.Int32).Value = Convert.ToInt32(idTicket)
                    cmd.Parameters.Add("p_mensaje", OracleDbType.Varchar2).Value = mensajeOficial

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outResultado.Value.ToString() = "EXITO" Then
                        txtNuevaRespuesta.Text = ""
                        CargarHiloRespuestas(idTicket) ' Recargar el chat
                    Else
                        MostrarMensaje("⚠️ Error al enviar: " & outResultado.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error interno: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnCerrarTicket_Click(sender As Object, e As EventArgs) Handles btnCerrarTicket.Click
        Dim idTicket As String = Request.QueryString("id").ToString()
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CERRAR_TICKET_SOPORTE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_ticket", OracleDbType.Int32).Value = Convert.ToInt32(idTicket)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outResultado.Value.ToString() = "EXITO" Then
                        ' Redirigimos a la bandeja principal limpia
                        Response.Redirect("BandejaTickets.aspx")
                    Else
                        MostrarMensaje("⚠️ Error al cerrar: " & outResultado.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error interno: " & ex.Message, False)
        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm")
    End Sub
End Class