using System.Collections.Generic;

namespace ProyectoPablito.Core.Entities;

public class Categoria : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ColorHex { get; set; }
    public string? Icono { get; set; }
    
    // Relación con Movimientos
    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
