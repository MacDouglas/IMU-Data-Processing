namespace Accelerometer.Simple.Plot.Interfaces;

public interface IDirectoryManager
{
  public string CreateDirectoryIfNotExist(string _targetDir, string? _rootDir = null);
  public IEnumerable<string> GetSamples(string _targetDir, string _searchPattern);
}