using Accelerometer.Simple.Plot.Models;

namespace Accelerometer.Simple.Plot.Modules.TrajectoryBuilder.Toolkit;

public class Calibration
{
  public double[] CalibrateAccelerometer(double[] _data, double _knownValue)
  {
    var mean = _data.Average();
    var offset = _knownValue - mean;

    return _data.Select(d => d + offset).ToArray();
  }

}