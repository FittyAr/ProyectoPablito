using System.Collections.Generic;
using System.Threading.Tasks;
using ElectroObraApp.Application.DTOs;

namespace ElectroObraApp.Application.Interfaces;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaDto>> GetAllAsync();
    Task<CategoriaDto?> GetByIdAsync(Guid id);
    Task<bool> CreateAsync(CategoriaDto dto);
    Task<bool> UpdateAsync(CategoriaDto dto);
    Task<bool> DeleteAsync(Guid id);
}

