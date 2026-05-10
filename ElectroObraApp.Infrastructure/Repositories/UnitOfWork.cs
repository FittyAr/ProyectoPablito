using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ElectroObraApp.Core.Interfaces;
using ElectroObraApp.Infrastructure.Data;

namespace ElectroObraApp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories;
    private IMovimientoRepository? _movimientoRepository;
    private IClienteRepository? _clienteRepository;
    private ILiquidacionRepository? _liquidacionRepository;
    private ITrabajoRepository? _trabajoRepository;
    private bool _disposed;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _repositories = new ConcurrentDictionary<Type, object>();
    }

    public IRepository<T> Repository<T>() where T : class
    {
        return (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new Repository<T>(_context));
    }

    public IMovimientoRepository Movimientos => _movimientoRepository ??= new MovimientoRepository(_context);
    public IClienteRepository Clientes => _clienteRepository ??= new ClienteRepository(_context);
    public ILiquidacionRepository Liquidaciones => _liquidacionRepository ??= new LiquidacionRepository(_context);
    public ITrabajoRepository Trabajos => _trabajoRepository ??= new TrabajoRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}

