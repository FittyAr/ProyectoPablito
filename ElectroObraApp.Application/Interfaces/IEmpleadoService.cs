using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectroObraApp.Application.DTOs;

namespace ElectroObraApp.Application.Interfaces;

public interface IEmpleadoService
{
    Task<IEnumerable<EmpleadoDto>> GetAllAsync();
    Task<bool> CreateAsync(EmpleadoDto dto);
    Task<bool> UpdateAsync(EmpleadoDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<EmpleadoDto?> GetByIdAsync(Guid id);
}
