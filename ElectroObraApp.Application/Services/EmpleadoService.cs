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

public class EmpleadoService : IEmpleadoService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<EmpleadoService> _logger;

    public EmpleadoService(IUnitOfWork uow, ILogger<EmpleadoService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<IEnumerable<EmpleadoDto>> GetAllAsync()
    {
        var entities = await _uow.Repository<Empleado>().GetAllAsync();
        return entities.Adapt<IEnumerable<EmpleadoDto>>();
    }

    public async Task<bool> CreateAsync(EmpleadoDto dto)
    {
        _logger.LogInformation("Registrando empleado: {Nombre}", dto.Nombre);
        var entity = dto.Adapt<Empleado>();
        await _uow.Repository<Empleado>().AddAsync(entity);
        return await _uow.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(EmpleadoDto dto)
    {
        _logger.LogInformation("Actualizando empleado: {Id}", dto.Id);
        var entity = dto.Adapt<Empleado>();
        _uow.Repository<Empleado>().Update(entity);
        return await _uow.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Eliminando empleado: {Id}", id);
        var repo = _uow.Repository<Empleado>();
        var entity = await repo.GetByIdAsync(id);
        if (entity == null) return false;
        repo.Remove(entity);
        return await _uow.SaveChangesAsync() > 0;
    }
}

