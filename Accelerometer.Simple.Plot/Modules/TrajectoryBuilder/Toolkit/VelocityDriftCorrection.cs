namespace Accelerometer.Simple.Plot.Modules.TrajectoryBuilder.Toolkit;

public class VelocityDriftCorrection
{
  public double[] CorrectDrift(double[] velocities, double intervalDuration)
  {
    var intervalSize = velocities.Length > 0 ? intervalDuration / velocities.Length : 1;
    double[] correctedVelocities = new double[velocities.Length];
    double drift = 0;

    for (int i = 0; i < velocities.Length; i++)
    {
      if (i % intervalSize == 0)
      {
        if (i + intervalSize < velocities.Length)
        {
          // Вычисляем среднюю скорость за интервал
          var intervalMean = velocities.Skip(i).Take((int)intervalSize).Average();
          drift = intervalMean; // Считаем дрейф как среднее значение скорости
        }
      }

      correctedVelocities[i] = velocities[i] - drift; // Корректируем скорость
    }

    return correctedVelocities;
  }
}