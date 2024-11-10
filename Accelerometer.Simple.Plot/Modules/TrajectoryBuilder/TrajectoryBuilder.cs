using Accelerometer.Simple.Plot.Models;
using OxyPlot;

namespace Accelerometer.Simple.Plot.Modules.TrajectoryBuilder;

public class TrajectoryBuilder
{
  public IntegrationResult GetRawData(IReadOnlyList<SamplePoint> _dataPoints)
  {
    var posX = new List<double>();
    var posY = new List<double>();
    var posZ = new List<double>();

    foreach (var sample in _dataPoints)
    {
      posX.Add(sample.AccX);
      posY.Add(sample.AccY);
      posZ.Add(sample.AccZ);
    }

    return new IntegrationResult(null, null, null, posX, posY, posZ);
  }

  public IntegrationResult SimpleIntegrate(IReadOnlyList<SamplePoint> _dataPoints)
  {
    var velX = new List<double> { 0.0 };
    var velY = new List<double> { 0.0 };
    var velZ = new List<double> { 0.0 };
    var posX = new List<double> { 0.0 };
    var posY = new List<double> { 0.0 };
    var posZ = new List<double> { 0.0 };

    for (var i = 1; i < _dataPoints.Count; i++)
    {
      var dt = (_dataPoints[i].Time - _dataPoints[i - 1].Time).TotalSeconds;

      var accX_avg = (_dataPoints[i - 1].AccX + _dataPoints[i].AccX) / 2.0;
      var accY_avg = (_dataPoints[i - 1].AccY + _dataPoints[i].AccY) / 2.0;
      var accZ_avg = (_dataPoints[i - 1].AccZ + _dataPoints[i].AccZ) / 2.0;

      velX.Add(velX[i - 1] + accX_avg * dt);
      velY.Add(velY[i - 1] + accY_avg * dt);
      velZ.Add(velZ[i - 1] + accZ_avg * dt);
    }

    for (var i = 1; i < velX.Count; i++)
    {
      var dt = (_dataPoints[i].Time - _dataPoints[i - 1].Time).TotalSeconds;

      var velX_avg = (velX[i - 1] + velX[i]) / 2.0;
      var velY_avg = (velY[i - 1] + velY[i]) / 2.0;
      var velZ_avg = (velZ[i - 1] + velZ[i]) / 2.0;

      posX.Add(posX[i - 1] + velX_avg * dt);
      posY.Add(posY[i - 1] + velY_avg * dt);
      posZ.Add(posZ[i - 1] + velZ_avg * dt);
    }

    return new IntegrationResult(velX, velY, velZ, posX, posY, posZ);
  }

  public IntegrationResult SimpleIntegrate2(IReadOnlyList<SamplePoint> _dataPoints)
  {
    var velX = new List<double> { 0.0 };
    var velY = new List<double> { 0.0 };
    var velZ = new List<double> { 0.0 };
    var posX = new List<double> { 0.0 };
    var posY = new List<double> { 0.0 };
    var posZ = new List<double> { 0.0 };

    var n = _dataPoints.Count;

    // Проверка на наличие достаточного количества точек
    if (n < 2)
    {
      throw new ArgumentException("Not enough data points for integration.");
    }

    // Первая интеграция: Ускорения в скорости
    for (var i = 1; i < n; i++)
    {
      var dt = (_dataPoints[i].Time - _dataPoints[i - 1].Time).TotalSeconds;

      // Простое интегрирование (метод трапеций) для ускорений
      var velX_prev = velX[velX.Count - 1];
      var velY_prev = velY[velY.Count - 1];
      var velZ_prev = velZ[velZ.Count - 1];

      var new_velX = velX_prev + (_dataPoints[i].AccX + _dataPoints[i - 1].AccX) / 2 * dt;
      var new_velY = velY_prev + (_dataPoints[i].AccY + _dataPoints[i - 1].AccY) / 2 * dt;
      var new_velZ = velZ_prev + (_dataPoints[i].AccZ + _dataPoints[i - 1].AccZ) / 2 * dt;

      velX.Add(new_velX);
      velY.Add(new_velY);
      velZ.Add(new_velZ);
    }

    for (var i = 1; i < velX.Count; i++)
    {
      var dt = (_dataPoints[i].Time - _dataPoints[i - 1].Time).TotalSeconds;

      var posX_prev = posX[posX.Count - 1];
      var posY_prev = posY[posY.Count - 1];
      var posZ_prev = posZ[posZ.Count - 1];

      var new_posX = posX_prev + (velX[i] + velX[i - 1]) / 2 * dt;
      var new_posY = posY_prev + (velY[i] + velY[i - 1]) / 2 * dt;
      var new_posZ = posZ_prev + (velZ[i] + velZ[i - 1]) / 2 * dt;

      posX.Add(new_posX);
      posY.Add(new_posY);
      posZ.Add(new_posZ);
    }

    return new IntegrationResult(velX, velY, velZ, posX, posY, posZ);
  }

  public IntegrationResult IntegrateTrapezoidal(IReadOnlyList<SamplePoint> _dataPoints)
  {
    var velX = new List<double> { 0.0 };
    var velY = new List<double> { 0.0 };
    var velZ = new List<double> { 0.0 };

    var posX = new List<double> { 0.0 };
    var posY = new List<double> { 0.0 };
    var posZ = new List<double> { 0.0 };

    for (var i = 1; i < _dataPoints.Count; i++)
    {
      var dt = (_dataPoints[i].Time - _dataPoints[i - 1].Time).TotalSeconds;

      var calcVelX = CalculateVelocity(_dataPoints, velX[i - 1], i, dt, SelectedCoordinate.X);
      var calcVelY = CalculateVelocity(_dataPoints, velY[i - 1], i, dt, SelectedCoordinate.Y);
      var calcVelZ = CalculateVelocity(_dataPoints, velZ[i - 1], i, dt, SelectedCoordinate.Z);

      velX.Add(calcVelX);
      velY.Add(calcVelY);
      velZ.Add(calcVelZ);

      var calcPosX = IntegratePosition(dt, i, posX, velX);
      var calcPosY = IntegratePosition(dt, i, posY, velY);
      var calcPosZ = IntegratePosition(dt, i, posZ, velZ);

      posX.Add(calcPosX);
      posY.Add(calcPosY);
      posZ.Add(calcPosZ);
    }

    return new IntegrationResult(velX, velY, velZ, posX, posY, posZ);
  }

  public IntegrationResult IntegrateSimpson(IReadOnlyList<SamplePoint> _dataPoints)
  {
    var dataPoints = _dataPoints;

    var velX = new List<double> { 0.0 };
    var velY = new List<double> { 0.0 };
    var velZ = new List<double> { 0.0 };

    var posX = new List<double> { 0.0 };
    var posY = new List<double> { 0.0 };
    var posZ = new List<double> { 0.0 };

    var dTime = new List<DateTime> { dataPoints.First().Time };


    var n = dataPoints.Count;

    if (n < 3 || n % 2 == 0)
    {
      var firstPoint = dataPoints.First();
      dataPoints = new List<SamplePoint> { firstPoint }.Concat(dataPoints).ToList();

      //throw new ArgumentException("Number of data points musts be odd and greater than 2.");
    }

    for (var i = 2; i < n; i += 2)
    {
      var dt1 = (dataPoints[i].Time - dataPoints[i - 2].Time).TotalSeconds;

      var velX_prev = velX[velX.Count - 1];
      var velY_prev = velY[velY.Count - 1];
      var velZ_prev = velZ[velZ.Count - 1];

      var velX_next = velX_prev + dt1 / 3.0 * (dataPoints[i - 2].AccX + 4 * dataPoints[i - 1].AccX + dataPoints[i].AccX);
      var velY_next = velY_prev + dt1 / 3.0 * (dataPoints[i - 2].AccY + 4 * dataPoints[i - 1].AccY + dataPoints[i].AccY);
      var velZ_next = velZ_prev + dt1 / 3.0 * (dataPoints[i - 2].AccZ + 4 * dataPoints[i - 1].AccZ + dataPoints[i].AccZ);

      velX.Add(velX_next);
      velY.Add(velY_next);
      velZ.Add(velZ_next);
    }

    for (var i = 2; i < velX.Count; i += 2)
    {
      var dt1 = (dataPoints[i].Time - dataPoints[i - 2].Time).TotalSeconds;
      dTime.Add(dataPoints[i - 2].Time);

      var posX_prev = posX[posX.Count - 1];
      var posY_prev = posY[posY.Count - 1];
      var posZ_prev = posZ[posZ.Count - 1];

      var posX_next = posX_prev + dt1 / 3.0 * (velX[i - 2] + 4 * velX[i - 1] + velX[i]);
      var posY_next = posY_prev + dt1 / 3.0 * (velY[i - 2] + 4 * velY[i - 1] + velY[i]);
      var posZ_next = posZ_prev + dt1 / 3.0 * (velZ[i - 2] + 4 * velZ[i - 1] + velZ[i]);

      posX.Add(posX_next);
      posY.Add(posY_next);
      posZ.Add(posZ_next);
    }

    return new IntegrationResult(velX, velY, velZ, posX, posY, posZ, dTime);
  }

  private static double IntegratePosition(
    double _dt,
    int _index,
    IReadOnlyList<double> _pos,
    IReadOnlyList<double> _vel)
  {
    var averageVelocity = CalculateAverageVelocity(_index, _vel);
    var lastPosition = _pos[_index - 1];

    var pos = lastPosition + averageVelocity * _dt;
    return pos;
  }

  private static double CalculateVelocity(
    IReadOnlyList<SamplePoint> _points,
    double _prevVel,
    int _index,
    double _dt,
    SelectedCoordinate _selectedCoordinate)
  {
    var averageAcc = _selectedCoordinate switch
    {
      SelectedCoordinate.X => (_points[_index].AccX + _points[_index - 1].AccX) / 2.0,
      SelectedCoordinate.Y => (_points[_index].AccY + _points[_index - 1].AccY) / 2.0,
      SelectedCoordinate.Z => (_points[_index].AccZ + _points[_index - 1].AccZ) / 2.0,
      _ => throw new ArgumentOutOfRangeException(nameof(_selectedCoordinate), _selectedCoordinate, null)
    };

    var curVel = _prevVel + averageAcc * _dt;
    return curVel;
  }

  private static double CalculateAverageVelocity(int _index, IReadOnlyList<double> _vel)
  {
    return (_vel[_index] + _vel[_index - 1]) / 2.0;
  }

  /*
 
  public void Integrate2()
  {
    velX = new List<double> { 0d };
    velY = new List<double> { 0d };
    velZ = new List<double> { 0d };
    posX = new List<double> { 0d };
    posY = new List<double> { 0d };
    posZ = new List<double> { 0d };

    for (var i = 1; i < DataPoints.Count; i++)
    {
      var dt = (DataPoints[i].Time - DataPoints[i - 1].Time).TotalSeconds;

      velX.Add(velX[i - 1] + DataPoints[i].AccX * dt);
      velY.Add(velY[i - 1] + DataPoints[i].AccY * dt);
      velZ.Add(velZ[i - 1] + DataPoints[i].AccZ * dt);

      posX.Add(posX[i - 1] + velX[i] * dt);
      posY.Add(posY[i - 1] + velY[i] * dt);
      posZ.Add(posZ[i - 1] + velZ[i] * dt);
    }
  }

  */
}

