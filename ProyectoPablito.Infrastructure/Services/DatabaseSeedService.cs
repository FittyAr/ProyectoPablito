using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;
using ProyectoPablito.Core.Entities;
using ProyectoPablito.Core.Enums;
using ProyectoPablito.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProyectoPablito.Infrastructure.Services;

public class DatabaseSeedService : IDatabaseSeedService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly Random _random = new();

    public DatabaseSeedService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public bool IsSeedEnabled()
    {
        return _configuration.GetValue<bool>("Application:SeedEnabled");
    }

    public async Task SeedAsync()
    {
        if (!IsSeedEnabled()) return;

        // Cargar Pool de datos
        var poolPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "SeedPool.json");
        if (!File.Exists(poolPath))
        {
            // Fallback si no está en la carpeta de ejecución (desarrollo)
            poolPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ProyectoPablito.Infrastructure", "Data", "SeedData", "SeedPool.json");
        }

        var json = await File.ReadAllTextAsync(poolPath);
        var pool = JsonSerializer.Deserialize<SeedDataPool>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (pool == null) return;

        // Limpiar tablas (excepto las de sistema si fuera necesario, pero aquí borramos todo para el sembrado completo)
        _context.Movimientos.RemoveRange(_context.Movimientos);
        _context.Trabajos.RemoveRange(_context.Trabajos);
        _context.Clientes.RemoveRange(_context.Clientes);
        _context.Empleados.RemoveRange(_context.Empleados);
        _context.Categorias.RemoveRange(_context.Categorias);
        
        // No borramos TiposMovimiento porque tienen datos de sistema en HasData
        // Pero si quisiéramos sobrescribir los que no son de sistema:
        var customTypes = _context.TiposMovimiento.Where(t => !t.EsSistema);
        _context.TiposMovimiento.RemoveRange(customTypes);

        await _context.SaveChangesAsync();

        // 1. Sembrar Categorías
        var categorias = pool.Categorias.Select(name => new Categoria { Nombre = name }).ToList();
        _context.Categorias.AddRange(categorias);

        // 2. Sembrar Tipos de Movimiento (adicionales)
        var existingTypes = await _context.TiposMovimiento.ToListAsync();
        foreach (var typeName in pool.TiposMovimiento)
        {
            if (!existingTypes.Any(t => t.Nombre == typeName))
            {
                _context.TiposMovimiento.Add(new TipoMovimiento { Nombre = typeName, EsIngreso = _random.Next(2) == 0 });
            }
        }
        await _context.SaveChangesAsync();
        
        var allTipos = await _context.TiposMovimiento.ToListAsync();

        // 3. Sembrar Empleados (5-10)
        int empCount = _random.Next(5, 11);
        var empleados = pool.NombresEmpleados.OrderBy(x => _random.Next()).Take(empCount)
            .Select(name => new Empleado 
            { 
                Nombre = name, 
                Dni = $"{_random.Next(10, 50)}.{_random.Next(100, 999)}.{_random.Next(100, 999)}",
                Cargo = "Operario",
                SueldoBase = _random.Next(200000, 500000),
                FechaIngreso = DateTime.Now.AddYears(-_random.Next(1, 5)),
                Activo = true
            }).ToList();
        _context.Empleados.AddRange(empleados);

        // 4. Sembrar Clientes (5-10)
        int cliCount = _random.Next(5, 11);
        var clientes = pool.NombresClientes.OrderBy(x => _random.Next()).Take(cliCount)
            .Select(name => new Cliente 
            { 
                Nombre = name, 
                Cuit = $"20-{_random.Next(10000000, 99999999)}-{_random.Next(0, 9)}",
                Direccion = $"Calle Falsa {_random.Next(100, 5000)}",
                Email = $"{name.Replace(" ", ".").ToLower()}@gmail.com",
                Telefono = $"54911{_random.Next(10000000, 99999999)}",
                CondicionIva = "Consumidor Final"
            }).ToList();
        _context.Clientes.AddRange(clientes);

        await _context.SaveChangesAsync();

        // 5. Sembrar Trabajos (20-30 por Cliente)
        foreach (var cliente in clientes)
        {
            int workCount = _random.Next(20, 31);
            for (int i = 0; i < workCount; i++)
            {
                var trabajo = new Trabajo
                {
                    Descripcion = $"{pool.DescripcionesTrabajos[_random.Next(pool.DescripcionesTrabajos.Count)]} #{i + 1}",
                    ClienteId = cliente.Id,
                    FechaInicio = DateTime.Now.AddDays(-_random.Next(1, 100)),
                    Presupuesto = _random.Next(5000, 50000),
                    Finalizado = _random.Next(2) == 0
                };
                if (trabajo.Finalizado) trabajo.FechaFin = trabajo.FechaInicio.AddDays(_random.Next(1, 30));
                
                _context.Trabajos.Add(trabajo);

                // 6. Sembrar Movimientos (20-30 por Trabajo/Empleado)
                int movCount = _random.Next(20, 31);
                for (int j = 0; j < movCount; j++)
                {
                    var mov = new Movimiento
                    {
                        Concepto = pool.ConceptosMovimientos[_random.Next(pool.ConceptosMovimientos.Count)],
                        Monto = _random.Next(100, 5000),
                        Cantidad = _random.Next(1, 5),
                        Fecha = trabajo.FechaInicio.AddDays(_random.Next(0, 30)),
                        TipoMovimientoId = allTipos[_random.Next(allTipos.Count)].Id,
                        CategoriaId = categorias[_random.Next(categorias.Count)].Id,
                        ClienteId = cliente.Id,
                        EmpleadoId = empleados[_random.Next(empleados.Count)].Id,
                        Trabajo = trabajo,
                        Moneda = Moneda.ARS
                    };
                    _context.Movimientos.Add(mov);
                }
            }
        }

        await _context.SaveChangesAsync();
    }
}
