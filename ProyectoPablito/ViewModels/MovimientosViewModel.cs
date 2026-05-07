using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Mapster;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class MovimientosViewModel : ViewModelBase
{
    private readonly IMovimientoService _movimientoService;
    private readonly ITipoMovimientoService _tipoMovimientoService;
    private readonly IExportService _exportService;
    private readonly IUserSettingsService _settingsService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<MovimientoDto> _movimientos = new();

    [ObservableProperty]
    private int _pageSize;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private ObservableCollection<int> _pageSizeOptions = new() { 10, 30, 50, 100, 0 }; // 0 = Todos

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private MovimientoEditViewModel? _editViewModel;

    [ObservableProperty]
    private ObservableCollection<TipoMovimientoDto> _tiposMovimiento = new();

    // Filtros
    [ObservableProperty] private string _filtroConcepto = string.Empty;
    [ObservableProperty] private Guid? _filtroTipoId;
    [ObservableProperty] private DateTime? _filtroFechaDesde;
    [ObservableProperty] private DateTime? _filtroFechaHasta;
    [ObservableProperty] private decimal? _filtroMontoMin;
    [ObservableProperty] private decimal? _filtroMontoMax;

    public MovimientosViewModel(
        IMovimientoService movimientoService, 
        ITipoMovimientoService tipoMovimientoService,
        IExportService exportService, 
        IUserSettingsService settingsService,
        IServiceProvider serviceProvider)
    {
        _movimientoService = movimientoService;
        _tipoMovimientoService = tipoMovimientoService;
        _exportService = exportService;
        _settingsService = settingsService;
        _serviceProvider = serviceProvider;
        _pageSize = _settingsService.GetPageSize();

        LoadMovimientosCommand = new AsyncRelayCommand(LoadMovimientosAsync);
        AddCommand = new AsyncRelayCommand(OnAddAsync);
        EditCommand = new AsyncRelayCommand<MovimientoDto>(OnEditAsync);
        LimpiarFiltrosCommand = new RelayCommand(LimpiarFiltros);
        
        ExportPdfCommand = new AsyncRelayCommand(ExportPdfAsync);
        ExportExcelCommand = new AsyncRelayCommand(ExportExcelAsync);
        ExportCsvCommand = new AsyncRelayCommand(ExportCsvAsync);
        ExportJsonCommand = new AsyncRelayCommand(ExportJsonAsync);
        ExportWordCommand = new AsyncRelayCommand(ExportWordAsync);

        _ = LoadInitialDataAsync();
    }

    public IAsyncRelayCommand LoadMovimientosCommand { get; }
    public IAsyncRelayCommand AddCommand { get; }
    public IAsyncRelayCommand<MovimientoDto> EditCommand { get; }
    public IRelayCommand LimpiarFiltrosCommand { get; }
    public IAsyncRelayCommand ExportPdfCommand { get; }
    public IAsyncRelayCommand ExportExcelCommand { get; }
    public IAsyncRelayCommand ExportCsvCommand { get; }
    public IAsyncRelayCommand ExportJsonCommand { get; }
    public IAsyncRelayCommand ExportWordCommand { get; }

    private async Task LoadInitialDataAsync()
    {
        var tipos = await _tipoMovimientoService.GetAllAsync();
        TiposMovimiento = new ObservableCollection<TipoMovimientoDto>(tipos);
        await LoadMovimientosAsync();
    }

    private async Task OnAddAsync()
    {
        var vm = _serviceProvider.GetRequiredService<MovimientoEditViewModel>();
        vm.Title = "Nuevo Movimiento";
        vm.CloseRequest += OnEditFinished;
        await vm.LoadDataAsync(); 
        vm.Movimiento = new MovimientoDto { Fecha = DateTime.Now }; // Initialize fresh
        EditViewModel = vm;
        IsEditing = true;
    }

    private async Task OnEditAsync(MovimientoDto? dto)
    {
        if (dto == null) return;
        var vm = _serviceProvider.GetRequiredService<MovimientoEditViewModel>();
        vm.Title = "Editar Movimiento";
        vm.CloseRequest += OnEditFinished;
        await vm.LoadDataAsync(); 
        vm.Movimiento = dto.Adapt<MovimientoDto>(); // Set data AFTER lists are loaded
        EditViewModel = vm;
        IsEditing = true;
    }

    private void OnEditFinished(object? sender, bool saved)
    {
        IsEditing = false;
        EditViewModel = null;
        if (saved) _ = LoadMovimientosAsync();
    }

    private void LimpiarFiltros()
    {
        FiltroConcepto = string.Empty;
        FiltroTipoId = null;
        FiltroFechaDesde = null;
        FiltroFechaHasta = null;
        FiltroMontoMin = null;
        FiltroMontoMax = null;
        _ = LoadMovimientosAsync();
    }

    // Filtros reactivos
    partial void OnFiltroConceptoChanged(string value) => _ = LoadMovimientosAsync();
    partial void OnFiltroTipoIdChanged(Guid? value) => _ = LoadMovimientosAsync();
    partial void OnFiltroFechaDesdeChanged(DateTime? value) => _ = LoadMovimientosAsync();
    partial void OnFiltroFechaHastaChanged(DateTime? value) => _ = LoadMovimientosAsync();
    partial void OnFiltroMontoMinChanged(decimal? value) => _ = LoadMovimientosAsync();
    partial void OnFiltroMontoMaxChanged(decimal? value) => _ = LoadMovimientosAsync();

    private async Task ExportPdfAsync()
    {
        var bytes = await _exportService.ExportMovimientosToPdfAsync(Movimientos);
        await SaveFileAsync(bytes, "pdf");
    }

    private async Task ExportExcelAsync()
    {
        var bytes = await _exportService.ExportMovimientosToExcelAsync(Movimientos);
        await SaveFileAsync(bytes, "xlsx");
    }

    private async Task ExportCsvAsync()
    {
        // TODO: Implementar en ExportService
        await Task.Yield();
    }

    private async Task ExportJsonAsync()
    {
        var json = System.Text.Json.JsonSerializer.Serialize(Movimientos);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        await SaveFileAsync(bytes, "json");
    }

    private async Task ExportWordAsync()
    {
        // TODO: Implementar en ExportService
        await Task.Yield();
    }

    private async Task SaveFileAsync(byte[] bytes, string ext)
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Movimientos_{DateTime.Now:yyyyMMddHHmmss}.{ext}");
        await File.WriteAllBytesAsync(path, bytes);
    }

    partial void OnPageSizeChanged(int value)
    {
        _ = _settingsService.SetPageSizeAsync(value);
        _ = LoadMovimientosAsync();
    }

    private async Task LoadMovimientosAsync()
    {
        var result = await _movimientoService.GetAllAsync();
        
        var query = result.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(FiltroConcepto))
            query = query.Where(m => m.Concepto.Contains(FiltroConcepto, StringComparison.OrdinalIgnoreCase));

        if (FiltroTipoId.HasValue)
            query = query.Where(m => m.TipoMovimientoId == FiltroTipoId.Value);

        if (FiltroFechaDesde.HasValue)
            query = query.Where(m => m.Fecha.Date >= FiltroFechaDesde.Value.Date);

        if (FiltroFechaHasta.HasValue)
            query = query.Where(m => m.Fecha.Date <= FiltroFechaHasta.Value.Date);

        if (FiltroMontoMin.HasValue)
            query = query.Where(m => m.Monto >= FiltroMontoMin.Value);

        if (FiltroMontoMax.HasValue)
            query = query.Where(m => m.Monto <= FiltroMontoMax.Value);

        // Ordenar y Paginar
        IEnumerable<MovimientoDto> paginated;
        if (PageSize > 0)
        {
            paginated = query.OrderByDescending(m => m.Fecha)
                             .Skip((CurrentPage - 1) * PageSize)
                             .Take(PageSize);
        }
        else
        {
            paginated = query.OrderByDescending(m => m.Fecha);
        }

        Movimientos.Clear();
        foreach (var item in paginated)
        {
            Movimientos.Add(item);
        }
    }
}
