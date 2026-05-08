using System;
using System.Threading.Tasks;

namespace ProyectoPablito.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    IMovimientoRepository Movimientos { get; }
    Task<int> SaveChangesAsync();
}
