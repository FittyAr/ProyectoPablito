using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoPablito.Core.Entities;

namespace ProyectoPablito.Core.Interfaces;

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
