using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;
using ProyectoPablito.Core.Entities;
using ProyectoPablito.Core.Interfaces;

namespace ProyectoPablito.Application.Services;

public class MovimientoService : IMovimientoService
{
    private readonly IUnitOfWork _uow;

    public MovimientoService(IUnitOfWork uow)
    {
        _uow = uow;
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
        var entity = dto.Adapt<Movimiento>();
        await _uow.Repository<Movimiento>().AddAsync(entity);
        return await _uow.SaveChangesAsync() > 0;
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
        var entity = await _uow.Repository<Movimiento>().GetByIdAsync(id);
        if (entity == null) return false;

        _uow.Repository<Movimiento>().Remove(entity);
        return await _uow.SaveChangesAsync() > 0;
    }
}
