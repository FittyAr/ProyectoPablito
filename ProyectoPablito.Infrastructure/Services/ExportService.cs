using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ProyectoPablito.Application.Interfaces;
using ProyectoPablito.Application.DTOs;

namespace ProyectoPablito.Infrastructure.Services;

public class ExportService : IExportService
{
    public ExportService()
    {
        // QuestPDF License
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> ExportMovimientosToPdfAsync(IEnumerable<object> movimientos)
    {
        return await Task.Run(() =>
        {
            var data = movimientos.Cast<MovimientoDto>().ToList();
            
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Text("Listado de Movimientos - Proyecto Pablito")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Fecha
                            columns.RelativeColumn(4); // Concepto
                            columns.RelativeColumn(2); // Tipo
                            columns.RelativeColumn(2); // Monto
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Fecha");
                            header.Cell().Element(CellStyle).Text("Concepto");
                            header.Cell().Element(CellStyle).Text("Tipo");
                            header.Cell().Element(CellStyle).Text("Monto");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold())
                                    .PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var item in data)
                        {
                            table.Cell().Element(ValueStyle).Text(item.Fecha.ToString("dd/MM/yyyy"));
                            table.Cell().Element(ValueStyle).Text(item.Concepto);
                            table.Cell().Element(ValueStyle).Text(item.TipoMovimientoNombre);
                            table.Cell().Element(ValueStyle).Text(item.Total.ToString("C"));

                            static IContainer ValueStyle(IContainer container)
                            {
                                return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                            }
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            });

            return document.GeneratePdf();
        });
    }

    public async Task<byte[]> ExportMovimientosToExcelAsync(IEnumerable<object> movimientos)
    {
        return await Task.Run(() =>
        {
            var data = movimientos.Cast<MovimientoDto>().ToList();
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Movimientos");

            // Cabeceras
            worksheet.Cell(1, 1).Value = "Fecha";
            worksheet.Cell(1, 2).Value = "Concepto";
            worksheet.Cell(1, 3).Value = "Tipo";
            worksheet.Cell(1, 4).Value = "Monto";
            worksheet.Cell(1, 5).Value = "Cantidad";
            worksheet.Cell(1, 6).Value = "Total";

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;

            // Datos
            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                var row = i + 2;
                worksheet.Cell(row, 1).Value = item.Fecha;
                worksheet.Cell(row, 2).Value = item.Concepto;
                worksheet.Cell(row, 3).Value = item.TipoMovimientoNombre;
                worksheet.Cell(row, 4).Value = item.Monto;
                worksheet.Cell(row, 5).Value = item.Cantidad;
                worksheet.Cell(row, 6).Value = item.Total;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        });
    }
}
