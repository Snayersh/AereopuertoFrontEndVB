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
    Public Function ProbarConexion() As Boolean
        Try
            Using conexion As OracleConnection = ObtenerConexion()
                conexion.Open()
                Return True ' Si llega aquí, la conexión fue exitosa
            End Using
        Catch ex As Exception
            ' Aquí podrías imprimir ex.Message en consola para ver el error exacto
            Return False
        End Try
    End Function

End Class