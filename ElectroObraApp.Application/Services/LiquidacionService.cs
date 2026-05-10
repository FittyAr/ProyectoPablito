using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.Extensions.Logging;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Interfaces;
using ElectroObraApp.Core.Entities;
using ElectroObraApp.Core.Interfaces;

namespace ElectroObraApp.Application.Services;

public class LiquidacionService : ILiquidacionService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<LiquidacionService> _logger;

    public LiquidacionService(IUnitOfWork uow, ILogger<LiquidacionService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<IEnumerable<LiquidacionDto>> GetAllAsync()
    {
        // CRÍTICO: Usar repo especializado que hace Include(Empleado)
        var entities = await _uow.Liquidaciones.GetAllWithEmpleadoAsync();
        return entities.Adapt<IEnumerable<LiquidacionDto>>();
    }

    public async Task<LiquidacionDto?> GetByIdAsync(Guid id)
    {
        var entity = await _uow.Repository<Liquidacion>().GetByIdAsync(id);
        return entity?.Adapt<LiquidacionDto>();
    }

    public async Task<LiquidacionDto> CreateAsync(LiquidacionDto dto)
    {
        var entity = dto.Adapt<Liquidacion>();
        await _uow.Repository<Liquidacion>().AddAsync(entity);
        await _uow.SaveChangesAsync();
        return entity.Adapt<LiquidacionDto>();
    }

    public async Task<bool> UpdateAsync(LiquidacionDto dto)
    {
        var entity = await _uow.Repository<Liquidacion>().GetByIdAsync(dto.Id);
        if (entity == null) return false;

        dto.Adapt(entity);
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Repository<Liquidacion>().Update(entity);
        return await _uow.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _uow.Repository<Liquidacion>().GetByIdAsync(id);
        if (entity == null) return false;

        _uow.Repository<Liquidacion>().Remove(entity);
        return await _uow.SaveChangesAsync() > 0;
    }

    public async Task<LiquidacionDto> SugerirLiquidacionAsync(Guid empleadoId, DateTime inicio, DateTime fin, decimal diasTrabajados)
    {
        var empleado = await _uow.Repository<Empleado>().GetByIdAsync(empleadoId);
        if (empleado == null) throw new Exception("Empleado no encontrado");

        // Buscar adelantos (Movimientos) en el periodo para este empleado
        // Importante: Usamos FindAsync con filtro por EmpleadoId y rango de fechas
        var movimientos = await _uow.Movimientos.FindAsync(m => 
            m.EmpleadoId == empleadoId && 
            m.Fecha >= inicio && 
            m.Fecha <= fin);
            
        // Filtrar por tipo "Adelanto" (id 3 según seed data o por nombre si fuera dinámico)
        // Por seguridad usamos el ID del seed data: 00000000-0000-0000-0000-000000000003
        var adelantoTypeId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var adelantos = movimientos.Where(m => m.TipoMovimientoId == adelantoTypeId).ToList();

        var totalAdelantos = adelantos.Sum(m => m.Total);

        return new LiquidacionDto
        {
            EmpleadoId = empleadoId,
            EmpleadoNombre = empleado.Nombre,
            FechaInicio = inicio,
            FechaFin = fin,
            DiasTrabajados = diasTrabajados,
            TarifaAplicada = empleado.TarifaDiaria,
            TotalAdelantos = totalAdelantos,
            TotalBruto = diasTrabajados * empleado.TarifaDiaria,
            TotalNeto = (diasTrabajados * empleado.TarifaDiaria) - totalAdelantos
        };
    }
}

