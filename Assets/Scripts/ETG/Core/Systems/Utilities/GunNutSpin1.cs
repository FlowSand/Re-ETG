// Decompiled with JetBrains decompiler
// Type: GunNutSpin1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using Dungeonator;
using FullInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("GunNut/ChainSpin1")]
public class GunNutSpin1 : Script
  {
    public static string[] Transforms = new string[4]
    {
      "bullet hand",
      "bullet limb 1",
      "bullet limb 2",
      "bullet limb 3"
    };
    public const int NumBullets = 9;
    public const int BaseTurnSpeed = 540;
    public const float MaxDist = 6f;
    public const int ExtendTime = 30;
    public const int Lifetime = 120;
    public const int ContractTime = 45;
    public const int TellTime = 30;
    public const int BolasThrowTime = 120;
    public float TurnSpeed;
    public int TicksRemaining;
    private List<GunNutSpin1.SpinBullet> bullets;

    public bool IsTellingBolas { get; set; }

    public bool ShouldThrowBolas { get; set; }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GunNutSpin1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public override void OnForceEnded()
    {
      base.OnForceEnded();
      this.bullets = (List<GunNutSpin1.SpinBullet>) null;
      this.BulletBank.aiAnimator.ChildAnimator.OverrideIdleAnimation = (string) null;
      this.BulletBank.aiAnimator.ChildAnimator.OverrideMoveAnimation = (string) null;
      this.BulletBank.aiAnimator.ChildAnimator.renderer.enabled = false;
    }

    private bool CanThrowBolas()
    {
      return (!GameManager.HasInstance || GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.CHARACTER_PAST || GameManager.Instance.PrimaryPlayer.characterIdentity == PlayableCharacters.Bullet) && ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIMES_REACHED_CATACOMBS) > 0.0 || GameStatsManager.Instance.QueryEncounterable(this.BulletBank.encounterTrackable) >= 15) && (bool) (Object) this.BulletBank && (bool) (Object) this.BulletBank.aiActor && this.BulletBank.aiActor.ParentRoom != null && this.BulletBank.aiActor.ParentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.RoomClear) < 2;
    }

    public void StartBolasTell()
    {
      this.IsTellingBolas = true;
      this.PostWwiseEvent("Play_ENM_CannonArmor_Charge_01");
      for (int index = 0; index < this.bullets.Count; ++index)
      {
        GunNutSpin1.SpinBullet bullet = this.bullets[index];
        if (bullet != null && (bool) (Object) bullet.Projectile)
          bullet.Projectile.spriteAnimator.Play();
      }
      this.BulletBank.aiAnimator.ChildAnimator.renderer.enabled = true;
    }

    public void WasThrown()
    {
      this.IsTellingBolas = false;
      this.PostWwiseEvent("Play_OBJ_Chainpot_Drop_01");
      for (int index = this.bullets.Count - 1; index > 2; --index)
      {
        this.bullets[index].Vanish(true);
        this.bullets.RemoveAt(index);
      }
      for (int index = 0; index < this.bullets.Count; ++index)
      {
        GunNutSpin1.SpinBullet bullet = this.bullets[index];
        if (bullet != null && (bool) (Object) bullet.Projectile)
          bullet.Projectile.spriteAnimator.StopAndResetFrameToDefault();
      }
      this.BulletBank.aiAnimator.ChildAnimator.renderer.enabled = false;
    }

    private class SpinBullet : Bullet
    {
      private GunNutSpin1 m_parentScript;
      private float m_maxDist;
      private bool m_isBall;

      public SpinBullet(GunNutSpin1 parentScript, float maxDist, bool isBall)
        : base(!isBall ? "link" : "ball")
      {
        this.m_parentScript = parentScript;
        this.m_maxDist = maxDist;
        this.m_isBall = isBall;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GunNutSpin1.SpinBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

    public class BolasBullet : Bullet
    {
      public const float ExpandTime = 60f;
      public const float RotationTime = 60f;
      private float m_offset;

      public BolasBullet(bool isBall, float offset)
        : base(!isBall ? "link" : "ball_trail")
      {
        this.m_offset = offset;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GunNutSpin1.BolasBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

