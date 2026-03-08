using DemoApi.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DemoApi.Services;

public class PdfService
{
    public byte[] GenerateProductReport(List<Product> products)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                // ── Header ────────────────────────────────────────────────
                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Product Catalog Report")
                            .FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                        row.ConstantItem(150).AlignRight()
                            .Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd}")
                            .FontSize(9).FontColor(Colors.Grey.Medium);
                    });
                    col.Item().PaddingTop(4)
                        .LineHorizontal(1).LineColor(Colors.Blue.Darken2);
                });

                // ── Content ───────────────────────────────────────────────
                page.Content().PaddingTop(16).Column(col =>
                {
                    col.Item().Text($"Total Products: {products.Count}")
                        .FontSize(12).FontColor(Colors.Grey.Darken2);

                    col.Item().PaddingTop(12).Table(table =>
                    {
                        // Define columns
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(3); // Name
                            cols.RelativeColumn(2); // Category
                            cols.RelativeColumn(2); // Author
                            cols.RelativeColumn(2); // Published
                        });

                        // Header row
                        table.Header(header =>
                        {
                            foreach (var title in new[] { "Name", "Category", "Author", "Published" })
                            {
                                header.Cell().Background(Colors.Blue.Darken2)
                                    .Padding(6)
                                    .Text(title).Bold().FontColor(Colors.White);
                            }
                        });

                        // Data rows
                        foreach (var (product, index) in products.Select((p, i) => (p, i)))
                        {
                            var bg = index % 2 == 0 ? Colors.White : Colors.Grey.Lighten3;

                            table.Cell().Background(bg).Padding(6)
                                .Text(product.Name);
                            table.Cell().Background(bg).Padding(6)
                                .Text(product.Category);
                            table.Cell().Background(bg).Padding(6)
                                .Text(product.Author);
                            table.Cell().Background(bg).Padding(6)
                                .Text(product.PublishedAt.ToString("yyyy-MM-dd"));
                        }
                    });

                    // Product detail cards
                    col.Item().PaddingTop(24)
                        .Text("Product Details").FontSize(16).Bold()
                        .FontColor(Colors.Blue.Darken2);

                    foreach (var product in products)
                    {
                        col.Item().PaddingTop(12).Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(12).Column(card =>
                        {
                            card.Item().Text(product.Name)
                                .FontSize(13).Bold();
                            card.Item().PaddingTop(4)
                                .Text(product.Description)
                                .FontColor(Colors.Grey.Darken1);
                            card.Item().PaddingTop(6).Row(row =>
                            {
                                row.RelativeItem()
                                    .Text($"Category: {product.Category}");
                                row.RelativeItem()
                                    .Text($"Author: {product.Author}");
                            });
                            card.Item().PaddingTop(4)
                                .Text($"Tags: {string.Join(", ", product.Tags)}")
                                .FontSize(9).FontColor(Colors.Grey.Medium);
                        });
                    }
                });

                // ── Footer ────────────────────────────────────────────────
                page.Footer().AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ").FontSize(9).FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Medium);
                        x.Span(" of ").FontSize(9).FontColor(Colors.Grey.Medium);
                        x.TotalPages().FontSize(9).FontColor(Colors.Grey.Medium);
                    });
            });
        });

        return document.GeneratePdf();
    }
}