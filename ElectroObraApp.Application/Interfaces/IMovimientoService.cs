using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectroObraApp.Application.DTOs;

namespace ElectroObraApp.Application.Interfaces;

public interface IMovimientoService
{
    Task<IEnumerable<MovimientoDto>> GetAllAsync();
    Task<MovimientoDto?> GetByIdAsync(Guid id);
    Task<bool> CreateAsync(MovimientoDto dto);
    Task<bool> UpdateAsync(MovimientoDto dto);
    Task<bool> DeleteAsync(Guid id);
}

