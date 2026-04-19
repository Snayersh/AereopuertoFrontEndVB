Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class GestionPuertas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        ' Rol 3 (Operaciones) es correcto para asignar puertas
        If Session("UserEmail") Is Nothing OrElse (idRol <> 3) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            pnlMensaje.Visible = False
            CargarDesplegables()
            CargarAsignaciones()
        End If
    End Sub

    ' =======================================================
    ' BOTÓN: ASIGNAR PUERTA DIRECTO A BASE DE DATOS
    ' =======================================================
    Protected Sub btnAsignar_Click(sender As Object, e As EventArgs) Handles btnAsignar.Click
        ' Validaciones básicas del front
        If String.IsNullOrEmpty(ddlVuelos.SelectedValue) OrElse String.IsNullOrEmpty(ddlPuertas.SelectedValue) OrElse String.IsNullOrEmpty(txtHora.Text) Then
            MostrarMensaje("⚠️ Por favor, completa todos los campos.", False)
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ASIGNAR_PUERTA_VUELO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    ' Pasamos los parámetros directo de los controles de la página
                    cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = Convert.ToInt32(ddlVuelos.SelectedValue)
                    cmd.Parameters.Add("p_id_puerta", OracleDbType.Int32).Value = Convert.ToInt32(ddlPuertas.SelectedValue)
                    cmd.Parameters.Add("p_hora", OracleDbType.TimeStamp).Value = Convert.ToDateTime(txtHora.Text)

                    Dim paramOut As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    paramOut.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(paramOut)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = paramOut.Value.ToString()

                    If resultado = "EXITO" Then
                        MostrarMensaje("✅ Puerta asignada correctamente al vuelo.", True)

                        ' Limpiar formulario y recargar la tabla
                        ddlVuelos.SelectedIndex = 0
                        ddlPuertas.SelectedIndex = 0
                        txtHora.Text = ""
                        CargarAsignaciones()
                    Else
                        MostrarMensaje("⚠️ Error: " & resultado, False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("❌ Error del sistema: " & ex.Message, False)
        End Try
    End Sub

    ' =======================================================
    ' MÉTODOS DE APOYO (100% PARAMETRIZADOS)
    ' =======================================================
    Private Sub CargarDesplegables()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                conn.Open()

                ' 1. Cargar Vuelos (Solo activos) usando SP
                Using cmdVuelos As New OracleCommand("SP_OBTENER_VUELOS_ACTIVOS_CBX", conn)
                    cmdVuelos.CommandType = CommandType.StoredProcedure

                    Dim cursorVuelos As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorVuelos.Direction = ParameterDirection.Output
                    cmdVuelos.Parameters.Add(cursorVuelos)

                    Using da As New OracleDataAdapter(cmdVuelos)
                        Dim dtVuelos As New DataTable()
                        da.Fill(dtVuelos)
                        ddlVuelos.DataSource = dtVuelos
                        ddlVuelos.DataValueField = "ID_VUELO"
                        ddlVuelos.DataTextField = "CODIGO_VUELO"
                        ddlVuelos.DataBind()
                        ddlVuelos.Items.Insert(0, New ListItem("-- Seleccione Vuelo --", ""))
                    End Using
                End Using

                ' 2. Cargar Puertas Físicas usando SP
                Using cmdPuertas As New OracleCommand("SP_OBTENER_PUERTAS_CBX", conn)
                    cmdPuertas.CommandType = CommandType.StoredProcedure

                    Dim cursorPuertas As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorPuertas.Direction = ParameterDirection.Output
                    cmdPuertas.Parameters.Add(cursorPuertas)

                    Using da As New OracleDataAdapter(cmdPuertas)
                        Dim dtPuertas As New DataTable()
                        da.Fill(dtPuertas)
                        ddlPuertas.DataSource = dtPuertas
                        ddlPuertas.DataValueField = "ID_PUERTA"
                        ddlPuertas.DataTextField = "NOMBRE_PUERTA"
                        ddlPuertas.DataBind()
                        ddlPuertas.Items.Insert(0, New ListItem("-- Seleccione Puerta --", ""))
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar datos: " & ex.Message, False)
        End Try
    End Sub

    Private Sub CargarAsignaciones()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_ASIGNACIONES_PUERTA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        rptAsignaciones.DataSource = dt
                        rptAsignaciones.DataBind()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar tabla: " & ex.Message, False)
        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success fw-bold text-center mb-3", "alert alert-danger fw-bold text-center mb-3")
    End Sub
End Class