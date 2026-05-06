using System;

namespace ProyectoPablito.Application.DTOs;

public class CategoriaDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ColorHex { get; set; }
    public string? Icono { get; set; }
}
