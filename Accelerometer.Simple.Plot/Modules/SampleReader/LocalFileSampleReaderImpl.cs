using System.Collections.Generic;
using System.Globalization;
using Accelerometer.Simple.Plot.Interfaces;
using Accelerometer.Simple.Plot.Models;

namespace Accelerometer.Simple.Plot.Modules.SampleReader;

public class LocalFileSampleReaderImpl : ISampleReader
{
  private readonly string[] p_dateFormats = {
    "yyyy-M-d HH:mm:s:f",
    "yyyy-M-d HH:mm:s:ff",
    "yyyy-M-d HH:mm:s:fff",
    "yyyy-M-d HH:mm:ss:f",
    "yyyy-M-d HH:mm:ss:ff",
    "yyyy-M-d HH:mm:ss:fff"
  };

  public SamplesResult ReadSample(string _pathToSample)
  {
    if (string.IsNullOrEmpty(_pathToSample))
      throw new ArgumentNullException("Неверный путь до файла замеров", nameof(_pathToSample));

    var time = new List<DateTime>();
    var trajectoryPoints = new List<SamplePoint>();

    using var sr = new StreamReader(_pathToSample);
    sr.ReadLine();
    while (sr.ReadLine() is { } line)
    {
      var parts = line.Split('\t');

      try
      {
        if (DateTime.TryParseExact(parts[0], p_dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
          time.Add(parsedDate);

          double.TryParse(parts[2], CultureInfo.InvariantCulture, out var accX);
          double.TryParse(parts[3], CultureInfo.InvariantCulture, out var accY);
          double.TryParse(parts[4], CultureInfo.InvariantCulture, out var accZ);

          trajectoryPoints.Add(new SamplePoint(parsedDate, accX, accY, accZ));
        }
        else
        {
          Console.WriteLine($"Error parsing date: {parts[0]}");
        }
      }
      catch (FormatException ex)
      {
        Console.WriteLine($"Error parsing line: {line}. Exception: {ex.Message}");
      }
    }

    return new SamplesResult(trajectoryPoints, time);
  }
}