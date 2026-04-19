Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Aeronaves
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        ' Ojo: Este módulo lo asignaste al rol 3 (Operaciones). Está perfecto.
        If Session("UserEmail") Is Nothing OrElse (idRol <> 3) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            pnlMensaje.Visible = False
            CargarAerolineas() ' Llamamos a la BD para llenar el desplegable
        End If
    End Sub

    ' -------------------------------------------------------------
    ' 1. MÉTODO PARA LLENAR EL DESPLEGABLE DESDE ORACLE (100% SP)
    ' -------------------------------------------------------------
    Private Sub CargarAerolineas()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_AEROLINEAS_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlAerolineas.DataSource = reader
                        ' Lo que el usuario lee
                        ddlAerolineas.DataTextField = "NOMBRE"
                        ' Lo que se guarda internamente (El ID)
                        ddlAerolineas.DataValueField = "ID_AEROLINEA"
                        ddlAerolineas.DataBind()
                    End Using
                End Using
            End Using

            ' Agregamos una opción por defecto al inicio de la lista
            ddlAerolineas.Items.Insert(0, New ListItem("-- Seleccione una Aerolínea --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar aerolíneas: " & ex.Message, False)
        End Try
    End Sub

    ' -------------------------------------------------------------
    ' 2. MÉTODO PARA GUARDAR LA AERONAVE
    ' -------------------------------------------------------------
    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        ' Validamos que hayan seleccionado una aerolínea
        If String.IsNullOrEmpty(ddlAerolineas.SelectedValue) Then
            MostrarMensaje("Por favor, seleccione una aerolínea válida.", False)
            Return
        End If

        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_INSERTAR_AERONAVE", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' Parámetros de Entrada
                    cmd.Parameters.Add("p_modelo", OracleDbType.Varchar2).Value = txtModelo.Text.Trim()
                    cmd.Parameters.Add("p_capacidad", OracleDbType.Int32).Value = Convert.ToInt32(txtCapacidad.Text)
                    cmd.Parameters.Add("p_id_aerolinea", OracleDbType.Int32).Value = Convert.ToInt32(ddlAerolineas.SelectedValue)

                    ' Parámetro de Salida
                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        MostrarMensaje("¡Aeronave agregada exitosamente a la flota!", True)
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
        txtModelo.Text = ""
        txtCapacidad.Text = ""
        ddlAerolineas.SelectedIndex = 0
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