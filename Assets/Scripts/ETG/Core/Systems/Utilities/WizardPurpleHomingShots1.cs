// Decompiled with JetBrains decompiler
// Type: WizardPurpleHomingShots1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class WizardPurpleHomingShots1 : Script
  {
    private const int NumBullets = 3;
    private const int Delay = 45;
    private const int AirTime = 300;
    private int[] m_bullets = new int[4]{ 0, 1, 2, 3 };

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new WizardPurpleHomingShots1__Topc__Iterator0()
      {
        _this = this
      };
    }

    protected static bool HasLostTarget(Bullet bullet)
    {
      AIActor aiActor = bullet.BulletBank.aiActor;
      return (bool) (UnityEngine.Object) aiActor && !(bool) (UnityEngine.Object) aiActor.TargetRigidbody && aiActor.CanTargetEnemies && !aiActor.CanTargetPlayers;
    }

    public class StoryBullet : Bullet
    {
      public float horizontalOffset;

      public StoryBullet(string name, float horizontalOffset)
        : base(name)
      {
        this.horizontalOffset = horizontalOffset;
      }

      public float OffsetAimDirection
      {
        get
        {
          Vector2 vector2_1 = this.BulletManager.PlayerPosition();
          Vector2 position = this.Position;
          Vector2 v = vector2_1 - position;
          Vector2 vector2_2 = (double) v.magnitude >= (double) Mathf.Abs(this.horizontalOffset) * 2.0 ? v.Rotate(90f).normalized * this.horizontalOffset : Vector2.zero;
          return (vector2_1 + vector2_2 - position).ToAngle();
        }
      }

      public bool HasLostTarget() => WizardPurpleHomingShots1.HasLostTarget((Bullet) this);
    }

    public class KnightBullet : WizardPurpleHomingShots1.StoryBullet
    {
      public KnightBullet()
        : base("knight", 1.5f)
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new WizardPurpleHomingShots1.KnightBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

    public class MageBullet : WizardPurpleHomingShots1.StoryBullet
    {
      private const int ShotCooldown = 60;
      private const int NumBullets = 3;

      public MageBullet()
        : base("mage", 0.5f)
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new WizardPurpleHomingShots1.MageBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

    public class BardBullet : WizardPurpleHomingShots1.StoryBullet
    {
      private const int NoTurnTime = 60;
      private BounceProjModifier m_bounceMod;
      private int m_noTurnTimer;

      public BardBullet()
        : base("bard", -1f)
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new WizardPurpleHomingShots1.BardBullet__Topc__Iterator0()
        {
          _this = this
        };
      }

      public override void OnBulletDestruction(
        Bullet.DestroyType destroyType,
        SpeculativeRigidbody hitRigidbody,
        bool preventSpawningProjectiles)
      {
        if (!(bool) (UnityEngine.Object) this.m_bounceMod)
          return;
        this.m_bounceMod.OnBounce -= new System.Action(this.OnBounce);
      }

      private void OnBounce() => this.m_noTurnTimer = 60;
    }

    public class RogueBullet : WizardPurpleHomingShots1.StoryBullet
    {
      private const int ClampedLifetime = 80 /*0x50*/;
      private TeleportProjModifier m_teleportMod;
      private bool m_clampLifetime;

      public RogueBullet()
        : base("rogue", -2f)
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new WizardPurpleHomingShots1.RogueBullet__Topc__Iterator0()
        {
          _this = this
        };
      }

      public override void OnBulletDestruction(
        Bullet.DestroyType destroyType,
        SpeculativeRigidbody hitRigidbody,
        bool preventSpawningProjectiles)
      {
        if (!(bool) (UnityEngine.Object) this.m_teleportMod)
          return;
        this.m_teleportMod.OnTeleport -= new System.Action(this.OnTeleport);
      }

      private void OnTeleport() => this.m_clampLifetime = true;
    }
  }

