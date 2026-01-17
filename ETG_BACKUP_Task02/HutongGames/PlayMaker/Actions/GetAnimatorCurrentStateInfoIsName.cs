// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorCurrentStateInfoIsName
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Check the current State name on a specified layer, this is more than the layer name, it holds the current state as well.")]
[ActionCategory(ActionCategory.Animator)]
public class GetAnimatorCurrentStateInfoIsName : FsmStateActionAnimatorBase
{
  [RequiredField]
  [CheckForComponent(typeof (Animator))]
  [HutongGames.PlayMaker.Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The layer's index")]
  [RequiredField]
  public FsmInt layerIndex;
  [HutongGames.PlayMaker.Tooltip("The name to check the layer against.")]
  public FsmString name;
  [HutongGames.PlayMaker.Tooltip("True if name matches")]
  [UIHint(UIHint.Variable)]
  [ActionSection("Results")]
  public FsmBool isMatching;
  [HutongGames.PlayMaker.Tooltip("Event send if name matches")]
  public FsmEvent nameMatchEvent;
  [HutongGames.PlayMaker.Tooltip("Event send if name doesn't match")]
  public FsmEvent nameDoNotMatchEvent;
  private Animator _animator;

  public override void Reset()
  {
    base.Reset();
    this.gameObject = (FsmOwnerDefault) null;
    this.layerIndex = (FsmInt) null;
    this.name = (FsmString) null;
    this.nameMatchEvent = (FsmEvent) null;
    this.nameDoNotMatchEvent = (FsmEvent) null;
    this.everyFrame = false;
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
        this.IsName();
        if (this.everyFrame)
          return;
        this.Finish();
      }
    }
  }

  public override void OnActionUpdate() => this.IsName();

  private void IsName()
  {
    if (!((Object) this._animator != (Object) null))
      return;
    AnimatorStateInfo animatorStateInfo = this._animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value);
    if (!this.isMatching.IsNone)
      this.isMatching.Value = animatorStateInfo.IsName(this.name.Value);
    if (animatorStateInfo.IsName(this.name.Value))
      this.Fsm.Event(this.nameMatchEvent);
    else
      this.Fsm.Event(this.nameDoNotMatchEvent);
  }
}
