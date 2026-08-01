using System.Text;

namespace HrSystem.Backend.Services;

/// <summary>
/// Simple CSV builder that escapes fields correctly (RFC 4180 style).
/// Produces UTF-8 with BOM so Excel opens Arabic text correctly.
/// </summary>
public static class CsvExporter
{
    public static byte[] Build(IEnumerable<string> headers, IEnumerable<IEnumerable<string>> rows)
    {
        var sb = new StringBuilder();

        AppendRow(sb, headers);
        foreach (var row in rows)
            AppendRow(sb, row);

        // BOM so Excel detects UTF-8
        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }

    private static void AppendRow(StringBuilder sb, IEnumerable<string> cells)
    {
        var parts = cells.Select(Escape).ToArray();
        sb.AppendLine(string.Join(",", parts));
    }

    private static string Escape(string? value)
    {
        var s = value ?? string.Empty;
        if (s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r'))
        {
            return $"\"{s.Replace("\"", "\"\"")}\"";
        }
        return s;
    }
}
