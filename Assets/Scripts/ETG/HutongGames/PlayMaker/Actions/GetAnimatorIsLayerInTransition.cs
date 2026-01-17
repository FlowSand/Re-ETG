// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorIsLayerInTransition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Animator)]
  [HutongGames.PlayMaker.Tooltip("Returns true if the specified layer is in a transition. Can also send events")]
  public class GetAnimatorIsLayerInTransition : FsmStateActionAnimatorBase
  {
    [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
    [CheckForComponent(typeof (Animator))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The layer's index")]
    [RequiredField]
    public FsmInt layerIndex;
    [ActionSection("Results")]
    [HutongGames.PlayMaker.Tooltip("True if automatic matching is active")]
    [UIHint(UIHint.Variable)]
    public FsmBool isInTransition;
    [HutongGames.PlayMaker.Tooltip("Event send if automatic matching is active")]
    public FsmEvent isInTransitionEvent;
    [HutongGames.PlayMaker.Tooltip("Event send if automatic matching is not active")]
    public FsmEvent isNotInTransitionEvent;
    private Animator _animator;

    public override void Reset()
    {
      base.Reset();
      this.gameObject = (FsmOwnerDefault) null;
      this.isInTransition = (FsmBool) null;
      this.isInTransitionEvent = (FsmEvent) null;
      this.isNotInTransitionEvent = (FsmEvent) null;
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
          this.DoCheckIsInTransition();
          if (this.everyFrame)
            return;
          this.Finish();
        }
      }
    }

    public override void OnActionUpdate() => this.DoCheckIsInTransition();

    private void DoCheckIsInTransition()
    {
      if ((Object) this._animator == (Object) null)
        return;
      bool flag = this._animator.IsInTransition(this.layerIndex.Value);
      if (!this.isInTransition.IsNone)
        this.isInTransition.Value = flag;
      if (flag)
        this.Fsm.Event(this.isInTransitionEvent);
      else
        this.Fsm.Event(this.isNotInTransitionEvent);
    }
  }
}
