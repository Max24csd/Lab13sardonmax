using ClosedXML.Excel;
using Lab13sardonmax.Application.Abstractions;
using Lab13sardonmax.Application.Reportes;
using Lab13sardonmax.Domain.Entities;
using Lab13sardonmax.Infrastructure.Excel;

namespace Lab13sardonmax.Tests;

public sealed class ReportesTests
{
    [Fact]
    public async Task TicketsHandler_CalculaRespuestasYHorasDeAtencion()
    {
        var usuario = new Usuario { NombreUsuario = "maria.cliente" };
        var ticket = new Ticket
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
            Titulo = "Problema de acceso",
            Estado = "cerrado",
            Usuario = usuario,
            FechaCreacion = new DateTime(2026, 6, 1, 8, 0, 0),
            FechaCierre = new DateTime(2026, 6, 1, 10, 30, 0),
            Respuestas =
            [
                new Respuesta { Mensaje = "Solucionado" }
            ]
        };
        var handler = new ObtenerTicketsHandler(new TicketRepositoryFalso([ticket]));

        var resultado = await handler.HandleAsync(new ObtenerTicketsQuery());

        Assert.Single(resultado);
        Assert.Equal("00000001", resultado[0].Codigo);
        Assert.Equal(1, resultado[0].TotalRespuestas);
        Assert.Equal(2.5, resultado[0].HorasAtencion);
    }

    [Fact]
    public void ReporteTickets_CreaTablaNativaConDatos()
    {
        var service = new ClosedXmlReporteExcelService();
        var datos = new List<TicketReporteDto>
        {
            new("ABC12345", "Error de acceso", "maria", "cerrado",
                new DateTime(2026, 6, 1, 8, 0, 0),
                new DateTime(2026, 6, 1, 10, 0, 0), 1, 2)
        };

        var archivo = service.CrearReporteTickets(datos);

        using var workbook = new XLWorkbook(new MemoryStream(archivo));
        var hoja = workbook.Worksheet("Tickets");
        Assert.Equal("REPORTE GENERAL DE TICKETS", hoja.Cell("A1").GetString());
        Assert.Equal("Error de acceso", hoja.Cell("B4").GetString());
        Assert.Equal("TablaTickets", hoja.Table("TablaTickets").Name);
    }

    [Fact]
    public void ReporteUsuarios_CreaSegundaTablaNativa()
    {
        var service = new ClosedXmlReporteExcelService();
        var datos = new List<ActividadUsuarioReporteDto>
        {
            new("luis.soporte", "luis@ticketera.pe", "soporte",
                new DateTime(2026, 4, 2), 0, 3, 0)
        };

        var archivo = service.CrearReporteActividadUsuarios(datos);

        using var workbook = new XLWorkbook(new MemoryStream(archivo));
        var hoja = workbook.Worksheet("Actividad usuarios");
        Assert.Equal("TablaActividadUsuarios", hoja.Table("TablaActividadUsuarios").Name);
        Assert.Equal("luis.soporte", hoja.Cell("A4").GetString());
    }

    [Fact]
    public void ModificarCelda_CambiaB2YConservaElLibro()
    {
        byte[] original;
        using (var workbook = new XLWorkbook())
        {
            var hoja = workbook.Worksheets.Add("Hoja1");
            hoja.Cell("A1").Value = "Nombre";
            hoja.Cell("B2").Value = 28;
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            original = stream.ToArray();
        }

        var service = new ClosedXmlReporteExcelService();
        var modificado = service.ModificarCelda(original, "B2", 30);

        using var resultado = new XLWorkbook(new MemoryStream(modificado));
        Assert.Equal(30, resultado.Worksheet(1).Cell("B2").GetValue<int>());
        Assert.Equal("Nombre", resultado.Worksheet(1).Cell("A1").GetString());
    }

    private sealed class TicketRepositoryFalso(IReadOnlyList<Ticket> tickets)
        : ITicketRepository
    {
        public Task<IReadOnlyList<Ticket>> ListarConDetallesAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(tickets);
        }
    }
}
