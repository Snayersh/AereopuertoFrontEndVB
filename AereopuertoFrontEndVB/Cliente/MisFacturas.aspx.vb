Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class MisFacturas
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("UserEmail") Is Nothing Then
            Response.Redirect("~/Account/Login.aspx")
            Return
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
End Class