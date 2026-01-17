// Decompiled with JetBrains decompiler
// Type: InControl.VersionInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Text.RegularExpressions;
using UnityEngine;

#nullable disable
namespace InControl
{
  public struct VersionInfo(int major, int minor, int patch, int build) : IComparable<VersionInfo>
  {
    public int Major = major;
    public int Minor = minor;
    public int Patch = patch;
    public int Build = build;

    public static VersionInfo InControlVersion()
    {
      return new VersionInfo()
      {
        Major = 1,
        Minor = 6,
        Patch = 17,
        Build = 9143
      };
    }

    public static VersionInfo UnityVersion()
    {
      Match match = Regex.Match(Application.unityVersion, "^(\\d+)\\.(\\d+)\\.(\\d+)");
      int num = 0;
      return new VersionInfo()
      {
        Major = Convert.ToInt32(match.Groups[1].Value),
        Minor = Convert.ToInt32(match.Groups[2].Value),
        Patch = Convert.ToInt32(match.Groups[3].Value),
        Build = num
      };
    }

    public static VersionInfo Min
    {
      get => new VersionInfo(int.MinValue, int.MinValue, int.MinValue, int.MinValue);
    }

    public static VersionInfo Max
    {
      get => new VersionInfo(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
    }

    public int CompareTo(VersionInfo other)
    {
      if (this.Major < other.Major)
        return -1;
      if (this.Major > other.Major)
        return 1;
      if (this.Minor < other.Minor)
        return -1;
      if (this.Minor > other.Minor)
        return 1;
      if (this.Patch < other.Patch)
        return -1;
      if (this.Patch > other.Patch)
        return 1;
      if (this.Build < other.Build)
        return -1;
      return this.Build > other.Build ? 1 : 0;
    }

    public static bool operator ==(VersionInfo a, VersionInfo b) => a.CompareTo(b) == 0;

    public static bool operator !=(VersionInfo a, VersionInfo b) => a.CompareTo(b) != 0;

    public static bool operator <=(VersionInfo a, VersionInfo b) => a.CompareTo(b) <= 0;

    public static bool operator >=(VersionInfo a, VersionInfo b) => a.CompareTo(b) >= 0;

    public static bool operator <(VersionInfo a, VersionInfo b) => a.CompareTo(b) < 0;

    public static bool operator >(VersionInfo a, VersionInfo b) => a.CompareTo(b) > 0;

    public override bool Equals(object other)
    {
      return other is VersionInfo versionInfo && this == versionInfo;
    }

    public override int GetHashCode()
    {
      return this.Major.GetHashCode() ^ this.Minor.GetHashCode() ^ this.Patch.GetHashCode() ^ this.Build.GetHashCode();
    }

    public override string ToString()
    {
      if (this.Build == 0)
        return $"{this.Major}.{this.Minor}.{this.Patch}";
      return $"{this.Major}.{this.Minor}.{this.Patch} build {this.Build}";
    }

    public string ToShortString()
    {
      if (this.Build == 0)
        return $"{this.Major}.{this.Minor}.{this.Patch}";
      return $"{this.Major}.{this.Minor}.{this.Patch}b{this.Build}";
    }
  }
}
