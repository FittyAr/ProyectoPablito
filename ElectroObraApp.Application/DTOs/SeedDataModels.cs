using System.Collections.Generic;

namespace ElectroObraApp.Application.DTOs;

public class SeedDataPool
{
    public List<string> Categorias { get; set; } = new();
    public List<string> TiposMovimiento { get; set; } = new();
    public List<string> NombresClientes { get; set; } = new();
    public List<string> NombresEmpleados { get; set; } = new();
    public List<string> ConceptosMovimientos { get; set; } = new();
    public List<string> DescripcionesTrabajos { get; set; } = new();
}

