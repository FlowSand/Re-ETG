// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animator)]
[HutongGames.PlayMaker.Tooltip("Gets the position and rotation of the target specified by SetTarget(AvatarTarget targetIndex, float targetNormalizedTime)).\nThe position and rotation are only valid when a frame has being evaluated after the SetTarget call")]
public class GetAnimatorTarget : FsmStateActionAnimatorBase
{
  [RequiredField]
  [CheckForComponent(typeof (Animator))]
  [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The target position")]
  [ActionSection("Results")]
  [UIHint(UIHint.Variable)]
  public FsmVector3 targetPosition;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The target rotation")]
  public FsmQuaternion targetRotation;
  [HutongGames.PlayMaker.Tooltip("If set, apply the position and rotation to this gameObject")]
  public FsmGameObject targetGameObject;
  private Animator _animator;
  private Transform _transform;

  public override void Reset()
  {
    base.Reset();
    this.gameObject = (FsmOwnerDefault) null;
    this.targetPosition = (FsmVector3) null;
    this.targetRotation = (FsmQuaternion) null;
    this.targetGameObject = (FsmGameObject) null;
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
        GameObject gameObject = this.targetGameObject.Value;
        if ((Object) gameObject != (Object) null)
          this._transform = gameObject.transform;
        this.DoGetTarget();
        if (this.everyFrame)
          return;
        this.Finish();
      }
    }
  }

  public override void OnActionUpdate() => this.DoGetTarget();

  private void DoGetTarget()
  {
    if ((Object) this._animator == (Object) null)
      return;
    this.targetPosition.Value = this._animator.targetPosition;
    this.targetRotation.Value = this._animator.targetRotation;
    if (!((Object) this._transform != (Object) null))
      return;
    this._transform.position = this._animator.targetPosition;
    this._transform.rotation = this._animator.targetRotation;
  }
}
