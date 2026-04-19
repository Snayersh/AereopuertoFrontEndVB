Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Text

Public Class Tripulacion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse (idRol <> 4) Then
            Response.Redirect("~/Account/Login.aspx")
        End If
        If Not IsPostBack Then
            CargarVuelos()
            CargarEmpleados(ddlPiloto, 1)    ' 1 = ID de Tipo Piloto
            CargarEmpleados(ddlCopiloto, 2)  ' 2 = ID de Tipo Copiloto
            CargarEmpleados(ddlSobrecargo, 3) ' 3 = ID de Tipo Sobrecargo
        End If
    End Sub

    Private Sub CargarVuelos()
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_VUELOS_TRIPULACION", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        ddlVuelos.DataSource = dt
                        ddlVuelos.DataTextField = "detalle_vuelo"
                        ddlVuelos.DataValueField = "id_vuelo"
                        ddlVuelos.DataBind()
                    End Using
                End Using
            End Using
            ddlVuelos.Items.Insert(0, New ListItem("-- Seleccione un vuelo --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar vuelos: " & ex.Message, False)
        End Try
    End Sub

    Private Sub CargarEmpleados(ddl As DropDownList, idTipoEmpleado As Integer)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_EMPLEADOS_TIPO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_id_tipo_empleado", OracleDbType.Int32).Value = idTipoEmpleado

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        ddl.DataSource = dt
                        ddl.DataTextField = "nombre_completo"
                        ddl.DataValueField = "id_empleado"
                        ddl.DataBind()
                    End Using
                End Using
            End Using
            ddl.Items.Insert(0, New ListItem("-- Opcional / Sin asignar --", ""))
        Catch ex As Exception
            MostrarMensaje("Error al cargar empleados: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnAsignar_Click(sender As Object, e As EventArgs) Handles btnAsignar.Click
        If String.IsNullOrEmpty(ddlVuelos.SelectedValue) Then
            MostrarMensaje("Debe seleccionar un vuelo para asignar la tripulación.", False)
            Return
        End If

        Dim idVuelo As Integer = Convert.ToInt32(ddlVuelos.SelectedValue)
        Dim asignacionesExitosas As Integer = 0
        Dim errores As New StringBuilder()

        ' Validamos e insertamos cada rol si fue seleccionado
        If Not String.IsNullOrEmpty(ddlPiloto.SelectedValue) Then
            GuardarAsignacion(idVuelo, Convert.ToInt32(ddlPiloto.SelectedValue), asignacionesExitosas, errores)
        End If

        If Not String.IsNullOrEmpty(ddlCopiloto.SelectedValue) Then
            GuardarAsignacion(idVuelo, Convert.ToInt32(ddlCopiloto.SelectedValue), asignacionesExitosas, errores)
        End If

        If Not String.IsNullOrEmpty(ddlSobrecargo.SelectedValue) Then
            GuardarAsignacion(idVuelo, Convert.ToInt32(ddlSobrecargo.SelectedValue), asignacionesExitosas, errores)
        End If

        If errores.Length > 0 Then
            MostrarMensaje("Atención: " & errores.ToString(), False)
        ElseIf asignacionesExitosas > 0 Then
            MostrarMensaje("¡Tripulación asignada exitosamente al vuelo!", True)
            ' Limpiamos los combos para permitir asignar más sobrecargos
            ddlPiloto.SelectedIndex = 0
            ddlCopiloto.SelectedIndex = 0
            ddlSobrecargo.SelectedIndex = 0
        Else
            MostrarMensaje("No seleccionó ningún empleado para asignar.", False)
        End If
    End Sub

    Private Sub GuardarAsignacion(idVuelo As Integer, idEmpleado As Integer, ByRef contadorExitos As Integer, ByRef errores As StringBuilder)
        Dim db As New ConexionDB()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ASIGNAR_TRIPULANTE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = idVuelo
                    cmd.Parameters.Add("p_id_empleado", OracleDbType.Int32).Value = idEmpleado

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()
                    If resultado = "EXITO" Then
                        contadorExitos += 1
                    Else
                        errores.AppendLine(resultado)
                    End If
                End Using
            End Using
        Catch ex As Exception
            errores.AppendLine("Error de BD: " & ex.Message)
        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, exito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(exito, "alert alert-success fw-bold text-center", "alert alert-danger fw-bold text-center")
    End Sub
End Class