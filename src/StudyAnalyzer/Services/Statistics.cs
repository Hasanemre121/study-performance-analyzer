namespace StudyAnalyzer.Services;

/// <summary>Basic descriptive statistics implemented by hand to understand the maths.</summary>
public static class Statistics
{
    public static double Mean(IReadOnlyList<double> values)
    {
        if (values.Count == 0) throw new ArgumentException("Sequence is empty.", nameof(values));
        return values.Sum() / values.Count;
    }

    public static double Median(IReadOnlyList<double> values)
    {
        if (values.Count == 0) throw new ArgumentException("Sequence is empty.", nameof(values));
        var sorted = values.OrderBy(v => v).ToArray();
        int mid = sorted.Length / 2;
        return sorted.Length % 2 == 1 ? sorted[mid] : (sorted[mid - 1] + sorted[mid]) / 2.0;
    }

    /// <summary>Population standard deviation.</summary>
    public static double StdDev(IReadOnlyList<double> values)
    {
        double mean = Mean(values);
        return Math.Sqrt(values.Sum(v => (v - mean) * (v - mean)) / values.Count);
    }

    /// <summary>Pearson correlation coefficient in the range [-1, 1]; 0 when a series has no variation.</summary>
    public static double Correlation(IReadOnlyList<double> x, IReadOnlyList<double> y)
    {
        if (x.Count != y.Count || x.Count == 0)
            throw new ArgumentException("Both series must have the same non-zero length.");

        double mx = Mean(x), my = Mean(y);
        double cov = 0, vx = 0, vy = 0;
        for (int i = 0; i < x.Count; i++)
        {
            double dx = x[i] - mx, dy = y[i] - my;
            cov += dx * dy;
            vx += dx * dx;
            vy += dy * dy;
        }
        return vx == 0 || vy == 0 ? 0 : cov / Math.Sqrt(vx * vy);
    }
}
