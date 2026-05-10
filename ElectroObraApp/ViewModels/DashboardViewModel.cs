using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectroObraApp.Application.Interfaces;

namespace ElectroObraApp.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IMovimientoService _movimientoService;

    [ObservableProperty]
    private string _title = "Dashboard Operativo";

    [ObservableProperty]
    private decimal _totalIngresos;

    [ObservableProperty]
    private decimal _totalGastos;

    [ObservableProperty]
    private decimal _balance;

    public DashboardViewModel(IMovimientoService movimientoService)
    {
        _movimientoService = movimientoService;
        LoadStatsCommand = new AsyncRelayCommand(LoadStatsAsync);
        _ = LoadStatsCommand.ExecuteAsync(null);
    }

    public IAsyncRelayCommand LoadStatsCommand { get; }

    public async Task LoadStatsAsync()
    {
        var movimientos = await _movimientoService.GetAllAsync();
        TotalIngresos = movimientos.Where(m => m.TipoMovimientoSuma).Sum(m => m.Total);
        TotalGastos = movimientos.Where(m => !m.TipoMovimientoSuma).Sum(m => m.Total);
        Balance = TotalIngresos - TotalGastos;
    }
}

