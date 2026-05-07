using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class TrabajoEditViewModel : ViewModelBase
{
    private readonly ITrabajoService _trabajoService;
    private readonly IClienteService _clienteService;

    [ObservableProperty]
    private TrabajoDto _trabajo = new() { FechaInicio = DateTime.Now };

    partial void OnTrabajoChanged(TrabajoDto value)
    {
        OnPropertyChanged(nameof(FechaInicioOffset));
    }

    [ObservableProperty]
    private ObservableCollection<ClienteDto> _clientes = new();

    [ObservableProperty]
    private string _title = "Nuevo Trabajo";

    public DateTimeOffset? FechaInicioOffset
    {
        get => new DateTimeOffset(Trabajo.FechaInicio);
        set
        {
            if (value.HasValue && new DateTimeOffset(Trabajo.FechaInicio) != value.Value)
            {
                Trabajo.FechaInicio = value.Value.DateTime;
                OnPropertyChanged(nameof(FechaInicioOffset));
            }
        }
    }

    public TrabajoEditViewModel(ITrabajoService trabajoService, IClienteService clienteService)
    {
        _trabajoService = trabajoService;
        _clienteService = clienteService;
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new RelayCommand(Cancel);
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
    }

    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IAsyncRelayCommand LoadDataCommand { get; }

    public async Task LoadDataAsync()
    {
        var list = await _clienteService.GetAllAsync();
        Clientes = new ObservableCollection<ClienteDto>(list);
    }

    private async Task SaveAsync()
    {
        bool success;
        if (Trabajo.Id == Guid.Empty)
            success = await _trabajoService.CreateAsync(Trabajo);
        else
            success = await _trabajoService.UpdateAsync(Trabajo);

        if (success)
        {
            CloseRequest?.Invoke(this, true);
        }
    }

    private void Cancel()
    {
        CloseRequest?.Invoke(this, false);
    }

    public event EventHandler<bool>? CloseRequest;
}
