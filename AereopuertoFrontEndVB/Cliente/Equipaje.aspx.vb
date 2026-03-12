Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Equipaje
    Inherits System.Web.UI.Page

    Private CorreoUsuario As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("UserRole") Is Nothing Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        CorreoUsuario = Session("UserEmail").ToString()

        If Not IsPostBack Then
            CargarBoletos()
        End If
    End Sub

    ' =========================================================
    ' CARGAR BOLETOS DEL USUARIO EN EL DROPDOWN
    ' =========================================================
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

    ' =========================================================
    ' EVENTO: AL SELECCIONAR UN BOLETO, MOSTRAR SU EQUIPAJE
    ' =========================================================
    Protected Sub ddlBoletos_SelectedIndexChanged(sender As Object, e As EventArgs)
        If String.IsNullOrEmpty(ddlBoletos.SelectedValue) Then
            pnlGestionEquipaje.Visible = False
            Return
        End If

        pnlGestionEquipaje.Visible = True
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
            MostrarMensaje("Error al cargar la lista de equipaje: " & ex.Message, False)
        End Try
    End Sub

    ' =========================================================
    ' EVENTO: BOTÓN DE REGISTRAR MALETA
    ' =========================================================
    Protected Sub btnRegistrarEquipaje_Click(sender As Object, e As EventArgs) Handles btnRegistrarEquipaje.Click
        Dim codigoBoleto As String = ddlBoletos.SelectedValue
        Dim peso As String = txtPeso.Text.Trim()
        Dim descripcion As String = txtDescripcion.Text.Trim()

        If String.IsNullOrEmpty(codigoBoleto) Then
            MostrarMensaje("Primero debes seleccionar un boleto.", False)
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

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        MostrarMensaje("¡Equipaje registrado exitosamente!", True)
                        txtPeso.Text = ""
                        txtDescripcion.Text = ""
                        CargarListaEquipaje(codigoBoleto) ' Recargamos la tablita para ver la nueva maleta
                    Else
                        MostrarMensaje("No se pudo registrar: " & resultado, False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error de conexión al registrar: " & ex.Message, False)
        End Try
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