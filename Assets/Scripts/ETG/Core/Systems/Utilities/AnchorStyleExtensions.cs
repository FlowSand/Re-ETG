#nullable disable

  public static class AnchorStyleExtensions
  {
    public static bool IsFlagSet(this dfAnchorStyle value, dfAnchorStyle flag)
    {
      return flag == (value & flag);
    }

    public static bool IsAnyFlagSet(this dfAnchorStyle value, dfAnchorStyle flag)
    {
      return dfAnchorStyle.None != (value & flag);
    }

    public static dfAnchorStyle SetFlag(this dfAnchorStyle value, dfAnchorStyle flag) => value | flag;

    public static dfAnchorStyle SetFlag(this dfAnchorStyle value, dfAnchorStyle flag, bool on)
    {
      return on ? value | flag : value & ~flag;
    }
  }

