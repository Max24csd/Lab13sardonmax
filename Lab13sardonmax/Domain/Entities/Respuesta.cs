namespace Lab13sardonmax.Domain.Entities;

public sealed class Respuesta
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public Guid RespondedorId { get; set; }
    public Usuario Respondedor { get; set; } = null!;
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}
