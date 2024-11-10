using System.Globalization;
using Accelerometer.Simple.Plot.Interfaces;
using Accelerometer.Simple.Plot.Models;
using Accelerometer.Simple.Plot.Modules.DirectoryManager;
using Accelerometer.Simple.Plot.Modules.Plotter;
using Accelerometer.Simple.Plot.Modules.SampleReader;
using Accelerometer.Simple.Plot.Modules.TrajectoryBuilder;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.SkiaSharp;

namespace Accelerometer.Simple.Plot;

public record struct NamesInfo(string Postfix, string DirName);

internal class Program
{
  static void Main(string[] _args)
  {
    var sampleReader = new LocalFileSampleReaderImpl();
    var trajectoryBuilder = new TrajectoryBuilder();

    var executingDirectory = AppContext.BaseDirectory;
    var dirManager = new DirectoryManagerImpl(executingDirectory);
    var plotter = new LocalFolderPlotter();

    var samplesDirectory = dirManager.CreateDirectoryIfNotExist("samples");
    var imagesDirectory = dirManager.CreateDirectoryIfNotExist("images");
    var txtFiles = dirManager.GetSamples(samplesDirectory, "*.txt");

    foreach (var file in txtFiles)
    {
      var fileName = Path.GetFileNameWithoutExtension(file);
      var sampleImagesDir = dirManager.CreateDirectoryIfNotExist(fileName, imagesDirectory);

      var samplePoints = sampleReader.ReadSample(file);

      var raw = trajectoryBuilder.GetRawData(samplePoints.TrajectoryPoints);
      PlotPositions(dirManager, plotter, raw, sampleImagesDir, samplePoints.SamplesTime, new NamesInfo("raw", "raw"));
 
      var positions = trajectoryBuilder.SimpleIntegrate(samplePoints.TrajectoryPoints);
      PlotPositions(dirManager, plotter, positions, sampleImagesDir, samplePoints.SamplesTime, new NamesInfo("int", "integrate"));
 
      var positions2 = trajectoryBuilder.SimpleIntegrate2(samplePoints.TrajectoryPoints);
      PlotPositions(dirManager, plotter, positions2, sampleImagesDir, samplePoints.SamplesTime, new NamesInfo("int2", "integrate2"));
 
      var positionsTrapezoidal = trajectoryBuilder.IntegrateTrapezoidal(samplePoints.TrajectoryPoints);
      PlotPositions(dirManager, plotter, positionsTrapezoidal, sampleImagesDir, samplePoints.SamplesTime, new NamesInfo("tr", "trapezoidal"));

      var positionsSimpson = trajectoryBuilder.IntegrateSimpson(samplePoints.TrajectoryPoints);
      PlotPositions(dirManager, plotter, positionsSimpson, sampleImagesDir, positionsSimpson.DTime, new NamesInfo("smp", "simpson"));
    }
  }

  private static void PlotPositions(
    IDirectoryManager _dirManager,
    IPlotter _plotter,
    IntegrationResult _positions,
    string _sampleImagesDir,
    IReadOnlyList<DateTime> _sampleTimes,
    NamesInfo _namePostfix)
  {
    var methodImagesDir = _dirManager.CreateDirectoryIfNotExist(_namePostfix.DirName, _sampleImagesDir);

    _plotter.PlotPositionByTime(_positions.PosX, _sampleTimes, $"X_{_namePostfix.Postfix}_time", methodImagesDir);
    _plotter.PlotPositionByTime(_positions.PosY, _sampleTimes, $"Y_{_namePostfix.Postfix}_time", methodImagesDir);
    _plotter.PlotPositionByTime(_positions.PosZ, _sampleTimes, $"Z_{_namePostfix.Postfix}_time", methodImagesDir);

    _plotter.PlotTwoPositionsByTime(
      _positions.PosX,
      _positions.PosY,
      _sampleTimes,
      $"X_{_namePostfix.Postfix}",
      $"Y_{_namePostfix.Postfix}",
      $"XY_{_namePostfix.Postfix}",
      methodImagesDir);

    _plotter.PlotTwoPositionsByTime(
      _positions.PosY,
      _positions.PosZ,
      _sampleTimes,
      $"Y_{_namePostfix.Postfix}",
      $"Z_{_namePostfix.Postfix}",
      $"YZ_{_namePostfix.Postfix}",
      methodImagesDir);

    _plotter.PlotTwoPositionsByTime(
      _positions.PosZ,
      _positions.PosX,
      _sampleTimes,
      $"Z_{_namePostfix.Postfix}",
      $"X_{_namePostfix.Postfix}",
      $"ZX_{_namePostfix.Postfix}",
      methodImagesDir);

    _plotter.PlotThreePositionsByTime(
      _positions.PosX,
      _positions.PosY,
      _positions.PosZ,
      _sampleTimes,
      $"X_{_namePostfix.Postfix}",
      $"Y_{_namePostfix.Postfix}",
      $"Z_{_namePostfix.Postfix}",
      $"XYZ_{_namePostfix.Postfix}",
      methodImagesDir);
  }

  private static List<double> CalculateIntersection(List<double> _pos1, List<double> _pos2)
  {
    var posCombined = new List<double>();
    for (var i = 1; i < _pos1.Count; i++)
    {
      var combined = CalculateDistance2D(_pos1[i - 1], _pos2[i - 1], _pos1[i], _pos2[i]);
      posCombined.Add(combined);
    }

    return posCombined;
  }

  public static double CalculateDistance2D(double x1, double y1, double x2, double y2)
  {
    return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
  }

  private static void PlotByTime(List<double> _pos, List<DateTime> _time, string _title)
  {
    var plotModel = new PlotModel { Title = _title, Background = OxyColors.White };

    var lineSeries = new LineSeries
    {
      MarkerType = MarkerType.None,
      LineStyle = LineStyle.Solid
    };

    for (var i = 0; i < _pos.Count; i++)
    {
      lineSeries.Points.Add(new DataPoint(i, _pos[i]));
    }

    plotModel.Series.Add(lineSeries);
    plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X Position (m)" });
    plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y Position (m)" });

    using (var stream = File.Create($"{_title}.png"))
    {
      var pngExporter = new PngExporter { Width = 600, Height = 400 };
      pngExporter.Export(plotModel, stream);
    }

    Console.WriteLine($"Trajectory plot saved as {_title}.png");
  }

  static List<double> Integrate(IReadOnlyList<DateTime> _time, IReadOnlyList<double> _values)
  {
    var integrated = new List<double> { 0 };
    for (var i = 1; i < _time.Count; i++)
    {
      var dt = (_time[i] - _time[i - 1]).TotalSeconds;
      var value = _values[i - 1];
      var integral = integrated[i - 1] + value * dt;
      integrated.Add(integral);
    }
    return integrated;
  }

}