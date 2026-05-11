using System;
using System.Text.Json.Serialization;

namespace ElectroObraApp.Application.DTOs;

public class DollarDto
{
    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("compra")]
    public decimal Compra { get; set; }

    [JsonPropertyName("venta")]
    public decimal Venta { get; set; }

    [JsonPropertyName("casa")]
    public string Casa { get; set; } = string.Empty;

    [JsonPropertyName("fechaActualizacion")]
    public DateTime FechaActualizacion { get; set; }

    public string DisplayInfo => $"{Nombre}: Compra ${Compra:N2} | Venta ${Venta:N2}";
}
