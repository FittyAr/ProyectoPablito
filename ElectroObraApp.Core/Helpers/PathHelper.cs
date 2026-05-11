using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ElectroObraApp.Core.Helpers;

public static class PathHelper
{
    private const string AppFolderName = "ElectroObraApp";

    public static string GetAppDataPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER")))
        {
            // En web/browser, usamos una ruta virtual o la base
            return "/AppData";
        }

        string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        
        if (string.IsNullOrEmpty(basePath))
        {
            basePath = AppDomain.CurrentDomain.BaseDirectory;
        }

        string path = Path.Combine(basePath, AppFolderName);

        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch
            {
                // Si falla, volvemos a la base del ejecutable como fallback de emergencia
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        return path;
    }

    public static string GetSettingsPath() => Path.Combine(GetAppDataPath(), "appsettings.json");
    
    public static string GetDatabasePath() => Path.Combine(GetAppDataPath(), "ElectroObraApp.db");

    public static string GetSqliteConnectionString()
    {
        return $"Data Source={GetDatabasePath()}";
    }
}
