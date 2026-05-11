using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ElectroObraApp.Application.Interfaces;

namespace ElectroObraApp.Application.Helpers;

public static class EmailHelper
{
    public static void OpenEmailClient(string email, IUserSettingsService settingsService)
    {
        if (string.IsNullOrWhiteSpace(email)) return;

        var client = settingsService.GetPreferredEmailClient();
        string url = client switch
        {
            "Gmail" => $"https://mail.google.com/mail/?view=cm&fs=1&to={email}",
            "Yahoo" => $"https://compose.mail.yahoo.com/?to={email}",
            "OutlookWeb" => $"https://outlook.live.com/owa/?path=/mail/action/compose&to={email}",
            _ => $"mailto:{email}" // SystemDefault or others
        };

        OpenUrl(url);
    }

    private static void OpenUrl(string url)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url.Replace("&", "^&")) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }
        catch
        {
            // Silently fail if unable to open browser/client
        }
    }
}
