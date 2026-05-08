using System;
using System.Collections.Generic;

namespace ProyectoPablito.Core.Entities;

public class OrdenTrabajo : BaseEntity
{
    public Guid TrabajoId { get; set; }
    public Trabajo Trabajo { get; set; } = null!;

    public string Titulo { get; set; } = string.Empty;
    public string? NumeroCertificado { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string? Observaciones { get; set; }
    
    public decimal AjusteUocraPorcentaje { get; set; } // Ej: 8% como se ve en la foto
    public decimal OtrosDescuentos { get; set; }

    public ICollection<OrdenTrabajoItem> Items { get; set; } = new List<OrdenTrabajoItem>();
}
