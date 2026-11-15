using StudyAnalyzer.Services;

namespace StudyAnalyzer.Tests;

public class ScorePredictorTests
{
    private static string DataPath()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "data", "students.csv")))
            dir = dir.Parent;
        return Path.Combine(dir!.FullName, "data", "students.csv");
    }

    [Fact]
    public void Train_OnSampleData_ExplainsMostOfTheVariance()
    {
        var records = CsvLoader.LoadFile(DataPath());
        var predictor = new ScorePredictor();

        var metrics = predictor.Train(records);

        Assert.True(metrics.RSquared > 0.6, $"R^2 was {metrics.RSquared}");
    }

    [Fact]
    public void Predict_MoreStudyHours_GivesHigherScore()
    {
        var predictor = new ScorePredictor();
        predictor.Train(CsvLoader.LoadFile(DataPath()));

        var low = predictor.Predict(3, 7, 80, 60);
        var high = predictor.Predict(25, 7, 80, 60);

        Assert.True(high > low);
    }

    [Fact]
    public void Predict_WithoutTraining_Throws() =>
        Assert.Throws<InvalidOperationException>(() => new ScorePredictor().Predict(10, 7, 80, 60));

    [Fact]
    public void Train_TooFewRecords_Throws() =>
        Assert.Throws<ArgumentException>(() => new ScorePredictor().Train(
            new[] { new StudyAnalyzer.Models.StudentRecord(1, 7, 80, 60, 70) }));
}
