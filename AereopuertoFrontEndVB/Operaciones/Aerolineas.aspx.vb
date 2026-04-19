Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Aerolineas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 3) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            pnlMensaje.Visible = False
        End If
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_INSERTAR_AEROLINEA", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' Parámetros de Entrada
                    cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = txtNombre.Text.Trim()
                    cmd.Parameters.Add("p_codigo_iata", OracleDbType.Varchar2).Value = txtIata.Text.Trim()
                    cmd.Parameters.Add("p_pais_origen", OracleDbType.Varchar2).Value = txtPais.Text.Trim()

                    ' Parámetro de Salida
                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        MostrarMensaje("¡Aerolínea registrada con éxito!", True)
                        LimpiarCampos()
                    Else
                        MostrarMensaje("Error en base de datos: " & resultado, False)
                    End If
                End Using
            End Using

        Catch ex As Exception
            MostrarMensaje("Error de conexión: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        LimpiarCampos()
        pnlMensaje.Visible = False
    End Sub

    Private Sub LimpiarCampos()
        txtNombre.Text = ""
        txtIata.Text = ""
        txtPais.Text = ""
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        If esExito Then
            pnlMensaje.CssClass = "alert alert-success text-center rounded-3 mb-4"
        Else
            pnlMensaje.CssClass = "alert alert-danger text-center rounded-3 mb-4"
        End If
    End Sub

End Class