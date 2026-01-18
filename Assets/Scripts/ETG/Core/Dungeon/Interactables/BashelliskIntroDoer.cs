// Decompiled with JetBrains decompiler
// Type: BashelliskIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class BashelliskIntroDoer : SpecificIntroDoer
  {
    private BashelliskIntroDoer.State m_state;
    private BashelliskHeadController m_head;

    private void Start() => this.m_head = this.GetComponent<BashelliskHeadController>();

    private void Update()
    {
    }

    protected override void OnDestroy() => base.OnDestroy();

    public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
    {
      int num = (int) AkSoundEngine.PostEvent("Play_BOSS_bashellisk_move_02", this.gameObject);
      this.m_head.aiAnimator.LockFacingDirection = true;
      this.m_head.aiAnimator.FacingDirection = -90f;
      this.m_head.aiAnimator.Update();
      animators.Add(this.m_head.spriteAnimator);
      this.GetComponent<GenericIntroDoer>().SkipFinalizeAnimation = true;
      this.StartCoroutine(this.PlayIntro());
    }

    public override void StartIntro(List<tk2dSpriteAnimator> animators)
    {
    }

    public override void EndIntro()
    {
      this.StopAllCoroutines();
      this.m_head.specRigidbody.Reinitialize();
      this.m_head.ReinitMovementDirection = true;
    }

    public override bool IsIntroFinished => this.m_state == BashelliskIntroDoer.State.Finished;

    public override void OnCleanup() => this.behaviorSpeculator.enabled = true;

    [DebuggerHidden]
    private IEnumerator PlayIntro()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BashelliskIntroDoer__PlayIntroc__Iterator0()
      {
        _this = this
      };
    }

    private enum State
    {
      Idle,
      Playing,
      Finished,
    }
  }

