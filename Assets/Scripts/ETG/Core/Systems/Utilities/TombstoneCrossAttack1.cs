// Decompiled with JetBrains decompiler
// Type: TombstoneCrossAttack1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class TombstoneCrossAttack1 : Script
  {
    private const int BulletSpeed = 10;
    private const float GapDist = 0.7f;

    protected override IEnumerator Top()
    {
      float aimDirection = this.GetAimDirection((double) Random.value >= 0.25 ? 0.0f : 1f, 10f);
      this.Fire(new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(10f), (Bullet) new TombstoneCrossAttack1.CrossBullet(new Vector2(0.7f, 0.0f), 0, 20));
      this.Fire(new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(10f), (Bullet) new TombstoneCrossAttack1.CrossBullet(new Vector2(0.0f, 0.0f), 0, 20));
      this.Fire(new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(10f), (Bullet) new TombstoneCrossAttack1.CrossBullet(new Vector2(-0.7f, 0.0f), 0, 20));
      this.Fire(new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(10f), (Bullet) new TombstoneCrossAttack1.CrossBullet(new Vector2(-1.4f, 0.0f), 0, 20));
      this.Fire(new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(10f), (Bullet) new TombstoneCrossAttack1.CrossBullet(new Vector2(0.0f, 0.7f), 18, 15));
      this.Fire(new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(10f), (Bullet) new TombstoneCrossAttack1.CrossBullet(new Vector2(0.0f, -0.7f), 18, 15));
      return (IEnumerator) null;
    }

    public class CrossBullet : Bullet
    {
      private Vector2 m_offset;
      private int m_setupDelay;
      private int m_setupTime;

      public CrossBullet(Vector2 offset, int setupDelay, int setupTime)
        : base()
      {
        this.m_offset = offset;
        this.m_setupDelay = setupDelay;
        this.m_setupTime = setupTime;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TombstoneCrossAttack1.CrossBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

