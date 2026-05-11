using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Interfaces;

namespace ElectroObraApp.ViewModels;

public partial class MovimientoEditViewModel : ViewModelBase
{
    private readonly IMovimientoService _movimientoService;
    private readonly ICategoriaService _categoriaService;
    private readonly ITipoMovimientoService _tipoMovimientoService;
    private readonly IEmpleadoService _empleadoService;

    [ObservableProperty]
    private MovimientoDto _movimiento = new() { Fecha = DateTime.Now, Cantidad = 1 };

    partial void OnMovimientoChanged(MovimientoDto value)
    {
        OnPropertyChanged(nameof(FechaProxy));
    }

    [ObservableProperty]
    private ObservableCollection<CategoriaDto> _categorias = new();

    [ObservableProperty]
    private ObservableCollection<TipoMovimientoDto> _tiposMovimiento = new();

    [ObservableProperty]
    private ObservableCollection<EmpleadoDto> _empleados = new();

    [ObservableProperty]
    private string _title = "Nuevo Movimiento";

    public DateTime? FechaProxy
    {
        get => Movimiento.Fecha;
        set
        {
            if (value.HasValue && Movimiento.Fecha != value.Value)
            {
                Movimiento.Fecha = value.Value;
                OnPropertyChanged(nameof(FechaProxy));
            }
        }
    }

    public MovimientoEditViewModel(
        IMovimientoService movimientoService,
        ICategoriaService categoriaService,
        ITipoMovimientoService tipoMovimientoService,
        IEmpleadoService empleadoService)
    {
        _movimientoService = movimientoService;
        _categoriaService = categoriaService;
        _tipoMovimientoService = tipoMovimientoService;
        _empleadoService = empleadoService;
        
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new RelayCommand(Cancel);
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
    }

    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IAsyncRelayCommand LoadDataCommand { get; }

    public async Task LoadDataAsync()
    {
        var cats = await _categoriaService.GetAllAsync();
        var tipos = await _tipoMovimientoService.GetAllAsync();

        // Asignamos las colecciones
        Categorias = new ObservableCollection<CategoriaDto>(cats);
        TiposMovimiento = new ObservableCollection<TipoMovimientoDto>(tipos);
        
        var emps = await _empleadoService.GetAllAsync();
        Empleados = new ObservableCollection<EmpleadoDto>(emps);

        // Solo establecemos el tipo por defecto si es un movimiento nuevo
        if (Movimiento.Id == Guid.Empty && Movimiento.TipoMovimientoId == Guid.Empty && TiposMovimiento.Any())
        {
            Movimiento.TipoMovimientoId = TiposMovimiento.First().Id;
        }
        
        // Disparamos notificaciones manuales para asegurar que los combos reflejen el valor correcto
        OnPropertyChanged(nameof(Movimiento));
        OnPropertyChanged(nameof(FechaProxy));
    }

    private async Task SaveAsync()
    {
        bool success;
        if (Movimiento.Id == Guid.Empty)
            success = await _movimientoService.CreateAsync(Movimiento);
        else
            success = await _movimientoService.UpdateAsync(Movimiento);

        if (success)
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

