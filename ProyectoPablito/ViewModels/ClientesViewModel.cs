using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class ClientesViewModel : ViewModelBase
{
    private readonly IClienteService _clienteService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<ClienteDto> _clientes = new();

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private ClienteEditViewModel? _editViewModel;

    public ClientesViewModel(IClienteService clienteService, IServiceProvider serviceProvider)
    {
        _clienteService = clienteService;
        _serviceProvider = serviceProvider;
        LoadClientesCommand = new AsyncRelayCommand(LoadClientesAsync);
        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand<ClienteDto>(Edit);
    }

    public IAsyncRelayCommand LoadClientesCommand { get; }
    public IRelayCommand AddCommand { get; }
    public IRelayCommand<ClienteDto> EditCommand { get; }

    public async Task LoadClientesAsync()
    {
        var list = await _clienteService.GetAllAsync();
        Clientes = new ObservableCollection<ClienteDto>(list);
    }

    private void Add()
    {
        var vm = _serviceProvider.GetRequiredService<ClienteEditViewModel>();
        vm.CloseRequest += (s, success) =>
        {
            IsEditing = false;
            if (success) _ = LoadClientesAsync();
        };
        EditViewModel = vm;
        IsEditing = true;
    }

    private void Edit(ClienteDto? dto)
    {
        if (dto == null) return;
        var vm = _serviceProvider.GetRequiredService<ClienteEditViewModel>();
        vm.Cliente = dto;
        vm.Title = "Editar Cliente";
        vm.CloseRequest += (s, success) =>
        {
            IsEditing = false;
            if (success) _ = LoadClientesAsync();
        };
        EditViewModel = vm;
        IsEditing = true;
    }
}
