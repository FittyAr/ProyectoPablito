using System;
using ElectroObraApp.Core.Enums;

namespace ElectroObraApp.Application.DTOs;

public class EmpleadoDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Dni { get; set; }
    public string? Cargo { get; set; }
    public decimal SueldoBase { get; set; }
    public PaymentFrequency PagoFrecuencia { get; set; }
    public decimal TarifaDiaria { get; set; }
    public DateTime FechaIngreso { get; set; }
    public bool Activo { get; set; }
}

