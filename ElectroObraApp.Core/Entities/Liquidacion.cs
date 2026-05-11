using System;
using System.Collections.Generic;

namespace ElectroObraApp.Core.Entities;

public class Liquidacion : BaseEntity
{
    public Guid EmpleadoId { get; set; }
    public Empleado Empleado { get; set; } = null!;

    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    
    public decimal DiasTrabajados { get; set; }
    public decimal TarifaAplicada { get; set; }
    
    // Configuración de días especiales para esta liquidación
    public bool IncluirSabados { get; set; }
    public bool IncluirDomingos { get; set; }
    public bool IncluirFeriados { get; set; }

    public decimal MultiplicadorSabado { get; set; } = 1.0m;
    public decimal MultiplicadorDomingo { get; set; } = 1.0m;
    public decimal MultiplicadorFeriado { get; set; } = 1.0m;

    public decimal TotalBruto { get; set; } // Ahora se guarda el total calculado
    public decimal TotalAdelantos { get; set; }
    public decimal TotalNeto => TotalBruto - TotalAdelantos;

    public string? Observaciones { get; set; }
}

