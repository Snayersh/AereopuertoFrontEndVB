Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Pagos
    Inherits System.Web.UI.Page

    Private CorreoUsuario As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("UserRole") Is Nothing Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        CorreoUsuario = If(Session("UserEmail") IsNot Nothing, Session("UserEmail").ToString(), "")

        If Not IsPostBack Then
            pnlError.Visible = False
            pnlExito.Visible = False

            If Request.QueryString("codigo") IsNot Nothing Then
                txtCodigoReserva.Text = Request.QueryString("codigo").ToString()
            End If
        End If
    End Sub

    Protected Sub btnPagar_Click(sender As Object, e As EventArgs) Handles btnPagar.Click
        Dim codigoReserva As String = txtCodigoReserva.Text.Trim().ToUpper()

        If String.IsNullOrEmpty(codigoReserva) Then
            MostrarError("Por favor, ingresa el código de tu reserva.")
            Return
        End If

        Dim idMetodoPago As Integer = 1
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_PROCESAR_PAGO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo_reserva", OracleDbType.Varchar2).Value = codigoReserva
                    cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = CorreoUsuario
                    cmd.Parameters.Add("p_id_metodo_pago", OracleDbType.Int32).Value = idMetodoPago

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultadoCompleto As String = outResultado.Value.ToString()
                    Dim partes() As String = resultadoCompleto.Split("|"c)

                    If partes(0) = "EXITO" Then
                        pnlFormulario.Visible = False
                        pnlError.Visible = False

                        lblFactura.Text = "FAC-" & partes(1).PadLeft(6, "0"c)
                        lblLocalizadorExito.Text = codigoReserva
                        pnlExito.Visible = True
                    ElseIf partes(0) = "ERROR_RESERVA_NO_VALIDA" Then
                        MostrarError("No se encontró una reserva pendiente de pago con ese código o ya fue pagada.")
                    Else
                        MostrarError("Error en base de datos: " & resultadoCompleto)
                    End If
                End Using
            End Using

        Catch ex As Exception
            MostrarError("Error al procesar el pago: " & ex.Message)
        End Try
    End Sub

    Private Sub MostrarError(mensaje As String)
        pnlExito.Visible = False
        pnlError.Visible = True
        lblError.Text = mensaje
    End Sub
End Class