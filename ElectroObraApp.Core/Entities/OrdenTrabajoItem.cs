using System;

namespace ElectroObraApp.Core.Entities;

public class OrdenTrabajoItem : BaseEntity
{
    public Guid OrdenTrabajoId { get; set; }
    public OrdenTrabajo OrdenTrabajo { get; set; } = null!;

    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public string Unidad { get; set; } = "u"; // m, u, kg, etc.
    public decimal PrecioUnitario { get; set; }
    
    public decimal PorcentajeAnterior { get; set; } // Avance acumulado anterior
    public decimal PorcentajeActual { get; set; }   // Avance de este certificado
    public decimal PorcentajeAcumulado => PorcentajeAnterior + PorcentajeActual;

    public decimal SubtotalActual => Cantidad * PrecioUnitario * (PorcentajeActual / 100);
    public decimal SubtotalAcumulado => Cantidad * PrecioUnitario * (PorcentajeAcumulado / 100);
}

