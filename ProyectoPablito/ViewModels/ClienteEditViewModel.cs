using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class ClienteEditViewModel : ViewModelBase
{
    private readonly IClienteService _clienteService;

    [ObservableProperty]
    private ClienteDto _cliente = new();

    [ObservableProperty]
    private string _title = "Nuevo Cliente";

    public ClienteEditViewModel(IClienteService clienteService)
    {
        _clienteService = clienteService;
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new RelayCommand(Cancel);
    }

    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }

    private async Task SaveAsync()
    {
        bool success;
        if (Cliente.Id == Guid.Empty)
            success = await _clienteService.CreateAsync(Cliente);
        else
            success = await _clienteService.UpdateAsync(Cliente);

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
