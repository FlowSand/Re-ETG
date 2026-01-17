// Decompiled with JetBrains decompiler
// Type: dfAnchorStyle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Core.Enums
{
    [Flags]
    public enum dfAnchorStyle
    {
      Top = 1,
      Bottom = 2,
      Left = 4,
      Right = 8,
      All = Right | Left | Bottom | Top, // 0x0000000F
      CenterHorizontal = 64, // 0x00000040
      CenterVertical = 128, // 0x00000080
      Proportional = 256, // 0x00000100
      None = 0,
    }

}
