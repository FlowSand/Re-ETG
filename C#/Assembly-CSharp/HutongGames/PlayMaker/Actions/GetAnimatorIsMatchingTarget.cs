// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorIsMatchingTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animator)]
[HutongGames.PlayMaker.Tooltip("Returns true if automatic matching is active. Can also send events")]
public class GetAnimatorIsMatchingTarget : FsmStateActionAnimatorBase
{
  [HutongGames.PlayMaker.Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
  [CheckForComponent(typeof (Animator))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("True if automatic matching is active")]
  [ActionSection("Results")]
  public FsmBool isMatchingActive;
  [HutongGames.PlayMaker.Tooltip("Event send if automatic matching is active")]
  public FsmEvent matchingActivatedEvent;
  [HutongGames.PlayMaker.Tooltip("Event send if automatic matching is not active")]
  public FsmEvent matchingDeactivedEvent;
  private Animator _animator;

  public override void Reset()
  {
    base.Reset();
    this.gameObject = (FsmOwnerDefault) null;
    this.isMatchingActive = (FsmBool) null;
    this.matchingActivatedEvent = (FsmEvent) null;
    this.matchingDeactivedEvent = (FsmEvent) null;
  }

  public override void OnEnter()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
    {
      this.Finish();
    }
    else
    {
      this._animator = ownerDefaultTarget.GetComponent<Animator>();
      if ((Object) this._animator == (Object) null)
      {
        this.Finish();
      }
      else
      {
        this.DoCheckIsMatchingActive();
        if (this.everyFrame)
          return;
        this.Finish();
      }
    }
  }

  public override void OnActionUpdate() => this.DoCheckIsMatchingActive();

  private void DoCheckIsMatchingActive()
  {
    if ((Object) this._animator == (Object) null)
      return;
    bool isMatchingTarget = this._animator.isMatchingTarget;
    this.isMatchingActive.Value = isMatchingTarget;
    if (isMatchingTarget)
      this.Fsm.Event(this.matchingActivatedEvent);
    else
      this.Fsm.Event(this.matchingDeactivedEvent);
  }
}
