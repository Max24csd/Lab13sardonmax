using Lab13sardonmax.Application.Abstractions;

namespace Lab13sardonmax.Application.Reportes;

public sealed record ObtenerActividadUsuariosQuery;

public sealed class ObtenerActividadUsuariosHandler(IUsuarioRepository repository)
    : IQueryHandler<ObtenerActividadUsuariosQuery, IReadOnlyList<ActividadUsuarioReporteDto>>
{
    public async Task<IReadOnlyList<ActividadUsuarioReporteDto>> HandleAsync(
        ObtenerActividadUsuariosQuery query,
        CancellationToken cancellationToken = default)
    {
        var usuarios = await repository.ListarConActividadAsync(cancellationToken);

        return usuarios
            .OrderBy(usuario => usuario.NombreUsuario)
            .Select(usuario => new ActividadUsuarioReporteDto(
                usuario.NombreUsuario,
                usuario.Correo ?? "Sin correo",
                string.Join(", ", usuario.Roles.Select(usuarioRol => usuarioRol.Rol.Nombre)),
                usuario.FechaCreacion,
                usuario.Tickets.Count,
                usuario.Respuestas.Count,
                usuario.Tickets.Count(ticket => ticket.Estado == "cerrado")))
            .ToList();
    }
}
