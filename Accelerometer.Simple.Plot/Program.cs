using Accelerometer.Simple.Plot.Modules.DirectoryManager;
using Accelerometer.Simple.Plot.Modules.Plotter;
using Accelerometer.Simple.Plot.Modules.PointsCalibrator;
using Accelerometer.Simple.Plot.Modules.SampleReader;
using Accelerometer.Simple.Plot.Modules.TrajectoryBuilder;
using Accelerometer.Simple.Plot.Modules.Worker;

namespace Accelerometer.Simple.Plot;

public class Program
{
  private static async Task Main(string[] _args)
  {
    var sampleReader = new LocalFileSampleReaderImpl();
    var trajectoryBuilder = new TrajectoryBuilder();
    var calibrator = new PointsCalibratorImpl();

    var executingDirectory = AppContext.BaseDirectory;
    var dirManager = new DirectoryManagerImpl(executingDirectory);
    var plotter = new LocalFolderPlotter();

    var samplesDirectory = dirManager.CreateDirectoryIfNotExist("samples");
    var imagesDirectory = dirManager.CreateDirectoryIfNotExist("images");
    var txtFiles = dirManager.GetSamples(samplesDirectory, "*.txt");

    var worker = new WorkerImpl(trajectoryBuilder, dirManager, plotter);

    foreach (var file in txtFiles)
    {
      var fileName = Path.GetFileNameWithoutExtension(file);
      var sampleImagesDir = dirManager.CreateDirectoryIfNotExist(fileName, imagesDirectory);

      var uncalibratedPoints = sampleReader.ReadSample(file);
      var calibratedPoints = calibrator.CalibratePoints(uncalibratedPoints);

      await worker.CalculationAndPlottingAsync(uncalibratedPoints, sampleImagesDir, false);
      await worker.CalculationAndPlottingAsync(calibratedPoints, sampleImagesDir, true);
    }
  }

}