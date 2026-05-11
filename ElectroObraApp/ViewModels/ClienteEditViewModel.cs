using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Interfaces;

namespace ElectroObraApp.ViewModels;

public partial class ClienteEditViewModel : ViewModelBase
{
    private readonly IClienteService _clienteService;
    private readonly IUserSettingsService _settingsService;

    [ObservableProperty]
    private ClienteDto _cliente = new();

    [ObservableProperty]
    private string _title = "Nuevo Cliente";

    public ClienteEditViewModel(IClienteService clienteService, IUserSettingsService settingsService)
    {
        _clienteService = clienteService;
        _settingsService = settingsService;
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new RelayCommand(Cancel);
        AddContactCommand = new RelayCommand(AddContact);
        RemoveContactCommand = new RelayCommand<ClienteContactoDto>(RemoveContact);
        OpenEmailCommand = new RelayCommand(OpenEmail);
    }

    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IRelayCommand AddContactCommand { get; }
    public IRelayCommand<ClienteContactoDto> RemoveContactCommand { get; }
    public IRelayCommand OpenEmailCommand { get; }

    private void OpenEmail()
    {
        if (!string.IsNullOrEmpty(Cliente.Email))
        {
            Application.Helpers.EmailHelper.OpenEmailClient(Cliente.Email, _settingsService);
        }
    }

    private void AddContact()
    {
        Cliente.Contactos.Add(new ClienteContactoDto { Etiqueta = "General" });
    }

    private void RemoveContact(ClienteContactoDto? contacto)
    {
        if (contacto != null)
        {
            Cliente.Contactos.Remove(contacto);
        }
    }

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

