// Decompiled with JetBrains decompiler
// Type: FuturegeonGroundLightDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class FuturegeonGroundLightDoer : MonoBehaviour
    {
      public GameObject lightNorthVFX;
      public GameObject lightEastVFX;
      public GameObject lightSouthVFX;
      public GameObject lightWestVFX;
      public int maxActiveLightTrails = 5;
      private int numActiveLightTrails;

      private void Start() => this.StartCoroutine(this.HandleLightningTrails());

      [DebuggerHidden]
      private IEnumerator HandleAllLightTrails()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FuturegeonGroundLightDoer__HandleAllLightTrailsc__Iterator0()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleLightningTrails()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FuturegeonGroundLightDoer__HandleLightningTrailsc__Iterator1()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleLightingLightTrail(
        DungeonData.Direction startDir,
        IntVector2 startPos,
        bool isBranch = false)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FuturegeonGroundLightDoer__HandleLightingLightTrailc__Iterator2()
        {
          startPos = startPos,
          startDir = startDir,
          isBranch = isBranch,
          _this = this
        };
      }

      private GameObject InstantiateVFXFromDirection(DungeonData.Direction dir)
      {
        GameObject prefab = (GameObject) null;
        switch (dir)
        {
          case DungeonData.Direction.NORTH:
            prefab = this.lightNorthVFX;
            break;
          case DungeonData.Direction.EAST:
            prefab = this.lightEastVFX;
            break;
          case DungeonData.Direction.SOUTH:
            prefab = this.lightSouthVFX;
            break;
          case DungeonData.Direction.WEST:
            prefab = this.lightWestVFX;
            break;
        }
        return SpawnManager.SpawnVFX(prefab);
      }

      [DebuggerHidden]
      private IEnumerator HandleSingleLightTrail()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FuturegeonGroundLightDoer__HandleSingleLightTrailc__Iterator3()
        {
          _this = this
        };
      }
    }

}
