using System.Text;
using Accelerometer.Simple.Plot.Interfaces;

namespace Accelerometer.Simple.Plot.Modules.DirectoryManager;

public class DirectoryManagerImpl : IDirectoryManager
{
  private readonly string p_root;

  public DirectoryManagerImpl(string _root)
  {
    p_root = _root;
  }

  public string CreateDirectoryIfNotExist(string _targetDir, string? _rootDir = null)
  {
    var targetDir = new StringBuilder();

    targetDir.Append(_rootDir != null 
      ? Path.Combine(_rootDir, _targetDir) 
      : Path.Combine(p_root, _targetDir));

    var path = targetDir.ToString();

    if (!Directory.Exists(path))
    {
      Directory.CreateDirectory(path);
    }

    return path;
  }

  public IEnumerable<string> GetSamples(string _targetDir, string _searchPattern)
  {
    var searchedFiles = Directory
      .GetFiles(_targetDir, _searchPattern);

    return searchedFiles;
  }
}