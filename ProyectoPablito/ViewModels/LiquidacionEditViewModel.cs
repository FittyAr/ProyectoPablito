using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class LiquidacionEditViewModel : ViewModelBase
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IEmpleadoService _empleadoService;

    [ObservableProperty]
    private LiquidacionDto _liquidacion = new() 
    { 
        FechaInicio = DateTime.Now.AddDays(-15), 
        FechaFin = DateTime.Now,
        DiasTrabajados = 13 // Default típico quincenal
    };

    [ObservableProperty]
    private ObservableCollection<EmpleadoDto> _empleados = new();

    public LiquidacionEditViewModel(ILiquidacionService liquidacionService, IEmpleadoService empleadoService)
    {
        _liquidacionService = liquidacionService;
        _empleadoService = empleadoService;

        SaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new RelayCommand(Cancel);
        SugerirCommand = new AsyncRelayCommand(SugerirAsync);
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
    }

    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IAsyncRelayCommand SugerirCommand { get; }
    public IAsyncRelayCommand LoadDataCommand { get; }

    public async Task LoadDataAsync()
    {
        var list = await _empleadoService.GetAllAsync();
        Empleados = new ObservableCollection<EmpleadoDto>(list);
    }

    private async Task SugerirAsync()
    {
        if (Liquidacion.EmpleadoId == Guid.Empty) return;

        var sugerencia = await _liquidacionService.SugerirLiquidacionAsync(
            Liquidacion.EmpleadoId, 
            Liquidacion.FechaInicio, 
            Liquidacion.FechaFin, 
            Liquidacion.DiasTrabajados);

        Liquidacion = sugerencia;
    }

    private async Task SaveAsync()
    {
        var success = await _liquidacionService.CreateAsync(Liquidacion);
        if (success != null)
        {
            CloseRequest?.Invoke(this, true);
        }
    }

    private void Cancel()
    {
        CloseRequest?.Invoke(this, false);
    }

    public event EventHandler<bool>? CloseRequest;
}
