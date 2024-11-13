using Accelerometer.Simple.Plot.Models;

namespace Accelerometer.Simple.Plot.Interfaces;

public interface IWorker
{
    public Task CalculationAndPlottingAsync(SamplesResult _samplePoints, string _sampleDir, bool _isCalibrated);
}