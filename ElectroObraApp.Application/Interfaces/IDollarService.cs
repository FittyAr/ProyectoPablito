using System.Collections.Generic;
using System.Threading.Tasks;
using ElectroObraApp.Application.DTOs;

namespace ElectroObraApp.Application.Interfaces;

public interface IDollarService
{
    Task<List<DollarDto>> GetDollarRatesAsync();
}
