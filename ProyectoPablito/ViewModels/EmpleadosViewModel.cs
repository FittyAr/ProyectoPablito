using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class EmpleadosViewModel : ViewModelBase
{
    private readonly IEmpleadoService _empleadoService;

    [ObservableProperty]
    private ObservableCollection<EmpleadoDto> _empleados = new();

    public EmpleadosViewModel(IEmpleadoService empleadoService)
    {
        _empleadoService = empleadoService;
        LoadEmpleadosCommand = new AsyncRelayCommand(LoadEmpleadosAsync);
    }

    public IAsyncRelayCommand LoadEmpleadosCommand { get; }

    public async Task LoadEmpleadosAsync()
    {
        var list = await _empleadoService.GetAllAsync();
        Empleados = new ObservableCollection<EmpleadoDto>(list);
    }
}
