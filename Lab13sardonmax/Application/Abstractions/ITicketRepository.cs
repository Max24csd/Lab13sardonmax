using Lab13sardonmax.Domain.Entities;

namespace Lab13sardonmax.Application.Abstractions;

public interface ITicketRepository
{
    Task<IReadOnlyList<Ticket>> ListarConDetallesAsync(
        CancellationToken cancellationToken = default);
}
