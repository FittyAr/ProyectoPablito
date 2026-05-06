using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
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
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<MovimientoDto> _movimientos = new();

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private MovimientoEditViewModel? _editViewModel;

    public MovimientosViewModel(IMovimientoService movimientoService, IExportService exportService, IServiceProvider serviceProvider)
    {
        _movimientoService = movimientoService;
        _exportService = exportService;
        _serviceProvider = serviceProvider;
        LoadMovimientosCommand = new AsyncRelayCommand(LoadMovimientosAsync);
        AddCommand = new RelayCommand(OnAdd);
        EditCommand = new RelayCommand<MovimientoDto>(OnEdit);
        ExportPdfCommand = new AsyncRelayCommand(ExportPdfAsync);
        ExportExcelCommand = new AsyncRelayCommand(ExportExcelAsync);
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
        EditViewModel.Movimiento = dto;
        EditViewModel.Title = "Editar Movimiento";
        EditViewModel.CloseRequest += OnEditFinished;
        _ = EditViewModel.LoadDataCommand.ExecuteAsync(null);
        IsEditing = true;
    }

    private void OnEditFinished(object? sender, bool saved)
    {
        IsEditing = false;
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
