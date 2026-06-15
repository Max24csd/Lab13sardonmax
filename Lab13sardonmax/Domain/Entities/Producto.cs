namespace Lab13sardonmax.Domain.Entities;

public sealed class Producto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
