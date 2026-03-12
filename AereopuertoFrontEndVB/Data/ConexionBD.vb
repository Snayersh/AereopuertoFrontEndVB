Imports System.Configuration
Imports Oracle.ManagedDataAccess.Client

Public Class ConexionDB
    ' Leemos la cadena de conexión desde el Web.config
    Private ReadOnly strConexion As String = ConfigurationManager.ConnectionStrings("OracleConexion").ConnectionString

    ''' <summary>
    ''' Devuelve un objeto de conexión de Oracle listo para abrirse.
    ''' </summary>
    Public Function ObtenerConexion() As OracleConnection
        Return New OracleConnection(strConexion)
    End Function

    ''' <summary>
    ''' Método de prueba para verificar que Visual Studio y Oracle se comunican.
    ''' </summary>
    Public Function ProbarConexion(ByRef mensajeError As String) As Boolean
        Try
            Using conexion As OracleConnection = ObtenerConexion()
                conexion.Open()
                mensajeError = "Conexión exitosa."
                Return True
            End Using
        Catch ex As OracleException
            ' Captura errores específicos de Oracle (ej. ORA-01017: usuario/contraseña inválidos)
            mensajeError = "Error de Oracle: " & ex.Message
            Return False
        Catch ex As Exception
            mensajeError = "Error general: " & ex.Message
            Return False
        End Try
    End Function

End Class