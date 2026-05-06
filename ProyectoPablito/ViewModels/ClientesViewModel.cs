using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class ClientesViewModel : ViewModelBase
{
    private readonly IClienteService _clienteService;

    [ObservableProperty]
    private ObservableCollection<ClienteDto> _clientes = new();

    public ClientesViewModel(IClienteService clienteService)
    {
        _clienteService = clienteService;
        LoadClientesCommand = new AsyncRelayCommand(LoadClientesAsync);
    }

    public IAsyncRelayCommand LoadClientesCommand { get; }

    public async Task LoadClientesAsync()
    {
        var list = await _clienteService.GetAllAsync();
        Clientes = new ObservableCollection<ClienteDto>(list);
    }
}
