Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class Activar
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Solo ejecutamos esto la primera vez que carga la página
        If Not IsPostBack Then
            ' Leemos el token que viene en la URL (ej. Activar.aspx?token=1234abcd...)
            Dim token As String = Request.QueryString("token")

            If String.IsNullOrEmpty(token) Then
                ' Si entran a la página directamente sin token, mostramos error
                MostrarPantalla(False, "No se proporcionó ningún código de activación.")
            Else
                ' Si hay token, vamos a la base de datos a verificarlo
                ProcesarActivacion(token)
            End If
        End If
    End Sub

    Private Sub ProcesarActivacion(token As String)
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_ACTIVAR_CUENTA", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' Parámetro de Entrada (El código largo de la URL)
                    cmd.Parameters.Add("p_token", OracleDbType.Varchar2).Value = token

                    ' Parámetro de Salida
                    Dim outResultado As New OracleParameter("p_resultado", OracleDbType.Varchar2, 200)
                    outResultado.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(outResultado)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Dim resultado As String = outResultado.Value.ToString()

                    If resultado = "EXITO" Then
                        MostrarPantalla(True, "")
                    ElseIf resultado = "TOKEN_INVALIDO" Then
                        MostrarPantalla(False, "El enlace de activación no es válido o la cuenta ya fue verificada.")
                    Else
                        MostrarPantalla(False, "Error interno del servidor: " & resultado)
                    End If
                End Using
            End Using

        Catch ex As Exception
            MostrarPantalla(False, "Error de conexión: " & ex.Message)
        End Try
    End Sub

    ' Función auxiliar para encender/apagar los paneles correctos
    Private Sub MostrarPantalla(esExito As Boolean, mensajeError As String)
        If esExito Then
            pnlExito.Visible = True
            pnlError.Visible = False
        Else
            pnlExito.Visible = False
            pnlError.Visible = True
            If Not String.IsNullOrEmpty(mensajeError) Then
                lblMensajeError.Text = mensajeError
            End If
        End If
    End Sub

End Class