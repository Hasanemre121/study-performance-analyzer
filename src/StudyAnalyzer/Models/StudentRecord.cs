namespace StudyAnalyzer.Models;

/// <summary>One row of the dataset: habits of a student and the final exam result.</summary>
public record StudentRecord(
    double StudyHoursPerWeek,
    double SleepHoursPerDay,
    double AttendancePercent,
    double PreviousScore,
    double FinalScore);
