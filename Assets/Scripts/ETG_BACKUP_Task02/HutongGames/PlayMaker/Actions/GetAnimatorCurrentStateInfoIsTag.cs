// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorCurrentStateInfoIsTag
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animator)]
[HutongGames.PlayMaker.Tooltip("Does tag match the tag of the active state in the statemachine")]
public class GetAnimatorCurrentStateInfoIsTag : FsmStateActionAnimatorBase
{
  [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
  [CheckForComponent(typeof (Animator))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The layer's index")]
  [RequiredField]
  public FsmInt layerIndex;
  [HutongGames.PlayMaker.Tooltip("The tag to check the layer against.")]
  public FsmString tag;
  [HutongGames.PlayMaker.Tooltip("True if tag matches")]
  [UIHint(UIHint.Variable)]
  [ActionSection("Results")]
  public FsmBool tagMatch;
  [HutongGames.PlayMaker.Tooltip("Event send if tag matches")]
  public FsmEvent tagMatchEvent;
  [HutongGames.PlayMaker.Tooltip("Event send if tag matches")]
  public FsmEvent tagDoNotMatchEvent;
  private Animator _animator;

  public override void Reset()
  {
    base.Reset();
    this.gameObject = (FsmOwnerDefault) null;
    this.layerIndex = (FsmInt) null;
    this.tag = (FsmString) null;
    this.tagMatch = (FsmBool) null;
    this.tagMatchEvent = (FsmEvent) null;
    this.tagDoNotMatchEvent = (FsmEvent) null;
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
        this.IsTag();
        if (this.everyFrame)
          return;
        this.Finish();
      }
    }
  }

  public override void OnActionUpdate() => this.IsTag();

  private void IsTag()
  {
    if (!((Object) this._animator != (Object) null))
      return;
    if (this._animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value).IsTag(this.tag.Value))
    {
      this.tagMatch.Value = true;
      this.Fsm.Event(this.tagMatchEvent);
    }
    else
    {
      this.tagMatch.Value = false;
      this.Fsm.Event(this.tagDoNotMatchEvent);
    }
  }
}
