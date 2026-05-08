using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProyectoPablito.Core.Entities;
using ProyectoPablito.Core.Interfaces;
using ProyectoPablito.Infrastructure.Data;

namespace ProyectoPablito.Infrastructure.Repositories;

public class TrabajoRepository : Repository<Trabajo>, ITrabajoRepository
{
    public TrabajoRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Trabajo>> GetAllWithClienteAsync()
    {
        return await _context.Trabajos
            .AsNoTracking()
            .Include(t => t.Cliente)
            .ToListAsync();
    }

    public async Task<Trabajo?> GetByIdWithOrdenesAsync(Guid id)
    {
        return await _context.Trabajos
            .AsNoTracking()
            .Include(t => t.Cliente)
            .Include(t => t.OrdenesTrabajo)
                .ThenInclude(o => o.Items)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
