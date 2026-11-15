using Microsoft.ML;
using Microsoft.ML.Data;
using StudyAnalyzer.Models;

namespace StudyAnalyzer.Services;

public class ModelInput
{
    public float StudyHours { get; set; }
    public float SleepHours { get; set; }
    public float Attendance { get; set; }
    public float PreviousScore { get; set; }

    [ColumnName("Label")]
    public float FinalScore { get; set; }
}

public class ModelOutput
{
    [ColumnName("Score")]
    public float PredictedScore { get; set; }
}

public record ModelMetrics(double RSquared, double Rmse);

/// <summary>Trains a linear regression model (ML.NET) that predicts the final score.</summary>
public class ScorePredictor
{
    private readonly MLContext _ml = new(seed: 42);
    private ITransformer? _model;
    private PredictionEngine<ModelInput, ModelOutput>? _engine;

    public ModelMetrics Train(IReadOnlyList<StudentRecord> records)
    {
        if (records.Count < 20)
            throw new ArgumentException("At least 20 records are needed to train a model.", nameof(records));

        var inputs = records.Select(r => new ModelInput
        {
            StudyHours = (float)r.StudyHoursPerWeek,
            SleepHours = (float)r.SleepHoursPerDay,
            Attendance = (float)r.AttendancePercent,
            PreviousScore = (float)r.PreviousScore,
            FinalScore = (float)r.FinalScore
        });

        var data = _ml.Data.LoadFromEnumerable(inputs);
        var split = _ml.Data.TrainTestSplit(data, testFraction: 0.2, seed: 42);

        var pipeline = _ml.Transforms
            .Concatenate("Features", nameof(ModelInput.StudyHours), nameof(ModelInput.SleepHours),
                         nameof(ModelInput.Attendance), nameof(ModelInput.PreviousScore))
            .Append(_ml.Transforms.NormalizeMinMax("Features"))
            .Append(_ml.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features",
                                                maximumNumberOfIterations: 200));

        _model = pipeline.Fit(split.TrainSet);
        _engine = _ml.Model.CreatePredictionEngine<ModelInput, ModelOutput>(_model);

        var metrics = _ml.Regression.Evaluate(_model.Transform(split.TestSet));
        return new ModelMetrics(metrics.RSquared, metrics.RootMeanSquaredError);
    }

    /// <summary>Returns the predicted final score, clamped to the valid 0-100 range.</summary>
    public double Predict(double studyHours, double sleepHours, double attendance, double previousScore)
    {
        if (_engine is null) throw new InvalidOperationException("Train the model before predicting.");

        var output = _engine.Predict(new ModelInput
        {
            StudyHours = (float)studyHours,
            SleepHours = (float)sleepHours,
            Attendance = (float)attendance,
            PreviousScore = (float)previousScore
        });
        return Math.Clamp(output.PredictedScore, 0, 100);
    }
}
