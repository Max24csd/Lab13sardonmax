namespace Lab13sardonmax.Domain.Entities;

public sealed class Ticket
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaCierre { get; set; }
    public ICollection<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
}
