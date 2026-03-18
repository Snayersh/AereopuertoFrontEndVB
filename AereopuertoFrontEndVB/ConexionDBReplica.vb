Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class ConexionDBReplica
    Private cadenaConexion As String = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.X.X)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=TU_SERVICIO_REPLICA)));User Id=tu_usuario;Password=tu_password;"

    Public Function ObtenerConexion() As OracleConnection
        Return New OracleConnection(cadenaConexion)
    End Function
End Class