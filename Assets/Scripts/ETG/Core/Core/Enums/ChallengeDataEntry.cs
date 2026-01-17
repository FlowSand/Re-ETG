// Decompiled with JetBrains decompiler
// Type: ChallengeDataEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable

namespace ETG.Core.Core.Enums
{
    [Serializable]
    public class ChallengeDataEntry
    {
      public string Annotation;
      public ChallengeModifier challenge;
      [EnumFlags]
      public GlobalDungeonData.ValidTilesets excludedTilesets;
      public List<GlobalDungeonData.ValidTilesets> tilesetsWithCustomValues;
      public List<int> CustomValues;

      public int GetWeightForFloor(GlobalDungeonData.ValidTilesets tileset)
      {
        return this.tilesetsWithCustomValues.Contains(tileset) ? this.CustomValues[this.tilesetsWithCustomValues.IndexOf(tileset)] : 1;
      }
    }

}
