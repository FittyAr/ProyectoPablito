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

public class CategoriaService : ICategoriaService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<CategoriaService> _logger;

    public CategoriaService(IUnitOfWork uow, ILogger<CategoriaService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<IEnumerable<CategoriaDto>> GetAllAsync()
    {
        var entities = await _uow.Repository<Categoria>().GetAllAsync();
        return entities.Adapt<IEnumerable<CategoriaDto>>();
    }

    public async Task<bool> CreateAsync(CategoriaDto dto)
    {
        _logger.LogInformation("Creando categoría: {Nombre}", dto.Nombre);
        var entity = dto.Adapt<Categoria>();
        await _uow.Repository<Categoria>().AddAsync(entity);
        return await _uow.SaveChangesAsync() > 0;
    }
}
