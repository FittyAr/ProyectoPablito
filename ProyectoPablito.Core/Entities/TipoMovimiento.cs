using System.Collections.Generic;

namespace ProyectoPablito.Core.Entities;

public class TipoMovimiento : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    
    /// <summary>
    /// Indica si el movimiento suma al balance (true) o resta (false).
    /// </summary>
    public bool EsIngreso { get; set; }
    
    /// <summary>
    /// Indica si es un tipo de sistema que no debería ser borrado.
    /// </summary>
    public bool EsSistema { get; set; }

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
