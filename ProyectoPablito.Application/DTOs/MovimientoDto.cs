using System;
using ProyectoPablito.Core.Enums;

namespace ProyectoPablito.Application.DTOs;

public class MovimientoDto
{
    public Guid Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public decimal Cantidad { get; set; }
    public decimal Total { get; set; }
    
    public Guid TipoMovimientoId { get; set; }
    public string? TipoMovimientoNombre { get; set; }
    public bool EsIngreso { get; set; }

    public Moneda Moneda { get; set; }
    
    public Guid CategoriaId { get; set; }
    public string? CategoriaNombre { get; set; }
}
