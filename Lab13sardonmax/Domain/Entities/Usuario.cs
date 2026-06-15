namespace Lab13sardonmax.Domain.Entities;

public sealed class Usuario
{
    public Guid Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public ICollection<UsuarioRol> Roles { get; set; } = new List<UsuarioRol>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
}
