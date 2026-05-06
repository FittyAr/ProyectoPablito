using System;
using ProyectoPablito.Core.Enums;

namespace ProyectoPablito.Core.Entities;

public class Movimiento : BaseEntity
{
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string Concepto { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public decimal Cantidad { get; set; } = 1;
    public decimal Total => Monto * Cantidad;
    
    public Guid TipoMovimientoId { get; set; }
    public virtual TipoMovimiento TipoMovimiento { get; set; } = null!;

    public Moneda Moneda { get; set; } = Moneda.ARS;
    
    // Relaciones
    public Guid CategoriaId { get; set; }
    public virtual Categoria Categoria { get; set; } = null!;
    
    public Guid? TrabajoId { get; set; }
    // public virtual Trabajo? Trabajo { get; set; } // Se agregará después
    
    public Guid? EmpleadoId { get; set; }
    // public virtual Empleado? Empleado { get; set; } // Se agregará después
    
    public Guid? FacturaId { get; set; }
    // public virtual Factura? Factura { get; set; } // Se agregará después
}
