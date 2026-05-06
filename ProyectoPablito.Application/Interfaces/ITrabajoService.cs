using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoPablito.Application.DTOs;

namespace ProyectoPablito.Application.Interfaces;

public interface ITrabajoService
{
    Task<IEnumerable<TrabajoDto>> GetAllAsync();
    Task<bool> CreateAsync(TrabajoDto dto);
    Task<bool> UpdateAsync(TrabajoDto dto);
    Task<bool> DeleteAsync(Guid id);
}
