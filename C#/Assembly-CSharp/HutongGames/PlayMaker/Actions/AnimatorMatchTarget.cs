// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AnimatorMatchTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animator)]
[HutongGames.PlayMaker.Tooltip("Automatically adjust the gameobject position and rotation so that the AvatarTarget reaches the matchPosition when the current state is at the specified progress")]
public class AnimatorMatchTarget : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
  [CheckForComponent(typeof (Animator))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The body part that is involved in the match")]
  public AvatarTarget bodyPart;
  [HutongGames.PlayMaker.Tooltip("The gameObject target to match")]
  public FsmGameObject target;
  [HutongGames.PlayMaker.Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
  public FsmVector3 targetPosition;
  [HutongGames.PlayMaker.Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
  public FsmQuaternion targetRotation;
  [HutongGames.PlayMaker.Tooltip("The MatchTargetWeightMask Position XYZ weight")]
  public FsmVector3 positionWeight;
  [HutongGames.PlayMaker.Tooltip("The MatchTargetWeightMask Rotation weight")]
  public FsmFloat rotationWeight;
  [HutongGames.PlayMaker.Tooltip("Start time within the animation clip (0 - beginning of clip, 1 - end of clip)")]
  public FsmFloat startNormalizedTime;
  [HutongGames.PlayMaker.Tooltip("End time within the animation clip (0 - beginning of clip, 1 - end of clip), values greater than 1 can be set to trigger a match after a certain number of loops. Ex: 2.3 means at 30% of 2nd loop")]
  public FsmFloat targetNormalizedTime;
  [HutongGames.PlayMaker.Tooltip("Should always be true")]
  public bool everyFrame;
  private Animator _animator;
  private Transform _transform;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.bodyPart = AvatarTarget.Root;
    this.target = (FsmGameObject) null;
    FsmVector3 fsmVector3 = new FsmVector3();
    fsmVector3.UseVariable = true;
    this.targetPosition = fsmVector3;
    FsmQuaternion fsmQuaternion = new FsmQuaternion();
    fsmQuaternion.UseVariable = true;
    this.targetRotation = fsmQuaternion;
    this.positionWeight = (FsmVector3) Vector3.one;
    this.rotationWeight = (FsmFloat) 0.0f;
    this.startNormalizedTime = (FsmFloat) null;
    this.targetNormalizedTime = (FsmFloat) null;
    this.everyFrame = true;
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
        GameObject gameObject = this.target.Value;
        if ((Object) gameObject != (Object) null)
          this._transform = gameObject.transform;
        this.DoMatchTarget();
        if (this.everyFrame)
          return;
        this.Finish();
      }
    }
  }

  public override void OnUpdate() => this.DoMatchTarget();

  private void DoMatchTarget()
  {
    if ((Object) this._animator == (Object) null)
      return;
    Vector3 matchPosition = Vector3.zero;
    Quaternion matchRotation = Quaternion.identity;
    if ((Object) this._transform != (Object) null)
    {
      matchPosition = this._transform.position;
      matchRotation = this._transform.rotation;
    }
    if (!this.targetPosition.IsNone)
      matchPosition += this.targetPosition.Value;
    if (!this.targetRotation.IsNone)
      matchRotation *= this.targetRotation.Value;
    MatchTargetWeightMask weightMask = new MatchTargetWeightMask(this.positionWeight.Value, this.rotationWeight.Value);
    this._animator.MatchTarget(matchPosition, matchRotation, this.bodyPart, weightMask, this.startNormalizedTime.Value, this.targetNormalizedTime.Value);
  }
}
