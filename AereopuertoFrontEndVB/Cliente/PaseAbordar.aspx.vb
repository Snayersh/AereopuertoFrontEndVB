Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class PaseAbordar
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("UserEmail") Is Nothing Then
            Response.Redirect("~/Account/Login.aspx")
            Return
        End If

        If Not IsPostBack Then
            Dim codigoReserva As String = Request.QueryString("codigo")

            If Not String.IsNullOrEmpty(codigoReserva) Then
                CargarDatosPase(codigoReserva)
            Else
                MostrarError("No se proporcionó un código de reserva válido.")
            End If
        End If
    End Sub

    Private Sub CargarDatosPase(codigoReserva As String)
        Dim db As New ConexionDB()
        Dim correoUsuario As String = Session("UserEmail").ToString()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_PASE_ABORDAR", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo_reserva", OracleDbType.Varchar2).Value = codigoReserva
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correoUsuario

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        ' Si encuentra boletos, simplemente los mandamos a dibujar TODOS
                        If dt.Rows.Count > 0 Then
                            rptPases.DataSource = dt
                            rptPases.DataBind()
                        Else
                            MostrarError("No se encontró el pase de abordar. Verifica que sea tuyo y esté pagado.")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarError("Error al generar el pase: " & ex.Message)
        End Try
    End Sub

    Private Sub MostrarError(mensaje As String)
        rptPases.Visible = False
        pnlError.Visible = True
        lblError.Text = mensaje
    End Sub
End Class