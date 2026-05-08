using System;

namespace ProyectoPablito.Application.DTOs;

public class ClienteContactoDto
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Etiqueta { get; set; } = "General";
}
