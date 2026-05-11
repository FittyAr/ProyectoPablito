using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Interfaces;

namespace ElectroObraApp.ViewModels;

public partial class LiquidacionEditViewModel : ViewModelBase
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IEmpleadoService _empleadoService;
    private readonly IUserSettingsService _settingsService;

    [ObservableProperty]
    private LiquidacionDto _liquidacion = new() 
    { 
        FechaInicio = DateTime.Now.AddDays(-15), 
        FechaFin = DateTime.Now,
        DiasTrabajados = 10,
        MultiplicadorSabado = 1.0m,
        MultiplicadorDomingo = 1.0m,
        MultiplicadorFeriado = 1.0m
    };
    
    partial void OnLiquidacionChanged(LiquidacionDto value)
    {
        OnPropertyChanged(nameof(FechaInicioProxy));
        OnPropertyChanged(nameof(FechaFinProxy));
    }

    public DateTime? FechaInicioProxy
    {
        get => Liquidacion.FechaInicio;
        set
        {
            if (value.HasValue && Liquidacion.FechaInicio != value.Value)
            {
                Liquidacion.FechaInicio = value.Value;
                OnPropertyChanged(nameof(FechaInicioProxy));
                _ = ReclacularAutomaticamente();
            }
        }
    }

    public DateTime? FechaFinProxy
    {
        get => Liquidacion.FechaFin;
        set
        {
            if (value.HasValue && Liquidacion.FechaFin != value.Value)
            {
                Liquidacion.FechaFin = value.Value;
                OnPropertyChanged(nameof(FechaFinProxy));
                _ = ReclacularAutomaticamente();
            }
        }
    }

    [ObservableProperty]
    private ObservableCollection<EmpleadoDto> _empleados = new();

    public LiquidacionEditViewModel(ILiquidacionService liquidacionService, IEmpleadoService empleadoService, IUserSettingsService settingsService)
    {
        _liquidacionService = liquidacionService;
        _empleadoService = empleadoService;
        _settingsService = settingsService;

        // Cargar defaults del config
        Liquidacion.MultiplicadorSabado = _settingsService.GetDefaultMultiplierSaturday();
        Liquidacion.MultiplicadorDomingo = _settingsService.GetDefaultMultiplierSunday();
        Liquidacion.MultiplicadorFeriado = _settingsService.GetDefaultMultiplierHoliday();

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

    private async Task ReclacularAutomaticamente()
    {
        if (Liquidacion.EmpleadoId == Guid.Empty) return;
        await SugerirAsync();
    }

    private async Task SugerirAsync()
    {
        if (Liquidacion.EmpleadoId == Guid.Empty) return;

        // Llamamos al servicio para obtener adelantos y tarifa base
        var sugerencia = await _liquidacionService.SugerirLiquidacionAsync(
            Liquidacion.EmpleadoId, 
            Liquidacion.FechaInicio, 
            Liquidacion.FechaFin, 
            Liquidacion.DiasTrabajados);

        // Aplicamos lógica de días trabajados avanzada localmente en el VM para respuesta inmediata
        // o actualizamos el DTO sugerido.
        // El servicio ya hace un cálculo base (L-V).
        
        var totalDias = 0m;
        var totalBruto = 0m;
        
        for (var date = Liquidacion.FechaInicio.Date; date <= Liquidacion.FechaFin.Date; date = date.AddDays(1))
        {
            var esSabado = date.DayOfWeek == DayOfWeek.Saturday;
            var esDomingo = date.DayOfWeek == DayOfWeek.Sunday;
            
            var multiplicador = 1.0m;
            
            if (esSabado)
            {
                multiplicador = Liquidacion.IncluirSabados ? Liquidacion.MultiplicadorSabado : 0.0m;
            }
            else if (esDomingo)
            {
                multiplicador = Liquidacion.IncluirDomingos ? Liquidacion.MultiplicadorDomingo : 0.0m;
            }
            // TODO: Feriados requerirían una lista
            
            if (multiplicador > 0)
            {
                totalDias += 1.0m;
                totalBruto += sugerencia.TarifaAplicada * multiplicador;
            }
        }

        Liquidacion.DiasTrabajados = totalDias;
        Liquidacion.TotalBruto = totalBruto;
        Liquidacion.TotalAdelantos = sugerencia.TotalAdelantos;
        Liquidacion.TotalNeto = totalBruto - sugerencia.TotalAdelantos;
        Liquidacion.TarifaAplicada = sugerencia.TarifaAplicada;
        
        OnPropertyChanged(nameof(Liquidacion));
        OnPropertyChanged(nameof(FechaInicioProxy));
        OnPropertyChanged(nameof(FechaFinProxy));
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

    // Propiedades para disparar el recalculo desde los checkboxes/updowns
    public bool IncluirSabados
    {
        get => Liquidacion.IncluirSabados;
        set { Liquidacion.IncluirSabados = value; OnPropertyChanged(); _ = SugerirAsync(); }
    }
    public bool IncluirDomingos
    {
        get => Liquidacion.IncluirDomingos;
        set { Liquidacion.IncluirDomingos = value; OnPropertyChanged(); _ = SugerirAsync(); }
    }
    public bool IncluirFeriados
    {
        get => Liquidacion.IncluirFeriados;
        set { Liquidacion.IncluirFeriados = value; OnPropertyChanged(); _ = SugerirAsync(); }
    }
}

