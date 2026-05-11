using Microsoft.Extensions.Configuration;
using ElectroObraApp.Application.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Globalization;

namespace ElectroObraApp.Infrastructure.Services;

public class UserSettingsService : IUserSettingsService
{
    private readonly IConfiguration _configuration;
    private readonly string _settingsPath;
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    public UserSettingsService(IConfiguration configuration)
    {
        _configuration = configuration;
        _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        
        // Ensure the file exists with basic structure if it doesn't
        if (!File.Exists(_settingsPath))
        {
            File.WriteAllText(_settingsPath, "{ \"Application\": { } }");
        }
    }

    public int GetPageSize() => _configuration.GetValue<int>("Application:LastPageSize", 30);
    public Task SetPageSizeAsync(int pageSize) => SetValueAsync("Application:LastPageSize", pageSize);

    public string GetTheme() => _configuration.GetValue<string>("Application:Appearance:Theme", "Dark") ?? "Dark";
    public Task SetThemeAsync(string theme) => SetValueAsync("Application:Appearance:Theme", theme);

    public string GetLogoPath() => _configuration.GetValue<string>("Application:Branding:LogoPath", "avares://ElectroObraApp/Assets/Images/electro-obra.png") ?? "";
    public Task SetLogoPathAsync(string path) => SetValueAsync("Application:Branding:LogoPath", path);

    public string GetBackgroundPath() => _configuration.GetValue<string>("Application:Branding:BackgroundPath", "avares://ElectroObraApp/Assets/Images/electro-obra3.png") ?? "";
    public Task SetBackgroundPathAsync(string path) => SetValueAsync("Application:Branding:BackgroundPath", path);

    public string GetAppName() => GetValue("Application:Name", "ElectroObraApp");
    public async Task SetAppNameAsync(string name) => await SetValueAsync("Application:Name", name);

    public string GetDashboardPeriod() => GetValue("Application:Dashboard:Period", "Mensual");
    public async Task SetDashboardPeriodAsync(string period) => await SetValueAsync("Application:Dashboard:Period", period);

    public bool GetIsPrivacyMode() => bool.TryParse(GetValue("Application:Dashboard:IsPrivacyMode", "false"), out var result) && result;
    public async Task SetIsPrivacyModeAsync(bool isPrivate) => await SetValueAsync("Application:Dashboard:IsPrivacyMode", isPrivate.ToString().ToLower());

    public string GetPreferredEmailClient() => GetValue("Application:Email:PreferredClient", "SystemDefault");
    public async Task SetPreferredEmailClientAsync(string client) => await SetValueAsync("Application:Email:PreferredClient", client);

    public decimal GetDefaultMultiplierSaturday() => decimal.TryParse(GetValue("Application:Settlement:MultiplierSaturday", "1.5"), CultureInfo.InvariantCulture, out var result) ? result : 1.5m;
    public async Task SetDefaultMultiplierSaturdayAsync(decimal multiplier) => await SetValueAsync("Application:Settlement:MultiplierSaturday", multiplier.ToString(CultureInfo.InvariantCulture));

    public decimal GetDefaultMultiplierSunday() => decimal.TryParse(GetValue("Application:Settlement:MultiplierSunday", "2.0"), CultureInfo.InvariantCulture, out var result) ? result : 2.0m;
    public async Task SetDefaultMultiplierSundayAsync(decimal multiplier) => await SetValueAsync("Application:Settlement:MultiplierSunday", multiplier.ToString(CultureInfo.InvariantCulture));

    public decimal GetDefaultMultiplierHoliday() => decimal.TryParse(GetValue("Application:Settlement:MultiplierHoliday", "2.0"), CultureInfo.InvariantCulture, out var result) ? result : 2.0m;
    public async Task SetDefaultMultiplierHolidayAsync(decimal multiplier) => await SetValueAsync("Application:Settlement:MultiplierHoliday", multiplier.ToString(CultureInfo.InvariantCulture));

    public string GetHolidaysJson() => GetValue("Application:Settlement:Holidays", "[]");
    public async Task SetHolidaysJsonAsync(string holidaysJson) => await SetValueAsync("Application:Settlement:Holidays", holidaysJson);

    public bool GetDefaultIncludeSaturday() => _configuration.GetValue<bool>("Application:Settlement:IncludeSaturday", false);
    public async Task SetDefaultIncludeSaturdayAsync(bool value) => await SetValueAsync("Application:Settlement:IncludeSaturday", value);

    public bool GetDefaultIncludeSunday() => _configuration.GetValue<bool>("Application:Settlement:IncludeSunday", false);
    public async Task SetDefaultIncludeSundayAsync(bool value) => await SetValueAsync("Application:Settlement:IncludeSunday", value);

    public bool GetDefaultIncludeHoliday() => _configuration.GetValue<bool>("Application:Settlement:IncludeHoliday", false);
    public async Task SetDefaultIncludeHolidayAsync(bool value) => await SetValueAsync("Application:Settlement:IncludeHoliday", value);

    public string GetHolidayApiUrl() => GetValue("Application:Settlement:HolidayApiUrl", "https://api.argentinadatos.com/v1/feriados/");
    public async Task SetHolidayApiUrlAsync(string url) => await SetValueAsync("Application:Settlement:HolidayApiUrl", url);

    private string GetValue(string key, string defaultValue)
    {
        try
        {
            if (!File.Exists(_settingsPath)) return defaultValue;
            var jsonString = File.ReadAllText(_settingsPath);
            var root = JsonNode.Parse(jsonString);
            var node = root;
            foreach (var part in key.Split(':'))
            {
                node = node?[part];
            }

            if (node is null) return defaultValue;
            
            // If it's a simple value (string, number, etc.)
            if (node is JsonValue jValue)
            {
                // Try to get as string first
                if (jValue.TryGetValue<string>(out var s)) return s;
                return jValue.ToString();
            }
            
            // If it's an object or array, return its JSON representation
            return node.ToJsonString();
        }
        catch
        {
            return defaultValue;
        }
    }

    private async Task SetValueAsync(string keyPath, object value)
    {
        await _fileLock.WaitAsync();
        try
        {
            var jsonString = File.Exists(_settingsPath) ? await File.ReadAllTextAsync(_settingsPath) : "{}";
            var root = JsonNode.Parse(jsonString) ?? new JsonObject();

            var parts = keyPath.Split(':');
            JsonNode currentNode = root;

            for (int i = 0; i < parts.Length - 1; i++)
            {
                var part = parts[i];
                if (currentNode[part] is not JsonObject)
                {
                    currentNode[part] = new JsonObject();
                }
                currentNode = currentNode[part]!;
            }

            // Special handling for strings that are valid JSON (like our Holidays)
            // We want to save them as native JSON nodes if possible
            if (value is string s && (s.TrimStart().StartsWith("[") || s.TrimStart().StartsWith("{")))
            {
                try
                {
                    var node = JsonNode.Parse(s);
                    if (node != null)
                    {
                        currentNode[parts[^1]] = node;
                        goto Save;
                    }
                }
                catch { /* Not valid JSON or parse failed, fall back to string */ }
            }

            currentNode[parts[^1]] = JsonValue.Create(value);

        Save:
            var options = new JsonSerializerOptions { WriteIndented = true };
            await File.WriteAllTextAsync(_settingsPath, root.ToJsonString(options));
        }
        catch (Exception)
        {
            // Fail silently
        }
        finally
        {
            _fileLock.Release();
        }
    }
}
