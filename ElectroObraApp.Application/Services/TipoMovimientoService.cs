using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Interfaces;
using ElectroObraApp.Core.Entities;
using ElectroObraApp.Core.Interfaces;

namespace ElectroObraApp.Application.Services;

public class TipoMovimientoService : ITipoMovimientoService
{
    private readonly IUnitOfWork _uow;

    public TipoMovimientoService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<TipoMovimientoDto>> GetAllAsync()
    {
        var entities = await _uow.Repository<TipoMovimiento>().GetAllAsync();
        return entities.Adapt<IEnumerable<TipoMovimientoDto>>();
    }
}

