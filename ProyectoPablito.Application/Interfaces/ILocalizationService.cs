namespace ProyectoPablito.Application.Interfaces;

public interface ILocalizationService
{
    string GetString(string key);
    void SetLanguage(string languageCode);
}
