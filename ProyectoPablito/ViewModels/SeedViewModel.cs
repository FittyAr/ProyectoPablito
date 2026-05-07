using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoPablito.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace ProyectoPablito.ViewModels;

public partial class SeedViewModel : ViewModelBase
{
    private readonly IDatabaseSeedService _seedService;
    private readonly ILocalizationService _localizationService;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _showWarning = true;

    [ObservableProperty]
    private bool _showConfirmation;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public SeedViewModel(IDatabaseSeedService seedService, ILocalizationService localizationService)
    {
        _seedService = seedService;
        _localizationService = localizationService;
        
        StatusMessage = _localizationService.GetString("Seed.StatusIdle");
        PrepareSeedCommand = new RelayCommand(() => ShowConfirmation = true);
        CancelConfirmationCommand = new RelayCommand(() => ShowConfirmation = false);
        SeedCommand = new AsyncRelayCommand(OnSeedAsync, () => !IsBusy);
    }

    public IRelayCommand PrepareSeedCommand { get; }
    public IRelayCommand CancelConfirmationCommand { get; }
    public IAsyncRelayCommand SeedCommand { get; }

    private async Task OnSeedAsync()
    {
        try
        {
            IsBusy = true;
            ShowWarning = false;
            ShowConfirmation = false;
            StatusMessage = _localizationService.GetString("Seed.StatusProcessing");

            await _seedService.SeedAsync();

            StatusMessage = _localizationService.GetString("Seed.StatusSuccess");
        }
        catch (Exception ex)
        {
            StatusMessage = $"{_localizationService.GetString("Seed.StatusError")}: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
