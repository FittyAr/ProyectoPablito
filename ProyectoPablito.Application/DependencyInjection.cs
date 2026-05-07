using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProyectoPablito.Application.Interfaces;
using ProyectoPablito.Application.Services;
using System.Reflection;

namespace ProyectoPablito.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        Mappings.MappingConfig.Configure();
        services.AddScoped<IMovimientoService, MovimientoService>();
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<ITipoMovimientoService, TipoMovimientoService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IEmpleadoService, EmpleadoService>();
        services.AddScoped<ITrabajoService, TrabajoService>();
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
