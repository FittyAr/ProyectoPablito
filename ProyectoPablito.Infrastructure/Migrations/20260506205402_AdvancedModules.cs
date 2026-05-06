using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoPablito.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdvancedModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_TipoMovimientos_TipoMovimientoId",
                table: "Movimientos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipoMovimientos",
                table: "TipoMovimientos");

            migrationBuilder.RenameTable(
                name: "TipoMovimientos",
                newName: "TiposMovimiento");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoriaId",
                table: "Movimientos",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<Guid>(
                name: "ClienteId",
                table: "Movimientos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TiposMovimiento",
                table: "TiposMovimiento",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Cuit = table.Column<string>(type: "TEXT", nullable: true),
                    Direccion = table.Column<string>(type: "TEXT", nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    CondicionIva = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Dni = table.Column<string>(type: "TEXT", nullable: true),
                    Cargo = table.Column<string>(type: "TEXT", nullable: true),
                    SueldoBase = table.Column<double>(type: "REAL", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trabajos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Presupuesto = table.Column<double>(type: "REAL", nullable: false),
                    Finalizado = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trabajos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trabajos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_ClienteId",
                table: "Movimientos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_EmpleadoId",
                table: "Movimientos",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_TrabajoId",
                table: "Movimientos",
                column: "TrabajoId");

            migrationBuilder.CreateIndex(
                name: "IX_Trabajos_ClienteId",
                table: "Trabajos",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Clientes_ClienteId",
                table: "Movimientos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Empleados_EmpleadoId",
                table: "Movimientos",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_TiposMovimiento_TipoMovimientoId",
                table: "Movimientos",
                column: "TipoMovimientoId",
                principalTable: "TiposMovimiento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Trabajos_TrabajoId",
                table: "Movimientos",
                column: "TrabajoId",
                principalTable: "Trabajos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Clientes_ClienteId",
                table: "Movimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Empleados_EmpleadoId",
                table: "Movimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_TiposMovimiento_TipoMovimientoId",
                table: "Movimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Trabajos_TrabajoId",
                table: "Movimientos");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "Trabajos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Movimientos_ClienteId",
                table: "Movimientos");

            migrationBuilder.DropIndex(
                name: "IX_Movimientos_EmpleadoId",
                table: "Movimientos");

            migrationBuilder.DropIndex(
                name: "IX_Movimientos_TrabajoId",
                table: "Movimientos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TiposMovimiento",
                table: "TiposMovimiento");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Movimientos");

            migrationBuilder.RenameTable(
                name: "TiposMovimiento",
                newName: "TipoMovimientos");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoriaId",
                table: "Movimientos",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipoMovimientos",
                table: "TipoMovimientos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_TipoMovimientos_TipoMovimientoId",
                table: "Movimientos",
                column: "TipoMovimientoId",
                principalTable: "TipoMovimientos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
