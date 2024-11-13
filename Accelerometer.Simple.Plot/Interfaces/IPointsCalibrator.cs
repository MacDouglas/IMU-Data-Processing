using Accelerometer.Simple.Plot.Models;

namespace Accelerometer.Simple.Plot.Interfaces;

public interface IPointsCalibrator
{
    public SamplesResult CalibratePoints(SamplesResult _dataPoints);
}