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
    private const string ApiUrl = "https://dolarapi.com/v1/dolares";

    public DollarService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DollarDto>> GetDollarRatesAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<DollarDto>>(ApiUrl);
            return response ?? new List<DollarDto>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener las cotizaciones del dólar desde {Url}", ApiUrl);
            return new List<DollarDto>();
        }
    }
}
