namespace Lab13sardonmax.Domain.Entities;

public sealed class Venta
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
