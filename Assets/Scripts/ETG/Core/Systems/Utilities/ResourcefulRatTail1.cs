// Decompiled with JetBrains decompiler
// Type: ResourcefulRatTail1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/ResourcefulRat/Tail1")]
public class ResourcefulRatTail1 : Script
  {
    public const int NumBullets = 33;
    public const int SpawnDelay = 3;
    public const float RotationSpeed = -360f;
    public const int RotationTime = 266;
    public const int FlashTime = 45;

    public bool ShouldTell { get; set; }

    public bool Done { get; set; }

    public float FireAngle { get; set; }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ResourcefulRatTail1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public Vector2 GetPosition(int index, int tick)
    {
      float a = this.Tick > 120 ? (float) ((double) (tick - 120) / 60.0 * -360.0 - 450.0) : (float) (-90.0 * ((double) tick / 60.0) * ((double) tick / 60.0) - 90.0);
      float num1 = BraveMathCollege.AbsAngleBetween(a, -90f);
      float num2 = Mathf.Lerp(0.5f, 0.75f, num1 / 70f);
      float magnitude = (float) (1.0 + (double) index * (double) num2);
      float num3 = (float) index * Mathf.Lerp(0.0f, 3f, num1 / 120f);
      return BraveMathCollege.DegreesToVector(a + num3, magnitude) + this.Position;
    }

    public class TailBullet : Bullet
    {
      private ResourcefulRatTail1 m_parentScript;
      private int m_index;
      private int m_spawnCountdown = -1;

      public TailBullet(ResourcefulRatTail1 parentScript, int index)
        : base("tail", true)
      {
        this.m_parentScript = parentScript;
        this.m_index = index;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ResourcefulRatTail1.TailBullet__Topc__Iterator0()
        {
          _this = this
        };
      }

      public void SpawnBullet()
      {
        if (this.m_spawnCountdown >= 0 || !(bool) (Object) this.Projectile)
        {
          UnityEngine.Debug.Log((object) "skipped");
        }
        else
        {
          UnityEngine.Debug.Log((object) this.Projectile);
          UnityEngine.Debug.Log((object) this.Projectile.sprite);
          UnityEngine.Debug.Log((object) this.Projectile.sprite.spriteAnimator);
          this.m_spawnCountdown = 45;
          this.Projectile.sprite.spriteAnimator.Play();
          UnityEngine.Debug.LogWarning((object) "STARTING SOME SHIT");
        }
      }
    }

    public class SubtailBullet : Bullet
    {
      private ResourcefulRatTail1 m_parentScript;

      public SubtailBullet(ResourcefulRatTail1 parentScript)
        : base(suppressVfx: true)
      {
        this.m_parentScript = parentScript;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ResourcefulRatTail1.SubtailBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

