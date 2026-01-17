// Decompiled with JetBrains decompiler
// Type: FacewallIndexGridDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class FacewallIndexGridDefinition
    {
      public TileIndexGrid grid;
      public int minWidth = 3;
      public int maxWidth = 10;
      [Header("Intermediary Tiles")]
      public bool hasIntermediaries;
      public int minIntermediaryBuffer = 4;
      public int maxIntermediaryBuffer = 20;
      public int minIntermediaryLength = 1;
      public int maxIntermediaryLength = 1;
      [Header("Options")]
      public bool topsMatchBottoms;
      public bool middleSectionSequential;
      public bool canExistInCorners = true;
      public bool forceEdgesInCorners;
      public bool canAcceptWallDecoration;
      public bool canAcceptFloorDecoration = true;
      public DungeonTileStampData.IntermediaryMatchingStyle forcedStampMatchingStyle;
      public bool canBePlacedInExits;
      public float chanceToPlaceIfPossible = 0.1f;
      public float perTileFailureRate = 0.05f;
    }

}
