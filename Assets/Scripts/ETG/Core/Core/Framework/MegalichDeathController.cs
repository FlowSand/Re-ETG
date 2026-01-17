// Decompiled with JetBrains decompiler
// Type: MegalichDeathController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class MegalichDeathController : BraveBehaviour
    {
      public List<GameObject> explosionVfx;
      public float explosionMidDelay = 0.3f;
      public int explosionCount = 10;
      public GameObject shellCasing;
      private InfinilichDeathController m_infinilich;
      private bool m_challengesSuppressed;

      [DebuggerHidden]
      public IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MegalichDeathController.\u003CStart\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      [DebuggerHidden]
      public IEnumerator LateStart()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MegalichDeathController.\u003CLateStart\u003Ec__Iterator1()
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
        this.aiAnimator.PlayUntilCancelled("death");
        this.aiAnimator.StopVfx("double_pound");
        this.aiAnimator.StopVfx("left_pound");
        this.aiAnimator.StopVfx("right_pound");
        GameManager.Instance.StartCoroutine(this.OnDeathExplosionsCR());
        GameManager.Instance.StartCoroutine(this.OnDeathCR());
      }

      [DebuggerHidden]
      private IEnumerator OnDeathExplosionsCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MegalichDeathController.\u003COnDeathExplosionsCR\u003Ec__Iterator2()
        {
          \u0024this = this
        };
      }

      private bool IsAnyPlayerFalling()
      {
        foreach (PlayerController allPlayer in GameManager.Instance.AllPlayers)
        {
          if ((bool) (Object) allPlayer && allPlayer.healthHaver.IsAlive && allPlayer.IsFalling)
            return true;
        }
        return false;
      }

      [DebuggerHidden]
      private IEnumerator OnDeathCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MegalichDeathController.\u003COnDeathCR\u003Ec__Iterator3()
        {
          \u0024this = this
        };
      }

      private void SpawnShellCasingAtPosition(Vector3 position)
      {
        if (!((Object) this.shellCasing != (Object) null))
          return;
        float num = Random.Range(-100f, -80f);
        GameObject gameObject = SpawnManager.SpawnDebris(this.shellCasing, position, Quaternion.Euler(0.0f, 0.0f, num));
        ShellCasing component1 = gameObject.GetComponent<ShellCasing>();
        if ((Object) component1 != (Object) null)
          component1.Trigger();
        DebrisObject component2 = gameObject.GetComponent<DebrisObject>();
        if (!((Object) component2 != (Object) null))
          return;
        Vector3 vector3Zup = BraveMathCollege.DegreesToVector(num, Random.Range(0.5f, 1f)).ToVector3ZUp((float) ((double) Random.value * 1.5 + 1.0));
        component2.Trigger(vector3Zup, Random.Range(8f, 10f));
      }
    }

}
