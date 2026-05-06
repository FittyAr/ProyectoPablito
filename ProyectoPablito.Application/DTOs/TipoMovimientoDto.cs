using System;

namespace ProyectoPablito.Application.DTOs;

public class TipoMovimientoDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool EsIngreso { get; set; }
}
