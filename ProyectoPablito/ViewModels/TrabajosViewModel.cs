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

public partial class TrabajosViewModel : ViewModelBase
{
    private readonly ITrabajoService _trabajoService;
    private readonly IClienteService _clienteService;
    private readonly IUserSettingsService _settingsService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<TrabajoDto> _trabajos = new();

    [ObservableProperty]
    private int _pageSize;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private ObservableCollection<int> _pageSizeOptions = new() { 10, 30, 50, 100, 0 };

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private TrabajoEditViewModel? _editViewModel;

    public TrabajosViewModel(
        ITrabajoService trabajoService, 
        IClienteService clienteService, 
        IUserSettingsService settingsService,
        IServiceProvider serviceProvider)
    {
        _trabajoService = trabajoService;
        _clienteService = clienteService;
        _settingsService = settingsService;
        _serviceProvider = serviceProvider;
        _pageSize = _settingsService.GetPageSize();

        LoadTrabajosCommand = new AsyncRelayCommand(LoadTrabajosAsync);
        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand<TrabajoDto>(Edit);

        _ = LoadTrabajosAsync();
    }

    public IAsyncRelayCommand LoadTrabajosCommand { get; }
    public IRelayCommand AddCommand { get; }
    public IRelayCommand<TrabajoDto> EditCommand { get; }

    partial void OnPageSizeChanged(int value)
    {
        _ = _settingsService.SetPageSizeAsync(value);
        _ = LoadTrabajosAsync();
    }

    public async Task LoadTrabajosAsync()
    {
        var result = await _trabajoService.GetAllAsync();
        IEnumerable<TrabajoDto> paginated;
        if (PageSize > 0)
        {
            paginated = result.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
        }
        else
        {
            paginated = result;
        }

        Trabajos = new ObservableCollection<TrabajoDto>(paginated);
    }

    private void Add()
    {
        var vm = _serviceProvider.GetRequiredService<TrabajoEditViewModel>();
        _ = vm.LoadDataAsync();
        vm.CloseRequest += (s, success) =>
        {
            IsEditing = false;
            EditViewModel = null;
            if (success) _ = LoadTrabajosAsync();
        };
        EditViewModel = vm;
        IsEditing = true;
    }

    private void Edit(TrabajoDto? dto)
    {
        if (dto == null) return;
        var vm = _serviceProvider.GetRequiredService<TrabajoEditViewModel>();
        vm.Trabajo = dto.Adapt<TrabajoDto>();
        vm.Title = "Editar Trabajo";
        _ = vm.LoadDataAsync();
        vm.CloseRequest += (s, success) =>
        {
            IsEditing = false;
            EditViewModel = null;
            if (success) _ = LoadTrabajosAsync();
        };
        EditViewModel = vm;
        IsEditing = true;
    }
}
