using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.SkiaSharp;
using OxyPlot;
using Accelerometer.Simple.Plot.Interfaces;
using OxyPlot.Legends;

namespace Accelerometer.Simple.Plot.Modules.Plotter;

public class LocalFolderPlotter : IPlotter
{
  public void PlotPosition(IReadOnlyList<double> _pos, string _title, string _path)
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

    plotModel.IsLegendVisible = true;

    ExportPlotToStream(_title, _path, plotModel);
  }

  public void PlotPositionByTime(
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
      lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(_time[i]), _pos[i]));
    }

    plotModel.Series.Add(lineSeries);
    plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Time" });
    plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Position (m)" });

    plotModel.IsLegendVisible = true;

    ExportPlotToStream(_title, _path, plotModel);
  }
  public void PlotTwoPositionsByTime(
    IReadOnlyList<double> _pos1,
    IReadOnlyList<double> _pos2,
    IReadOnlyList<DateTime> _time,
    string _posTitle1,
    string _posTitle2,
    string _title,
    string _path)
  {
    if (_pos1.Count != _time.Count || _pos2.Count != _time.Count)
    {
      throw new ArgumentException("The number of positions and time points must be the same.");
    }

    var plotModel = new PlotModel { Title = _title, Background = OxyColors.White };

    var lineSeries1 = new LineSeries
    {
      Title = _posTitle1,
      MarkerType = MarkerType.None,
      LineStyle = LineStyle.Solid,
      Color = OxyColors.Blue
    };

    var lineSeries2 = new LineSeries
    {
      Title = _posTitle2,
      MarkerType = MarkerType.None,
      LineStyle = LineStyle.Solid,
      Color = OxyColors.Red
    };

    for (var i = 0; i < _time.Count; i++)
    {
      lineSeries1.Points.Add(new DataPoint(DateTimeAxis.ToDouble(_time[i]), _pos1[i]));
      lineSeries2.Points.Add(new DataPoint(DateTimeAxis.ToDouble(_time[i]), _pos2[i]));
    }

    plotModel.Series.Add(lineSeries1);
    plotModel.Series.Add(lineSeries2);
    plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Time" });
    plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Position (m)" });

    plotModel.IsLegendVisible = true;

    ExportPlotToStream(_title, _path, plotModel);
  }

  public void PlotThreePositionsByTime(
      IReadOnlyList<double> _pos1,
      IReadOnlyList<double> _pos2,
      IReadOnlyList<double> _pos3,
      IReadOnlyList<DateTime> _time,
      string _posTitle1,
      string _posTitle2,
      string _posTitle3,
      string _title,
      string _path)
  {
    if (_pos1.Count != _time.Count || _pos2.Count != _time.Count || _pos3.Count != _time.Count)
    {
      throw new ArgumentException("The number of positions and time points must be the same.");
    }

    var plotModel = new PlotModel { Title = _title, Background = OxyColors.White };

    var lineSeries1 = new LineSeries
    {
      Title = _posTitle1,
      MarkerType = MarkerType.None,
      LineStyle = LineStyle.Solid,
      Color = OxyColors.Blue
    };

    var lineSeries2 = new LineSeries
    {
      Title = _posTitle2,
      MarkerType = MarkerType.None,
      LineStyle = LineStyle.Solid,
      Color = OxyColors.Red
    };

    var lineSeries3 = new LineSeries
    {
      Title = _posTitle3,
      MarkerType = MarkerType.None,
      LineStyle = LineStyle.Solid,
      Color = OxyColors.Green
    };

    for (var i = 0; i < _time.Count; i++)
    {
      lineSeries1.Points.Add(new DataPoint(DateTimeAxis.ToDouble(_time[i]), _pos1[i]));
      lineSeries2.Points.Add(new DataPoint(DateTimeAxis.ToDouble(_time[i]), _pos2[i]));
      lineSeries3.Points.Add(new DataPoint(DateTimeAxis.ToDouble(_time[i]), _pos3[i]));
    }

    plotModel.Series.Add(lineSeries1);
    plotModel.Series.Add(lineSeries2);
    plotModel.Series.Add(lineSeries3);
    plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Time" });
    plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Position (m)" });

    plotModel.IsLegendVisible = true;

    ExportPlotToStream(_title, _path, plotModel);
  }

  private void ExportPlotToStream(string _title, string _path, IPlotModel _plotModel)
  {
    using var stream = File.Create($"{_path}/{_title}.png");
    var pngExporter = new PngExporter { Width = 600, Height = 400 };
    pngExporter.Export(_plotModel, stream);

    Console.WriteLine($"Trajectory plot saved as {_title}.png");
  }
}

