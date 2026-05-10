using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ElectroObraApp.Core.Entities;
using ElectroObraApp.Core.Interfaces;
using ElectroObraApp.Infrastructure.Data;

namespace ElectroObraApp.Infrastructure.Repositories;

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

