using System;

namespace ProyectoPablito.Application.DTOs;

public class TrabajoDto
{
    public Guid Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public decimal Presupuesto { get; set; }
    public bool Finalizado { get; set; }
    
    public Guid ClienteId { get; set; }
    public string? ClienteNombre { get; set; }
}
