using System.Globalization;
using StudyAnalyzer.Models;

namespace StudyAnalyzer.Services;

public static class CsvLoader
{
    private const int ExpectedColumns = 5;

    /// <summary>Parses CSV lines (first line is the header). Throws FormatException with the line number on bad data.</summary>
    public static IReadOnlyList<StudentRecord> Parse(IEnumerable<string> lines)
    {
        var records = new List<StudentRecord>();
        int lineNumber = 0;

        foreach (var raw in lines)
        {
            lineNumber++;
            if (lineNumber == 1 || string.IsNullOrWhiteSpace(raw)) continue;

            var parts = raw.Split(',');
            if (parts.Length != ExpectedColumns)
                throw new FormatException($"Line {lineNumber}: expected {ExpectedColumns} columns but found {parts.Length}.");

            var values = new double[ExpectedColumns];
            for (int i = 0; i < ExpectedColumns; i++)
            {
                if (!double.TryParse(parts[i].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out values[i]))
                    throw new FormatException($"Line {lineNumber}: '{parts[i]}' is not a valid number.");
            }

            if (values[2] is < 0 or > 100 || values[3] is < 0 or > 100 || values[4] is < 0 or > 100)
                throw new FormatException($"Line {lineNumber}: attendance and scores must be between 0 and 100.");

            records.Add(new StudentRecord(values[0], values[1], values[2], values[3], values[4]));
        }

        if (records.Count == 0)
            throw new FormatException("The file contains no data rows.");

        return records;
    }

    public static IReadOnlyList<StudentRecord> LoadFile(string path) => Parse(File.ReadAllLines(path));
}
