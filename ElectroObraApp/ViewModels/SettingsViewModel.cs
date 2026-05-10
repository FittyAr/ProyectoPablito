using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectroObraApp.Application.Interfaces;

namespace ElectroObraApp.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly IUserSettingsService _settingsService;

    [ObservableProperty]
    private string _appName;

    [ObservableProperty]
    private string _logoPath;

    [ObservableProperty]
    private string _backgroundPath;

    [ObservableProperty]
    private string _selectedTheme;

    public ObservableCollection<string> Themes { get; } = new() { "Oscuro", "Media Noche", "Industrial" };

    public SettingsViewModel(IUserSettingsService settingsService)
    {
        _settingsService = settingsService;
        
        _appName = _settingsService.GetAppName();
        _logoPath = _settingsService.GetLogoPath();
        _backgroundPath = _settingsService.GetBackgroundPath();
        _selectedTheme = _settingsService.GetTheme();
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        await _settingsService.SetAppNameAsync(AppName);
        await _settingsService.SetLogoPathAsync(LogoPath);
        await _settingsService.SetBackgroundPathAsync(BackgroundPath);
        await _settingsService.SetThemeAsync(SelectedTheme);
        
        if (Avalonia.Application.Current is App app)
        {
            app.SetTheme(SelectedTheme);
        }
    }
}
