using System;

namespace ElectroObraApp.Core.Entities;

public class ClienteContacto : BaseEntity
{
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public string Email { get; set; } = string.Empty;
    public string Etiqueta { get; set; } = "General"; // Personal, Oficina, etc.
}

