namespace Accelerometer.Simple.Plot.Models;

public record IntegrationResult(
  IReadOnlyList<double>? VelX = null,
  IReadOnlyList<double>? VelY = null,
  IReadOnlyList<double>? VelZ = null,
  IReadOnlyList<double>? PosX = null,
  IReadOnlyList<double>? PosY = null,
  IReadOnlyList<double>? PosZ = null,
  IReadOnlyList<DateTime>? DTime = null);