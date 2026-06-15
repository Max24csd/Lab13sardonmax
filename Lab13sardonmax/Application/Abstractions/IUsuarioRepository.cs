using Lab13sardonmax.Domain.Entities;

namespace Lab13sardonmax.Application.Abstractions;

public interface IUsuarioRepository
{
    Task<IReadOnlyList<Usuario>> ListarConActividadAsync(
        CancellationToken cancellationToken = default);
}
