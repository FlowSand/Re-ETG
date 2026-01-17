// Decompiled with JetBrains decompiler
// Type: LichDeathController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class LichDeathController : BraveBehaviour
{
  public GameObject HellDragVFX;
  public ScreenShakeSettings hellDragScreenShake;
  public ScreenShakeSettings dualLichShake1;
  public ScreenShakeSettings dualLichShake2;
  public GameObject explosionVfx;
  private float explosionMidDelay = 0.1f;
  private int explosionCount = 55;
  public GameObject bigExplosionVfx;
  private float bigExplosionMidDelay = 0.2f;
  private int bigExplosionCount = 15;
  private MegalichDeathController m_megalich;
  private bool m_challengesSuppressed;

  public bool IsDoubleFinalDeath { get; set; }

  [DebuggerHidden]
  public IEnumerator Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new LichDeathController.\u003CStart\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  public IEnumerator LateStart()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new LichDeathController.\u003CLateStart\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  protected override void OnDestroy()
  {
    if (ChallengeManager.CHALLENGE_MODE_ACTIVE && this.m_challengesSuppressed)
    {
      ChallengeManager.Instance.SuppressChallengeStart = false;
      this.m_challengesSuppressed = false;
    }
    base.OnDestroy();
  }

  private void OnBossDeath(Vector2 dir)
  {
    if (LichIntroDoer.DoubleLich)
    {
      this.IsDoubleFinalDeath = true;
      foreach (AIActor activeEnemy in this.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
      {
        if (activeEnemy.healthHaver.IsBoss && !activeEnemy.healthHaver.IsSubboss && (Object) activeEnemy != (Object) this.aiActor && activeEnemy.healthHaver.IsAlive)
        {
          this.IsDoubleFinalDeath = false;
          break;
        }
      }
    }
    if (!LichIntroDoer.DoubleLich || this.IsDoubleFinalDeath)
    {
      int num = (int) AkSoundEngine.PostEvent("Play_MUS_Lich_Transition_01", GameManager.Instance.gameObject);
    }
    if (this.IsDoubleFinalDeath)
    {
      this.aiAnimator.PlayUntilCancelled("death_real", true);
      this.healthHaver.OverrideKillCamTime = new float?(11.5f);
      GameManager.Instance.StartCoroutine(this.HandleDoubleLichPostDeathCR());
      GameManager.Instance.StartCoroutine(this.HandleDoubleLichExtraExplosionsCR());
    }
    else
    {
      this.aiAnimator.PlayUntilCancelled("death", true);
      GameManager.Instance.StartCoroutine(this.HandlePostDeathCR());
    }
  }

  [DebuggerHidden]
  private IEnumerator HandlePostDeathCR()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new LichDeathController.\u003CHandlePostDeathCR\u003Ec__Iterator2()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator HandleDoubleLichExtraExplosionsCR()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new LichDeathController.\u003CHandleDoubleLichExtraExplosionsCR\u003Ec__Iterator3()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator HandleDoubleLichPostDeathCR()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new LichDeathController.\u003CHandleDoubleLichPostDeathCR\u003Ec__Iterator4()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator HandlePastBeingShot()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    LichDeathController.\u003CHandlePastBeingShot\u003Ec__Iterator5 beingShotCIterator5 = new LichDeathController.\u003CHandlePastBeingShot\u003Ec__Iterator5();
    return (IEnumerator) beingShotCIterator5;
  }

  [DebuggerHidden]
  private IEnumerator HandleSplashBody(
    PlayerController sourcePlayer,
    bool isPrimaryPlayer,
    TitleDioramaController diorama)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new LichDeathController.\u003CHandleSplashBody\u003Ec__Iterator6()
    {
      sourcePlayer = sourcePlayer,
      diorama = diorama
    };
  }
}
