using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.Extensions.Logging;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;
using ProyectoPablito.Core.Entities;
using ProyectoPablito.Core.Interfaces;

namespace ProyectoPablito.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(IUnitOfWork uow, ILogger<ClienteService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<IEnumerable<ClienteDto>> GetAllAsync()
    {
        var entities = await _uow.Repository<Cliente>().GetAllAsync();
        return entities.Adapt<IEnumerable<ClienteDto>>();
    }

    public async Task<ClienteDto?> GetByIdAsync(Guid id)
    {
        var entity = await _uow.Repository<Cliente>().GetByIdAsync(id);
        return entity?.Adapt<ClienteDto>();
    }

    public async Task<bool> CreateAsync(ClienteDto dto)
    {
        _logger.LogInformation("Creando cliente: {Nombre}", dto.Nombre);
        var entity = dto.Adapt<Cliente>();
        await _uow.Repository<Cliente>().AddAsync(entity);
        return await _uow.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(ClienteDto dto)
    {
        _logger.LogInformation("Actualizando cliente: {Id}", dto.Id);
        var entity = dto.Adapt<Cliente>();
        _uow.Repository<Cliente>().Update(entity);
        return await _uow.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Eliminando cliente: {Id}", id);
        var repo = _uow.Repository<Cliente>();
        var entity = await repo.GetByIdAsync(id);
        if (entity == null) return false;
        repo.Remove(entity);
        return await _uow.SaveChangesAsync() > 0;
    }
}
