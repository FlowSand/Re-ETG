#nullable disable

  public static class dfSpriteFlipExtensions
  {
    public static bool IsSet(this dfSpriteFlip value, dfSpriteFlip flag) => flag == (value & flag);

    public static dfSpriteFlip SetFlag(this dfSpriteFlip value, dfSpriteFlip flag, bool on)
    {
      return on ? value | flag : value & ~flag;
    }
  }

