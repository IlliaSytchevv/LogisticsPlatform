using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Orders;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LogisticsPlatform.Infrastructure.Services;

public sealed class OrderBolPdfService : IOrderBolPdfService
{
    static OrderBolPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task WriteAsync(OrderDocumentData order, Stream output, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Header().Text($"Bill of Lading — {order.Number}").SemiBold().FontSize(18);
                page.Content().Column(col =>
                {
                    col.Spacing(8);
                    col.Item().Text($"Reference: {order.PrimaryReference ?? "—"}");
                    col.Item().Text($"Customer: {order.CustomerName ?? "—"}");
                    col.Item().Text($"Hub: {order.HubName}");
                    col.Item().Text($"Carrier: {order.CarrierName ?? "—"}");
                    col.Item().Text($"Phone: {order.Phone ?? "—"}");
                    col.Item().Text($"Scheduled: {order.ScheduledAt:u}");
                    col.Item().Text($"Declared qty: {order.DeclaredQty?.ToString() ?? "—"}");
                    col.Item().Text($"Actual qty: {order.ActualQty?.ToString() ?? "—"}");
                    col.Item().Text($"Truck / Trailer: {order.TruckNumber ?? "—"} / {order.TrailerNumber ?? "—"}");
                    col.Item().Text($"Dock: {order.DockCode ?? "—"} {order.DockBay}");
                });
                page.Footer().AlignCenter().Text("FREITTY BOL").FontSize(10).FontColor(Colors.Grey.Darken1);
            });
        }).GeneratePdf(output);

        return Task.CompletedTask;
    }
}