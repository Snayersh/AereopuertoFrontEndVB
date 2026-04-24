Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Equipaje
    Inherits System.Web.UI.Page

    Private CorreoUsuario As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 2) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        CorreoUsuario = Session("UserEmail").ToString()

        If Not IsPostBack Then
            CargarBoletos()
            CargarTiposEquipaje()
        End If
    End Sub

    Private Sub CargarTiposEquipaje()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_TIPOS_EQUIPAJE_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cur As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cur.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cur)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlTipoEquipaje.DataSource = reader
                        ddlTipoEquipaje.DataTextField = "NOMBRE"
                        ddlTipoEquipaje.DataValueField = "ID_TIPO_EQUIPAJE"
                        ddlTipoEquipaje.DataBind()
                    End Using
                End Using
            End Using
            ddlTipoEquipaje.Items.Insert(0, New ListItem("-- Selecciona el Tipo --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar los tipos de equipaje: " & ex.Message, False)
        End Try
    End Sub

    Private Sub CargarBoletos()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CARGAR_BOLETOS_EQUIPAJE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_correo_usuario", OracleDbType.Varchar2).Value = CorreoUsuario

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlBoletos.DataSource = reader
                        ddlBoletos.DataTextField = "descripcion_boleto"
                        ddlBoletos.DataValueField = "codigo_boleto"
                        ddlBoletos.DataBind()
                    End Using
                End Using
            End Using

            ddlBoletos.Items.Insert(0, New ListItem("-- Selecciona una de tus reservas --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar reservas: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub ddlBoletos_SelectedIndexChanged(sender As Object, e As EventArgs)
        If String.IsNullOrEmpty(ddlBoletos.SelectedValue) Then
            pnlGestionEquipaje.Visible = False
            Return
        End If

        pnlGestionEquipaje.Visible = True
        pnlTracking.Visible = False ' Ocultamos tracking al cambiar de vuelo
        CargarListaEquipaje(ddlBoletos.SelectedValue)
    End Sub

    Private Sub CargarListaEquipaje(codigoBoleto As String)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_EQUIPAJE_BOLETO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo_boleto", OracleDbType.Varchar2).Value = codigoBoleto

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptEquipaje.DataSource = dt
                            rptEquipaje.DataBind()
                            rptEquipaje.Visible = True
                            pnlVacio.Visible = False
                        Else
                            rptEquipaje.Visible = False
                            pnlVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar la lista: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnRegistrarEquipaje_Click(sender As Object, e As EventArgs) Handles btnRegistrarEquipaje.Click
        Dim codigoBoleto As String = ddlBoletos.SelectedValue
        Dim tipoEquipaje As String = ddlTipoEquipaje.SelectedValue
        Dim peso As String = txtPeso.Text.Trim()
        Dim descripcion As String = txtDescripcion.Text.Trim()

        If String.IsNullOrEmpty(codigoBoleto) OrElse String.IsNullOrEmpty(tipoEquipaje) Then
            MostrarMensaje("Por favor completa todos los campos obligatorios.", False)
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_EQUIPAJE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_codigo_boleto", OracleDbType.Varchar2).Value = codigoBoleto
                    cmd.Parameters.Add("p_peso", OracleDbType.Decimal).Value = Convert.ToDecimal(peso)
                    cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = descripcion
                    cmd.Parameters.Add("p_id_tipo_equipaje", OracleDbType.Int32).Value = Convert.ToInt32(tipoEquipaje)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado.StartsWith("EXITO") Then
                        Dim partes = resultado.Split("|"c)
                        If partes.Length > 1 Then
                            MostrarMensaje("✅ " & partes(1), True)
                        Else
                            MostrarMensaje("¡Equipaje registrado exitosamente!", True)
                        End If

                        txtPeso.Text = ""
                        txtDescripcion.Text = ""
                        ddlTipoEquipaje.SelectedIndex = 0
                        CargarListaEquipaje(codigoBoleto)
                        pnlTracking.Visible = False
                    Else
                        MostrarMensaje("No se pudo registrar: " & resultado, False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error de conexión al registrar: " & ex.Message, False)
        End Try
    End Sub

    ' ====================================================================
    ' EVENTO PARA CARGAR EL HISTORIAL DE TRACKING AL DAR CLIC EN RASTREAR
    ' ====================================================================
    Protected Sub rptEquipaje_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptEquipaje.ItemCommand
        If e.CommandName = "Rastrear" Then
            Dim idEquipaje As String = e.CommandArgument.ToString()
            CargarHistorialTracking(idEquipaje)
            pnlTracking.Visible = True
        End If
    End Sub

    Private Sub CargarHistorialTracking(idEquipaje As String)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_VER_TRACKING_EQUIPAJE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_equipaje", OracleDbType.Int32).Value = Convert.ToInt32(idEquipaje)

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptTrackingLine.DataSource = dt
                            rptTrackingLine.DataBind()
                            rptTrackingLine.Visible = True
                            pnlTrackingVacio.Visible = False
                        Else
                            rptTrackingLine.Visible = False
                            pnlTrackingVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar rastreo: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnCerrarTracking_Click(sender As Object, e As EventArgs)
        pnlTracking.Visible = False
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        If esExito Then
            pnlMensaje.CssClass = "alert alert-success text-center rounded-3 mb-4 fw-bold"
        Else
            pnlMensaje.CssClass = "alert alert-danger text-center rounded-3 mb-4 fw-bold"
        End If
    End Sub
End Class