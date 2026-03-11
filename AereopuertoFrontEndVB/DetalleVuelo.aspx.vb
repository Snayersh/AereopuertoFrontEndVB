Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Public Class DetalleVuelo
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' 1. Capturamos el ID que viene en la URL (ej. DetalleVuelo.aspx?id=5)
            Dim idVueloString As String = Request.QueryString("id")
            Dim idVuelo As Integer

            ' Validamos que sí venga un ID y que sea un número válido
            If Not String.IsNullOrEmpty(idVueloString) AndAlso Integer.TryParse(idVueloString, idVuelo) Then
                CargarDatosDelVuelo(idVuelo)
            Else
                ' Si alteran la URL (ej. ponen letras), mostramos error
                pnlDetalle.Visible = False
                pnlError.Visible = True
            End If
        End If
    End Sub

    Private Sub CargarDatosDelVuelo(id As Integer)
        Dim db As New ConexionDB()

        Try
            Using conn As OracleConnection = db.ObtenerConexion()
                Using cmd As New OracleCommand("SP_DETALLE_VUELO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.BindByName = True ' Siempre seguro

                    ' Parámetro de entrada
                    cmd.Parameters.Add("p_id_vuelo", OracleDbType.Int32).Value = id

                    ' Parámetro de salida (El Cursor con los datos)
                    Dim cursorParam As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    cursorParam.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(cursorParam)

                    conn.Open()

                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            ' Si el vuelo existe, llenamos todos los Label
                            Dim row As DataRow = dt.Rows(0)

                            lblCodigoVuelo.Text = row("codigo_vuelo").ToString().ToUpper()
                            lblAerolineaHead.Text = row("aerolinea").ToString()
                            lblAerolinea.Text = row("aerolinea").ToString()

                            lblEstado.Text = row("estado_vuelo").ToString()
                            ConfigurarColorEstado(lblEstado.Text)

                            ' Origen
                            lblOrigenCiudad.Text = row("origen_ciudad").ToString()
                            lblOrigenIata.Text = row("origen_iata").ToString()
                            lblOrigenAero.Text = row("origen_aeropuerto").ToString()
                            lblOrigenPais.Text = row("origen_pais").ToString()

                            ' Destino
                            lblDestinoCiudad.Text = row("destino_ciudad").ToString()
                            lblDestinoIata.Text = row("destino_iata").ToString()
                            lblDestinoAero.Text = row("destino_aeropuerto").ToString()
                            lblDestinoPais.Text = row("destino_pais").ToString()

                            ' Fechas y Aeronave
                            Dim fechaSalida As DateTime = Convert.ToDateTime(row("fecha_salida"))
                            Dim fechaLlegada As DateTime = Convert.ToDateTime(row("fecha_llegada"))

                            lblFechaSalida.Text = fechaSalida.ToString("dd/MM/yyyy HH:mm")
                            lblFechaLlegada.Text = fechaLlegada.ToString("dd/MM/yyyy HH:mm")

                            lblAeronaveModelo.Text = row("aeronave_modelo").ToString()
                            lblCapacidad.Text = row("aeronave_capacidad").ToString()

                            ' Cálculo dinámico de la duración del vuelo
                            Dim duracion As TimeSpan = fechaLlegada - fechaSalida
                            lblDuracion.Text = $"{duracion.Hours} horas y {duracion.Minutes} minutos"

                            pnlDetalle.Visible = True
                            pnlError.Visible = False
                        Else
                            ' Si el SP no devuelve nada (ID no existe en la BD)
                            pnlDetalle.Visible = False
                            pnlError.Visible = True
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            pnlDetalle.Visible = False
            pnlError.Visible = True
            ' En un entorno real, no mostramos el error técnico al usuario final, 
            ' pero para tu proyecto está bien para depurar.
        End Try
    End Sub

    ' Función para que el color del estado sea dinámico
    Private Sub ConfigurarColorEstado(estado As String)
        estado = estado.ToUpper()
        If estado = "PROGRAMADO" Then
            lblEstado.CssClass = "badge-estado bg-programado"
        ElseIf estado = "ATERRIZÓ" Or estado = "FINALIZADO" Then
            lblEstado.CssClass = "badge-estado bg-finalizado"
        Else
            lblEstado.CssClass = "badge-estado bg-activo"
        End If
    End Sub

End Class