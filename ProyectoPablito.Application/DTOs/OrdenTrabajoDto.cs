using System;
using System.Collections.Generic;

namespace ProyectoPablito.Application.DTOs;

public class OrdenTrabajoDto
{
    public Guid Id { get; set; }
    public Guid TrabajoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? NumeroCertificado { get; set; }
    public DateTime Fecha { get; set; }
    public string? Observaciones { get; set; }
    public decimal AjusteUocraPorcentaje { get; set; }
    public decimal OtrosDescuentos { get; set; }
    public List<OrdenTrabajoItemDto> Items { get; set; } = new();
}
