Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class GestionReservas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("UserRole") Is Nothing OrElse (Session("UserRole").ToString() <> "Empleado" AndAlso Session("UserRole").ToString() <> "Admin") Then
            Response.Redirect("~/Default.aspx")
        End If



        If Not IsPostBack Then
            pnlMensaje.Visible = False
            CargarTodasLasReservas()
        End If
    End Sub

    Private Sub CargarTodasLasReservas()
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_TODAS_RESERVAS", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()

                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptReservas.DataSource = dt
                            rptReservas.DataBind()

                            pnlVacio.Visible = False
                            rptReservas.Visible = True
                        Else
                            pnlVacio.Visible = True
                            rptReservas.Visible = False
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            pnlMensaje.Visible = True
            pnlMensaje.CssClass = "alert alert-danger text-center rounded-3 mb-4"
            lblMensaje.Text = "Error al cargar los datos: " & ex.Message
            rptReservas.Visible = False
        End Try
    End Sub

    ' -------------------------------------------------------------
    ' FUNCIÓN PARA PINTAR LOS ESTADOS EN LA TABLA HTML
    ' -------------------------------------------------------------
    Public Function ObtenerClaseEstado(estado As String) As String
        Select Case estado.ToUpper()
            Case "PAGADO"
                Return "badge-pagado"
            Case "CANCELADO"
                Return "badge-cancelado"
            Case Else
                Return "badge-reservado"
        End Select
    End Function

End Class