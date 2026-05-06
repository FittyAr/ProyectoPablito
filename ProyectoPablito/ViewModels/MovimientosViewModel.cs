using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class MovimientosViewModel : ViewModelBase
{
    private readonly IMovimientoService _movimientoService;

    [ObservableProperty]
    private ObservableCollection<MovimientoDto> _movimientos = new();

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private MovimientoEditViewModel? _editViewModel;

    private readonly IServiceProvider _serviceProvider;

    public MovimientosViewModel(IMovimientoService movimientoService, IServiceProvider serviceProvider)
    {
        _movimientoService = movimientoService;
        _serviceProvider = serviceProvider;
        LoadMovimientosCommand = new AsyncRelayCommand(LoadMovimientosAsync);
        AddCommand = new RelayCommand(OnAdd);
        EditCommand = new RelayCommand<MovimientoDto>(OnEdit);
    }

    public IAsyncRelayCommand LoadMovimientosCommand { get; }
    public IRelayCommand AddCommand { get; }
    public IRelayCommand<MovimientoDto> EditCommand { get; }

    private void OnAdd()
    {
        EditViewModel = _serviceProvider.GetRequiredService<MovimientoEditViewModel>();
        EditViewModel.Title = "Nuevo Movimiento";
        EditViewModel.CloseRequest += OnEditFinished;
        _ = EditViewModel.LoadDataCommand.ExecuteAsync(null);
        IsEditing = true;
    }

    private void OnEdit(MovimientoDto? dto)
    {
        if (dto == null) return;
        EditViewModel = _serviceProvider.GetRequiredService<MovimientoEditViewModel>();
        EditViewModel.Movimiento = dto;
        EditViewModel.Title = "Editar Movimiento";
        EditViewModel.CloseRequest += OnEditFinished;
        _ = EditViewModel.LoadDataCommand.ExecuteAsync(null);
        IsEditing = true;
    }

    private void OnEditFinished(object? sender, bool saved)
    {
        IsEditing = false;
        if (saved) _ = LoadMovimientosAsync();
    }

    private async Task LoadMovimientosAsync()
    {
        var result = await _movimientoService.GetAllAsync();
        Movimientos.Clear();
        foreach (var item in result)
        {
            Movimientos.Add(item);
        }
    }
}
