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

public partial class ClientesViewModel : ViewModelBase
{
    private readonly IClienteService _clienteService;
    private readonly IUserSettingsService _settingsService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<ClienteDto> _clientes = new();

    [ObservableProperty]
    private int _pageSize;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private ObservableCollection<int> _pageSizeOptions = new() { 10, 30, 50, 100, 0 };

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private ClienteEditViewModel? _editViewModel;
    
    [ObservableProperty]
    private string _filtroNombre = string.Empty;

    public ClientesViewModel(IClienteService clienteService, IUserSettingsService settingsService, IServiceProvider serviceProvider)
    {
        _clienteService = clienteService;
        _settingsService = settingsService;
        _serviceProvider = serviceProvider;
        _pageSize = _settingsService.GetPageSize();

        LoadClientesCommand = new AsyncRelayCommand(LoadClientesAsync);
        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand<ClienteDto>(Edit);
        LimpiarFiltrosCommand = new RelayCommand(LimpiarFiltros);

        _ = LoadClientesAsync();
    }

    public IAsyncRelayCommand LoadClientesCommand { get; }
    public IRelayCommand AddCommand { get; }
    public IRelayCommand<ClienteDto> EditCommand { get; }
    public IRelayCommand LimpiarFiltrosCommand { get; }

    partial void OnPageSizeChanged(int value)
    {
        _ = _settingsService.SetPageSizeAsync(value);
        _ = LoadClientesAsync();
    }

    partial void OnFiltroNombreChanged(string value) => _ = LoadClientesAsync();

    private void LimpiarFiltros()
    {
        FiltroNombre = string.Empty;
        _ = LoadClientesAsync();
    }

    public async Task LoadClientesAsync()
    {
        var result = await _clienteService.GetAllAsync();
        var query = result.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(FiltroNombre))
        {
            query = query.Where(c => c.Nombre.Contains(FiltroNombre, StringComparison.OrdinalIgnoreCase) || 
                                    (c.Cuit != null && c.Cuit.Contains(FiltroNombre)));
        }

        IEnumerable<ClienteDto> paginated;
        if (PageSize > 0)
        {
            paginated = query.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
        }
        else
        {
            paginated = query;
        }

        Clientes = new ObservableCollection<ClienteDto>(paginated);
    }

    private void Add()
    {
        var vm = _serviceProvider.GetRequiredService<ClienteEditViewModel>();
        vm.CloseRequest += (s, success) =>
        {
            IsEditing = false;
            EditViewModel = null;
            if (success) _ = LoadClientesAsync();
        };
        EditViewModel = vm;
        IsEditing = true;
    }

    private void Edit(ClienteDto? dto)
    {
        if (dto == null) return;
        var vm = _serviceProvider.GetRequiredService<ClienteEditViewModel>();
        vm.Cliente = dto.Adapt<ClienteDto>();
        vm.Title = "Editar Cliente";
        vm.CloseRequest += (s, success) =>
        {
            IsEditing = false;
            EditViewModel = null;
            if (success) _ = LoadClientesAsync();
        };
        EditViewModel = vm;
        IsEditing = true;
    }
}
