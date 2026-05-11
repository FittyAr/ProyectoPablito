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

        // Configuración por defecto (esto podría venir de parámetros o de IUserSettingsService inyectado)
        // Por ahora usamos valores que el VM pasará o calculamos aquí.
        // Nota: Como este método es de "sugerencia", calculamos el valor ideal.
        
        var totalDias = 0m;
        var totalBruto = 0m;
        
        // Obtenemos los feriados del config si es necesario (inyectar IUserSettingsService si se prefiere)
        // Por ahora, simularemos la lógica que el VM enviará, pero implementamos el cálculo base aquí.
        
        for (var date = inicio.Date; date <= fin.Date; date = date.AddDays(1))
        {
            var esSabado = date.DayOfWeek == DayOfWeek.Saturday;
            var esDomingo = date.DayOfWeek == DayOfWeek.Sunday;
            
            // Lógica por defecto: Lunes a Viernes se abonan 1.0. 
            // Sabados y Domingos NO se abonan por defecto (0.0).
            var multiplicador = 1.0m;
            
            if (esSabado || esDomingo)
            {
                multiplicador = 0.0m;
            }
            
            if (multiplicador > 0)
            {
                totalDias += 1.0m;
                totalBruto += empleado.TarifaDiaria * multiplicador;
            }
        }

        // Buscar adelantos
        var movimientos = await _uow.Movimientos.FindAsync(m => 
            m.EmpleadoId == empleadoId && 
            m.Fecha >= inicio && 
            m.Fecha <= fin);
            
        var adelantoTypeId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var adelantos = movimientos.Where(m => m.TipoMovimientoId == adelantoTypeId).ToList();
        var totalAdelantos = adelantos.Sum(m => m.Total);

        return new LiquidacionDto
        {
            EmpleadoId = empleadoId,
            EmpleadoNombre = empleado.Nombre,
            FechaInicio = inicio,
            FechaFin = fin,
            DiasTrabajados = totalDias,
            TarifaAplicada = empleado.TarifaDiaria,
            TotalAdelantos = totalAdelantos,
            TotalBruto = totalBruto,
            TotalNeto = totalBruto - totalAdelantos,
            // Valores por defecto para la UI
            IncluirSabados = false,
            IncluirDomingos = false,
            IncluirFeriados = false,
            MultiplicadorSabado = 1.0m,
            MultiplicadorDomingo = 1.0m,
            MultiplicadorFeriado = 1.0m
        };
    }
}

