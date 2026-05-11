using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Interfaces;
using Serilog;

namespace ElectroObraApp.Infrastructure.Services;

public class DollarService : IDollarService
{
    private readonly HttpClient _httpClient;
    private readonly IUserSettingsService _settingsService;

    public DollarService(HttpClient httpClient, IUserSettingsService settingsService)
    {
        _httpClient = httpClient;
        _settingsService = settingsService;
    }

    public async Task<List<DollarDto>> GetDollarRatesAsync()
    {
        var apiUrl = _settingsService.GetDollarApiUrl();
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<DollarDto>>(apiUrl);
            return response ?? new List<DollarDto>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener las cotizaciones del dólar desde {Url}", apiUrl);
            return new List<DollarDto>();
        }
    }
}
