using StudyAnalyzer.Services;

namespace StudyAnalyzer.Tests;

public class StatisticsTests
{
    [Fact]
    public void Mean_OfKnownValues_IsCorrect() =>
        Assert.Equal(3.0, Statistics.Mean(new double[] { 1, 2, 3, 4, 5 }), 6);

    [Theory]
    [InlineData(new double[] { 3, 1, 2 }, 2.0)]
    [InlineData(new double[] { 4, 1, 3, 2 }, 2.5)]
    public void Median_HandlesOddAndEvenCounts(double[] values, double expected) =>
        Assert.Equal(expected, Statistics.Median(values), 6);

    [Fact]
    public void StdDev_OfKnownValues_IsCorrect() =>
        Assert.Equal(2.0, Statistics.StdDev(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 }), 6);

    [Fact]
    public void Correlation_PerfectPositiveAndNegative()
    {
        var x = new double[] { 1, 2, 3, 4 };
        Assert.Equal(1.0, Statistics.Correlation(x, new double[] { 2, 4, 6, 8 }), 6);
        Assert.Equal(-1.0, Statistics.Correlation(x, new double[] { 8, 6, 4, 2 }), 6);
    }

    [Fact]
    public void Correlation_ConstantSeries_ReturnsZero() =>
        Assert.Equal(0.0, Statistics.Correlation(new double[] { 1, 2, 3 }, new double[] { 5, 5, 5 }));

    [Fact]
    public void Mean_EmptySequence_Throws() =>
        Assert.Throws<ArgumentException>(() => Statistics.Mean(Array.Empty<double>()));
}
