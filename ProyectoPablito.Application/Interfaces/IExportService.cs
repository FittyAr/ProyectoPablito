using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProyectoPablito.Application.Interfaces;

public interface IExportService
{
    Task<byte[]> ExportMovimientosToPdfAsync(IEnumerable<object> movimientos);
    Task<byte[]> ExportMovimientosToExcelAsync(IEnumerable<object> movimientos);
    Task<byte[]> ExportMovimientosToWordAsync(IEnumerable<object> movimientos);
    Task<byte[]> ExportMovimientosToCsvAsync(IEnumerable<object> movimientos);
    Task<byte[]> ExportMovimientosToJsonAsync(IEnumerable<object> movimientos);
}
