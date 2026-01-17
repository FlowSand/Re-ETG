// Decompiled with JetBrains decompiler
// Type: AnchorStyleExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Systems.Utilities
{
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

}
