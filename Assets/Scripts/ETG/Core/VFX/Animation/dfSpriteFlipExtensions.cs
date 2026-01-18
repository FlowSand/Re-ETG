// Decompiled with JetBrains decompiler
// Type: dfSpriteFlipExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

  public static class dfSpriteFlipExtensions
  {
    public static bool IsSet(this dfSpriteFlip value, dfSpriteFlip flag) => flag == (value & flag);

    public static dfSpriteFlip SetFlag(this dfSpriteFlip value, dfSpriteFlip flag, bool on)
    {
      return on ? value | flag : value & ~flag;
    }
  }

