using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoPablito.Core.Entities;

namespace ProyectoPablito.Core.Interfaces;

/// <summary>
/// Repositorio específico para Trabajo que garantiza la carga de
/// OrdenesTrabajo e Items para el formulario de edición.
/// </summary>
public interface ITrabajoRepository : IRepository<Trabajo>
{
    /// <summary>
    /// Obtiene todos los trabajos incluyendo Cliente (para mostrar nombre).
    /// </summary>
    Task<IEnumerable<Trabajo>> GetAllWithDeepLoadAsync();
    Task<Trabajo?> GetByIdWithDeepLoadAsync(Guid id);
}
