using System.Threading.Tasks;

namespace ElectroObraApp.Application.Interfaces;

public interface IDatabaseSeedService
{
    Task SeedAsync();
    bool IsSeedEnabled();
}

