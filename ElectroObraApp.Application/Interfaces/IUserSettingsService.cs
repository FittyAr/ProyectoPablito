using System.Threading.Tasks;

namespace ElectroObraApp.Application.Interfaces;

public interface IUserSettingsService
{
    int GetPageSize();
    Task SetPageSizeAsync(int pageSize);
    
    string GetTheme();
    Task SetThemeAsync(string theme);
    
    string GetLogoPath();
    Task SetLogoPathAsync(string path);
    
    string GetBackgroundPath();
    Task SetBackgroundPathAsync(string path);
    
    string GetAppName();
    Task SetAppNameAsync(string name);

    string GetDashboardPeriod();
    Task SetDashboardPeriodAsync(string period);

    bool GetIsPrivacyMode();
    Task SetIsPrivacyModeAsync(bool isPrivate);
}

