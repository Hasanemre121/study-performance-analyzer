using StudyAnalyzer.Services;

namespace StudyAnalyzer.Tests;

public class CsvLoaderTests
{
    private const string Header = "StudyHoursPerWeek,SleepHoursPerDay,AttendancePercent,PreviousScore,FinalScore";

    [Fact]
    public void Parse_ValidLines_ReturnsRecords()
    {
        var records = CsvLoader.Parse(new[] { Header, "10.5,7,90,60,75", "", "5,6.5,70,50,55" });

        Assert.Equal(2, records.Count);
        Assert.Equal(10.5, records[0].StudyHoursPerWeek);
        Assert.Equal(55, records[1].FinalScore);
    }

    [Fact]
    public void Parse_WrongColumnCount_ThrowsWithLineNumber()
    {
        var ex = Assert.Throws<FormatException>(() => CsvLoader.Parse(new[] { Header, "1,2,3" }));
        Assert.Contains("Line 2", ex.Message);
    }

    [Fact]
    public void Parse_NonNumericValue_Throws() =>
        Assert.Throws<FormatException>(() => CsvLoader.Parse(new[] { Header, "a,7,90,60,75" }));

    [Fact]
    public void Parse_ScoreOutOfRange_Throws() =>
        Assert.Throws<FormatException>(() => CsvLoader.Parse(new[] { Header, "10,7,90,60,150" }));

    [Fact]
    public void Parse_HeaderOnly_Throws() =>
        Assert.Throws<FormatException>(() => CsvLoader.Parse(new[] { Header }));
}
