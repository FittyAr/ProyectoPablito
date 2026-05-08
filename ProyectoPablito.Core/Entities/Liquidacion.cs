using System;
using System.Collections.Generic;

namespace ProyectoPablito.Core.Entities;

public class Liquidacion : BaseEntity
{
    public Guid EmpleadoId { get; set; }
    public Empleado Empleado { get; set; } = null!;

    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    
    public decimal DiasTrabajados { get; set; }
    public decimal TarifaAplicada { get; set; }
    
    public decimal TotalBruto => DiasTrabajados * TarifaAplicada;
    public decimal TotalAdelantos { get; set; }
    public decimal TotalNeto => TotalBruto - TotalAdelantos;

    public string? Observaciones { get; set; }
}
