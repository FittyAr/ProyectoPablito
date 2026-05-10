using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.Extensions.Logging;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Interfaces;
using ElectroObraApp.Core.Entities;
using ElectroObraApp.Core.Interfaces;

namespace ElectroObraApp.Application.Services;

public class TrabajoService : ITrabajoService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<TrabajoService> _logger;

    public TrabajoService(IUnitOfWork uow, ILogger<TrabajoService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<IEnumerable<TrabajoDto>> GetAllAsync()
    {
        // Incluye Cliente para mostrar ClienteNombre en la lista
        var entities = await _uow.Trabajos.GetAllWithDeepLoadAsync();
        return entities.Adapt<IEnumerable<TrabajoDto>>();
    }

    public async Task<TrabajoDto?> GetByIdAsync(Guid id)
    {
        // CRÍTICO: Incluye OrdenesTrabajo → Items para el formulario de edición
        var entity = await _uow.Trabajos.GetByIdWithDeepLoadAsync(id);
        return entity?.Adapt<TrabajoDto>();
    }

    public async Task<bool> CreateAsync(TrabajoDto dto)
    {
        _logger.LogInformation("Registrando nuevo trabajo: {Descripcion}", dto.Descripcion);
        var entity = dto.Adapt<Trabajo>();
        await _uow.Repository<Trabajo>().AddAsync(entity);
        return await _uow.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(TrabajoDto dto)
    {
        _logger.LogInformation("Actualizando trabajo: {Id}", dto.Id);
        var entity = dto.Adapt<Trabajo>();
        _uow.Repository<Trabajo>().Update(entity);
        return await _uow.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Eliminando trabajo: {Id}", id);
        var repo = _uow.Repository<Trabajo>();
        var entity = await repo.GetByIdAsync(id);
        if (entity == null) return false;
        repo.Remove(entity);
        return await _uow.SaveChangesAsync() > 0;
    }
}

