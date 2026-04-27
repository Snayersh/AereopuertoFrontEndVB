Public Class MisFacturas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = 0
        If Session("IdRol") IsNot Nothing Then idRol = Convert.ToInt32(Session("IdRol"))

        ' Rol 2 = Clientes
        If Session("UserEmail") Is Nothing OrElse (idRol <> 2) Then
            Response.Redirect("~/Account/Login.aspx")
            Return
        End If

        If Not IsPostBack Then
            CargarFacturas()
        End If
    End Sub

    Private Sub CargarFacturas()
        Dim correoUsuario As String = Session("UserEmail").ToString()

        ' 🔥 Llamamos a nuestro servicio de facturación
        Dim respuesta = ClienteFacturaService.ObtenerMisFacturas(correoUsuario)

        If respuesta.success Then
            Dim listaFacturas = CType(respuesta.facturas, List(Of Dictionary(Of String, Object)))

            If listaFacturas.Count > 0 Then
                rptFacturas.DataSource = listaFacturas
                rptFacturas.DataBind()
                pnlDatos.Visible = True
                pnlVacio.Visible = False
                pnlError.Visible = False
            Else
                pnlDatos.Visible = False
                pnlVacio.Visible = True
                pnlError.Visible = False
            End If
        Else
            pnlError.Visible = True
            lblError.Text = respuesta.mensaje
            pnlDatos.Visible = False
            pnlVacio.Visible = False
        End If
    End Sub

    ' =================================================================
    ' EVENTO: Formatea la moneda y el estado fila por fila
    ' =================================================================
    Protected Sub rptFacturas_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptFacturas.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then

            ' Extraemos la fila como Diccionario (nuestra utilidad usa llaves en minúsculas)
            Dim itemData As Dictionary(Of String, Object) = CType(e.Item.DataItem, Dictionary(Of String, Object))

            ' 1. Formatear el Total a Moneda
            Dim lblTotal As Label = CType(e.Item.FindControl("lblTotal"), Label)
            Dim totalObj = itemData("total")

            If totalObj IsNot Nothing AndAlso Not IsDBNull(totalObj) Then
                Dim totalDec As Decimal = Convert.ToDecimal(totalObj)
                lblTotal.Text = "Q " & totalDec.ToString("N2")
            Else
                lblTotal.Text = "Q 0.00"
            End If

            ' 2. Formatear el Badge de Estado (Color Verde o Rojo)
            Dim lblEstado As Label = CType(e.Item.FindControl("lblEstado"), Label)
            Dim estadoStr As String = If(itemData("estado") IsNot Nothing, itemData("estado").ToString(), "DESCONOCIDO")

            lblEstado.Text = estadoStr
            If estadoStr.ToUpper() = "PAGADA" Then
                lblEstado.CssClass = "badge-pagado"
            Else
                lblEstado.CssClass = "badge-anulado"
            End If

        End If
    End Sub

End Class