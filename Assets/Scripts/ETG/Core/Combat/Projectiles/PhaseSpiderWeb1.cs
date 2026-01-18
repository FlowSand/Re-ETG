// Decompiled with JetBrains decompiler
// Type: PhaseSpiderWeb1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class PhaseSpiderWeb1 : Script
  {
    private const int NumWaves = 7;
    private const int BulletsPerWave = 13;
    private const float WebDegrees = 120f;
    private const float BulletSpeed = 9f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new PhaseSpiderWeb1__Topc__Iterator0()
      {
        _this = this
      };
    }

    private class WebBullet : Bullet
    {
      private int m_delayFrames;
      private bool m_spawnGoop;

      public WebBullet(int delayFrames, bool spawnGoop = false)
        : base(!spawnGoop ? "default" : "web")
      {
        this.m_delayFrames = delayFrames;
        this.m_spawnGoop = spawnGoop;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PhaseSpiderWeb1.WebBullet__Topc__Iterator0()
        {
          _this = this
        };
      }

      public override void OnBulletDestruction(
        Bullet.DestroyType destroyType,
        SpeculativeRigidbody hitRigidbody,
        bool preventSpawningProjectiles)
      {
        if (this.m_spawnGoop && destroyType != Bullet.DestroyType.DieInAir && (bool) (Object) this.BulletBank)
          DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.BulletBank.GetComponent<GoopDoer>().goopDefinition).AddGoopCircle(this.Position, 1.5f);
        base.OnBulletDestruction(destroyType, hitRigidbody, preventSpawningProjectiles);
      }
    }
  }

