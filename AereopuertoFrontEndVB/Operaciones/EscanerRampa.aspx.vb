Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class EscanerRampa
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 3) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarEstadosEquipaje()
            txtIdEquipaje.Focus()
        End If
    End Sub

    ' Llenar el DropDown con los datos de AUR_ESTADO_EQUIPAJE
    Private Sub CargarEstadosEquipaje()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                ' Consultamos directamente tu tabla catálogo
                Dim sql As String = "SELECT ID_ESTADO_EQUIPAJE, NOMBRE FROM AUR_ESTADO_EQUIPAJE ORDER BY ID_ESTADO_EQUIPAJE"
                Using cmd As New OracleCommand(sql, conn)
                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlEstado.DataSource = reader
                        ddlEstado.DataTextField = "NOMBRE"
                        ddlEstado.DataValueField = "ID_ESTADO_EQUIPAJE"
                        ddlEstado.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar estados: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnActualizar_Click(sender As Object, e As EventArgs) Handles btnActualizar.Click
        Dim idEquipaje As String = txtIdEquipaje.Text.Trim()
        Dim idEstadoNuevo As String = ddlEstado.SelectedValue

        If String.IsNullOrEmpty(idEquipaje) Then
            MostrarMensaje("⚠️ Escanea o ingresa el ID de la maleta.", False)
            txtIdEquipaje.Focus()
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ACTUALIZAR_ESTADO_MALETA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_equipaje", OracleDbType.Int32).Value = Convert.ToInt32(idEquipaje)
                    cmd.Parameters.Add("p_id_estado", OracleDbType.Int32).Value = Convert.ToInt32(idEstadoNuevo)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        MostrarMensaje("✅ ¡Estado actualizado correctamente!", True)
                        txtIdEquipaje.Text = "" ' Limpiar para escanear la siguiente maleta
                    Else
                        MostrarMensaje("⚠️ Error: " & resultado, False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error interno: " & ex.Message, False)
        End Try

        txtIdEquipaje.Focus()
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        If esExito Then
            pnlMensaje.CssClass = "alert alert-success text-center fw-bold rounded-3 mb-4 fs-5"
        Else
            pnlMensaje.CssClass = "alert alert-danger text-center fw-bold rounded-3 mb-4 fs-5"
        End If
    End Sub
End Class