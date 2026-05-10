using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ElectroObraApp.Application;
using ElectroObraApp.Application.Interfaces;
using ElectroObraApp.Infrastructure;
using ElectroObraApp.Infrastructure.Data;
using ElectroObraApp.ViewModels;
using ElectroObraApp.Views;
using Serilog;

namespace ElectroObraApp;

public partial class App : Avalonia.Application
{
    public IServiceProvider? Services { get; private set; }
    public IConfiguration? Configuration { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // 1. Configuración
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        Configuration = builder.Build();

        // 2. Logging
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .CreateLogger();

        // 3. DI Container
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();

        // 4. Inicialización de Base de Datos
        InitializeDatabase();

        // Global Exception Handling
        AppDomain.CurrentDomain.UnhandledException += (sender, e) => 
        {
            Log.Fatal(e.ExceptionObject as Exception, "Error no controlado (AppDomain)");
        };

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Log.Error(e.Exception, "Error en tarea asíncrona no observada");
            e.SetObserved();
        };

        // 5. Inicialización de UI
        var mainViewModel = Services.GetRequiredService<MainViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Configuration!);
        services.AddLogging(builder => builder.AddSerilog());
        services.AddApplication();
        services.AddInfrastructure(Configuration!);

        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<MovimientosViewModel>();
        services.AddTransient<MovimientoEditViewModel>();
        services.AddTransient<ClientesViewModel>();
        services.AddTransient<ClienteEditViewModel>();
        services.AddTransient<EmpleadosViewModel>();
        services.AddTransient<EmpleadoEditViewModel>();
        services.AddTransient<TrabajosViewModel>();
        services.AddTransient<TrabajoEditViewModel>();
        services.AddTransient<LiquidacionesViewModel>();
        services.AddTransient<LiquidacionEditViewModel>();
        services.AddTransient<SeedViewModel>();
    }

    private void InitializeDatabase()
    {
        using var scope = Services!.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<App>>();

        try
        {
            logger.LogInformation("Verificando y aplicando migraciones de base de datos...");
            context.Database.Migrate();

            var seedService = scope.ServiceProvider.GetRequiredService<IDatabaseSeedService>();
            if (seedService.IsSeedEnabled())
            {
                // Solo sembrar si la base de datos está vacía (ej: sin movimientos)
                if (!context.Movimientos.Any())
                {
                    logger.LogInformation("Base de datos vacía detectada. Sembrando datos iniciales...");
                    Task.Run(async () => await seedService.SeedAsync()).GetAwaiter().GetResult();
                }
            }
            logger.LogInformation("Base de datos inicializada correctamente.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al inicializar la base de datos.");
        }
    }
}
