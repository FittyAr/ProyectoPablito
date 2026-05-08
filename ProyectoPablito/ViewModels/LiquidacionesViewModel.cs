using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;
using System.Linq;

namespace ProyectoPablito.ViewModels;

public partial class LiquidacionesViewModel : ViewModelBase
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IExportService _exportService;
    private readonly IMovimientoService _movimientoService;

    [ObservableProperty]
    private ObservableCollection<LiquidacionDto> _liquidaciones = new();

    [ObservableProperty]
    private bool _isLoading;

    public bool HasLiquidaciones => Liquidaciones.Count > 0;
    public bool ShowEmptyMessage => !IsLoading && Liquidaciones.Count == 0;

    public LiquidacionesViewModel(
        ILiquidacionService liquidacionService, 
        IExportService exportService,
        IMovimientoService movimientoService)
    {
        _liquidacionService = liquidacionService;
        _exportService = exportService;
        _movimientoService = movimientoService;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        ExportPdfCommand = new AsyncRelayCommand<LiquidacionDto>(ExportPdfAsync);
    }

    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand<LiquidacionDto> ExportPdfCommand { get; }

    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _liquidacionService.GetAllAsync();
            Liquidaciones = new ObservableCollection<LiquidacionDto>(list);
            OnPropertyChanged(nameof(HasLiquidaciones));
            OnPropertyChanged(nameof(ShowEmptyMessage));
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(ShowEmptyMessage));
        }
    }

    private async Task ExportPdfAsync(LiquidacionDto? dto)
    {
        if (dto == null) return;

        // Buscar los adelantos que formaron parte de esta liquidación
        var todosMovimientos = await _movimientoService.GetAllAsync();
        var adelantos = todosMovimientos.Where(m => 
            m.EmpleadoId == dto.EmpleadoId && 
            m.Fecha >= dto.FechaInicio && 
            m.Fecha <= dto.FechaFin &&
            m.TipoMovimientoId == Guid.Parse("00000000-0000-0000-0000-000000000003")); // Adelantos

        var pdf = await _exportService.ExportLiquidacionToPdfAsync(dto, adelantos);
        
        // Lógica para guardar el archivo (usualmente delegada a un servicio de UI o guardado automático en carpeta temporal)
        var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Liquidacion_{dto.EmpleadoNombre}_{dto.FechaFin:yyyyMMdd}.pdf");
        await System.IO.File.WriteAllBytesAsync(path, pdf);
        
        // Notificar al usuario (aquí se podría usar un servicio de notificaciones)
    }
}
