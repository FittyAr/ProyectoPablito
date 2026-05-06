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

    public MainViewModel(ILocalizationService localizationService, IServiceProvider serviceProvider)
    {
        _localizationService = localizationService;
        _serviceProvider = serviceProvider;
        _greeting = _localizationService.GetString("General.AppName");
        
        NavigateToDashboardCommand = new RelayCommand(NavigateToDashboard);
        NavigateToMovimientosCommand = new RelayCommand(NavigateToMovimientos);
        NavigateToClientesCommand = new RelayCommand(NavigateToClientes);
        NavigateToEmpleadosCommand = new RelayCommand(NavigateToEmpleados);
        NavigateToTrabajosCommand = new RelayCommand(NavigateToTrabajos);
        
        // Pagina inicial
        NavigateToDashboard();
    }

    public IRelayCommand NavigateToDashboardCommand { get; }
    public IRelayCommand NavigateToMovimientosCommand { get; }
    public IRelayCommand NavigateToClientesCommand { get; }
    public IRelayCommand NavigateToEmpleadosCommand { get; }
    public IRelayCommand NavigateToTrabajosCommand { get; }

    private void NavigateToDashboard() => CurrentPage = _serviceProvider.GetRequiredService<DashboardViewModel>();
    private void NavigateToMovimientos() => CurrentPage = _serviceProvider.GetRequiredService<MovimientosViewModel>();
    private void NavigateToClientes() => CurrentPage = _serviceProvider.GetRequiredService<ClientesViewModel>();
    private void NavigateToEmpleados() => CurrentPage = _serviceProvider.GetRequiredService<EmpleadosViewModel>();
    private void NavigateToTrabajos() => CurrentPage = _serviceProvider.GetRequiredService<TrabajosViewModel>();
}
