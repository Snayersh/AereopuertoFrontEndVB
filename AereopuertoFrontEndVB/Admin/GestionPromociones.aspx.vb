Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class GestionPromociones
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 🔥 SEGURIDAD: Solo Administradores (Rol 1)
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        If Session("UserEmail") Is Nothing OrElse idRol <> 1 Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarHistorial()
        End If
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If String.IsNullOrEmpty(txtDescripcion.Text) OrElse String.IsNullOrEmpty(txtDescuento.Text) Then
            MostrarMensaje("Complete todos los campos obligatorios.", False)
            Return
        End If

        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_REGISTRAR_PROMOCION", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = txtDescripcion.Text.Trim()
                    cmd.Parameters.Add("p_descuento", OracleDbType.Decimal).Value = Convert.ToDecimal(txtDescuento.Text)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    If outResultado.Value.ToString() = "EXITO" Then
                        MostrarMensaje("✅ Promoción registrada y lista para usarse.", True)
                        txtDescripcion.Text = ""
                        txtDescuento.Text = ""
                        CargarHistorial()
                    Else
                        MostrarMensaje(outResultado.Value.ToString(), False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error interno: " & ex.Message, False)
        End Try
    End Sub

    Private Sub CargarHistorial()
        Dim db As New ConexionDBReplica()
        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_LISTAR_PROMOCIONES", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptPromociones.DataSource = dt
                            rptPromociones.DataBind()
                            rptPromociones.Visible = True
                            pnlVacio.Visible = False
                        Else
                            rptPromociones.Visible = False
                            pnlVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar historial: " & ex.Message, False)
        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, esExito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        pnlMensaje.CssClass = If(esExito, "alert alert-success text-center fw-bold rounded-3 mb-4 shadow-sm", "alert alert-danger text-center fw-bold rounded-3 mb-4 shadow-sm")
    End Sub
End Class