Imports Microsoft.Owin
Imports Owin

' Importante: Si esto te da error, puedes borrar la línea de <Assembly... >
<Assembly: OwinStartup(GetType(AereopuertoFrontEndVB.Startup))>

Public Class Startup
    Public Sub Configuration(app As IAppBuilder)
        ' Esto enciende el túnel de WebSockets
        app.MapSignalR()
    End Sub
End Class