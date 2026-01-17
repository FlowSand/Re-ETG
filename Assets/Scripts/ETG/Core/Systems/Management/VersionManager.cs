// Decompiled with JetBrains decompiler
// Type: VersionManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.IO;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Management
{
    public class VersionManager
    {
      private static bool s_initialized;
      private static string s_displayVersionNumber = string.Empty;
      private static string s_realVersionNumber;

      public static string DisplayVersionNumber
      {
        get
        {
          if (!VersionManager.s_initialized)
            VersionManager.Initialize();
          return VersionManager.s_displayVersionNumber;
        }
      }

      public static string UniqueVersionNumber
      {
        get
        {
          if (!VersionManager.s_initialized)
            VersionManager.Initialize();
          return VersionManager.s_realVersionNumber ?? VersionManager.s_displayVersionNumber;
        }
      }

      private static void Initialize()
      {
        try
        {
          string path = Path.Combine(Application.streamingAssetsPath, "version.txt");
          if (File.Exists(path))
          {
            string[] strArray = File.ReadAllLines(path);
            if (strArray.Length > 0)
            {
              VersionManager.s_initialized = true;
              VersionManager.s_displayVersionNumber = strArray[0];
              VersionManager.s_realVersionNumber = strArray.Length <= 1 ? (string) null : strArray[1];
              return;
            }
          }
        }
        catch
        {
        }
        VersionManager.s_initialized = true;
        VersionManager.s_displayVersionNumber = string.Empty;
        VersionManager.s_realVersionNumber = (string) null;
      }
    }

}
