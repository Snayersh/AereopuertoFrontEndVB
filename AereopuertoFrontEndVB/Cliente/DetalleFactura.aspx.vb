Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class DetalleFactura
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("UserEmail") Is Nothing Then
            Response.Redirect("~/Account/Login.aspx")
            Return
        End If

        If Not IsPostBack Then
            Dim idFacturaStr As String = Request.QueryString("id")
            If Not String.IsNullOrEmpty(idFacturaStr) Then
                CargarFactura(Convert.ToInt32(idFacturaStr))
            Else
                MostrarError("No se especificó la factura.")
            End If
        End If
    End Sub

    Private Sub CargarFactura(idFactura As Integer)
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                conn.Open()

                Using cmdCabecera As New OracleCommand("SP_FACTURA_CABECERA", conn)
                    cmdCabecera.CommandType = CommandType.StoredProcedure
                    cmdCabecera.BindByName = True
                    cmdCabecera.Parameters.Add("p_id_factura", OracleDbType.Int32).Value = idFactura

                    Dim cursorCabecera As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorCabecera.Direction = ParameterDirection.Output
                    cmdCabecera.Parameters.Add(cursorCabecera)

                    Using daCabecera As New OracleDataAdapter(cmdCabecera)
                        Dim dtCabecera As New DataTable()
                        daCabecera.Fill(dtCabecera)

                        If dtCabecera.Rows.Count > 0 Then
                            Dim row As DataRow = dtCabecera.Rows(0)
                            lblNumeroFactura.Text = row("numero_factura").ToString()
                            lblFecha.Text = row("fecha_emision").ToString()
                            lblCliente.Text = row("cliente").ToString()
                            lblCorreo.Text = row("correo").ToString()
                            lblTotal.Text = Convert.ToDecimal(row("total")).ToString("N2")
                        Else
                            MostrarError("No se encontró la factura solicitada.")
                            Return
                        End If
                    End Using
                End Using

                Using cmdDetalle As New OracleCommand("SP_FACTURA_DETALLE", conn)
                    cmdDetalle.CommandType = CommandType.StoredProcedure
                    cmdDetalle.BindByName = True
                    cmdDetalle.Parameters.Add("p_id_factura", OracleDbType.Int32).Value = idFactura

                    Dim cursorDetalle As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorDetalle.Direction = ParameterDirection.Output
                    cmdDetalle.Parameters.Add(cursorDetalle)

                    Using daDetalle As New OracleDataAdapter(cmdDetalle)
                        Dim dtDetalle As New DataTable()
                        daDetalle.Fill(dtDetalle)

                        rptDetalles.DataSource = dtDetalle
                        rptDetalles.DataBind()
                    End Using
                End Using
            End Using

        Catch ex As Exception
            MostrarError("Error al cargar el detalle: " & ex.Message)
        End Try
    End Sub

    Private Sub MostrarError(mensaje As String)
        pnlFactura.Visible = False
        pnlError.Visible = True
        lblError.Text = mensaje
    End Sub
End Class