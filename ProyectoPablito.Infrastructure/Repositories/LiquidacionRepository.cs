using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProyectoPablito.Core.Entities;
using ProyectoPablito.Core.Interfaces;
using ProyectoPablito.Infrastructure.Data;

namespace ProyectoPablito.Infrastructure.Repositories;

public class LiquidacionRepository : Repository<Liquidacion>, ILiquidacionRepository
{
    public LiquidacionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Liquidacion>> GetAllWithEmpleadoAsync()
    {
        return await _context.Liquidaciones
            .AsNoTracking()
            .Include(l => l.Empleado)
            .ToListAsync();
    }
}
