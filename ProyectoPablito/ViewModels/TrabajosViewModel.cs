using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<TrabajoDto> _trabajos = new();

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private TrabajoEditViewModel? _editViewModel;

    public TrabajosViewModel(ITrabajoService trabajoService, IClienteService clienteService, IServiceProvider serviceProvider)
    {
        _trabajoService = trabajoService;
        _clienteService = clienteService;
        _serviceProvider = serviceProvider;
        LoadTrabajosCommand = new AsyncRelayCommand(LoadTrabajosAsync);
        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand<TrabajoDto>(Edit);
    }

    public IAsyncRelayCommand LoadTrabajosCommand { get; }
    public IRelayCommand AddCommand { get; }
    public IRelayCommand<TrabajoDto> EditCommand { get; }

    public async Task LoadTrabajosAsync()
    {
        var list = await _trabajoService.GetAllAsync();
        Trabajos = new ObservableCollection<TrabajoDto>(list);
    }

    private void Add()
    {
        var vm = _serviceProvider.GetRequiredService<TrabajoEditViewModel>();
        _ = vm.LoadDataAsync();
        vm.CloseRequest += (s, success) =>
        {
            IsEditing = false;
            if (success) _ = LoadTrabajosAsync();
        };
        EditViewModel = vm;
        IsEditing = true;
    }

    private void Edit(TrabajoDto? dto)
    {
        if (dto == null) return;
        var vm = _serviceProvider.GetRequiredService<TrabajoEditViewModel>();
        vm.Trabajo = dto;
        vm.Title = "Editar Trabajo";
        _ = vm.LoadDataAsync();
        vm.CloseRequest += (s, success) =>
        {
            IsEditing = false;
            if (success) _ = LoadTrabajosAsync();
        };
        EditViewModel = vm;
        IsEditing = true;
    }
}
