using System.Globalization;
using StudyAnalyzer.Models;
using StudyAnalyzer.Services;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

const string Usage = """
Study Performance Analyzer

Usage:
  StudyAnalyzer analyze <file.csv>
  StudyAnalyzer predict <file.csv> <studyHoursPerWeek> <sleepHoursPerDay> <attendancePercent> <previousScore>
""";

if (args.Length < 2)
{
    Console.WriteLine(Usage);
    return 1;
}

try
{
    var records = CsvLoader.LoadFile(args[1]);

    switch (args[0].ToLowerInvariant())
    {
        case "analyze":
            Analyze(records);
            return 0;

        case "predict" when args.Length == 6:
            Predict(records, args.Skip(2).Select(Parse).ToArray());
            return 0;

        default:
            Console.WriteLine(Usage);
            return 1;
    }
}
catch (Exception ex) when (ex is FormatException or IOException or ArgumentException)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Error: {ex.Message}");
    Console.ResetColor();
    return 2;
}

static double Parse(string text)
{
    if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        throw new FormatException($"'{text}' is not a valid number.");
    return value;
}

static void Analyze(IReadOnlyList<StudentRecord> records)
{
    Console.WriteLine($"Records: {records.Count}");
    Console.WriteLine();

    var columns = new (string Name, Func<StudentRecord, double> Selector)[]
    {
        ("Study hours/week", r => r.StudyHoursPerWeek),
        ("Sleep hours/day", r => r.SleepHoursPerDay),
        ("Attendance %", r => r.AttendancePercent),
        ("Previous score", r => r.PreviousScore),
        ("Final score", r => r.FinalScore)
    };

    Console.WriteLine($"{"Column",-18}{"Mean",8}{"Median",8}{"StdDev",8}{"Corr. with final",18}");
    var final = records.Select(r => r.FinalScore).ToList();
    foreach (var (name, selector) in columns)
    {
        var values = records.Select(selector).ToList();
        Console.WriteLine($"{name,-18}{Statistics.Mean(values),8:F1}{Statistics.Median(values),8:F1}" +
                          $"{Statistics.StdDev(values),8:F1}{Statistics.Correlation(values, final),18:F2}");
    }

    var predictor = new ScorePredictor();
    var metrics = predictor.Train(records);
    Console.WriteLine();
    Console.WriteLine($"Linear regression on a held-out 20% test set: R^2 = {metrics.RSquared:F2}, RMSE = {metrics.Rmse:F2} points");
}

static void Predict(IReadOnlyList<StudentRecord> records, double[] input)
{
    var predictor = new ScorePredictor();
    var metrics = predictor.Train(records);
    var result = predictor.Predict(input[0], input[1], input[2], input[3]);
    Console.WriteLine($"Predicted final score: {result:F1} (model R^2 = {metrics.RSquared:F2}, typical error about {metrics.Rmse:F1} points)");
}
