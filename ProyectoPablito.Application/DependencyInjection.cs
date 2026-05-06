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
        services.AddScoped<IMovimientoService, MovimientoService>();
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
