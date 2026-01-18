using System.IO;
using UnityEngine;

#nullable disable

public class AkBasePathGetter
  {
    public static string GetPlatformName()
    {
      string empty = string.Empty;
      return !string.IsNullOrEmpty(empty) ? empty : "Windows";
    }

    public static string GetPlatformBasePath()
    {
      string platformName = AkBasePathGetter.GetPlatformName();
      string path = Path.Combine(AkBasePathGetter.GetFullSoundBankPath(), platformName);
      AkBasePathGetter.FixSlashes(ref path);
      return path;
    }

    public static string GetFullSoundBankPath()
    {
      string path = Path.Combine(Application.streamingAssetsPath, AkInitializer.GetBasePath());
      AkBasePathGetter.FixSlashes(ref path);
      return path;
    }

    public static void FixSlashes(
      ref string path,
      char separatorChar,
      char badChar,
      bool addTrailingSlash)
    {
      if (string.IsNullOrEmpty(path))
        return;
      path = path.Trim().Replace(badChar, separatorChar).TrimStart('\\');
      if (!addTrailingSlash || path.EndsWith(separatorChar.ToString()))
        return;
      path += (string) (object) separatorChar;
    }

    public static void FixSlashes(ref string path)
    {
      char directorySeparatorChar = Path.DirectorySeparatorChar;
      char badChar = directorySeparatorChar != '\\' ? '\\' : '/';
      AkBasePathGetter.FixSlashes(ref path, directorySeparatorChar, badChar, true);
    }

    public static string GetSoundbankBasePath()
    {
      string platformBasePath = AkBasePathGetter.GetPlatformBasePath();
      bool flag = true;
      if (!File.Exists(Path.Combine(platformBasePath, "Init.bnk")))
        flag = false;
      if (platformBasePath == string.Empty || !flag)
      {
        Debug.Log((object) ("WwiseUnity: Looking for SoundBanks in " + platformBasePath));
        Debug.LogError((object) "WwiseUnity: Could not locate the SoundBanks. Did you make sure to copy them to the StreamingAssets folder?");
      }
      return platformBasePath;
    }
  }

