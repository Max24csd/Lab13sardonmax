using Lab13sardonmax.Application.Abstractions;
using Lab13sardonmax.Domain.Entities;
using Lab13sardonmax.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab13sardonmax.Infrastructure.Repositories;

public sealed class TicketRepository(LaboratorioDbContext dbContext) : ITicketRepository
{
    public async Task<IReadOnlyList<Ticket>> ListarConDetallesAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Tickets
            .AsNoTracking()
            .Include(ticket => ticket.Usuario)
            .Include(ticket => ticket.Respuestas)
            .ToListAsync(cancellationToken);
    }
}
