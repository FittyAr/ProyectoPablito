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
        var entities = await _uow.Clientes.GetAllWithContactosAsync();
        return entities.Adapt<IEnumerable<ClienteDto>>();
    }

    public async Task<ClienteDto?> GetByIdAsync(Guid id)
    {
        var entity = await _uow.Clientes.GetByIdWithContactosAsync(id);
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
        await _uow.Clientes.UpdateWithContactosAsync(entity);
        return await _uow.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Eliminando cliente: {Id}", id);
        var entity = await _uow.Repository<Cliente>().GetByIdAsync(id);
        if (entity == null) return false;
        _uow.Repository<Cliente>().Remove(entity);
        return await _uow.SaveChangesAsync() > 0;
    }
}

