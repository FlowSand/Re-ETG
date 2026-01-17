// Decompiled with JetBrains decompiler
// Type: tk2dTileFlags
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Core.Enums
{
    [Flags]
    public enum tk2dTileFlags
    {
      None = 0,
      FlipX = 16777216, // 0x01000000
      FlipY = 33554432, // 0x02000000
      Rot90 = 67108864, // 0x04000000
    }

}
