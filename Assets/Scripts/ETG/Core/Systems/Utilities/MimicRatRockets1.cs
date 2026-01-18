// Decompiled with JetBrains decompiler
// Type: MimicRatRockets1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class MimicRatRockets1 : Script
  {
    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MimicRatRockets1__Topc__Iterator0()
      {
        _this = this
      };
    }

    private void FireRocket(Vector2 start, int xOffset, int yOffset)
    {
      for (int index = 0; index < 3; ++index)
      {
        if (index != 1 || !BraveUtility.RandomBool())
        {
          Vector2 target = (!BraveUtility.RandomBool() ? this.GetPredictedTargetPosition(1f, 14f) : this.BulletManager.PlayerPosition()) + BraveMathCollege.DegreesToVector(this.RandomAngle(), Random.Range(0.0f, 2.5f));
          float offsetAngle = (float) (index - 1) * Random.Range(25f, 90f);
          this.Fire(Offset.OverridePosition(start + new Vector2((float) xOffset / 16f, (float) yOffset / 16f)), new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(Random.Range(10f, 11f)), (Bullet) new MimicRatRockets1.ArcBullet(target, offsetAngle));
        }
      }
    }

    public class ArcBullet : Bullet
    {
      private Vector2 m_target;
      private float m_offsetAngle;

      public ArcBullet(Vector2 target, float offsetAngle)
        : base()
      {
        this.m_target = target;
        this.m_offsetAngle = offsetAngle;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MimicRatRockets1.ArcBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

