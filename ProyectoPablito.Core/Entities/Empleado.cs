using System;
using System.Collections.Generic;

namespace ProyectoPablito.Core.Entities;

public class Empleado : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Dni { get; set; }
    public string? Cargo { get; set; }
    public decimal SueldoBase { get; set; }
    public DateTime FechaIngreso { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Movimiento> AdelantosYPagos { get; set; } = new List<Movimiento>();
}
