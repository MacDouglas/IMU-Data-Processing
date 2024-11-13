using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.SkiaSharp;
using OxyPlot;
using Accelerometer.Simple.Plot.Interfaces;
using OxyPlot.Legends;
using Accelerometer.Simple.Plot.Models;

namespace Accelerometer.Simple.Plot.Modules.Plotter;

[Flags]
public enum DrawMode
{
  X = 1,
  Y = 2,
  Z = 4
}

public class LocalFolderPlotter : IPlotter
{
  public void PlotVelocity(IntegrationResult _points, string _title, string _path, DrawMode _mode)
  {
    var plotModel = new PlotModel { Title = _title, Background = OxyColors.White };

    if ((_mode & DrawMode.X) == DrawMode.X)
    {
      if (_points.VelX == null || _points.VelX.Count == 0)
        return;

      FillSeriesWithDots(plotModel, _points.VelX, "X", OxyColors.Red);
    }

    if ((_mode & DrawMode.Y) == DrawMode.Y)
    {
      if (_points.VelY == null || _points.VelY.Count == 0)
        return;

      FillSeriesWithDots(plotModel, _points.VelY, "Y", OxyColors.Blue, LineStyle.DashDashDot);
    }

    if ((_mode & DrawMode.Z) == DrawMode.Z)
    {
      if (_points.VelZ == null || _points.VelZ.Count == 0)
        return;

      FillSeriesWithDots(plotModel, _points.VelZ, "Z", OxyColors.Green, LineStyle.Dash);
    }

    plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X Velocity (m)" });
    plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y Velocity (m)" });

    plotModel.IsLegendVisible = true;

    ExportPlotToStream(_title, _path, plotModel);
  }

  public void PlotPositions(IntegrationResult _points, string _title, string _path, DrawMode _mode)
  {
    var plotModel = new PlotModel { Title = _title, Background = OxyColors.White };

    if ((_mode & DrawMode.X) == DrawMode.X)
    {
      FillSeriesWithDots(plotModel, _points.PosX, "X", OxyColors.Red);
    }

    if ((_mode & DrawMode.Y) == DrawMode.Y)
    {
      FillSeriesWithDots(plotModel, _points.PosY, "Y", OxyColors.Blue, LineStyle.DashDashDot);
    }

    if ((_mode & DrawMode.Z) == DrawMode.Z)
    {
      FillSeriesWithDots(plotModel, _points.PosZ, "Z", OxyColors.Green, LineStyle.Dash);
    }

    plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X Position (m)" });
    plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y Position (m)" });

    plotModel.IsLegendVisible = true;

    ExportPlotToStream(_title, _path, plotModel);
  }

  private static void FillSeriesWithDots(PlotModel _plotModel,
    IReadOnlyList<double> _pos,
    string _title,
    OxyColor _color,
    LineStyle? _style = null)
  {
    var lineSeries = new LineSeries
    {
      Title = _title,
      MarkerType = MarkerType.None,
      LineStyle = _style ?? LineStyle.Solid,
      Color = _color
    };

    for (var i = 0; i < _pos.Count; i++)
    {
      lineSeries.Points.Add(new DataPoint(i, _pos[i]));
    }

    _plotModel.Series.Add(lineSeries);
  }

  public void PlotPositionsByTime(
    IReadOnlyList<double> _pos,
    IReadOnlyList<DateTime> _time,
    string _title,
    string _path)
  {
    if (_pos.Count != _time.Count)
    {
      throw new ArgumentException("The number of positions and time points must be the same.");
    }

    var plotModel = new PlotModel { Title = _title, Background = OxyColors.White };

    var lineSeries = new LineSeries
    {
      MarkerType = MarkerType.None,
      LineStyle = LineStyle.Solid
    };

    for (var i = 0; i < _pos.Count; i++)
    {
      lineSeries.Points.Add(new DataPoint(i, _pos[i]));
      //lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(_time[i]), _pos[i]));
    }

    plotModel.Series.Add(lineSeries);
    plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Time" });
    plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Position (m)" });

    plotModel.IsLegendVisible = true;

    ExportPlotToStream(_title, _path, plotModel);
  }

  private static void ExportPlotToStream(string _title, string _path, IPlotModel _plotModel)
  {
    using var stream = File.Create($"{_path}/{_title}.png");
    var pngExporter = new PngExporter { Width = 600, Height = 400 };
    pngExporter.Export(_plotModel, stream);

    Console.WriteLine($"Trajectory plot saved as {_title}.png");
  }
}

