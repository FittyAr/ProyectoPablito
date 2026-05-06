using System.Threading.Tasks;

namespace ProyectoPablito.Application.Interfaces;

public interface IDatabaseSeedService
{
    Task SeedAsync();
    bool IsSeedEnabled();
}
