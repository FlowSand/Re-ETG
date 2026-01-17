// Decompiled with JetBrains decompiler
// Type: DemonWallDeathController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class DemonWallDeathController : BraveBehaviour
    {
      public GameObject deathEyes;
      public GameObject deathOil;
      private bool m_isDying;

      public void Start()
      {
        this.healthHaver.ManualDeathHandling = true;
        this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
        this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void OnBossDeath(Vector2 dir)
      {
        IntVector2 intVector2_1 = (this.specRigidbody.HitboxPixelCollider.UnitBottomLeft + new Vector2(0.0f, -1f)).ToIntVector2(VectorConversions.Floor);
        IntVector2 intVector2_2 = this.specRigidbody.HitboxPixelCollider.UnitTopRight.ToIntVector2(VectorConversions.Ceil);
        DungeonData data = GameManager.Instance.Dungeon.data;
        for (int x = intVector2_1.x; x <= intVector2_2.x; ++x)
        {
          if (x != (intVector2_2.x + intVector2_1.x) / 2 && x != (intVector2_2.x + intVector2_1.x) / 2 - 1)
          {
            for (int y = intVector2_1.y; y <= intVector2_2.y; ++y)
            {
              if (data.CheckInBoundsAndValid(new IntVector2(x, y)))
              {
                CellData cellData = data[x, y];
                if (cellData.type == CellType.FLOOR)
                  cellData.isOccupied = true;
              }
            }
          }
        }
        this.aiActor.ParentRoom.OverrideBossPedestalLocation = new IntVector2?(this.specRigidbody.UnitCenter.ToIntVector2() + new IntVector2(-1, 7));
        this.StartCoroutine(this.OnDeathAnimationCR());
      }

      [DebuggerHidden]
      private IEnumerator OnDeathAnimationCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DemonWallDeathController.<OnDeathAnimationCR>c__Iterator0()
        {
          $this = this
        };
      }

      private void AnimationEventTriggered(
        tk2dSpriteAnimator spriteAnimator,
        tk2dSpriteAnimationClip clip,
        int frame)
      {
        if (!this.m_isDying || !(clip.GetFrame(frame).eventInfo == "oil"))
          return;
        this.deathOil.SetActive(true);
        this.deathOil.GetComponent<tk2dSpriteAnimator>().Play();
      }
    }

}
