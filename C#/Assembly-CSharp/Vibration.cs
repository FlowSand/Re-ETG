// Decompiled with JetBrains decompiler
// Type: Vibration
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public static class Vibration
{
  public static float ConvertFromShakeMagnitude(float magnitude)
  {
    return (double) magnitude < 0.0099999997764825821 ? 0.0f : (float) (0.40000000596046448 + (double) Mathf.InverseLerp(0.0f, 1f, magnitude) * 0.60000002384185791);
  }

  public static float ConvertTime(Vibration.Time time)
  {
    switch (time)
    {
      case Vibration.Time.Instant:
        return 0.0f;
      case Vibration.Time.Quick:
        return 0.15f;
      case Vibration.Time.Normal:
        return 0.25f;
      case Vibration.Time.Slow:
        return 0.5f;
      default:
        return 0.0f;
    }
  }

  public static float ConvertStrength(Vibration.Strength strength)
  {
    switch (strength)
    {
      case Vibration.Strength.UltraLight:
        return 0.2f;
      case Vibration.Strength.Light:
        return 0.4f;
      case Vibration.Strength.Medium:
        return 0.7f;
      case Vibration.Strength.Hard:
        return 1f;
      default:
        return 0.5f;
    }
  }

  public enum Time
  {
    Instant = 5,
    Quick = 10, // 0x0000000A
    Normal = 20, // 0x00000014
    Slow = 30, // 0x0000001E
  }

  public enum Strength
  {
    UltraLight = 5,
    Light = 10, // 0x0000000A
    Medium = 20, // 0x00000014
    Hard = 30, // 0x0000001E
  }
}
