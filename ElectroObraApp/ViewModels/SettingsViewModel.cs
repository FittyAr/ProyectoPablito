using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectroObraApp.Application.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;

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

    [ObservableProperty]
    private string _selectedDashboardPeriod;

    [ObservableProperty]
    private string _selectedEmailClient;

    [ObservableProperty]
    private decimal _multiplierSaturday;

    [ObservableProperty]
    private decimal _multiplierSunday;

    [ObservableProperty]
    private decimal _multiplierHoliday;

    [ObservableProperty]
    private DateTime? _newHolidayDate = DateTime.Now;

    public ObservableCollection<DateTime> Holidays { get; } = new();

    public ObservableCollection<string> Themes { get; } = new() { "Oscuro", "Media Noche", "Industrial", "Solar", "Cibernético", "Océano", "Claro" };
    public ObservableCollection<string> DashboardPeriods { get; } = new() { "Mensual", "Anual", "Total" };
    public ObservableCollection<string> EmailClients { get; } = new() { "SystemDefault", "Gmail", "Yahoo", "OutlookWeb" };

    public SettingsViewModel(IUserSettingsService settingsService)
    {
        _settingsService = settingsService;
        
        _appName = _settingsService.GetAppName();
        _logoPath = _settingsService.GetLogoPath();
        _backgroundPath = _settingsService.GetBackgroundPath();
        _selectedTheme = _settingsService.GetTheme();
        _selectedDashboardPeriod = _settingsService.GetDashboardPeriod();
        _selectedEmailClient = _settingsService.GetPreferredEmailClient();
        _multiplierSaturday = _settingsService.GetDefaultMultiplierSaturday();
        _multiplierSunday = _settingsService.GetDefaultMultiplierSunday();
        _multiplierHoliday = _settingsService.GetDefaultMultiplierHoliday();

        var holidaysJson = _settingsService.GetHolidaysJson();
        try
        {
            var dates = System.Text.Json.JsonSerializer.Deserialize<List<DateTime>>(holidaysJson);
            if (dates != null)
            {
                foreach (var d in dates.OrderBy(x => x)) Holidays.Add(d);
            }
        }
        catch { }
    }

    [RelayCommand]
    public void AddHoliday()
    {
        if (NewHolidayDate.HasValue && !Holidays.Contains(NewHolidayDate.Value.Date))
        {
            Holidays.Add(NewHolidayDate.Value.Date);
            // Sort holidays
            var sorted = Holidays.OrderBy(x => x).ToList();
            Holidays.Clear();
            foreach (var h in sorted) Holidays.Add(h);
        }
    }

    [RelayCommand]
    public void RemoveHoliday(DateTime holiday)
    {
        Holidays.Remove(holiday);
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        await _settingsService.SetAppNameAsync(AppName);
        await _settingsService.SetLogoPathAsync(LogoPath);
        await _settingsService.SetBackgroundPathAsync(BackgroundPath);
        await _settingsService.SetThemeAsync(SelectedTheme);
        await _settingsService.SetDashboardPeriodAsync(SelectedDashboardPeriod);
        await _settingsService.SetPreferredEmailClientAsync(SelectedEmailClient);
        await _settingsService.SetDefaultMultiplierSaturdayAsync(MultiplierSaturday);
        await _settingsService.SetDefaultMultiplierSundayAsync(MultiplierSunday);
        await _settingsService.SetDefaultMultiplierHolidayAsync(MultiplierHoliday);
        
        var json = System.Text.Json.JsonSerializer.Serialize(Holidays.ToList());
        await _settingsService.SetHolidaysJsonAsync(json);
        
        if (Avalonia.Application.Current is App app)
        {
            app.SetTheme(SelectedTheme);
        }
    }
}
