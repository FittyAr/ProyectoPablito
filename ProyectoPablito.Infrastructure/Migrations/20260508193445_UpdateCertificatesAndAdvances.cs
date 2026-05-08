using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoPablito.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCertificatesAndAdvances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PorcentajeAvance",
                table: "OrdenTrabajoItems",
                newName: "PorcentajeAnterior");

            migrationBuilder.AddColumn<double>(
                name: "PorcentajeActual",
                table: "OrdenTrabajoItems",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AjusteUocraPorcentaje",
                table: "OrdenesTrabajo",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OtrosDescuentos",
                table: "OrdenesTrabajo",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PorcentajeActual",
                table: "OrdenTrabajoItems");

            migrationBuilder.DropColumn(
                name: "AjusteUocraPorcentaje",
                table: "OrdenesTrabajo");

            migrationBuilder.DropColumn(
                name: "OtrosDescuentos",
                table: "OrdenesTrabajo");

            migrationBuilder.RenameColumn(
                name: "PorcentajeAnterior",
                table: "OrdenTrabajoItems",
                newName: "PorcentajeAvance");
        }
    }
}
