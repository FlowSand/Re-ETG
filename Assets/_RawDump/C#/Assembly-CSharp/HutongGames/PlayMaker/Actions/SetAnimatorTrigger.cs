// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetAnimatorTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets a trigger parameter to active. Triggers are parameters that act mostly like booleans, but get reset to inactive when they are used in a transition.")]
[ActionCategory(ActionCategory.Animator)]
public class SetAnimatorTrigger : FsmStateAction
{
  [CheckForComponent(typeof (Animator))]
  [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The trigger name")]
  [UIHint(UIHint.AnimatorTrigger)]
  [RequiredField]
  public FsmString trigger;
  private Animator _animator;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.trigger = (FsmString) null;
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
        this.SetTrigger();
        this.Finish();
      }
    }
  }

  private void SetTrigger()
  {
    if (!((Object) this._animator != (Object) null))
      return;
    this._animator.SetTrigger(this.trigger.Value);
  }
}
