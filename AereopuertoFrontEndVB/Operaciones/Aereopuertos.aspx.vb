Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Aeropuertos
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
            ' Convertimos los textos de coordenadas a Decimal de forma súper segura
            Dim latitud As Decimal = 0
            Dim longitud As Decimal = 0

            ' Intentamos con la cultura actual, si falla forzamos el reemplazo de punto por coma
            If Not Decimal.TryParse(txtLatitud.Text.Trim().Replace(".", ","), latitud) Then
                Decimal.TryParse(txtLatitud.Text.Trim().Replace(",", "."), latitud)
            End If

            If Not Decimal.TryParse(txtLongitud.Text.Trim().Replace(".", ","), longitud) Then
                Decimal.TryParse(txtLongitud.Text.Trim().Replace(",", "."), longitud)
            End If

            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_INSERTAR_AEROPUERTO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    ' Parámetros de Entrada
                    cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = txtNombre.Text.Trim()
                    cmd.Parameters.Add("p_codigo_iata", OracleDbType.Varchar2).Value = txtIata.Text.Trim().ToUpper()
                    cmd.Parameters.Add("p_pais", OracleDbType.Varchar2).Value = txtPais.Text.Trim()
                    cmd.Parameters.Add("p_ciudad", OracleDbType.Varchar2).Value = txtCiudad.Text.Trim()
                    cmd.Parameters.Add("p_latitud", OracleDbType.Decimal).Value = latitud
                    cmd.Parameters.Add("p_longitud", OracleDbType.Decimal).Value = longitud

                    ' Parámetro de Salida
                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        MostrarMensaje("¡Aeropuerto registrado exitosamente con sus datos de geolocalización!", True)
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
        txtCiudad.Text = ""
        txtLatitud.Text = ""
        txtLongitud.Text = ""
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center rounded-3 mb-4", "alert alert-danger text-center rounded-3 mb-4")
    End Sub
End Class