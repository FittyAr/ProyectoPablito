# 🏗️ Proyecto Pablito: Consolidación Arquitectónica y Modelo de Negocio

Este documento consolida la arquitectura, el modelo de dominio y el plan técnico para el sistema de gestión operativa de la empresa de construcción/mantenimiento.

## 🎯 Objetivo Principal
Desarrollar un sistema de gestión operativa escalable, centrado en el seguimiento financiero, la deuda de clientes y la rentabilidad de los trabajos, con la capacidad de manejar múltiples usuarios, web y APIs REST.

## ⚙️ Restricciones y Preferencias
*   **Alcance:** Gestión operativa (no contabilidad formal). Enfoque en el seguimiento de flujo de caja, deuda de clientes y rentabilidad de trabajos.
*   **Arquitectura:** Clean Architecture (Core $\to$ Application $\to$ Infrastructure $\to$ UI).
*   **Escalabilidad:** Debe estar diseñado pensando en la expansión a usuarios múltiples, web y APIs REST.
*   **Tecnología:** .NET 10, Avalonia UI, EF Core, SQLite (con migración planificada a PostgreSQL).
*   **Funcionalidades Clave:**
    *   **Movimiento (Financial Core):** Entidad central para todo impacto financiero (Ingreso, Gasto, PagoEmpleado, Ajuste).
    *   **Rentabilidad de Trabajo:** Seguimiento del costo total asignado a un `Trabajo` específico.
    *   **Deuda de Cliente:** Cálculo preciso: `TotalFacturado - TotalCobrado`.
    *   **Multi-divisa:** Soporte para múltiples monedas (Base: ARS).
    *   **Nómina Detallada:** Seguimiento exhaustivo de pagos de empleados (anticipos, deducciones, tasas variables).
    *   **Categorías de Gasto:** Estructura jerárquica para gastos.
    *   **Exportaciones:** Soporte para PDF, Excel, CSV, JSON.

## 🚀 Estado del Proyecto
### ✅ Completado
*   Definición completa del modelo de dominio, incluyendo 12 entidades centrales y sus relaciones.
*   Establecimiento del stack tecnológico y capas arquitectónicas (Clean Architecture).
*   Esquematización de la hoja de ruta de desarrollo en fases funcionales: Movimiento $\to$ Cliente/Facturación $\to$ Nómina $\to$ Trabajos $\to$ Dashboard.
*   Definición de reglas de negocio críticas (ej. cálculo de deuda, asignación de costos a trabajos).

### 🚧 En Curso
*   Implementación detallada de la estructura de EF Core y las configuraciones.

### 🛑 Bloqueado
*   Ninguno.

## 🔑 Decisiones Clave
*   **Centralización:** `Movimiento` es la entidad primaria para todo impacto financiero.
*   **Foco en el Trabajo:** `Trabajo` es el eje central para el cálculo de rentabilidad y análisis de costos.
*   **Modelado de Datos:** `Factura` y `Liquidacion` se mantienen separadas de `Movimiento` para rastrear estados financieros específicos (ej. pagos parciales, ciclos de nómina).
*   **Base de Datos:** Uso de `GUID` para claves primarias y configuración de EF Core para manejar la conversión de precisión decimal para compatibilidad con SQLite.

## 🗺️ Próximos Pasos (Plan de Acción)
1.  **Implementación Técnica:** Proceder con la generación del `DbContext` completo y las configuraciones de EF Core.
2.  **Desarrollo:** Comenzar con el módulo funcional más básico (ej. CRUD de `Movimiento`) en la UI de Avalonia.
3.  **Configuración Inicial:** Ejecutar el script de migración y sembrar datos iniciales (ej. monedas base).

## 📚 Contexto de Referencia
*   **Arquitectura:** Separación estricta de Clean Architecture (Core, Application, Infrastructure, UI).
*   **EF Core/SQLite:** Uso de `IEntityTypeConfiguration<T>` para configuraciones limpias y escalables.
*   **Relaciones:** Múltiples relaciones son nulas (`?`) para mantener flexibilidad.
*   **Stack Tecnológico:** `Microsoft.Extensions.DependencyInjection`, `CommunityToolkit.Mvvm`, `FluentValidation`, `Serilog`.
*   **Documentos de Referencia:**
    *   `Docs/nuevo10.txt`: Estructura inicial de DbContext y configuraciones de entidades.
    *   `Docs/nuevo11.txt`: Diagrama ER y análisis de flujo.
    *   `Docs/nuevo12.txt`: Stack tecnológico recomendado (Mapster, FluentValidation, Serilog, etc.).
    *   `Docs/nuevo6.txt`: Estructura de Clean Architecture y modelo inicial de dominio.
    *   `Docs/nuevo7.txt`: Requerimientos confirmados (multi-divisa, rentabilidad, nómina, exportaciones).
    *   `Docs/nuevo8.txt`: Refinamiento crítico: Rentabilidad de Trabajo obligatoria; lógica refinada de nómina/caja.
    *   `Docs/nuevo9.txt`: Modelo de entidades y relaciones detallado y finalizado.