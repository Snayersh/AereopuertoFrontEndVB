Imports System.Web.Script.Serialization

Public Class Radar
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CargarVuelosParaRadar()
        End If
    End Sub

    Private Sub CargarVuelosParaRadar()
        ' 🔥 Llamamos al nuevo servicio
        Dim respuesta = DashboardService.ObtenerVuelosParaRadar()

        If respuesta.success Then
            ' Usamos el serializador de .NET para transformar la lista en texto JSON perfecto
            Dim jsSerializer As New JavaScriptSerializer()
            hfDatosVuelos.Value = jsSerializer.Serialize(respuesta.vuelos_radar)
        Else
            ' Si falla, devolvemos un arreglo vacío para que Google Maps no tire error en JS
            hfDatosVuelos.Value = "[]"
        End If
    End Sub

End Class