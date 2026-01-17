// Decompiled with JetBrains decompiler
// Type: dfStringExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public static class dfStringExtensions
    {
      public static string MakeRelativePath(this string path)
      {
        return string.IsNullOrEmpty(path) ? string.Empty : path.Substring(path.IndexOf("Assets/", StringComparison.OrdinalIgnoreCase));
      }

      public static bool Contains(this string value, string pattern, bool caseInsensitive)
      {
        return caseInsensitive ? value.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) != -1 : value.IndexOf(pattern) != -1;
      }
    }

}
