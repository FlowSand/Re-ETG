// Decompiled with JetBrains decompiler
// Type: ExplodeOnDeath
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class ExplodeOnDeath : OnDeathBehavior
    {
      public ExplosionData explosionData;
      public bool immuneToIBombApp;
      public bool LinearChainExplosion;
      public float ChainDuration = 1f;
      public float ChainDistance = 10f;
      public int ChainNumExplosions = 5;
      public bool ChainIsReversed;
      public GameObject ChainTargetSprite;
      public ExplosionData LinearChainExplosionData;

      protected override void OnDestroy() => base.OnDestroy();

      protected override void OnTrigger(Vector2 dirVec)
      {
        if (!this.enabled)
          return;
        Exploder.Explode((Vector3) this.specRigidbody.GetUnitCenter(ColliderType.HitBox), this.explosionData, Vector2.zero);
        if (!this.LinearChainExplosion)
          return;
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleChainExplosion());
      }

      [DebuggerHidden]
      public IEnumerator HandleChainExplosion()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ExplodeOnDeath.<HandleChainExplosion>c__Iterator0()
        {
          _this = this
        };
      }

      private bool ValidExplosionPosition(Vector2 pos)
      {
        IntVector2 intVector2 = pos.ToIntVector2(VectorConversions.Floor);
        return GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2) && GameManager.Instance.Dungeon.data[intVector2].type != CellType.WALL;
      }
    }

}
