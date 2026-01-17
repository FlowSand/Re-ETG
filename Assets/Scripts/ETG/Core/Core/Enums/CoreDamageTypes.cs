// Decompiled with JetBrains decompiler
// Type: CoreDamageTypes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Core.Enums
{
    [Flags]
    public enum CoreDamageTypes
    {
      None = 0,
      Void = 1,
      Magic = 2,
      Fire = 4,
      Ice = 8,
      Poison = 16, // 0x00000010
      Water = 32, // 0x00000020
      Electric = 64, // 0x00000040
      SpecialBossDamage = 128, // 0x00000080
    }

}
