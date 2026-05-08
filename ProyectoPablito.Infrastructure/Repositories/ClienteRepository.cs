using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProyectoPablito.Core.Entities;
using ProyectoPablito.Core.Interfaces;
using ProyectoPablito.Infrastructure.Data;

namespace ProyectoPablito.Infrastructure.Repositories;

public class ClienteRepository : Repository<Cliente>, IClienteRepository
{
    public ClienteRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Cliente>> GetAllWithContactosAsync()
    {
        return await _context.Clientes
            .AsNoTracking()
            .Include(c => c.Contactos)
            .ToListAsync();
    }

    public async Task<Cliente?> GetByIdWithContactosAsync(Guid id)
    {
        return await _context.Clientes
            .AsNoTracking()
            .Include(c => c.Contactos)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Actualiza el cliente y sincroniza sus contactos correctamente,
    /// manejando inserts de nuevos y deletes de eliminados.
    /// </summary>
    public async Task UpdateWithContactosAsync(Cliente cliente)
    {
        // 1. Cargar la entidad existente CON sus contactos (sin AsNoTracking)
        var existing = await _context.Clientes
            .Include(c => c.Contactos)
            .FirstOrDefaultAsync(c => c.Id == cliente.Id);

        if (existing == null) return;

        // 2. Actualizar propiedades escalares
        _context.Entry(existing).CurrentValues.SetValues(cliente);

        // 3. Sincronizar contactos
        // - Eliminar los que ya no están
        var contactosToDelete = existing.Contactos
            .Where(ec => !cliente.Contactos.Any(nc => nc.Id == ec.Id))
            .ToList();
        _context.Set<ClienteContacto>().RemoveRange(contactosToDelete);

        // - Actualizar los existentes y agregar los nuevos
        foreach (var newContacto in cliente.Contactos)
        {
            var existingContacto = existing.Contactos.FirstOrDefault(c => c.Id == newContacto.Id);
            if (existingContacto != null)
            {
                _context.Entry(existingContacto).CurrentValues.SetValues(newContacto);
            }
            else
            {
                newContacto.ClienteId = cliente.Id;
                await _context.Set<ClienteContacto>().AddAsync(newContacto);
            }
        }
    }
}
