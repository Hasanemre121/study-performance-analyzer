# Study Performance Analyzer

A command-line data analysis tool written in **C# / .NET 10**. It loads a CSV dataset of student habits, computes descriptive statistics and correlations, and trains a **linear regression model with ML.NET** to predict the final exam score.

> The included `data/students.csv` is a **synthetic dataset** (200 generated rows) created for learning and demonstration. It does not describe real students and results must not be interpreted as real-world findings.

## Features
- CSV parsing with validation and line-numbered error messages
- Hand-written statistics: mean, median, standard deviation, Pearson correlation
- ML.NET regression pipeline (feature concatenation, normalization, SDCA trainer)
- Evaluation on a held-out 20% test set (R² and RMSE)
- Score prediction from the command line
- 16 xUnit tests

## Usage
Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run --project src/StudyAnalyzer -- analyze data/students.csv
dotnet run --project src/StudyAnalyzer -- predict data/students.csv 15 7 85 65
```
Arguments of `predict`: study hours per week, sleep hours per day, attendance %, previous score.

Example output (values vary slightly between runs):
```
Records: 200

Column              Mean  Median  StdDev  Corr. with final
Study hours/week    12.5    12.9     5.0              0.56
Sleep hours/day      7.0     7.0     1.1              0.19
Attendance %        81.7    82.0    11.5              0.28
Previous score      60.2    61.0    14.8              0.53
Final score         63.4    64.0     9.4              1.00

Linear regression on a held-out 20% test set: R^2 = 0.74, RMSE = 5.64 points
```

## Project structure
```
src/StudyAnalyzer
  Models/     StudentRecord
  Services/   CsvLoader, Statistics, ScorePredictor (ML.NET)
  Program.cs  command-line entry point
tests/StudyAnalyzer.Tests   xUnit tests for parsing, statistics and the model
data/students.csv           synthetic sample data
```

## Concepts practiced
Data loading and validation, descriptive statistics, correlation vs. causation, train/test split, regression metrics (R², RMSE), ML.NET pipelines, unit testing.

## About this project
A learning project developed with the help of an AI assistant while I study C#, machine learning and data science.
