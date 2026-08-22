using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using LogisticsPlatform.Application.Interfaces.FileExport;

namespace LogisticsPlatform.Infrastructure.FileExport;

public sealed class CsvExportWriter : IFileWriter
{
    public async Task WriteAsync<T>(
        Stream output,
        string title,
        IReadOnlyList<string> headers,
        IAsyncEnumerable<T> rows,
        Func<T, IReadOnlyList<object?>> mapRow,
        CancellationToken cancellationToken = default)
    {
        await using var writer = new StreamWriter(output, leaveOpen: true);
        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        };

        await using var csv = new CsvWriter(writer, configuration);

        foreach (string header in headers)
        {
            csv.WriteField(header);
        }

        await csv.NextRecordAsync();

        await foreach (T row in rows.WithCancellation(cancellationToken))
        {
            IReadOnlyList<object?> values = mapRow(row);
            foreach (object? value in values)
            {
                csv.WriteField(value);
            }

            await csv.NextRecordAsync();
        }

        await writer.FlushAsync(cancellationToken);
    }
}