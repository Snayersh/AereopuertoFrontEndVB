Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class MiPerfil
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Si no hay sesión, lo mandamos al login
        If Session("UserEmail") Is Nothing Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarDatosPerfil()
        End If
    End Sub

    Private Sub CargarDatosPerfil()
        Dim db As New ConexionDB()
        Dim correoUsuario As String = Session("UserEmail").ToString()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_OBTENER_PERFIL", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correoUsuario

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            Dim row As DataRow = dt.Rows(0)
                            txtPrimerNombre.Text = row("primer_nombre").ToString()
                            txtSegundoNombre.Text = row("segundo_nombre").ToString()
                            txtPrimerApellido.Text = row("primer_apellido").ToString()
                            txtSegundoApellido.Text = row("segundo_apellido").ToString()
                            txtTelefono.Text = row("telefono").ToString()
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al cargar los datos: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Dim db As New ConexionDB()
        Dim correoUsuario As String = Session("UserEmail").ToString()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ACTUALIZAR_PERFIL", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correoUsuario
                    cmd.Parameters.Add("p_primer_nombre", OracleDbType.Varchar2).Value = txtPrimerNombre.Text.Trim()
                    cmd.Parameters.Add("p_segundo_nombre", OracleDbType.Varchar2).Value = txtSegundoNombre.Text.Trim()
                    cmd.Parameters.Add("p_primer_apellido", OracleDbType.Varchar2).Value = txtPrimerApellido.Text.Trim()
                    cmd.Parameters.Add("p_segundo_apellido", OracleDbType.Varchar2).Value = txtSegundoApellido.Text.Trim()
                    cmd.Parameters.Add("p_telefono", OracleDbType.Varchar2).Value = txtTelefono.Text.Trim()

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        MostrarMensaje("¡Tu perfil se ha actualizado correctamente!", True)

                        ' TRUCO DE ORO: Actualizamos la variable de sesión para que el saludo cambie en toda la web
                        Session("UserName") = txtPrimerNombre.Text.Trim()
                    Else
                        MostrarMensaje(resultado, False)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MostrarMensaje("Error al guardar: " & ex.Message, False)
        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, exito As Boolean)
        pnlMensaje.Visible = True
        lblMensaje.Text = mensaje
        If exito Then
            pnlMensaje.CssClass = "alert alert-success text-center fw-bold"
        Else
            pnlMensaje.CssClass = "alert alert-danger text-center fw-bold"
        End If
    End Sub
End Class