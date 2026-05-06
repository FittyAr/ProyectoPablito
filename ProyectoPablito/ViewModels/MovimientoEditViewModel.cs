using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class MovimientoEditViewModel : ViewModelBase
{
    private readonly IMovimientoService _movimientoService;
    private readonly ICategoriaService _categoriaService;
    private readonly ITipoMovimientoService _tipoMovimientoService;

    [ObservableProperty]
    private MovimientoDto _movimiento = new() { Fecha = DateTime.Now, Cantidad = 1 };

    [ObservableProperty]
    private ObservableCollection<CategoriaDto> _categorias = new();

    [ObservableProperty]
    private ObservableCollection<TipoMovimientoDto> _tiposMovimiento = new();

    [ObservableProperty]
    private string _title = "Nuevo Movimiento";

    public MovimientoEditViewModel(
        IMovimientoService movimientoService,
        ICategoriaService categoriaService,
        ITipoMovimientoService tipoMovimientoService)
    {
        _movimientoService = movimientoService;
        _categoriaService = categoriaService;
        _tipoMovimientoService = tipoMovimientoService;
        
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
    }

    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand LoadDataCommand { get; }

    public async Task LoadDataAsync()
    {
        var cats = await _categoriaService.GetAllAsync();
        Categorias = new ObservableCollection<CategoriaDto>(cats);

        var tipos = await _tipoMovimientoService.GetAllAsync();
        TiposMovimiento = new ObservableCollection<TipoMovimientoDto>(tipos);

        if (Movimiento.TipoMovimientoId == Guid.Empty && TiposMovimiento.Any())
        {
            Movimiento.TipoMovimientoId = TiposMovimiento.First().Id;
        }
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
            // Podríamos usar un Messenger para avisar a la lista que refresque
            CloseRequest?.Invoke(this, true);
        }
    }

    public event EventHandler<bool>? CloseRequest;
}
