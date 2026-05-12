using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Interfaces;
using System.Linq;

namespace ElectroObraApp.ViewModels;

public partial class LiquidacionesViewModel : ViewModelBase
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IExportService _exportService;
    private readonly IMovimientoService _movimientoService;
    private readonly IEmpleadoService _empleadoService;
    private readonly IUserSettingsService _settingsService;

    [ObservableProperty]
    private ObservableCollection<LiquidacionDto> _liquidaciones = new();

    [ObservableProperty]
    private bool _isLoading;

    public bool HasLiquidaciones => Liquidaciones.Count > 0;
    public bool ShowEmptyMessage => !IsLoading && Liquidaciones.Count == 0;

    public LiquidacionesViewModel(
        ILiquidacionService liquidacionService, 
        IExportService exportService,
        IMovimientoService movimientoService,
        IEmpleadoService empleadoService,
        IUserSettingsService settingsService)
    {
        _liquidacionService = liquidacionService;
        _exportService = exportService;
        _movimientoService = movimientoService;
        _empleadoService = empleadoService;
        _settingsService = settingsService;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        ExportPdfCommand = new AsyncRelayCommand<LiquidacionDto>(ExportPdfAsync);
        ShareEmailCommand = new AsyncRelayCommand<LiquidacionDto>(ShareEmailAsync);
        ShareWhatsAppCommand = new AsyncRelayCommand<LiquidacionDto>(ShareWhatsAppAsync);
        NuevaLiquidacionCommand = new RelayCommand(NuevaLiquidacion);
    }

    // Acción de navegación inyectada desde MainViewModel
    public Action? OnNuevaLiquidacion { get; set; }

    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand<LiquidacionDto> ExportPdfCommand { get; }
    public IAsyncRelayCommand<LiquidacionDto> ShareEmailCommand { get; }
    public IAsyncRelayCommand<LiquidacionDto> ShareWhatsAppCommand { get; }
    public IRelayCommand NuevaLiquidacionCommand { get; }

    private void NuevaLiquidacion() => OnNuevaLiquidacion?.Invoke();

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
        var path = await GenerateAndSavePdfAsync(dto);
        if (path != null)
        {
            // Podrías abrir el archivo automáticamente si quieres
            // System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path) { UseShellExecute = true });
        }
    }

    private async Task<string?> GenerateAndSavePdfAsync(LiquidacionDto? dto)
    {
        if (dto == null) return null;

        // Buscar los adelantos que formaron parte de esta liquidación
        var todosMovimientos = await _movimientoService.GetAllAsync();
        var adelantos = todosMovimientos.Where(m => 
            m.EmpleadoId == dto.EmpleadoId && 
            m.Fecha >= dto.FechaInicio && 
            m.Fecha <= dto.FechaFin &&
            m.TipoMovimientoId == Guid.Parse("00000000-0000-0000-0000-000000000003")); // Adelantos

        var pdf = await _exportService.ExportLiquidacionToPdfAsync(dto, adelantos);
        
        var fileName = $"Reporte_Liquidacion_{dto.EmpleadoNombre}_{dto.FechaFin:yyyyMMdd}.pdf";
        var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
        await System.IO.File.WriteAllBytesAsync(path, pdf);
        return path;
    }

    private async Task ShareEmailAsync(LiquidacionDto? dto)
    {
        if (dto == null) return;
        
        var empleado = await _empleadoService.GetByIdAsync(dto.EmpleadoId);
        if (empleado == null || string.IsNullOrWhiteSpace(empleado.Email))
        {
            Serilog.Log.Warning("No se puede enviar email: El empleado {Nombre} no tiene un email configurado.", dto.EmpleadoNombre);
            return;
        }

        try 
        {
            var path = await GenerateAndSavePdfAsync(dto);
            Application.Helpers.EmailHelper.OpenEmailClient(empleado.Email, _settingsService);
            Serilog.Log.Information("Abriendo cliente de correo para {Email}. Archivo guardado en {Path}", empleado.Email, path);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error al compartir liquidación por email");
        }
    }

    private async Task ShareWhatsAppAsync(LiquidacionDto? dto)
    {
        if (dto == null) return;
        
        var empleado = await _empleadoService.GetByIdAsync(dto.EmpleadoId);
        if (empleado == null || string.IsNullOrWhiteSpace(empleado.Telefono))
        {
            Serilog.Log.Warning("No se puede enviar WhatsApp: El empleado {Nombre} no tiene un teléfono configurado.", dto.EmpleadoNombre);
            return;
        }

        try 
        {
            var path = await GenerateAndSavePdfAsync(dto);
            
            var mensaje = $"Hola {empleado.Nombre}, te envío el reporte de tu liquidación del periodo {dto.FechaInicio:dd/MM/yyyy} al {dto.FechaFin:dd/MM/yyyy}.";
            var url = $"https://api.whatsapp.com/send?phone={empleado.Telefono}&text={Uri.EscapeDataString(mensaje)}";
            
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
            Serilog.Log.Information("Abriendo WhatsApp para {Telefono}. Archivo guardado en {Path}", empleado.Telefono, path);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error al compartir liquidación por WhatsApp");
        }
    }
}
