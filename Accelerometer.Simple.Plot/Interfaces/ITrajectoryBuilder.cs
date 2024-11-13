using Accelerometer.Simple.Plot.Models;

namespace Accelerometer.Simple.Plot.Interfaces;

public interface ITrajectoryBuilder
{
    public IntegrationResult GetRawData(IReadOnlyList<SamplePoint> _dataPoints);
    public IntegrationResult SimpleIntegrate(IReadOnlyList<SamplePoint> _dataPoints);
    public IntegrationResult SimpleIntegrate2(IReadOnlyList<SamplePoint> _dataPoints);
    public IntegrationResult IntegrateTrapezoidal(IReadOnlyList<SamplePoint> _dataPoints);
    public IntegrationResult IntegrateSimpson(IReadOnlyList<SamplePoint> _dataPoints);
}