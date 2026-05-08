using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ProyectoPablito.Application.DTOs;

namespace ProyectoPablito.Application.Interfaces;

public interface IExportService
{
    Task<byte[]> ExportMovimientosToPdfAsync(IEnumerable<object> movimientos);
    Task<byte[]> ExportMovimientosToExcelAsync(IEnumerable<object> movimientos);
    Task<byte[]> ExportMovimientosToWordAsync(IEnumerable<object> movimientos);
    Task<byte[]> ExportMovimientosToCsvAsync(IEnumerable<object> movimientos);
    Task<byte[]> ExportMovimientosToJsonAsync(IEnumerable<object> movimientos);
    
    Task<byte[]> ExportLiquidacionToPdfAsync(LiquidacionDto liquidacion, IEnumerable<MovimientoDto> adelantos);
    Task<byte[]> ExportCertificadoToPdfAsync(OrdenTrabajoDto certificado, TrabajoDto trabajo);
}
