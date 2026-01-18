// Decompiled with JetBrains decompiler
// Type: SpecificIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

  public abstract class SpecificIntroDoer : BraveBehaviour
  {
    public virtual IntVector2 OverrideExitBasePosition(
      DungeonData.Direction directionToWalk,
      IntVector2 exitBaseCenter)
    {
      return exitBaseCenter;
    }

    public virtual Vector2? OverrideIntroPosition => new Vector2?();

    public virtual Vector2? OverrideOutroPosition => new Vector2?();

    public virtual string OverrideBossMusicEvent => (string) null;

    public virtual void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
    {
    }

    public virtual void OnCameraIntro()
    {
    }

    public virtual void StartIntro(List<tk2dSpriteAnimator> animators)
    {
    }

    public virtual bool IsIntroFinished => true;

    public virtual void OnBossCard()
    {
    }

    public virtual void OnCameraOutro()
    {
    }

    public virtual void OnCleanup()
    {
    }

    public virtual void EndIntro()
    {
    }

[DebuggerHidden]
    public IEnumerator TimeInvariantWait(float duration)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new SpecificIntroDoer__TimeInvariantWaitc__Iterator0()
      {
        duration = duration
      };
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

