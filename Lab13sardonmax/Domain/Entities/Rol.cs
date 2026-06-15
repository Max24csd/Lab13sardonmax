namespace Lab13sardonmax.Domain.Entities;

public sealed class Rol
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public ICollection<UsuarioRol> Usuarios { get; set; } = new List<UsuarioRol>();
}
