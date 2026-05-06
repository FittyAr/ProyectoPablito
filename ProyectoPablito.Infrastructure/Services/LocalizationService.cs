using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ProyectoPablito.Application.Interfaces;

namespace ProyectoPablito.Infrastructure.Services;

public class LocalizationService : ILocalizationService
{
    private Dictionary<string, JsonElement> _translations = new();
    private string _currentLanguage = "es";

    public LocalizationService()
    {
        LoadTranslations();
    }

    private void LoadTranslations()
    {
        try
        {
            // En una app real, esto debería venir de una ruta configurada o recursos embebidos
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "i18n", $"{_currentLanguage}.json");
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                _translations = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? new();
            }
        }
        catch
        {
            _translations = new();
        }
    }

    public string GetString(string key)
    {
        // Soporte simple para claves anidadas (ej: "General.AppName")
        var parts = key.Split('.');
        JsonElement current = default;
        bool found = false;

        if (_translations.TryGetValue(parts[0], out var firstPart))
        {
            current = firstPart;
            found = true;
            for (int i = 1; i < parts.Length; i++)
            {
                if (current.TryGetProperty(parts[i], out var next))
                {
                    current = next;
                }
                else
                {
                    found = false;
                    break;
                }
            }
        }

        return found ? current.ToString() : key;
    }

    public void SetLanguage(string languageCode)
    {
        _currentLanguage = languageCode;
        LoadTranslations();
    }
}
