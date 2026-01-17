// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorIsHuman
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Returns true if the current rig is humanoid, false if it is generic. Can also sends events")]
[ActionCategory(ActionCategory.Animator)]
public class GetAnimatorIsHuman : FsmStateAction
{
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
  [CheckForComponent(typeof (Animator))]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("True if the current rig is humanoid, False if it is generic")]
  [ActionSection("Results")]
  [UIHint(UIHint.Variable)]
  public FsmBool isHuman;
  [HutongGames.PlayMaker.Tooltip("Event send if rig is humanoid")]
  public FsmEvent isHumanEvent;
  [HutongGames.PlayMaker.Tooltip("Event send if rig is generic")]
  public FsmEvent isGenericEvent;
  private Animator _animator;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.isHuman = (FsmBool) null;
    this.isHumanEvent = (FsmEvent) null;
    this.isGenericEvent = (FsmEvent) null;
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
        this.DoCheckIsHuman();
        this.Finish();
      }
    }
  }

  private void DoCheckIsHuman()
  {
    if ((Object) this._animator == (Object) null)
      return;
    bool isHuman = this._animator.isHuman;
    if (!this.isHuman.IsNone)
      this.isHuman.Value = isHuman;
    if (isHuman)
      this.Fsm.Event(this.isHumanEvent);
    else
      this.Fsm.Event(this.isGenericEvent);
  }
}
