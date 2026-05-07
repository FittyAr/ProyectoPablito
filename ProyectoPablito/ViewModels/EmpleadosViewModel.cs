using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class EmpleadosViewModel : ViewModelBase
{
    private readonly IEmpleadoService _empleadoService;
    private readonly IUserSettingsService _settingsService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<EmpleadoDto> _empleados = new();

    [ObservableProperty]
    private int _pageSize;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private ObservableCollection<int> _pageSizeOptions = new() { 10, 30, 50, 100, 0 };

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private EmpleadoEditViewModel? _editViewModel;

    public EmpleadosViewModel(IEmpleadoService empleadoService, IUserSettingsService settingsService, IServiceProvider serviceProvider)
    {
        _empleadoService = empleadoService;
        _settingsService = settingsService;
        _serviceProvider = serviceProvider;
        _pageSize = _settingsService.GetPageSize();

        LoadEmpleadosCommand = new AsyncRelayCommand(LoadEmpleadosAsync);
        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand<EmpleadoDto>(Edit);

        _ = LoadEmpleadosAsync();
    }

    public IAsyncRelayCommand LoadEmpleadosCommand { get; }
    public IRelayCommand AddCommand { get; }
    public IRelayCommand<EmpleadoDto> EditCommand { get; }

    partial void OnPageSizeChanged(int value)
    {
        _ = _settingsService.SetPageSizeAsync(value);
        _ = LoadEmpleadosAsync();
    }

    public async Task LoadEmpleadosAsync()
    {
        var result = await _empleadoService.GetAllAsync();
        IEnumerable<EmpleadoDto> paginated;
        if (PageSize > 0)
        {
            paginated = result.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
        }
        else
        {
            paginated = result;
        }

        Empleados = new ObservableCollection<EmpleadoDto>(paginated);
    }

    private void Add()
    {
        var vm = _serviceProvider.GetRequiredService<EmpleadoEditViewModel>();
        vm.CloseRequest += (s, success) =>
        {
            IsEditing = false;
            EditViewModel = null;
            if (success) _ = LoadEmpleadosAsync();
        };
        EditViewModel = vm;
        IsEditing = true;
    }

    private void Edit(EmpleadoDto? dto)
    {
        if (dto == null) return;
        var vm = _serviceProvider.GetRequiredService<EmpleadoEditViewModel>();
        vm.Empleado = dto.Adapt<EmpleadoDto>();
        vm.Title = "Editar Empleado";
        vm.CloseRequest += (s, success) =>
        {
            IsEditing = false;
            EditViewModel = null;
            if (success) _ = LoadEmpleadosAsync();
        };
        EditViewModel = vm;
        IsEditing = true;
    }
}
