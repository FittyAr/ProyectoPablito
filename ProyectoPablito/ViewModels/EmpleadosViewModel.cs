using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class EmpleadosViewModel : ViewModelBase
{
    private readonly IEmpleadoService _empleadoService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<EmpleadoDto> _empleados = new();

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private EmpleadoEditViewModel? _editViewModel;

    public EmpleadosViewModel(IEmpleadoService empleadoService, IServiceProvider serviceProvider)
    {
        _empleadoService = empleadoService;
        _serviceProvider = serviceProvider;
        LoadEmpleadosCommand = new AsyncRelayCommand(LoadEmpleadosAsync);
        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand<EmpleadoDto>(Edit);
    }

    public IAsyncRelayCommand LoadEmpleadosCommand { get; }
    public IRelayCommand AddCommand { get; }
    public IRelayCommand<EmpleadoDto> EditCommand { get; }

    public async Task LoadEmpleadosAsync()
    {
        var list = await _empleadoService.GetAllAsync();
        Empleados = new ObservableCollection<EmpleadoDto>(list);
    }

    private void Add()
    {
        var vm = _serviceProvider.GetRequiredService<EmpleadoEditViewModel>();
        vm.CloseRequest += (s, success) =>
        {
            IsEditing = false;
            if (success) _ = LoadEmpleadosAsync();
        };
        EditViewModel = vm;
        IsEditing = true;
    }

    private void Edit(EmpleadoDto? dto)
    {
        if (dto == null) return;
        var vm = _serviceProvider.GetRequiredService<EmpleadoEditViewModel>();
        vm.Empleado = dto;
        vm.Title = "Editar Empleado";
        vm.CloseRequest += (s, success) =>
        {
            IsEditing = false;
            if (success) _ = LoadEmpleadosAsync();
        };
        EditViewModel = vm;
        IsEditing = true;
    }
}
