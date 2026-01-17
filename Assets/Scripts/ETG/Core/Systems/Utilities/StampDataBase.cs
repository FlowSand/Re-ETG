// Decompiled with JetBrains decompiler
// Type: StampDataBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public abstract class StampDataBase
    {
      public int width = 1;
      public int height = 1;
      public float relativeWeight = 1f;
      public DungeonTileStampData.StampPlacementRule placementRule;
      public DungeonTileStampData.StampSpace occupySpace;
      public DungeonTileStampData.StampCategory stampCategory;
      public int preferredIntermediaryStamps;
      public DungeonTileStampData.IntermediaryMatchingStyle intermediaryMatchingStyle;
      public bool requiresForcedMatchingStyle;
      public Opulence opulence;
      public List<StampPerRoomPlacementSettings> roomTypeData;
      public int indexOfSymmetricPartner = -1;
      public bool preventRoomRepeats;

      public float GetRelativeWeight(int roomSubType)
      {
        for (int index = 0; index < this.roomTypeData.Count; ++index)
        {
          if (this.roomTypeData[index].roomSubType == roomSubType)
            return this.roomTypeData[index].roomRelativeWeight;
        }
        return this.relativeWeight;
      }
    }

}
