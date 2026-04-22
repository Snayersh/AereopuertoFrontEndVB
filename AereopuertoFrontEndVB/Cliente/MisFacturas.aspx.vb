Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class MisFacturas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim idRol As Integer = Convert.ToInt32(Session("IdRol"))
        ' Rol 2 = Clientes
        If Session("UserEmail") Is Nothing OrElse (idRol <> 2) Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            CargarFacturas()
        End If
    End Sub

    Private Sub CargarFacturas()
        Dim db As New ConexionDB()
        Dim correoUsuario As String = Session("UserEmail").ToString()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_CONSULTAR_MIS_FACTURAS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True

                    cmd.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = correoUsuario

                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            rptFacturas.DataSource = dt
                            rptFacturas.DataBind()
                            pnlDatos.Visible = True
                            pnlVacio.Visible = False
                        Else
                            pnlDatos.Visible = False
                            pnlVacio.Visible = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            pnlError.Visible = True
            lblError.Text = "Error al obtener tus facturas: " & ex.Message
        End Try
    End Sub

    ' =================================================================
    ' EVENTO: Formatea la moneda y el estado fila por fila
    ' =================================================================
    Protected Sub rptFacturas_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptFacturas.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then

            ' 1. Formatear el Total a Moneda
            Dim lblTotal As Label = CType(e.Item.FindControl("lblTotal"), Label)
            Dim totalObj As Object = DataBinder.Eval(e.Item.DataItem, "total")

            If Not IsDBNull(totalObj) AndAlso totalObj IsNot Nothing Then
                Dim totalDec As Decimal = Convert.ToDecimal(totalObj)
                lblTotal.Text = "Q " & totalDec.ToString("N2")
            Else
                lblTotal.Text = "Q 0.00"
            End If

            ' 2. Formatear el Badge de Estado (Color Verde o Rojo)
            Dim lblEstado As Label = CType(e.Item.FindControl("lblEstado"), Label)
            Dim estadoStr As String = DataBinder.Eval(e.Item.DataItem, "estado").ToString()

            lblEstado.Text = estadoStr
            If estadoStr.ToUpper() = "PAGADA" Then
                lblEstado.CssClass = "badge-pagado"
            Else
                lblEstado.CssClass = "badge-anulado"
            End If

        End If
    End Sub

End Class