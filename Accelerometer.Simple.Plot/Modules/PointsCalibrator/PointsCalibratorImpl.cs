using Accelerometer.Simple.Plot.Interfaces;
using Accelerometer.Simple.Plot.Models;
using Accelerometer.Simple.Plot.Modules.TrajectoryBuilder.Toolkit;

namespace Accelerometer.Simple.Plot.Modules.PointsCalibrator;

public class PointsCalibratorImpl : IPointsCalibrator
{
    public SamplesResult CalibratePoints(SamplesResult _dataPoints)
    {
        var calibration = new Calibration();
        var accX = _dataPoints.TrajectoryPoints.Select(_p => _p.AccX).ToArray();
        var accY = _dataPoints.TrajectoryPoints.Select(_p => _p.AccY).ToArray();
        var accZ = _dataPoints.TrajectoryPoints.Select(_p => _p.AccZ).ToArray();

        accX = calibration.CalibrateAccelerometer(accX, 0.0);
        accY = calibration.CalibrateAccelerometer(accY, 0.0);
        accZ = calibration.CalibrateAccelerometer(accZ, 1.0);

        var kalmanFilterX = new KalmanFilter(0.001, 0.1, 1.0, accX[0]);
        var kalmanFilterY = new KalmanFilter(0.001, 0.1, 1.0, accY[0]);
        var kalmanFilterZ = new KalmanFilter(0.001, 0.1, 1.0, accZ[0]);

        accX = accX.Select(_acc => kalmanFilterX.Filter(_acc)).ToArray();
        accY = accY.Select(_acc => kalmanFilterY.Filter(_acc)).ToArray();
        accZ = accZ.Select(_acc => kalmanFilterZ.Filter(_acc)).ToArray();

        var samplePoints = _dataPoints.TrajectoryPoints
          .Zip(accX)
          .Zip(accY)
          .Zip(accZ)
          .Select(_seq
            => new SamplePoint(_seq.First.First.First.Time,
              _seq.First.First.Second,
              _seq.First.Second,
              _seq.Second))
          .ToList();

        return new SamplesResult(samplePoints);
    }
}