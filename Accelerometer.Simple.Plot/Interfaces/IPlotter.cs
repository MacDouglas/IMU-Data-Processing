namespace Accelerometer.Simple.Plot.Interfaces;

public interface IPlotter
{
  public void PlotPositionByTime(IReadOnlyList<double> _pos, IReadOnlyList<DateTime> _time, string _title, string _path);
  public void PlotPosition(IReadOnlyList<double> _pos, string _title, string _path);

  public void PlotTwoPositionsByTime(
    IReadOnlyList<double> _pos1,
    IReadOnlyList<double> _pos2,
    IReadOnlyList<DateTime> _time,
    string _posTitle1,
    string _posTitle2,
    string _title,
    string _path);

  public void PlotThreePositionsByTime(
    IReadOnlyList<double> _pos1,
    IReadOnlyList<double> _pos2,
    IReadOnlyList<double> _pos3,
    IReadOnlyList<DateTime> _time,
    string _posTitle1,
    string _posTitle2,
    string _posTitle3,
    string _title,
    string _path);
}