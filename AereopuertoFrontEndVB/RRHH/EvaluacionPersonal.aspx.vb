Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class EvaluacionPersonal
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Solo Admin (1) y Recursos Humanos (4)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 1 AndAlso idRol <> 4) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarEmpleados()
            CargarHistorialEvaluaciones()
        End If
    End Sub

    Private Sub CargarEmpleados()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_EMPLEADOS_CBX", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' Parámetro de salida tipo Cursor
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        ddlEmpleado.DataSource = reader
                        ddlEmpleado.DataTextField = "NOMBRE_COMPLETO"
                        ddlEmpleado.DataValueField = "ID_EMPLEADO"
                        ddlEmpleado.DataBind()
                    End Using
                End Using
            End Using
            ddlEmpleado.Items.Insert(0, New ListItem("-- Seleccione un Colaborador --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar empleados: " & ex.Message, False)
        End Try
    End Sub

    Private Sub CargarHistorialEvaluaciones()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_EVALUACIONES", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptEvaluaciones.DataSource = dt
                            rptEvaluaciones.DataBind()
                            rptEvaluaciones.Visible = True
                            pnlVacio.Visible = False
                        Else
                            rptEvaluaciones.Visible = False
                            pnlVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Error silencioso en la carga inicial
        End Try
    End Sub

    Protected Sub btnGuardarEvaluacion_Click(sender As Object, e As EventArgs) Handles btnGuardarEvaluacion.Click
        If String.IsNullOrEmpty(ddlEmpleado.SelectedValue) OrElse String.IsNullOrEmpty(txtPuntaje.Text) OrElse String.IsNullOrEmpty(txtComentario.Text) Then
            MostrarMensaje("Por favor complete todos los campos obligatorios.", False)
            Return
        End If

        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_EVALUACION", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_empleado", OracleDbType.Int32).Value = Convert.ToInt32(ddlEmpleado.SelectedValue)
                    cmd.Parameters.Add("p_puntaje", OracleDbType.Decimal).Value = Convert.ToDecimal(txtPuntaje.Text)
                    cmd.Parameters.Add("p_comentario", OracleDbType.Varchar2).Value = txtComentario.Text.Trim()

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        MostrarMensaje("✅ Evaluación registrada exitosamente en el expediente.", True)
                        txtPuntaje.Text = ""
                        txtComentario.Text = ""
                        ddlEmpleado.SelectedIndex = 0
                        CargarHistorialEvaluaciones() ' Refrescar tabla
                    Else
                        MostrarMensaje("Error: " & resultado, False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error interno: " & ex.Message, False)
        End Try
    End Sub

    ' LA CORRECCIÓN ESTÁ AQUÍ (Public)
    ' Lógica visual para la tabla: Colores basados en el rendimiento
    ' =================================================================
    ' EVENTO: Pinta los colores de los puntajes fila por fila
    ' =================================================================
    Protected Sub rptEvaluaciones_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptEvaluaciones.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            ' Buscamos el Label que pusimos en el HTML
            Dim lblBadge As Label = CType(e.Item.FindControl("lblBadgePuntaje"), Label)

            ' Obtenemos el dato crudo de la base de datos
            Dim puntaje As Object = DataBinder.Eval(e.Item.DataItem, "puntaje")

            ' Validamos nulos y aplicamos colores
            If Not IsDBNull(puntaje) AndAlso puntaje IsNot Nothing Then
                Dim valor As Decimal = Convert.ToDecimal(puntaje)
                lblBadge.Text = valor.ToString("0.##") & " / 100"

                If valor >= 85 Then
                    lblBadge.CssClass = "badge bg-success badge-score shadow-sm"
                ElseIf valor >= 60 Then
                    lblBadge.CssClass = "badge bg-warning text-dark badge-score shadow-sm"
                Else
                    lblBadge.CssClass = "badge bg-danger badge-score shadow-sm"
                End If
            Else
                lblBadge.Text = "N/A"
                lblBadge.CssClass = "badge bg-secondary badge-score shadow-sm"
            End If
        End If
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success fw-bold text-center rounded-3 mb-4 shadow-sm", "alert alert-danger fw-bold text-center rounded-3 mb-4 shadow-sm")
    End Sub

End Class