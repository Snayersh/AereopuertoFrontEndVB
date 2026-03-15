Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class MisBoletos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 1. Verificación de Seguridad
        If Session("UserEmail") Is Nothing Then
            Response.Redirect("~/Account/Login.aspx")
            Return
        End If

        If Not IsPostBack Then
            ' La primera vez carga con el valor por defecto del DropDown (ID 1 - Pendientes)
            CargarDataBoletos(ddlFiltroEstado.SelectedValue)
        End If
    End Sub

    ' Evento del DropDown: Al cambiar el filtro, recarga la lista
    Protected Sub ddlFiltroEstado_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlFiltroEstado.SelectedIndexChanged
        CargarDataBoletos(ddlFiltroEstado.SelectedValue)
    End Sub

    ' =================================================================
    ' FUNCIÓN UNIFICADA PARA CARGAR BOLETOS (CON FILTRO)
    ' =================================================================
    Private Sub CargarDataBoletos(idEstadoFiltro As String)
        Dim db As New ConexionDB()
        Dim correoUsuario As String = Session("UserEmail").ToString()

        ' Limpiamos mensajes de error previos
        pnlError.Visible = False

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                ' Usamos el nuevo SP que creamos con filtro
                Using cmd As New OracleCommand("SP_CONSULTAR_MIS_BOLETOS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True ' Muy importante para evitar errores de orden de parámetros

                    ' Agregamos los parámetros EXACTOS que definimos en el SP de Oracle
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correoUsuario
                    cmd.Parameters.Add("p_id_estado_filtro", OracleDbType.Int32).Value = Convert.ToInt32(idEstadoFiltro)

                    ' Parámetro de salida (CURSOR)
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()

                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptBoletos.DataSource = dt
                            rptBoletos.DataBind()
                            rptBoletos.Visible = True
                            pnlVacio.Visible = False
                        Else
                            rptBoletos.Visible = False
                            pnlVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            pnlError.Visible = True
            lblError.Text = "Error al obtener tus vuelos: " & ex.Message
        End Try
    End Sub

    ' =================================================================
    ' MANEJO DE BOTONES (CANCELAR)
    ' =================================================================
    Protected Sub rptBoletos_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptBoletos.ItemCommand
        If e.CommandName = "CancelarReserva" Then
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

                    If outResultado.Value.ToString() = "EXITO" Then
                        ' REFRESCAMOS la lista usando el mismo filtro que está seleccionado
                        CargarDataBoletos(ddlFiltroEstado.SelectedValue)
                    Else
                        pnlError.Visible = True
                        lblError.Text = "No se pudo cancelar: " & outResultado.Value.ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            pnlError.Visible = True
            lblError.Text = "Error al procesar cancelación: " & ex.Message
        End Try
    End Sub
End Class