using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectroObraApp.Core.Entities;

namespace ElectroObraApp.Core.Interfaces;

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

