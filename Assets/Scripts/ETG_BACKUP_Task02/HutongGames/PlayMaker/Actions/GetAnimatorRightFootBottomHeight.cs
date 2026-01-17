// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorRightFootBottomHeight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animator)]
[HutongGames.PlayMaker.Tooltip("Get the right foot bottom height.")]
public class GetAnimatorRightFootBottomHeight : FsmStateAction
{
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
  [CheckForComponent(typeof (Animator))]
  public FsmOwnerDefault gameObject;
  [ActionSection("Result")]
  [HutongGames.PlayMaker.Tooltip("The right foot bottom height.")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmFloat rightFootHeight;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame during LateUpdate. Useful when value is subject to change over time.")]
  public bool everyFrame;
  private Animator _animator;

  public override void Reset()
  {
    base.Reset();
    this.gameObject = (FsmOwnerDefault) null;
    this.rightFootHeight = (FsmFloat) null;
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
        this._getRightFootBottonHeight();
        if (this.everyFrame)
          return;
        this.Finish();
      }
    }
  }

  public override void OnLateUpdate() => this._getRightFootBottonHeight();

  private void _getRightFootBottonHeight()
  {
    if (!((Object) this._animator != (Object) null))
      return;
    this.rightFootHeight.Value = this._animator.rightFeetBottomHeight;
  }
}
