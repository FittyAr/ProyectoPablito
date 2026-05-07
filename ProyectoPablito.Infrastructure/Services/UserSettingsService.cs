using Microsoft.Extensions.Configuration;
using ProyectoPablito.Application.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ProyectoPablito.Infrastructure.Services;

public class UserSettingsService : IUserSettingsService
{
    private readonly IConfiguration _configuration;
    private readonly string _settingsPath;

    public UserSettingsService(IConfiguration configuration)
    {
        _configuration = configuration;
        _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
    }

    public int GetPageSize()
    {
        return _configuration.GetValue<int>("Application:LastPageSize", 30);
    }

    public async Task SetPageSizeAsync(int pageSize)
    {
        if (pageSize <= 0) return; // Don't save "All" as default

        try
        {
            var jsonString = await File.ReadAllTextAsync(_settingsPath);
            var root = JsonNode.Parse(jsonString);
            if (root != null)
            {
                var appSection = root["Application"] ?? (root["Application"] = new JsonObject());
                appSection["LastPageSize"] = pageSize;

                var options = new JsonSerializerOptions { WriteIndented = true };
                await File.WriteAllTextAsync(_settingsPath, root.ToJsonString(options));
            }
        }
        catch (Exception)
        {
            // Silently fail if cannot save (e.g. read-only file)
        }
    }
}
