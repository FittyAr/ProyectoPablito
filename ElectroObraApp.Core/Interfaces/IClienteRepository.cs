using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectroObraApp.Core.Entities;

namespace ElectroObraApp.Core.Interfaces;

/// <summary>
/// Repositorio específico para Cliente que garantiza la carga
/// de Contactos y permite actualizaciones correctas con relaciones.
/// </summary>
public interface IClienteRepository : IRepository<Cliente>
{
    Task<IEnumerable<Cliente>> GetAllWithContactosAsync();
    Task<Cliente?> GetByIdWithContactosAsync(Guid id);
    Task UpdateWithContactosAsync(Cliente cliente);
}

