using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.ViewModels;

public partial class TrabajosViewModel : ViewModelBase
{
    private readonly ITrabajoService _trabajoService;

    [ObservableProperty]
    private ObservableCollection<TrabajoDto> _trabajos = new();

    public TrabajosViewModel(ITrabajoService trabajoService)
    {
        _trabajoService = trabajoService;
        LoadTrabajosCommand = new AsyncRelayCommand(LoadTrabajosAsync);
    }

    public IAsyncRelayCommand LoadTrabajosCommand { get; }

    public async Task LoadTrabajosAsync()
    {
        var list = await _trabajoService.GetAllAsync();
        Trabajos = new ObservableCollection<TrabajoDto>(list);
    }
}
