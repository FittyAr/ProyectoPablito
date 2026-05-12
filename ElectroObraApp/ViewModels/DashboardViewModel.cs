using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectroObraApp.Application.Interfaces;
using ElectroObraApp.Application.DTOs;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore.Measure;

namespace ElectroObraApp.ViewModels;

public class TopClienteDto
{
    public string Nombre { get; set; } = string.Empty;
    public decimal Total { get; set; }
}

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IMovimientoService _movimientoService;
    private readonly IClienteService _clienteService;
    private readonly ITrabajoService _trabajoService;
    private readonly IUserSettingsService _settingsService;
    private readonly IDollarService _dollarService;
    private readonly ILiquidacionService _liquidacionService;
    private readonly IEmpleadoService _empleadoService;

    [ObservableProperty]
    private string _title = "Dashboard Operativo";

    [ObservableProperty]
    private ObservableCollection<DollarDto> _dollarRates = new();

    [ObservableProperty]
    private bool _showDollarRates = false;

    [ObservableProperty]
    private decimal _totalIngresos;

    [ObservableProperty]
    private decimal _totalGastos;

    [ObservableProperty]
    private decimal _balance;

    [ObservableProperty]
    private int _clientesActivos;

    [ObservableProperty]
    private int _trabajosPendientes;

    [ObservableProperty]
    private int _liquidacionesPendientesCount;

    [ObservableProperty]
    private string _databaseStatus = "Saludable";

    [ObservableProperty]
    private bool _isPrivacyModeActive;

    partial void OnIsPrivacyModeActiveChanged(bool value)
    {
        _ = _settingsService.SetIsPrivacyModeAsync(value);
        OnPropertyChanged(nameof(DisplayTotalIngresos));
        OnPropertyChanged(nameof(DisplayTotalGastos));
        OnPropertyChanged(nameof(DisplayBalance));
        OnPropertyChanged(nameof(ChartTooltipPosition));
        OnPropertyChanged(nameof(PieTooltipPosition));
    }

    [ObservableProperty]
    private string _currentTimeRange = "Mensual"; // Mensual, Anual, Total

    partial void OnCurrentTimeRangeChanged(string value)
    {
        _ = LoadStatsAsync();
    }

    public ObservableCollection<string> TimeRanges { get; } = new() { "Mensual", "Anual", "Total" };
    public ObservableCollection<TopClienteDto> TopClientes { get; } = new();
    public ObservableCollection<MovimientoDto> RecentMovimientos { get; } = new();

    public string DisplayTotalIngresos => IsPrivacyModeActive ? "$ *********" : TotalIngresos.ToString("C");
    public string DisplayTotalGastos => IsPrivacyModeActive ? "$ *********" : TotalGastos.ToString("C");
    public string DisplayBalance => IsPrivacyModeActive ? "$ *********" : Balance.ToString("C");

    public LiveChartsCore.Measure.TooltipPosition ChartTooltipPosition => IsPrivacyModeActive 
        ? LiveChartsCore.Measure.TooltipPosition.Hidden 
        : LiveChartsCore.Measure.TooltipPosition.Bottom;

    public LiveChartsCore.Measure.TooltipPosition PieTooltipPosition => IsPrivacyModeActive 
        ? LiveChartsCore.Measure.TooltipPosition.Hidden 
        : LiveChartsCore.Measure.TooltipPosition.Right;

    public string LiquidacionesPendientesText => LiquidacionesPendientesCount > 0 
        ? $"{LiquidacionesPendientesCount} Liquidaciones Pendientes" 
        : "Personal al día";

    public bool ShowLiquidacionesAlert => LiquidacionesPendientesCount > 0;

    public Func<LiveChartsCore.Kernel.ChartPoint, string> PieFormatter => 
        point => $"{point.Context.Series.Name}: {point.Coordinate.PrimaryValue:C}";

    // Charts
    public ObservableCollection<ISeries> Series { get; set; } = new();
    public ObservableCollection<ISeries> CategorySeries { get; set; } = new();

    public Axis[] XAxes { get; set; } = 
    { 
        new Axis { Labels = new string[] { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" } } 
    };

    public DashboardViewModel(
        IMovimientoService movimientoService, 
        IClienteService clienteService,
        ITrabajoService trabajoService,
        IUserSettingsService settingsService,
        IDollarService dollarService,
        ILiquidacionService liquidacionService,
        IEmpleadoService empleadoService)
    {
        _movimientoService = movimientoService;
        _clienteService = clienteService;
        _trabajoService = trabajoService;
        _settingsService = settingsService;
        _dollarService = dollarService;
        _liquidacionService = liquidacionService;
        _empleadoService = empleadoService;
        
        CurrentTimeRange = settingsService.GetDashboardPeriod();
        IsPrivacyModeActive = settingsService.GetIsPrivacyMode();

        LoadStatsCommand = new AsyncRelayCommand(LoadStatsAsync);
        NavigateToAlertCommand = new RelayCommand<string>(NavigateToAlert);
        TogglePrivacyModeCommand = new RelayCommand(() => {
            IsPrivacyModeActive = !IsPrivacyModeActive;
        });
        _ = LoadStatsAsync();
        _ = LoadDollarRatesAsync();
    }

    private async Task LoadDollarRatesAsync()
    {
        if (!_settingsService.GetAutoUpdateDollar()) return;

        var rates = await _dollarService.GetDollarRatesAsync();
        DollarRates.Clear();
        foreach (var rate in rates.Where(r => r.Casa == "oficial" || r.Casa == "blue"))
        {
            DollarRates.Add(rate);
        }
        ShowDollarRates = DollarRates.Any();
    }

    public IAsyncRelayCommand LoadStatsCommand { get; }
    public IRelayCommand TogglePrivacyModeCommand { get; }
    public IRelayCommand<string> NavigateToAlertCommand { get; }

    private void NavigateToAlert(string? destination)
    {
        if (string.IsNullOrEmpty(destination)) return;
        WeakReferenceMessenger.Default.Send(destination);
    }

    public async Task LoadStatsAsync()
    {
        var movimientos = await _movimientoService.GetAllAsync();
        var filterDate = CurrentTimeRange switch
        {
            "Mensual" => DateTime.Now.AddMonths(-1),
            "Anual" => DateTime.Now.AddYears(-1),
            _ => DateTime.MinValue
        };

        var filteredMovimientos = movimientos.Where(m => m.Fecha >= filterDate).ToList();

        TotalIngresos = filteredMovimientos.Where(m => m.TipoMovimientoSuma).Sum(m => m.Total);
        TotalGastos = filteredMovimientos.Where(m => !m.TipoMovimientoSuma).Sum(m => m.Total);
        Balance = TotalIngresos - TotalGastos;

        // Trabajos Pendientes
        var trabajos = await _trabajoService.GetAllAsync();
        TrabajosPendientes = trabajos.Count(t => !t.Finalizado);

        // Alertas: Liquidaciones Pendientes (Empleados activos sin liquidación este mes)
        var empleados = await _empleadoService.GetAllAsync();
        var liquidaciones = await _liquidacionService.GetAllAsync();
        var mesActual = DateTime.Now.Month;
        var añoActual = DateTime.Now.Year;

        LiquidacionesPendientesCount = empleados
            .Where(e => e.Activo)
            .Count(e => !liquidaciones.Any(l => l.EmpleadoId == e.Id && l.FechaFin.Month == mesActual && l.FechaFin.Year == añoActual));
        
        OnPropertyChanged(nameof(LiquidacionesPendientesText));
        OnPropertyChanged(nameof(ShowLiquidacionesAlert));

        // Estado de DB
        DatabaseStatus = "Saludable"; // Podríamos agregar un check real aquí si fuera necesario

        // Top Clientes (Income)
        TopClientes.Clear();
        var top = filteredMovimientos
            .Where(m => m.TipoMovimientoSuma && m.ClienteId != null)
            .GroupBy(m => m.ClienteNombre)
            .Select(g => new TopClienteDto { Nombre = g.Key ?? "Sin Nombre", Total = g.Sum(m => m.Total) })
            .OrderByDescending(x => x.Total)
            .Take(3);
        
        foreach(var t in top) TopClientes.Add(t);

        // Recent Activity
        RecentMovimientos.Clear();
        var recent = movimientos.OrderByDescending(m => m.Fecha).Take(5);
        foreach(var r in recent) RecentMovimientos.Add(r);

        // Bar Chart Data (Keep monthly for comparison even if filtered for cards)
        var monthlyIncome = new double[12];
        var monthlyExpenses = new double[12];

        foreach (var m in movimientos.Where(m => m.Fecha.Year == DateTime.Now.Year))
        {
            int month = m.Fecha.Month - 1;
            if (m.TipoMovimientoSuma) monthlyIncome[month] += (double)m.Total;
            else monthlyExpenses[month] += (double)m.Total;
        }

        Series.Clear();
        Series.Add(new ColumnSeries<double>
        {
            Name = "Ingresos",
            Values = monthlyIncome,
            Stroke = new SolidColorPaint(SKColors.LightGreen) { StrokeThickness = 2 },
            Fill = new SolidColorPaint(SKColors.LightGreen.WithAlpha(100)),
            YToolTipLabelFormatter = point => $"Ingresos: {point.Coordinate.PrimaryValue:C}"
        });
        Series.Add(new ColumnSeries<double>
        {
            Name = "Gastos",
            Values = monthlyExpenses,
            Stroke = new SolidColorPaint(SKColors.Salmon) { StrokeThickness = 2 },
            Fill = new SolidColorPaint(SKColors.Salmon.WithAlpha(100)),
            YToolTipLabelFormatter = point => $"Gastos: {point.Coordinate.PrimaryValue:C}"
        });

        // Category Distribution
        CategorySeries.Clear();
        var categoryGroups = movimientos
            .Where(m => !m.TipoMovimientoSuma && !string.IsNullOrEmpty(m.CategoriaNombre))
            .GroupBy(m => m.CategoriaNombre)
            .Select(g => new { Name = g.Key, Value = (double)g.Sum(m => m.Total) })
            .OrderByDescending(x => x.Value)
            .Take(5);

        foreach (var cat in categoryGroups)
        {
            CategorySeries.Add(new PieSeries<double>
            {
                Name = cat.Name,
                Values = new double[] { cat.Value },
                ToolTipLabelFormatter = point => $"{point.Coordinate.PrimaryValue:C}",
                DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue:C}"
            });
        }
    }
}
