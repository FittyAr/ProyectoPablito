using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoPablito.Core.Entities;

namespace ProyectoPablito.Core.Interfaces;

/// <summary>
/// Repositorio específico para Movimiento que garantiza la carga de
/// entidades de navegación requeridas para el mapeo correcto de EsIngreso y Total.
/// </summary>
public interface IMovimientoRepository : IRepository<Movimiento>
{
    /// <summary>
    /// Obtiene todos los movimientos incluyendo TipoMovimiento y Categoria.
    /// CRÍTICO: Sin este Include, EsIngreso/TipoMovimientoSuma siempre serán false.
    /// </summary>
    Task<IEnumerable<Movimiento>> GetAllWithIncludesAsync();

    /// <summary>
    /// Obtiene un movimiento por ID incluyendo TipoMovimiento y Categoria.
    /// </summary>
    Task<Movimiento?> GetByIdWithIncludesAsync(Guid id);
}
