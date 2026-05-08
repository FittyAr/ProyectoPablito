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
        NavigateToLiquidacionesCommand = new RelayCommand(NavigateToLiquidaciones);
        NavigateToSeedCommand = new RelayCommand(NavigateToSeed);
        NavigateToCommand = new RelayCommand<string>(NavigateTo);
        
        // Pagina inicial
        NavigateToDashboard();
    }

    public IRelayCommand NavigateToDashboardCommand { get; }
    public IRelayCommand NavigateToMovimientosCommand { get; }
    public IRelayCommand NavigateToClientesCommand { get; }
    public IRelayCommand NavigateToEmpleadosCommand { get; }
    public IRelayCommand NavigateToTrabajosCommand { get; }
    public IRelayCommand NavigateToLiquidacionesCommand { get; }
    public IRelayCommand NavigateToSeedCommand { get; }
    public IRelayCommand<string> NavigateToCommand { get; }

    private void NavigateToDashboard() { CurrentPage = _serviceProvider.GetRequiredService<DashboardViewModel>(); CurrentSection = "Dashboard"; }
    private void NavigateToMovimientos() { CurrentPage = _serviceProvider.GetRequiredService<MovimientosViewModel>(); CurrentSection = "Movimientos"; }
    private void NavigateToClientes() { CurrentPage = _serviceProvider.GetRequiredService<ClientesViewModel>(); CurrentSection = "Clientes"; }
    private void NavigateToEmpleados() { CurrentPage = _serviceProvider.GetRequiredService<EmpleadosViewModel>(); CurrentSection = "Empleados"; }
    private void NavigateToTrabajos() { CurrentPage = _serviceProvider.GetRequiredService<TrabajosViewModel>(); CurrentSection = "Trabajos"; }
    private void NavigateToLiquidaciones() { CurrentPage = _serviceProvider.GetRequiredService<LiquidacionesViewModel>(); CurrentSection = "Liquidaciones"; }
    private void NavigateToSeed() { CurrentPage = _serviceProvider.GetRequiredService<SeedViewModel>(); CurrentSection = "Seed"; }

    private void NavigateTo(string? destination)
    {
        if (destination == "LiquidacionEdit")
        {
            var vm = _serviceProvider.GetRequiredService<LiquidacionEditViewModel>();
            vm.CloseRequest += (s, success) => NavigateToLiquidaciones();
            CurrentPage = vm;
            CurrentSection = "Liquidaciones";
        }
    }
}
