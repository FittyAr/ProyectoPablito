using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class EmpleadoEditViewModel : ViewModelBase
{
    private readonly IEmpleadoService _empleadoService;

    [ObservableProperty]
    private EmpleadoDto _empleado = new() { FechaIngreso = DateTime.Now };

    partial void OnEmpleadoChanged(EmpleadoDto value)
    {
        OnPropertyChanged(nameof(FechaIngresoOffset));
    }

    [ObservableProperty]
    private string _title = "Nuevo Empleado";

    public DateTimeOffset? FechaIngresoOffset
    {
        get => new DateTimeOffset(Empleado.FechaIngreso);
        set
        {
            if (value.HasValue)
            {
                Empleado.FechaIngreso = value.Value.DateTime;
                OnPropertyChanged(nameof(FechaIngresoOffset));
            }
        }
    }

    public EmpleadoEditViewModel(IEmpleadoService empleadoService)
    {
        _empleadoService = empleadoService;
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new RelayCommand(Cancel);
    }

    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }

    private async Task SaveAsync()
    {
        bool success;
        if (Empleado.Id == Guid.Empty)
            success = await _empleadoService.CreateAsync(Empleado);
        else
            success = await _empleadoService.UpdateAsync(Empleado);

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
