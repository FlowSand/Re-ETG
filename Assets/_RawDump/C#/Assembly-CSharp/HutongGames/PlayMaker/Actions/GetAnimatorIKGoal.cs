// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorIKGoal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Gets the position, rotation and weights of an IK goal. A GameObject can be set to use for the position and rotation")]
[ActionCategory(ActionCategory.Animator)]
public class GetAnimatorIKGoal : FsmStateActionAnimatorBase
{
  [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
  [CheckForComponent(typeof (Animator))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [ObjectType(typeof (AvatarIKGoal))]
  [HutongGames.PlayMaker.Tooltip("The IK goal")]
  public FsmEnum iKGoal;
  [HutongGames.PlayMaker.Tooltip("The gameObject to apply ik goal position and rotation to")]
  [UIHint(UIHint.Variable)]
  [ActionSection("Results")]
  public FsmGameObject goal;
  [HutongGames.PlayMaker.Tooltip("Gets The position of the ik goal. If Goal GameObject define, position is used as an offset from Goal")]
  [UIHint(UIHint.Variable)]
  public FsmVector3 position;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Gets The rotation of the ik goal.If Goal GameObject define, rotation is used as an offset from Goal")]
  public FsmQuaternion rotation;
  [HutongGames.PlayMaker.Tooltip("Gets The translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal)")]
  [UIHint(UIHint.Variable)]
  public FsmFloat positionWeight;
  [HutongGames.PlayMaker.Tooltip("Gets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal)")]
  [UIHint(UIHint.Variable)]
  public FsmFloat rotationWeight;
  private Animator _animator;
  private Transform _transform;
  private AvatarIKGoal _iKGoal;

  public override void Reset()
  {
    base.Reset();
    this.gameObject = (FsmOwnerDefault) null;
    this.iKGoal = (FsmEnum) null;
    this.goal = (FsmGameObject) null;
    this.position = (FsmVector3) null;
    this.rotation = (FsmQuaternion) null;
    this.positionWeight = (FsmFloat) null;
    this.rotationWeight = (FsmFloat) null;
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
        GameObject gameObject = this.goal.Value;
        if ((Object) gameObject != (Object) null)
          this._transform = gameObject.transform;
        this.DoGetIKGoal();
        if (this.everyFrame)
          return;
        this.Finish();
      }
    }
  }

  public override void OnActionUpdate() => this.DoGetIKGoal();

  private void DoGetIKGoal()
  {
    if ((Object) this._animator == (Object) null)
      return;
    this._iKGoal = (AvatarIKGoal) this.iKGoal.Value;
    if ((Object) this._transform != (Object) null)
    {
      this._transform.position = this._animator.GetIKPosition(this._iKGoal);
      this._transform.rotation = this._animator.GetIKRotation(this._iKGoal);
    }
    if (!this.position.IsNone)
      this.position.Value = this._animator.GetIKPosition(this._iKGoal);
    if (!this.rotation.IsNone)
      this.rotation.Value = this._animator.GetIKRotation(this._iKGoal);
    if (!this.positionWeight.IsNone)
      this.positionWeight.Value = this._animator.GetIKPositionWeight(this._iKGoal);
    if (this.rotationWeight.IsNone)
      return;
    this.rotationWeight.Value = this._animator.GetIKRotationWeight(this._iKGoal);
  }
}
