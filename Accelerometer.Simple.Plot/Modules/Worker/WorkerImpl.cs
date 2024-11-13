using Accelerometer.Simple.Plot.Interfaces;
using Accelerometer.Simple.Plot.Models;
using Accelerometer.Simple.Plot.Modules.Plotter;
using Accelerometer.Simple.Plot.Modules.TrajectoryBuilder;

namespace Accelerometer.Simple.Plot.Modules.Worker;

public class WorkerImpl : IWorker
{
  private readonly ITrajectoryBuilder p_trajectoryBuilder;
  private readonly IDirectoryManager p_dirManager;
  private readonly IPlotter p_plotter;

  public WorkerImpl(ITrajectoryBuilder _trajectoryBuilder,
    IDirectoryManager _dirManager,
    IPlotter _plotter)
  {
    p_trajectoryBuilder = _trajectoryBuilder;
    p_dirManager = _dirManager;
    p_plotter = _plotter;
  }

  public async Task CalculationAndPlottingAsync(
    SamplesResult _samplePoints,
    string _sampleDir,
    bool _isCalibrated)
  {
    var sampleImagesDir = _isCalibrated switch
    {
      true => p_dirManager.CreateDirectoryIfNotExist("calibrated", _sampleDir),
      false => p_dirManager.CreateDirectoryIfNotExist("uncalibrated", _sampleDir),
    };

    var rowDataWork = Task.Factory.StartNew(() =>
    {
      ChooseIntegrationMethod(
        _samplePoints,
        IntegrateMode.Raw,
        sampleImagesDir,
        _isCalibrated);
    });

    var integrateWork = Task.Factory.StartNew(() =>
    {
      ChooseIntegrationMethod(
        _samplePoints,
        IntegrateMode.Simple1,
        sampleImagesDir,
        _isCalibrated);
    });

    var integrate2Work = Task.Factory.StartNew(() =>
    {
      ChooseIntegrationMethod(
        _samplePoints,
        IntegrateMode.Simple2,
        sampleImagesDir,
        _isCalibrated);
    });

    var integrateTrapezoidalWork = Task.Factory.StartNew(() =>
    {
      ChooseIntegrationMethod(
        _samplePoints,
        IntegrateMode.Trapezoidal,
        sampleImagesDir,
        _isCalibrated);
    });

    var integrateSimpsonWork = Task.Factory.StartNew(() =>
    {
      ChooseIntegrationMethod(
        _samplePoints,
        IntegrateMode.Simpson,
        sampleImagesDir,
        _isCalibrated);
    });

    await Task.WhenAll(rowDataWork, integrateWork, integrate2Work, integrateTrapezoidalWork, integrateSimpsonWork);
  }

  private void ChooseIntegrationMethod(
    SamplesResult _samplePoints,
    IntegrateMode _mode,
    string _sampleImagesDir,
    bool _isCalibrated)
  {
    var prefix = _mode switch
    {
      IntegrateMode.Raw => _isCalibrated ? "cal.raw" : "uncal.raw",
      IntegrateMode.Simple1 => _isCalibrated ? "cal.int" : "uncal.int",
      IntegrateMode.Simple2 => _isCalibrated ? "cal.int2" : "uncal.int2",
      IntegrateMode.Simple3 => _isCalibrated ? "cal.int3" : "uncal.int3",
      IntegrateMode.Trapezoidal => _isCalibrated ? "cal.tr" : "uncal.tr",
      IntegrateMode.Simpson => _isCalibrated ? "cal.smp" : "uncal.smp",
      _ => throw new ArgumentOutOfRangeException("IntegrateMode doesn't exist", nameof(_mode))
    };

    var dirName = _mode switch
    {
      IntegrateMode.Raw => _isCalibrated ? "cal.raw" : "uncal.raw",
      IntegrateMode.Simple1 => _isCalibrated ? "cal.integrate1" : "uncal.integrate1",
      IntegrateMode.Simple2 => _isCalibrated ? "cal.integrate2" : "uncal.integrate2",
      IntegrateMode.Simple3 => _isCalibrated ? "cal.integrate3" : "uncal.integrate3",
      IntegrateMode.Trapezoidal => _isCalibrated ? "cal.trapezoidal" : "uncal.trapezoidal",
      IntegrateMode.Simpson => _isCalibrated ? "cal.simpson" : "uncal.simpson",
      _ => throw new ArgumentOutOfRangeException("IntegrateMode doesn't exist", nameof(_mode))
    };

    var info = new NamesInfo(prefix, dirName);

    var positions = _mode switch
    {
      IntegrateMode.Raw => p_trajectoryBuilder.GetRawData(_samplePoints.TrajectoryPoints),
      IntegrateMode.Simple1 => p_trajectoryBuilder.SimpleIntegrate(_samplePoints.TrajectoryPoints),
      IntegrateMode.Simple2 => p_trajectoryBuilder.SimpleIntegrate2(_samplePoints.TrajectoryPoints),
      IntegrateMode.Simple3 => throw new ArgumentException(),
      IntegrateMode.Trapezoidal => p_trajectoryBuilder.IntegrateTrapezoidal(_samplePoints.TrajectoryPoints),
      IntegrateMode.Simpson => p_trajectoryBuilder.IntegrateSimpson(_samplePoints.TrajectoryPoints),
      _ => throw new ArgumentOutOfRangeException("IntegrateMode doesn't exist", nameof(_mode))
    };

    PlotPositions(positions, _sampleImagesDir, info);
    PlotVelocity(positions, _sampleImagesDir, info);
  }


  private void PlotPositions(
    IntegrationResult _positions,
    string _sampleImagesDir,
    NamesInfo _namePostfix)
  {
    var methodImagesDir = p_dirManager.CreateDirectoryIfNotExist(_namePostfix.DirName, _sampleImagesDir);

    p_plotter.PlotPositions(_positions, $"pos.{_namePostfix.Prefix}.X", methodImagesDir, DrawMode.X);
    p_plotter.PlotPositions(_positions, $"pos.{_namePostfix.Prefix}.Y", methodImagesDir, DrawMode.Y);
    p_plotter.PlotPositions(_positions, $"pos.{_namePostfix.Prefix}.Z", methodImagesDir, DrawMode.Z);

    p_plotter.PlotPositions(_positions, $"pos.{_namePostfix.Prefix}.XY", methodImagesDir, DrawMode.X | DrawMode.Y);
    p_plotter.PlotPositions(_positions, $"pos.{_namePostfix.Prefix}.YZ", methodImagesDir, DrawMode.Y | DrawMode.Z);
    p_plotter.PlotPositions(_positions, $"pos.{_namePostfix.Prefix}.ZX", methodImagesDir, DrawMode.Z | DrawMode.X);

    p_plotter.PlotPositions(_positions, $"pos.{_namePostfix.Prefix}.XYZ", methodImagesDir, DrawMode.X | DrawMode.Y | DrawMode.Z);
  }

  private void PlotVelocity(
    IntegrationResult _positions,
    string _sampleImagesDir,
    NamesInfo _namePostfix)
  {
    var methodImagesDir = p_dirManager.CreateDirectoryIfNotExist(_namePostfix.DirName, _sampleImagesDir);

    p_plotter.PlotVelocity(_positions, $"vel.{_namePostfix.Prefix}.X", methodImagesDir, DrawMode.X);
    p_plotter.PlotVelocity(_positions, $"vel.{_namePostfix.Prefix}.Y", methodImagesDir, DrawMode.Y);
    p_plotter.PlotVelocity(_positions, $"vel.{_namePostfix.Prefix}.Z", methodImagesDir, DrawMode.Z);

    p_plotter.PlotVelocity(_positions, $"vel.{_namePostfix.Prefix}.XY", methodImagesDir, DrawMode.X | DrawMode.Y);
    p_plotter.PlotVelocity(_positions, $"vel.{_namePostfix.Prefix}.YZ", methodImagesDir, DrawMode.Y | DrawMode.Z);
    p_plotter.PlotVelocity(_positions, $"vel.{_namePostfix.Prefix}.ZX", methodImagesDir, DrawMode.Z | DrawMode.X);

    p_plotter.PlotVelocity(_positions, $"vel.{_namePostfix.Prefix}.XYZ", methodImagesDir, DrawMode.X | DrawMode.Y | DrawMode.Z);
  }

}

