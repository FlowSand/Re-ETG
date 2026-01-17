// Decompiled with JetBrains decompiler
// Type: WeightedRoom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class WeightedRoom
    {
      public PrototypeDungeonRoom room;
      public float weight;
      public bool limitedCopies;
      public int maxCopies = 1;
      public DungeonPrerequisite[] additionalPrerequisites;

      public bool CheckPrerequisites()
      {
        if (this.additionalPrerequisites == null || this.additionalPrerequisites.Length == 0)
          return true;
        for (int index = 0; index < this.additionalPrerequisites.Length; ++index)
        {
          if (!this.additionalPrerequisites[index].CheckConditionsFulfilled())
            return false;
        }
        return true;
      }
    }

}
