Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class MisBoletos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Seguridad: Validamos que haya una sesión iniciada
        If Session("UserEmail") Is Nothing Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarMisBoletos()
        End If
    End Sub

    Private Sub CargarMisBoletos()
        Dim db As New ConexionDB()
        Dim correoUsuario As String = Session("UserEmail").ToString()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_MIS_BOLETOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' 1. Parámetro de Entrada: El correo del usuario logueado
                    cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = correoUsuario

                    ' 2. Parámetro de Salida: El Cursor con las filas
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()

                    ' 3. Leemos los datos hacia un DataTable
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        ' 4. Lógica de presentación
                        If dt.Rows.Count > 0 Then
                            rptBoletos.DataSource = dt
                            rptBoletos.DataBind()

                            pnlVacio.Visible = False
                            rptBoletos.Visible = True
                        Else
                            ' Si no ha comprado nada, mostramos el mensaje amigable
                            pnlVacio.Visible = True
                            rptBoletos.Visible = False
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            pnlError.Visible = True
            lblError.Text = "No se pudieron cargar tus viajes: " & ex.Message
            rptBoletos.Visible = False
        End Try
    End Sub
    ' =================================================================
    ' EVENTO: CUANDO ALGUIEN HACE CLIC EN UN BOTÓN DENTRO DEL REPEATER
    ' =================================================================
    Protected Sub rptBoletos_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptBoletos.ItemCommand
        If e.CommandName = "CancelarReserva" Then
            ' Atrapamos el código de la reserva que enviamos desde el HTML
            Dim codigoReserva As String = e.CommandArgument.ToString()
            ProcesarCancelacion(codigoReserva)
        End If
    End Sub

    Private Sub ProcesarCancelacion(codigoReserva As String)
        Dim db As New ConexionDB()
        Dim correoUsuario As String = Session("UserEmail").ToString()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CANCELAR_RESERVA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo_reserva", OracleDbType.Varchar2).Value = codigoReserva
                    cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = correoUsuario

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        ' Recargamos la lista para que el boleto ahora aparezca como "Cancelado"
                        CargarMisBoletos()
                    Else
                        pnlError.Visible = True
                        lblError.Text = "No se pudo cancelar: " & resultado
                    End If
                End Using
            End Using
        Catch ex As Exception
            pnlError.Visible = True
            lblError.Text = "Error de conexión al cancelar: " & ex.Message
        End Try
    End Sub

End Class