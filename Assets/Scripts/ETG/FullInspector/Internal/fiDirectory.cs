// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiDirectory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
