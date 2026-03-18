Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class CheckIn
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("UserRole") Is Nothing OrElse (Session("UserRole").ToString() <> "Empleado" AndAlso Session("UserRole").ToString() <> "Admin") Then
            Response.Redirect("~/Default.aspx")
        End If

        If Not IsPostBack Then
            txtCodigo.Focus()
        End If
    End Sub

    Protected Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        Dim codigo As String = txtCodigo.Text.Trim().ToUpper()

        If String.IsNullOrEmpty(codigo) Then
            MostrarAlerta("Por favor, ingresa o escanea un código.", "alert-warning")
            Return
        End If

        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_BUSCAR_BOLETO_CHECKIN", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo_boleto", OracleDbType.Varchar2).Value = codigo

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            Dim row As DataRow = dt.Rows(0)
                            Dim estadoId As Integer = Convert.ToInt32(row("id_estado_boleto"))

                            ' Llenar datos visuales
                            lblPasajero.Text = row("pasajero").ToString()
                            lblVuelo.Text = row("codigo_vuelo").ToString()
                            lblAsiento.Text = row("asiento").ToString()
                            lblRuta.Text = row("origen_iata").ToString() & " ✈️ " & row("destino_iata").ToString()

                            pnlResultado.Visible = True

                            ' Lógica de validación según el estado
                            If estadoId = 2 Then ' PAGADO (Listo para abordar)
                                MostrarAlerta("Boleto válido. Listo para abordar.", "alert-success")
                                btnConfirmar.Visible = True
                            ElseIf estadoId = 4 Then ' ABORDADO
                                MostrarAlerta("¡ALERTA! Este pasajero ya registró su abordaje.", "alert-danger")
                                btnConfirmar.Visible = False
                            ElseIf estadoId = 3 Then ' CANCELADO
                                MostrarAlerta("Boleto cancelado. No puede abordar.", "alert-danger")
                                btnConfirmar.Visible = False
                            Else ' RESERVADO (No pagado)
                                MostrarAlerta("El boleto aún no ha sido pagado.", "alert-warning")
                                btnConfirmar.Visible = False
                            End If

                        Else
                            pnlResultado.Visible = False
                            MostrarAlerta("Código de boleto no encontrado en el sistema.", "alert-danger")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            pnlResultado.Visible = False
            MostrarAlerta("Error del sistema: " & ex.Message, "alert-danger")
        End Try

        ' Retornar el foco por si quieren escanear de nuevo
        txtCodigo.Focus()
    End Sub

    Protected Sub btnConfirmar_Click(sender As Object, e As EventArgs) Handles btnConfirmar.Click
        Dim codigo As String = txtCodigo.Text.Trim().ToUpper()
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CONFIRMAR_ABORDAJE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo_boleto", OracleDbType.Varchar2).Value = codigo

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim res As String = outResultado.Value.ToString()

                    If res = "EXITO" Then
                        MostrarAlerta("¡Ingreso autorizado con éxito!", "alert-success")
                        btnConfirmar.Visible = False
                        txtCodigo.Text = "" ' Limpiamos para el siguiente pasajero
                    Else
                        MostrarAlerta("Error de validación: " & res, "alert-danger")
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarAlerta("Error al confirmar: " & ex.Message, "alert-danger")
        End Try

        txtCodigo.Focus()
    End Sub

    Private Sub MostrarAlerta(mensaje As String, claseCss As String)
        pnlMensaje.Visible = True
        pnlMensaje.CssClass = "alert text-center fw-bold fs-5 rounded-3 mb-4 " & claseCss
        lblMensaje.Text = mensaje
    End Sub
End Class