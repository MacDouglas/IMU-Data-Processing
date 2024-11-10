using Accelerometer.Simple.Plot.Models;

namespace Accelerometer.Simple.Plot.Interfaces;

public interface ISampleReader
{
  public SamplesResult ReadSample(string _pathToSample);
}