using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Quaternion)]
  [HutongGames.PlayMaker.Tooltip("Rotates a rotation from towards to. This is essentially the same as Quaternion.Slerp but instead the function will ensure that the angular speed never exceeds maxDegreesDelta. Negative values of maxDegreesDelta pushes the rotation away from to.")]
  public class QuaternionRotateTowards : QuaternionBaseAction
  {
    [HutongGames.PlayMaker.Tooltip("From Quaternion.")]
    [RequiredField]
    public FsmQuaternion fromQuaternion;
    [HutongGames.PlayMaker.Tooltip("To Quaternion.")]
    [RequiredField]
    public FsmQuaternion toQuaternion;
    [HutongGames.PlayMaker.Tooltip("The angular speed never exceeds maxDegreesDelta. Negative values of maxDegreesDelta pushes the rotation away from to.")]
    [RequiredField]
    public FsmFloat maxDegreesDelta;
    [HutongGames.PlayMaker.Tooltip("Store the result in this quaternion variable.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmQuaternion storeResult;

    public override void Reset()
    {
      FsmQuaternion fsmQuaternion1 = new FsmQuaternion();
      fsmQuaternion1.UseVariable = true;
      this.fromQuaternion = fsmQuaternion1;
      FsmQuaternion fsmQuaternion2 = new FsmQuaternion();
      fsmQuaternion2.UseVariable = true;
      this.toQuaternion = fsmQuaternion2;
      this.maxDegreesDelta = (FsmFloat) 10f;
      this.storeResult = (FsmQuaternion) null;
      this.everyFrame = true;
      this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
    }

    public override void OnEnter()
    {
      this.DoQuatRotateTowards();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.Update)
        return;
      this.DoQuatRotateTowards();
    }

    public override void OnLateUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.LateUpdate)
        return;
      this.DoQuatRotateTowards();
    }

    public override void OnFixedUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.FixedUpdate)
        return;
      this.DoQuatRotateTowards();
    }

    private void DoQuatRotateTowards()
    {
      this.storeResult.Value = Quaternion.RotateTowards(this.fromQuaternion.Value, this.toQuaternion.Value, this.maxDegreesDelta.Value);
    }
  }
}
