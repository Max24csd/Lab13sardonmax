using Lab13sardonmax.Application.Abstractions;

namespace Lab13sardonmax.Application.Reportes;

public sealed record ObtenerTicketsQuery;

public sealed class ObtenerTicketsHandler(ITicketRepository repository)
    : IQueryHandler<ObtenerTicketsQuery, IReadOnlyList<TicketReporteDto>>
{
    public async Task<IReadOnlyList<TicketReporteDto>> HandleAsync(
        ObtenerTicketsQuery query,
        CancellationToken cancellationToken = default)
    {
        var tickets = await repository.ListarConDetallesAsync(cancellationToken);

        return tickets
            .OrderByDescending(ticket => ticket.FechaCreacion)
            .Select(ticket => new TicketReporteDto(
                ticket.Id.ToString()[^8..].ToUpperInvariant(),
                ticket.Titulo,
                ticket.Usuario.NombreUsuario,
                ticket.Estado,
                ticket.FechaCreacion,
                ticket.FechaCierre,
                ticket.Respuestas.Count,
                Math.Round(((ticket.FechaCierre ?? DateTime.Now) - ticket.FechaCreacion).TotalHours, 1)))
            .ToList();
    }
}
