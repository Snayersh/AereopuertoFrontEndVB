Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Imports System.Security.Cryptography
Imports System.Text

Public Class Login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            pnlError.Visible = False

            ' --- MAGIA AQUÍ: Detecta si viene de registrarse ---
            If Request.QueryString("registro") = "exitoso" Then
                pnlExito.Visible = True
            End If
            ' ---------------------------------------------------
        End If
    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        ' Si hace clic en login, apagamos el mensaje de éxito por si acaso
        pnlExito.Visible = False

        Dim email As String = txtEmail.Text.Trim()
        Dim passPlana As String = txtPassword.Text.Trim()

        If String.IsNullOrEmpty(email) OrElse String.IsNullOrEmpty(passPlana) Then
            MostrarError("Por favor, ingrese correo y contraseña.")
            Return
        End If

        ' 1. Convertimos la contraseña plana a su equivalente en Hash SHA-256
        Dim passHasheada As String = EncriptarSHA256(passPlana)

        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_VALIDAR_LOGIN", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' 2. Enviamos el hash a la base de datos, ¡NUNCA la clave plana!
                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = email
                    cmd.Parameters.Add("p_password_hash", OracleDbType.Varchar2).Value = passHasheada

                    Dim outRol As New OracleParameter("p_id_rol", OracleDbType.Int32)
                    outRol.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outRol)

                    Dim outNombre As New OracleParameter("p_nombre_completo", OracleDbType.Varchar2, 200)
                    outNombre.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outNombre)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    ' 3. Manejo de todos los posibles escenarios que nos responde Oracle

                    If resultado = "EXITO" Then
                        Dim idRol As Integer = Convert.ToInt32(outRol.Value.ToString())
                        Session("UserName") = outNombre.Value.ToString()

                        ' Importante: Guardamos el correo en sesión para que la pantalla de Reservas lo pueda usar
                        Session("UserEmail") = email

                        If idRol = 1 Then
                            Session("UserRole") = "Admin"
                        ElseIf idRol = 2 Then
                            Session("UserRole") = "Cliente"
                        ElseIf idRol = 3 Then
                            Session("UserRole") = "Empleado"
                        Else
                            Session("UserRole") = "Invitado"
                        End If

                        Response.Redirect("~/Default.aspx")

                    ElseIf resultado = "CUENTA_PENDIENTE" Then
                        MostrarError("Tu cuenta aún no está activada. Por favor revisa la bandeja de entrada de tu correo electrónico.")

                    ElseIf resultado = "CUENTA_INACTIVA" Then
                        MostrarError("⛔ Tu cuenta ha sido desactivada. Por favor, contacta a la administración del aeropuerto.")

                    ElseIf resultado = "CREDENCIALES_INVALIDAS" Then
                        MostrarError("Correo o contraseña incorrectos.")

                    Else
                        MostrarError("Aviso del sistema: " & resultado)
                    End If

                End Using
            End Using

        Catch ex As Exception
            MostrarError("No se pudo conectar a la base de datos: " & ex.Message)
        End Try
    End Sub

    ' =========================================================
    ' FUNCIÓN PARA ENCRIPTAR LA CONTRASEÑA (SHA-256)
    ' =========================================================
    Private Function EncriptarSHA256(texto As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto))
            Dim builder As New StringBuilder()
            For i As Integer = 0 To bytes.Length - 1
                builder.Append(bytes(i).ToString("x2"))
            Next
            Return builder.ToString()
        End Using
    End Function

    Private Sub MostrarError(mensaje As String)
        pnlError.Visible = True
        lblError.Text = mensaje
    End Sub
End Class