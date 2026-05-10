using System.Collections.Generic;
using System.Threading.Tasks;
using ElectroObraApp.Core.Entities;

namespace ElectroObraApp.Core.Interfaces;

/// <summary>
/// Repositorio específico para Liquidacion que garantiza la carga
/// de la entidad Empleado para mapear EmpleadoNombre correctamente.
/// </summary>
public interface ILiquidacionRepository : IRepository<Liquidacion>
{
    /// <summary>
    /// Obtiene todas las liquidaciones incluyendo el Empleado asociado.
    /// CRÍTICO: Sin este Include, EmpleadoNombre siempre será null.
    /// </summary>
    Task<IEnumerable<Liquidacion>> GetAllWithEmpleadoAsync();
}

