using System;
using System.Collections.Generic;

namespace ProyectoPablito.Core.Entities;

public class Trabajo : BaseEntity
{
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public decimal Presupuesto { get; set; }
    public bool Finalizado { get; set; }

    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public ICollection<Movimiento> GastosEIngresos { get; set; } = new List<Movimiento>();
}
