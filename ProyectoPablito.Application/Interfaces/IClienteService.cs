using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoPablito.Application.DTOs;

namespace ProyectoPablito.Application.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<ClienteDto>> GetAllAsync();
    Task<ClienteDto?> GetByIdAsync(Guid id);
    Task<bool> CreateAsync(ClienteDto dto);
    Task<bool> UpdateAsync(ClienteDto dto);
    Task<bool> DeleteAsync(Guid id);
}
