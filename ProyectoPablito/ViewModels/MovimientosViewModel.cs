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

    public MovimientosViewModel(
        IMovimientoService movimientoService, 
        IExportService exportService, 
        IUserSettingsService settingsService,
        IServiceProvider serviceProvider)
    {
        _movimientoService = movimientoService;
        _exportService = exportService;
        _settingsService = settingsService;
        _serviceProvider = serviceProvider;
        _pageSize = _settingsService.GetPageSize();

        LoadMovimientosCommand = new AsyncRelayCommand(LoadMovimientosAsync);
        AddCommand = new RelayCommand(OnAdd);
        EditCommand = new RelayCommand<MovimientoDto>(OnEdit);
        ExportPdfCommand = new AsyncRelayCommand(ExportPdfAsync);
        ExportExcelCommand = new AsyncRelayCommand(ExportExcelAsync);

        _ = LoadMovimientosAsync();
    }

    public IAsyncRelayCommand LoadMovimientosCommand { get; }
    public IRelayCommand AddCommand { get; }
    public IRelayCommand<MovimientoDto> EditCommand { get; }
    public IAsyncRelayCommand ExportPdfCommand { get; }
    public IAsyncRelayCommand ExportExcelCommand { get; }

    private void OnAdd()
    {
        EditViewModel = _serviceProvider.GetRequiredService<MovimientoEditViewModel>();
        EditViewModel.Title = "Nuevo Movimiento";
        EditViewModel.CloseRequest += OnEditFinished;
        _ = EditViewModel.LoadDataCommand.ExecuteAsync(null);
        IsEditing = true;
    }

    private void OnEdit(MovimientoDto? dto)
    {
        if (dto == null) return;
        EditViewModel = _serviceProvider.GetRequiredService<MovimientoEditViewModel>();
        EditViewModel.Movimiento = dto.Adapt<MovimientoDto>();
        EditViewModel.Title = "Editar Movimiento";
        EditViewModel.CloseRequest += OnEditFinished;
        _ = EditViewModel.LoadDataCommand.ExecuteAsync(null);
        IsEditing = true;
    }

    private void OnEditFinished(object? sender, bool saved)
    {
        IsEditing = false;
        EditViewModel = null;
        if (saved) _ = LoadMovimientosAsync();
    }

    private async Task ExportPdfAsync()
    {
        var bytes = await _exportService.ExportMovimientosToPdfAsync(Movimientos);
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Movimientos_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        await File.WriteAllBytesAsync(path, bytes);
    }

    private async Task ExportExcelAsync()
    {
        var bytes = await _exportService.ExportMovimientosToExcelAsync(Movimientos);
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Movimientos_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
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
        
        // Paginar localmente por ahora
        IEnumerable<MovimientoDto> paginated;
        if (PageSize > 0)
        {
            paginated = result.OrderByDescending(m => m.Fecha)
                              .Skip((CurrentPage - 1) * PageSize)
                              .Take(PageSize);
        }
        else
        {
            paginated = result.OrderByDescending(m => m.Fecha);
        }

        Movimientos.Clear();
        foreach (var item in paginated)
        {
            Movimientos.Add(item);
        }
    }
}
