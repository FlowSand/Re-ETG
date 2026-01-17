// Decompiled with JetBrains decompiler
// Type: TertiaryBossRewardSet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class TertiaryBossRewardSet
    {
      public string annotation = "reward";
      public float weight = 1f;
      [PickupIdentifier]
      public List<int> dropIds;
    }

}
