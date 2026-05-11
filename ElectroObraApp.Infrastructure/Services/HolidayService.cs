using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ElectroObraApp.Application.Interfaces;

namespace ElectroObraApp.Infrastructure.Services;

public class HolidayService : IHolidayService
{
    private readonly HttpClient _httpClient;
    private readonly IUserSettingsService _settingsService;

    public HolidayService(HttpClient httpClient, IUserSettingsService settingsService)
    {
        _httpClient = httpClient;
        _settingsService = settingsService;
    }

    public async Task<List<HolidayModel>> GetHolidaysAsync(int year)
    {
        var holidays = new List<HolidayModel>();
        var baseUrl = _settingsService.GetHolidayApiUrl();
        
        if (string.IsNullOrWhiteSpace(baseUrl)) return holidays;

        // Ensure baseUrl ends with /
        if (!baseUrl.EndsWith("/")) baseUrl += "/";

        var url = $"{baseUrl}{year}";

        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var apiHolidays = JsonSerializer.Deserialize<List<ApiHoliday>>(response);

            if (apiHolidays != null)
            {
                foreach (var item in apiHolidays)
                {
                    if (DateTime.TryParse(item.Fecha, out var date))
                    {
                        holidays.Add(new HolidayModel 
                        { 
                            Date = date, 
                            Name = item.Nombre 
                        });
                    }
                }
            }
        }
        catch (Exception)
        {
            // Log error or handle as needed
        }

        return holidays;
    }

    private class ApiHoliday
    {
        [JsonPropertyName("fecha")]
        public string Fecha { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }
}
