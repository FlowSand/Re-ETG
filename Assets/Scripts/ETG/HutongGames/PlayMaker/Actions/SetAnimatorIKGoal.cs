using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the position, rotation and weights of an IK goal. A GameObject can be set to control the position and rotation, or it can be manually expressed.")]
  [ActionCategory(ActionCategory.Animator)]
  public class SetAnimatorIKGoal : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The target.")]
    [CheckForComponent(typeof (Animator))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The IK goal")]
    public AvatarIKGoal iKGoal;
    [HutongGames.PlayMaker.Tooltip("The gameObject target of the ik goal")]
    public FsmGameObject goal;
    [HutongGames.PlayMaker.Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
    public FsmVector3 position;
    [HutongGames.PlayMaker.Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
    public FsmQuaternion rotation;
    [HutongGames.PlayMaker.Tooltip("The translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal)")]
    [HasFloatSlider(0.0f, 1f)]
    public FsmFloat positionWeight;
    [HutongGames.PlayMaker.Tooltip("Sets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal)")]
    [HasFloatSlider(0.0f, 1f)]
    public FsmFloat rotationWeight;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when changing over time.")]
    public bool everyFrame;
    private Animator _animator;
    private Transform _transform;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.goal = (FsmGameObject) null;
      FsmVector3 fsmVector3 = new FsmVector3();
      fsmVector3.UseVariable = true;
      this.position = fsmVector3;
      FsmQuaternion fsmQuaternion = new FsmQuaternion();
      fsmQuaternion.UseVariable = true;
      this.rotation = fsmQuaternion;
      this.positionWeight = (FsmFloat) 1f;
      this.rotationWeight = (FsmFloat) 1f;
      this.everyFrame = false;
    }

    public override void OnPreprocess() => this.Fsm.HandleAnimatorIK = true;

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
          if (!((Object) gameObject != (Object) null))
            return;
          this._transform = gameObject.transform;
        }
      }
    }

    public override void DoAnimatorIK(int layerIndex)
    {
      this.DoSetIKGoal();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    private void DoSetIKGoal()
    {
      if ((Object) this._animator == (Object) null)
        return;
      if ((Object) this._transform != (Object) null)
      {
        if (this.position.IsNone)
          this._animator.SetIKPosition(this.iKGoal, this._transform.position);
        else
          this._animator.SetIKPosition(this.iKGoal, this._transform.position + this.position.Value);
        if (this.rotation.IsNone)
          this._animator.SetIKRotation(this.iKGoal, this._transform.rotation);
        else
          this._animator.SetIKRotation(this.iKGoal, this._transform.rotation * this.rotation.Value);
      }
      else
      {
        if (!this.position.IsNone)
          this._animator.SetIKPosition(this.iKGoal, this.position.Value);
        if (!this.rotation.IsNone)
          this._animator.SetIKRotation(this.iKGoal, this.rotation.Value);
      }
      if (!this.positionWeight.IsNone)
        this._animator.SetIKPositionWeight(this.iKGoal, this.positionWeight.Value);
      if (this.rotationWeight.IsNone)
        return;
      this._animator.SetIKRotationWeight(this.iKGoal, this.rotationWeight.Value);
    }
  }
}
