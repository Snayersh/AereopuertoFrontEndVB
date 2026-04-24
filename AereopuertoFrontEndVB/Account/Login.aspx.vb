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
        pnlExito.Visible = False

        Dim email As String = txtEmail.Text.Trim().ToLower()
        Dim passPlana As String = txtPassword.Text.Trim()

        If String.IsNullOrEmpty(email) OrElse String.IsNullOrEmpty(passPlana) Then
            MostrarError("Por favor, ingrese correo y contraseña.")
            Return
        End If

        Dim passHasheada As String = EncriptarSHA256(passPlana)
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_VALIDAR_LOGIN", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = email
                    cmd.Parameters.Add("p_password_hash", OracleDbType.Varchar2).Value = passHasheada

                    Dim outRol As New OracleParameter("p_id_rol", OracleDbType.Int32)
                    outRol.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outRol)

                    Dim outNombre As New OracleParameter("p_nombre_completo", OracleDbType.Varchar2, 200)
                    outNombre.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outNombre)

                    ' 🔥 AGREGAMOS EL PARÁMETRO DEL TOKEN PARA QUE ORACLE NO FALLE
                    Dim outToken As New OracleParameter("p_token_sesion", OracleDbType.Varchar2, 255)
                    outToken.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outToken)

                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        Dim idRol As Integer = Convert.ToInt32(outRol.Value.ToString())
                        Session("IdRol") = idRol
                        Session("UserName") = outNombre.Value.ToString()
                        Session("UserEmail") = email

                        ' Guardamos el token en sesión web también por si luego quieres aplicar la sesión única en la página
                        Session("TokenSesion") = If(IsDBNull(outToken.Value), "", outToken.Value.ToString())

                        If idRol = 1 Then
                            Session("UserRole") = "Admin"
                        ElseIf idRol = 2 Then
                            Session("UserRole") = "Pasajero"
                        ElseIf idRol = 3 Then
                            Session("UserRole") = "Operaciones"
                        ElseIf idRol = 4 Then
                            Session("UserRole") = "Recursos_Humanos"
                        ElseIf idRol = 5 Then
                            Session("UserRole") = "Seguridad"
                        ElseIf idRol = 6 Then
                            Session("UserRole") = "Servicio_Al_Cliente"
                        ElseIf idRol = 7 Then
                            Session("UserRole") = "Mantenimiento_Tecnico"

                        Else
                            Session("UserRole") = "Invitado"
                        End If

                        Response.Redirect("~/Default.aspx")

                    ElseIf resultado = "BLOQUEO_TEMPORAL" Then
                        MostrarError("⛔ Demasiados intentos fallidos. Tu cuenta ha sido bloqueada temporalmente por 1 minuto por seguridad.")

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