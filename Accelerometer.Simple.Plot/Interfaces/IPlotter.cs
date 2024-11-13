using Accelerometer.Simple.Plot.Models;
using Accelerometer.Simple.Plot.Modules.Plotter;

namespace Accelerometer.Simple.Plot.Interfaces;

public interface IPlotter
{
  public void PlotPositionsByTime(IReadOnlyList<double> _pos, IReadOnlyList<DateTime> _time, string _title, string _path);
  public void PlotPositions(IntegrationResult _points, string _title, string _path, DrawMode _mode);
  public void PlotVelocity(IntegrationResult _points, string _title, string _path, DrawMode _mode);
}