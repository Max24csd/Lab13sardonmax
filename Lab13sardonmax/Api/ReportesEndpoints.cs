using Lab13sardonmax.Application.Abstractions;
using Lab13sardonmax.Application.Reportes;

namespace Lab13sardonmax.Api;

public static class ReportesEndpoints
{
    private const string TipoExcel =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public static IEndpointRouteBuilder MapReportesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var grupo = endpoints.MapGroup("/api/reportes")
            .WithTags("Reportes de Ticketera");

        grupo.MapGet("/tickets/datos", ObtenerTickets)
            .WithSummary("Consulta los tickets de la base de datos")
            .Produces<IReadOnlyList<TicketReporteDto>>();
        grupo.MapGet("/tickets/excel", DescargarTickets)
            .WithSummary("Genera el reporte Excel general de tickets")
            .Produces(StatusCodes.Status200OK, contentType: TipoExcel);
        grupo.MapGet("/usuarios/datos", ObtenerActividadUsuarios)
            .WithSummary("Consulta la actividad de usuarios")
            .Produces<IReadOnlyList<ActividadUsuarioReporteDto>>();
        grupo.MapGet("/usuarios/excel", DescargarActividadUsuarios)
            .WithSummary("Genera el reporte Excel de actividad de usuarios")
            .Produces(StatusCodes.Status200OK, contentType: TipoExcel);
        grupo.MapGet("/resumen", ObtenerResumen)
            .WithSummary("Obtiene indicadores generales de la ticketera")
            .Produces<ResumenReportesDto>();
        grupo.MapPost("/modificar-excel", ModificarExcel)
            .WithSummary("Sube un Excel, cambia la celda B2 a 30 y descarga el resultado")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(StatusCodes.Status200OK, contentType: TipoExcel)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();

        return endpoints;
    }

    private static async Task<IResult> ObtenerTickets(
        IQueryHandler<ObtenerTicketsQuery, IReadOnlyList<TicketReporteDto>> handler,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await handler.HandleAsync(new ObtenerTicketsQuery(), cancellationToken));
    }

    private static async Task<IResult> DescargarTickets(
        IQueryHandler<ObtenerTicketsQuery, IReadOnlyList<TicketReporteDto>> handler,
        IReporteExcelService excelService,
        CancellationToken cancellationToken)
    {
        var datos = await handler.HandleAsync(new ObtenerTicketsQuery(), cancellationToken);
        return Results.File(
            excelService.CrearReporteTickets(datos),
            TipoExcel,
            $"reporte-tickets-{DateTime.Now:yyyyMMdd}.xlsx");
    }

    private static async Task<IResult> ObtenerActividadUsuarios(
        IQueryHandler<ObtenerActividadUsuariosQuery, IReadOnlyList<ActividadUsuarioReporteDto>> handler,
        CancellationToken cancellationToken)
    {
        return Results.Ok(
            await handler.HandleAsync(new ObtenerActividadUsuariosQuery(), cancellationToken));
    }

    private static async Task<IResult> DescargarActividadUsuarios(
        IQueryHandler<ObtenerActividadUsuariosQuery, IReadOnlyList<ActividadUsuarioReporteDto>> handler,
        IReporteExcelService excelService,
        CancellationToken cancellationToken)
    {
        var datos = await handler.HandleAsync(
            new ObtenerActividadUsuariosQuery(),
            cancellationToken);
        return Results.File(
            excelService.CrearReporteActividadUsuarios(datos),
            TipoExcel,
            $"reporte-actividad-usuarios-{DateTime.Now:yyyyMMdd}.xlsx");
    }

    private static async Task<IResult> ObtenerResumen(
        IQueryHandler<ObtenerTicketsQuery, IReadOnlyList<TicketReporteDto>> ticketsHandler,
        IQueryHandler<ObtenerActividadUsuariosQuery, IReadOnlyList<ActividadUsuarioReporteDto>> usuariosHandler,
        CancellationToken cancellationToken)
    {
        var tickets = await ticketsHandler.HandleAsync(new ObtenerTicketsQuery(), cancellationToken);
        var usuarios = await usuariosHandler.HandleAsync(
            new ObtenerActividadUsuariosQuery(),
            cancellationToken);

        return Results.Ok(new ResumenReportesDto(
            tickets.Count,
            tickets.Count(ticket => ticket.Estado == "abierto"),
            tickets.Count(ticket => ticket.Estado == "en_proceso"),
            tickets.Count(ticket => ticket.Estado == "cerrado"),
            usuarios.Count,
            tickets.Sum(ticket => ticket.TotalRespuestas)));
    }

    private static async Task<IResult> ModificarExcel(
        IFormFile archivo,
        IReporteExcelService excelService,
        CancellationToken cancellationToken)
    {
        if (archivo.Length == 0 ||
            !string.Equals(Path.GetExtension(archivo.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                "Selecciona un archivo valido con extension .xlsx.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        await using var stream = new MemoryStream();
        await archivo.CopyToAsync(stream, cancellationToken);

        try
        {
            var modificado = excelService.ModificarCelda(stream.ToArray(), "B2", 30);
            var nombre = $"{Path.GetFileNameWithoutExtension(archivo.FileName)}-modificado.xlsx";
            return Results.File(modificado, TipoExcel, nombre);
        }
        catch
        {
            return Results.Problem(
                "El archivo no pudo abrirse como un libro Excel valido.",
                statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
