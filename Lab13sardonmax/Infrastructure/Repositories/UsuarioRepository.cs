using Lab13sardonmax.Application.Abstractions;
using Lab13sardonmax.Domain.Entities;
using Lab13sardonmax.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab13sardonmax.Infrastructure.Repositories;

public sealed class UsuarioRepository(LaboratorioDbContext dbContext) : IUsuarioRepository
{
    public async Task<IReadOnlyList<Usuario>> ListarConActividadAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Usuarios
            .AsNoTracking()
            .Include(usuario => usuario.Roles)
            .ThenInclude(usuarioRol => usuarioRol.Rol)
            .Include(usuario => usuario.Tickets)
            .Include(usuario => usuario.Respuestas)
            .ToListAsync(cancellationToken);
    }
}
