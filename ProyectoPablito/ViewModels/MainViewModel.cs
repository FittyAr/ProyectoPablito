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
        
        // Pagina inicial
        NavigateToDashboard();
    }

    [RelayCommand]
    private void NavigateToDashboard() => CurrentPage = _serviceProvider.GetRequiredService<DashboardViewModel>();

    [RelayCommand]
    private void NavigateToMovimientos() => CurrentPage = _serviceProvider.GetRequiredService<MovimientosViewModel>();
}
