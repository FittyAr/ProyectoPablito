using System;
using System.Collections.Generic;

namespace ProyectoPablito.Core.Entities;

public class Cliente : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Cuit { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? CondicionIva { get; set; } // Responsable Inscripto, Monotributo, etc.
    
    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
