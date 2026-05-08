using Mapster;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Core.Entities;

namespace ProyectoPablito.Application.Mappings;

public class MappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Movimiento, MovimientoDto>.NewConfig()
            .Map(dest => dest.TipoMovimientoNombre, src => src.TipoMovimiento.Nombre)
            .Map(dest => dest.TipoMovimientoSuma, src => src.TipoMovimiento.EsIngreso)
            .Map(dest => dest.EsIngreso, src => src.TipoMovimiento.EsIngreso)
            .Map(dest => dest.CategoriaNombre, src => src.Categoria != null ? src.Categoria.Nombre : null);

        TypeAdapterConfig<Trabajo, TrabajoDto>.NewConfig()
            .Map(dest => dest.ClienteNombre, src => src.Cliente.Nombre)
            .Map(dest => dest.OrdenesTrabajo, src => src.OrdenesTrabajo);

        TypeAdapterConfig<Liquidacion, LiquidacionDto>.NewConfig()
            .Map(dest => dest.EmpleadoNombre, src => src.Empleado.Nombre);

        TypeAdapterConfig<Cliente, ClienteDto>.NewConfig()
            .Map(dest => dest.Contactos, src => src.Contactos);

        TypeAdapterConfig<OrdenTrabajo, OrdenTrabajoDto>.NewConfig()
            .Map(dest => dest.Items, src => src.Items);
    }
}
