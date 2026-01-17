// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorDelta
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Gets the avatar delta position and rotation for the last evaluated frame.")]
[ActionCategory(ActionCategory.Animator)]
public class GetAnimatorDelta : FsmStateActionAnimatorBase
{
  [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
  [CheckForComponent(typeof (Animator))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The avatar delta position for the last evaluated frame")]
  [UIHint(UIHint.Variable)]
  public FsmVector3 deltaPosition;
  [HutongGames.PlayMaker.Tooltip("The avatar delta position for the last evaluated frame")]
  [UIHint(UIHint.Variable)]
  public FsmQuaternion deltaRotation;
  private Transform _transform;
  private Animator _animator;

  public override void Reset()
  {
    base.Reset();
    this.gameObject = (FsmOwnerDefault) null;
    this.deltaPosition = (FsmVector3) null;
    this.deltaRotation = (FsmQuaternion) null;
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
        this.DoGetDeltaPosition();
        this.Finish();
      }
    }
  }

  public override void OnActionUpdate() => this.DoGetDeltaPosition();

  private void DoGetDeltaPosition()
  {
    if ((Object) this._animator == (Object) null)
      return;
    this.deltaPosition.Value = this._animator.deltaPosition;
    this.deltaRotation.Value = this._animator.deltaRotation;
  }
}
