using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;
using ProyectoPablito.Core.Entities;
using ProyectoPablito.Core.Interfaces;

using Microsoft.Extensions.Logging;

namespace ProyectoPablito.Application.Services;

public class MovimientoService : IMovimientoService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<MovimientoService> _logger;

    public MovimientoService(IUnitOfWork uow, ILogger<MovimientoService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<IEnumerable<MovimientoDto>> GetAllAsync()
    {
        var entities = await _uow.Repository<Movimiento>().GetAllAsync();
        return entities.Adapt<IEnumerable<MovimientoDto>>();
    }

    public async Task<MovimientoDto?> GetByIdAsync(Guid id)
    {
        var entity = await _uow.Repository<Movimiento>().GetByIdAsync(id);
        return entity?.Adapt<MovimientoDto>();
    }

    public async Task<bool> CreateAsync(MovimientoDto dto)
    {
        _logger.LogInformation("Iniciando creación de movimiento: {Concepto} por {Monto}", dto.Concepto, dto.Monto);
        var entity = dto.Adapt<Movimiento>();
        await _uow.Repository<Movimiento>().AddAsync(entity);
        var result = await _uow.SaveChangesAsync() > 0;
        
        if (result)
            _logger.LogInformation("Movimiento creado exitosamente con ID: {Id}", entity.Id);
        else
            _logger.LogWarning("No se pudo persistir el movimiento: {Concepto}", dto.Concepto);

        return result;
    }

    public async Task<bool> UpdateAsync(MovimientoDto dto)
    {
        var entity = await _uow.Repository<Movimiento>().GetByIdAsync(dto.Id);
        if (entity == null) return false;

        dto.Adapt(entity);
        entity.UpdatedAt = DateTime.UtcNow;
        
        _uow.Repository<Movimiento>().Update(entity);
        return await _uow.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Intentando eliminar movimiento con ID: {Id}", id);
        var repo = _uow.Repository<Movimiento>();
        var entity = await repo.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("No se encontró el movimiento con ID: {Id} para eliminar", id);
            return false;
        }

        repo.Remove(entity);
        var result = await _uow.SaveChangesAsync() > 0;
        
        if (result)
            _logger.LogInformation("Movimiento con ID: {Id} eliminado correctamente", id);
            
        return result;
    }
}
