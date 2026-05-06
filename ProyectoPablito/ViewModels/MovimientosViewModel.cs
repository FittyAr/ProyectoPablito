using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class MovimientosViewModel : ViewModelBase
{
    private readonly IMovimientoService _movimientoService;

    [ObservableProperty]
    private ObservableCollection<MovimientoDto> _movimientos = new();

    public MovimientosViewModel(IMovimientoService movimientoService)
    {
        _movimientoService = movimientoService;
        LoadMovimientosCommand = new AsyncRelayCommand(LoadMovimientosAsync);
    }

    public IAsyncRelayCommand LoadMovimientosCommand { get; }

    private async Task LoadMovimientosAsync()
    {
        var result = await _movimientoService.GetAllAsync();
        Movimientos.Clear();
        foreach (var item in result)
        {
            Movimientos.Add(item);
        }
    }
}
