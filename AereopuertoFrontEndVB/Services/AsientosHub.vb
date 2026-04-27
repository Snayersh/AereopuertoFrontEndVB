Imports Microsoft.AspNet.SignalR
Imports System.Threading.Tasks

Public Class AsientosHub
    Inherits Hub

    ' Función que llama el JavaScript cuando alguien selecciona un asiento
    Public Sub BloquearAsientoTemporal(idVuelo As String, asiento As String)
        ' Le avisamos a TODOS los demás usuarios (excepto al que hizo clic)
        ' que pongan ese asiento en naranja/bloqueado
        Clients.Others.alguienBloqueoAsiento(idVuelo, asiento)
    End Sub

    ' Función que llama el JavaScript cuando alguien quita la selección
    Public Sub LiberarAsientoTemporal(idVuelo As String, asiento As String)
        Clients.Others.alguienLiberoAsiento(idVuelo, asiento)
    End Sub

End Class