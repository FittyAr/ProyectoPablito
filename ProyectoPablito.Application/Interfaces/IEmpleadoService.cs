using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoPablito.Application.DTOs;

namespace ProyectoPablito.Application.Interfaces;

public interface IEmpleadoService
{
    Task<IEnumerable<EmpleadoDto>> GetAllAsync();
    Task<bool> CreateAsync(EmpleadoDto dto);
    Task<bool> UpdateAsync(EmpleadoDto dto);
    Task<bool> DeleteAsync(Guid id);
}
