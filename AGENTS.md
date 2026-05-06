# AGENTS.md - Plan de Implementación "Proyecto Pablito"

Este documento detalla la hoja de ruta para el desarrollo de **Proyecto Pablito**, un sistema de gestión operativa profesional.

## 🚀 Objetivo
Construir un sistema robusto, escalable y mantenible para el control operativo y flujo de caja, utilizando las mejores prácticas de ingeniería de software.

## 🏗️ Arquitectura y Patrones de Diseño

### Arquitectura Limpia (Clean Architecture)
El proyecto se dividirá en las siguientes capas para asegurar el desacoplamiento:

1.  **Core (Dominio)**:
    *   **Entidades**: Modelos puros del negocio (Movimiento, Cliente, Trabajo, etc.).
    *   **Enums**: Tipos definidos (TipoMovimiento, EstadoFactura, etc.).
    *   **Interfaces de Repositorio**: Definiciones de cómo acceder a los datos.
    *   **Especificaciones**: Lógica de consulta compleja.

2.  **Application (Aplicación)**:
    *   **DTOs**: Objetos de transferencia de datos para desacoplar la UI del dominio.
    *   **Interfaces de Servicios**: Definiciones de lógica de negocio.
    *   **Implementación de Servicios**: Orquestación de casos de uso.
    *   **Mappings**: Conversión entre Entidades y DTOs (usando Mapster).
    *   **Validaciones**: Reglas de negocio aplicadas a DTOs (usando FluentValidation).

3.  **Infrastructure (Infraestructura)**:
    *   **Persistencia**: Implementación de `DbContext` de EF Core y Repositorios.
    *   **SQLite**: Configuración y migraciones.
    *   **Servicios Externos**: Implementación de exportación (PDF/Excel), Logging, y acceso a archivos.

4.  **UI (Avalonia)**:
    *   **MVVM**: Uso estricto de Model-View-ViewModel con `CommunityToolkit.Mvvm`.
    *   **Views**: Definiciones XAML multiplataforma.
    *   **ViewModels**: Lógica de presentación y comandos.
    *   **Localización**: Gestión de textos mediante archivos JSON.

### Patrones de Diseño
*   **Repository & Unit of Work**: Para abstraer la persistencia y manejar transacciones.
*   **Dependency Injection**: Inyección de dependencias nativa de .NET.
*   **Options Pattern**: Para configuración tipada desde archivos JSON.
*   **Observer Pattern**: Implementado a través de `ObservableObject` y `Messenger` de CommunityToolkit.

## 🛠️ Estándares de Implementación

### 1. Cero Hardcoding
*   **Configuración**: Todo valor variable (cadenas de conexión, rutas, flags) irá en `appsettings.json`.
*   **Textos (i18n)**: Los textos de la UI se cargarán desde `i18n/es.json`.
*   **Constantes**: Valores fijos de negocio se definirán en clases de constantes dedicadas.

### 2. Calidad y Testing
*   **Unit Testing**: Cada objeto (Servicio, ViewModel, Helper) DEBE tener su correspondiente proyecto de test unitario usando `xUnit`.
*   **Mocking**: Uso de `NSubstitute` o `Moq` para aislar dependencias en tests.
*   **FluentAssertions**: Para tests más legibles.

### 3. Librerías a Utilizar
*   **UI**: Avalonia 11+
*   **ORM**: EF Core 9+ (SQLite)
*   **Validación**: FluentValidation
*   **Mapping**: Mapster
*   **Logging**: Serilog
*   **Reportes**: QuestPDF & ClosedXML

## 📅 Fases de Implementación

### Fase 1: Cimientos y Configuración
- [x] Configuración de `Directory.Packages.props`.
- [x] Creación de proyectos `ProyectoPablito.Core`, `ProyectoPablito.Application`, `ProyectoPablito.Infrastructure` y `ProyectoPablito.Tests`.
- [x] Implementación del sistema de Localización (JSON).
- [x] Configuración de Serilog y DI.

### Fase 2: Dominio y Persistencia (Movimientos)
- [x] Entidades de Movimiento, Categoría y Tipos de Movimiento (Tabla).
- [x] DbContext y Migraciones iniciales.
- [x] Repositorios Base y Unit of Work.

### Fase 3: Lógica de Aplicación
- [ ] DTOs y Mappings.
- [ ] Servicios de Movimientos con validación.
- [ ] Tests Unitarios de servicios.

### Fase 4: UI Base
- [ ] MainView con navegación.
- [ ] Listado de Movimientos.
- [ ] Formulario de Alta/Edición.

### Fase 5: Módulos Avanzados
- [ ] Clientes y Facturación.
- [ ] Empleados y Liquidaciones.
- [ ] Gestión de Trabajos y Rentabilidad.

### Fase 6: Reportes y Pulido
- [ ] Exportación a PDF y Excel.
- [ ] Dashboard de estadísticas.
- [ ] Optimización de UI y UX.

---
**Nota**: Cada paso de la implementación será validado con sus respectivos tests antes de pasar al siguiente.
