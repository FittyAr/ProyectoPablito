using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectroObraApp.Application.Interfaces;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore.Measure;

namespace ElectroObraApp.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IMovimientoService _movimientoService;
    private readonly IClienteService _clienteService;
    private readonly ITrabajoService _trabajoService;

    [ObservableProperty]
    private string _title = "Dashboard Operativo";

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
    private bool _isPrivacyModeActive;

    public string DisplayTotalIngresos => IsPrivacyModeActive ? "$ *********" : TotalIngresos.ToString("C");
    public string DisplayTotalGastos => IsPrivacyModeActive ? "$ *********" : TotalGastos.ToString("C");
    public string DisplayBalance => IsPrivacyModeActive ? "$ *********" : Balance.ToString("C");

    public LiveChartsCore.Measure.TooltipPosition ChartTooltipPosition => IsPrivacyModeActive 
        ? LiveChartsCore.Measure.TooltipPosition.Hidden 
        : LiveChartsCore.Measure.TooltipPosition.Bottom;

    public LiveChartsCore.Measure.TooltipPosition PieTooltipPosition => IsPrivacyModeActive 
        ? LiveChartsCore.Measure.TooltipPosition.Hidden 
        : LiveChartsCore.Measure.TooltipPosition.Right;

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
        ITrabajoService trabajoService)
    {
        _movimientoService = movimientoService;
        _clienteService = clienteService;
        _trabajoService = trabajoService;
        
        LoadStatsCommand = new AsyncRelayCommand(LoadStatsAsync);
        NavigateToAlertCommand = new RelayCommand<string>(NavigateToAlert);
        TogglePrivacyModeCommand = new RelayCommand(() => {
            IsPrivacyModeActive = !IsPrivacyModeActive;
            OnPropertyChanged(nameof(DisplayTotalIngresos));
            OnPropertyChanged(nameof(DisplayTotalGastos));
            OnPropertyChanged(nameof(DisplayBalance));
            OnPropertyChanged(nameof(ChartTooltipPosition));
            OnPropertyChanged(nameof(PieTooltipPosition));
        });
        _ = LoadStatsAsync();
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
        var movimientos = (await _movimientoService.GetAllAsync()).ToList();
        var clientes = await _clienteService.GetAllAsync();
        var trabajos = await _trabajoService.GetAllAsync();

        TotalIngresos = movimientos.Where(m => m.TipoMovimientoSuma).Sum(m => m.Total);
        TotalGastos = movimientos.Where(m => !m.TipoMovimientoSuma).Sum(m => m.Total);
        Balance = TotalIngresos - TotalGastos;

        ClientesActivos = clientes.Count();
        TrabajosPendientes = trabajos.Count(t => t.FechaFin == null);

        // Prepare Chart Data (Current Year)
        var currentYear = DateTime.Now.Year;
        var monthlyIncome = new double[12];
        var monthlyExpenses = new double[12];

        foreach (var m in movimientos.Where(m => m.Fecha.Year == currentYear))
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
                DataLabelsFormatter = point => $"{cat.Name}: {point.Coordinate.PrimaryValue:C}"
            });
        }
    }
}
