Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class BitacoraSistemas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        ' Solo Administrador (1)
        If Session("UserEmail") Is Nothing OrElse idRol <> 1 Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarLogs("", "", "")
        End If
    End Sub

    Protected Sub btnBuscarLogs_Click(sender As Object, e As EventArgs) Handles btnBuscarLogs.Click
        CargarLogs(txtPalabraClave.Text.Trim(), txtUsuario.Text.Trim(), ddlTipoEvento.SelectedValue)
    End Sub

    Private Sub CargarLogs(palabra As String, usuario As String, tipo As String)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CONSULTAR_BITACORA_LOG", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(palabra), DBNull.Value, palabra)
                    cmd.Parameters.Add("p_usuario", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(usuario), DBNull.Value, usuario)
                    cmd.Parameters.Add("p_accion", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(tipo), DBNull.Value, tipo)

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptBitacora.DataSource = dt
                        rptBitacora.DataBind()

                        If dt.Rows.Count = 0 Then
                            MostrarMensaje("No se encontraron eventos con los filtros aplicados.", False)
                        Else
                            pnlMensaje.Visible = False
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar la bitácora: " & ex.Message, False)
        End Try
    End Sub

    ' -------------------------------------------------------------
    ' CORRECCIÓN: PUBLIC para que el HTML pueda leerla
    ' -------------------------------------------------------------
    Public Function ObtenerBadgeAccion(accion As Object) As String
        If IsDBNull(accion) OrElse accion Is Nothing Then Return "badge-info"

        Select Case accion.ToString().ToUpper()
            Case "ERROR", "CRITICO", "FALLO"
                Return "badge-error"
            Case "SEGURIDAD", "ALERTA"
                Return "badge-warning"
            Case Else
                Return "badge-info"
        End Select
    End Function

    Private Sub MostrarMensaje(mensaje As String, esInfo As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esInfo, "alert alert-info fw-bold text-center mb-4", "alert alert-secondary fw-bold text-center mb-4")
    End Sub

    ' =========================================================================
    ' 🌟 FUNCIÓN GLOBAL PARA GUARDAR LOGS DESDE CUALQUIER PARTE DE TU CÓDIGO 🌟
    ' =========================================================================
    Public Shared Sub RegistrarEventoApp(modulo As String, accion As String, usuario As String, descripcion As String)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_INSERTAR_BITACORA_LOG", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_tabla_modulo", OracleDbType.Varchar2).Value = modulo
                    cmd.Parameters.Add("p_accion", OracleDbType.Varchar2).Value = accion
                    cmd.Parameters.Add("p_usuario", OracleDbType.Varchar2).Value = usuario
                    cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = descripcion

                    conn.Open()
                    cmd.ExecuteNonQuery() ' Se guarda en silencio
                End Using
            End Using
        Catch ex As Exception
            ' Como es un log, si falla no queremos tumbar la aplicación entera, se ignora silenciosamente.
        End Try
    End Sub
End Class