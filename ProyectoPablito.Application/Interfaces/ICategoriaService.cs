using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoPablito.Application.DTOs;

namespace ProyectoPablito.Application.Interfaces;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaDto>> GetAllAsync();
    Task<bool> CreateAsync(CategoriaDto dto);
    Task<bool> UpdateAsync(CategoriaDto dto);
    Task<bool> DeleteAsync(Guid id);
}
