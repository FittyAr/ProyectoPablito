using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ILocalizationService _localizationService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private string _greeting;

    [ObservableProperty]
    private ViewModelBase? _currentPage;

    [ObservableProperty]
    private string _currentSection = "Dashboard";

    [ObservableProperty]
    private bool _isSeedEnabled;

    public MainViewModel(ILocalizationService localizationService, IServiceProvider serviceProvider, IDatabaseSeedService seedService)
    {
        _localizationService = localizationService;
        _serviceProvider = serviceProvider;
        _greeting = _localizationService.GetString("General.AppName");
        _isSeedEnabled = seedService.IsSeedEnabled();
        
        NavigateToDashboardCommand = new RelayCommand(NavigateToDashboard);
        NavigateToMovimientosCommand = new RelayCommand(NavigateToMovimientos);
        NavigateToClientesCommand = new RelayCommand(NavigateToClientes);
        NavigateToEmpleadosCommand = new RelayCommand(NavigateToEmpleados);
        NavigateToTrabajosCommand = new RelayCommand(NavigateToTrabajos);
        NavigateToSeedCommand = new RelayCommand(NavigateToSeed);
        
        // Pagina inicial
        NavigateToDashboard();
    }

    public IRelayCommand NavigateToDashboardCommand { get; }
    public IRelayCommand NavigateToMovimientosCommand { get; }
    public IRelayCommand NavigateToClientesCommand { get; }
    public IRelayCommand NavigateToEmpleadosCommand { get; }
    public IRelayCommand NavigateToTrabajosCommand { get; }
    public IRelayCommand NavigateToSeedCommand { get; }

    private void NavigateToDashboard() { CurrentPage = _serviceProvider.GetRequiredService<DashboardViewModel>(); CurrentSection = "Dashboard"; }
    private void NavigateToMovimientos() { CurrentPage = _serviceProvider.GetRequiredService<MovimientosViewModel>(); CurrentSection = "Movimientos"; }
    private void NavigateToClientes() { CurrentPage = _serviceProvider.GetRequiredService<ClientesViewModel>(); CurrentSection = "Clientes"; }
    private void NavigateToEmpleados() { CurrentPage = _serviceProvider.GetRequiredService<EmpleadosViewModel>(); CurrentSection = "Empleados"; }
    private void NavigateToTrabajos() { CurrentPage = _serviceProvider.GetRequiredService<TrabajosViewModel>(); CurrentSection = "Trabajos"; }
    private void NavigateToSeed() { CurrentPage = _serviceProvider.GetRequiredService<SeedViewModel>(); CurrentSection = "Seed"; }
}
