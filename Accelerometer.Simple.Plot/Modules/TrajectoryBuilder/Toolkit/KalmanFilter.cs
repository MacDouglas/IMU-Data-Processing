namespace Accelerometer.Simple.Plot.Modules.TrajectoryBuilder.Toolkit;

public class KalmanFilter
{
    private double Q; // Процессное шумовое ковариационное значение
    private double R; // Измерительное шумовое ковариационное значение
    private double P; // Ошибка оценки
    private double K; // Калмановское усиление
    private double X; // Значение фильтра

    public KalmanFilter(
      double _processNoise, 
      double _measurementNoise, 
      double _estimationError, 
      double _initialValue)
    {
        Q = _processNoise;
        R = _measurementNoise;
        P = _estimationError;
        X = _initialValue;
    }

    public double Filter(double _measurement)
    {
        P += Q;
        K = P / (P + R);
        X += K * (_measurement - X);
        
        P = (1 - K) * P;

        return X;
    }
}