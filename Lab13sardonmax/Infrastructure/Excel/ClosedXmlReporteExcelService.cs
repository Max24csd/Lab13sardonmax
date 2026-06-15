using ClosedXML.Excel;
using Lab13sardonmax.Application.Abstractions;
using Lab13sardonmax.Application.Reportes;

namespace Lab13sardonmax.Infrastructure.Excel;

public sealed class ClosedXmlReporteExcelService : IReporteExcelService
{
    private static readonly XLColor ColorPrincipal = XLColor.FromHtml("#0F766E");

    public byte[] CrearReporteTickets(IReadOnlyList<TicketReporteDto> tickets)
    {
        using var workbook = new XLWorkbook();
        var hoja = workbook.Worksheets.Add("Tickets");

        CrearTitulo(hoja, "REPORTE GENERAL DE TICKETS", 8);
        var encabezados = new[]
        {
            "Codigo", "Titulo", "Usuario", "Estado", "Fecha de creacion",
            "Fecha de cierre", "Respuestas", "Horas de atencion"
        };
        EscribirEncabezados(hoja, encabezados);

        for (var indice = 0; indice < tickets.Count; indice++)
        {
            var fila = indice + 4;
            var ticket = tickets[indice];
            hoja.Cell(fila, 1).Value = ticket.Codigo;
            hoja.Cell(fila, 2).Value = ticket.Titulo;
            hoja.Cell(fila, 3).Value = ticket.Usuario;
            hoja.Cell(fila, 4).Value = ticket.Estado;
            hoja.Cell(fila, 5).Value = ticket.FechaCreacion;
            if (ticket.FechaCierre.HasValue)
            {
                hoja.Cell(fila, 6).Value = ticket.FechaCierre.Value;
            }
            else
            {
                hoja.Cell(fila, 6).Value = "Pendiente";
            }
            hoja.Cell(fila, 7).Value = ticket.TotalRespuestas;
            hoja.Cell(fila, 8).Value = ticket.HorasAtencion;
        }

        var ultimaFila = tickets.Count + 3;
        var tabla = hoja.Range(3, 1, ultimaFila, 8).CreateTable("TablaTickets");
        tabla.Theme = XLTableTheme.TableStyleMedium4;
        tabla.ShowTotalsRow = true;
        tabla.Field("Titulo").TotalsRowLabel = "TOTAL";
        tabla.Field("Respuestas").TotalsRowFunction = XLTotalsRowFunction.Sum;

        hoja.Range(4, 5, ultimaFila, 6).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
        hoja.Range(4, 8, ultimaFila, 8).Style.NumberFormat.Format = "0.0";
        AplicarFormatoFinal(hoja, 8);
        return Guardar(workbook);
    }

    public byte[] CrearReporteActividadUsuarios(
        IReadOnlyList<ActividadUsuarioReporteDto> usuarios)
    {
        using var workbook = new XLWorkbook();
        var hoja = workbook.Worksheets.Add("Actividad usuarios");

        CrearTitulo(hoja, "REPORTE DE ACTIVIDAD DE USUARIOS", 7);
        var encabezados = new[]
        {
            "Usuario", "Correo", "Roles", "Fecha de registro",
            "Tickets creados", "Respuestas realizadas", "Tickets cerrados"
        };
        EscribirEncabezados(hoja, encabezados);

        for (var indice = 0; indice < usuarios.Count; indice++)
        {
            var fila = indice + 4;
            var usuario = usuarios[indice];
            hoja.Cell(fila, 1).Value = usuario.Usuario;
            hoja.Cell(fila, 2).Value = usuario.Correo;
            hoja.Cell(fila, 3).Value = usuario.Roles;
            hoja.Cell(fila, 4).Value = usuario.FechaRegistro;
            hoja.Cell(fila, 5).Value = usuario.TicketsCreados;
            hoja.Cell(fila, 6).Value = usuario.RespuestasRealizadas;
            hoja.Cell(fila, 7).Value = usuario.TicketsCerrados;
        }

        var ultimaFila = usuarios.Count + 3;
        var tabla = hoja.Range(3, 1, ultimaFila, 7).CreateTable("TablaActividadUsuarios");
        tabla.Theme = XLTableTheme.TableStyleMedium4;
        tabla.ShowTotalsRow = true;
        tabla.Field("Fecha de registro").TotalsRowLabel = "TOTALES";
        tabla.Field("Tickets creados").TotalsRowFunction = XLTotalsRowFunction.Sum;
        tabla.Field("Respuestas realizadas").TotalsRowFunction = XLTotalsRowFunction.Sum;
        tabla.Field("Tickets cerrados").TotalsRowFunction = XLTotalsRowFunction.Sum;

        hoja.Range(4, 4, ultimaFila, 4).Style.DateFormat.Format = "dd/MM/yyyy";
        AplicarFormatoFinal(hoja, 7);
        return Guardar(workbook);
    }

    public byte[] ModificarCelda(byte[] archivoExcel, string celda, int nuevoValor)
    {
        using var entrada = new MemoryStream(archivoExcel);
        using var workbook = new XLWorkbook(entrada);
        workbook.Worksheet(1).Cell(celda).Value = nuevoValor;
        return Guardar(workbook);
    }

    private static void CrearTitulo(IXLWorksheet hoja, string titulo, int ultimaColumna)
    {
        var rango = hoja.Range(1, 1, 1, ultimaColumna);
        rango.Merge();
        rango.Value = titulo;
        rango.Style.Font.Bold = true;
        rango.Style.Font.FontSize = 18;
        rango.Style.Font.FontColor = XLColor.White;
        rango.Style.Fill.BackgroundColor = ColorPrincipal;
        rango.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        rango.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        hoja.Row(1).Height = 30;

        hoja.Cell(2, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
        hoja.Range(2, 1, 2, ultimaColumna).Merge();
        hoja.Cell(2, 1).Style.Font.Italic = true;
        hoja.Cell(2, 1).Style.Font.FontColor = XLColor.DarkSlateGray;
    }

    private static void EscribirEncabezados(IXLWorksheet hoja, IReadOnlyList<string> encabezados)
    {
        for (var columna = 0; columna < encabezados.Count; columna++)
        {
            hoja.Cell(3, columna + 1).Value = encabezados[columna];
        }
    }

    private static void AplicarFormatoFinal(IXLWorksheet hoja, int ultimaColumna)
    {
        hoja.Columns(1, ultimaColumna).AdjustToContents();
        hoja.Column(2).Width = Math.Min(Math.Max(hoja.Column(2).Width, 24), 42);
        hoja.Columns().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        hoja.SheetView.FreezeRows(3);
        hoja.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        hoja.PageSetup.FitToPages(1, 0);
    }

    private static byte[] Guardar(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
