using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectroObraApp.Application.DTOs;

namespace ElectroObraApp.Application.Interfaces;

public interface ILiquidacionService
{
    Task<IEnumerable<LiquidacionDto>> GetAllAsync();
    Task<LiquidacionDto?> GetByIdAsync(Guid id);
    Task<LiquidacionDto> CreateAsync(LiquidacionDto dto);
    Task<bool> UpdateAsync(LiquidacionDto dto);
    Task<bool> DeleteAsync(Guid id);
    
    /// <summary>
    /// Calcula una pre-liquidación basada en el empleado y rango de fechas.
    /// Busca adelantos registrados en Movimientos.
    /// </summary>
    Task<LiquidacionDto> SugerirLiquidacionAsync(Guid empleadoId, DateTime inicio, DateTime fin, decimal diasTrabajados);
}

