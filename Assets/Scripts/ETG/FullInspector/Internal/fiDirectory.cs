using System.Collections.Generic;
using System.IO;

#nullable disable
namespace FullInspector.Internal
{
  public static class fiDirectory
  {
    public static bool Exists(string path) => Directory.Exists(path);

    public static void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public static IEnumerable<string> GetDirectories(string path)
    {
      return (IEnumerable<string>) Directory.GetDirectories(path);
    }
  }
}
