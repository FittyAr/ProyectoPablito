using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProyectoPablito.Application.Interfaces;
using ProyectoPablito.Core.Interfaces;
using ProyectoPablito.Infrastructure.Data;
using ProyectoPablito.Infrastructure.Repositories;
using ProyectoPablito.Infrastructure.Services;

namespace ProyectoPablito.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<IDatabaseSeedService, DatabaseSeedService>();
        
        return services;
    }
}
