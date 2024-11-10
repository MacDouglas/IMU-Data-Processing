namespace Accelerometer.Simple.Plot.Models;

public record struct SamplesResult(
  IReadOnlyList<SamplePoint> TrajectoryPoints,
  IReadOnlyList<DateTime> SamplesTime);