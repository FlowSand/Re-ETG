using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Quaternion)]
  [HutongGames.PlayMaker.Tooltip("Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).")]
  public class QuaternionEuler : QuaternionBaseAction
  {
    [HutongGames.PlayMaker.Tooltip("The Euler angles.")]
    [RequiredField]
    public FsmVector3 eulerAngles;
    [HutongGames.PlayMaker.Tooltip("Store the euler angles of this quaternion variable.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmQuaternion result;

    public override void Reset()
    {
      this.eulerAngles = (FsmVector3) null;
      this.result = (FsmQuaternion) null;
      this.everyFrame = true;
      this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
    }

    public override void OnEnter()
    {
      this.DoQuatEuler();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.Update)
        return;
      this.DoQuatEuler();
    }

    public override void OnLateUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.LateUpdate)
        return;
      this.DoQuatEuler();
    }

    public override void OnFixedUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.FixedUpdate)
        return;
      this.DoQuatEuler();
    }

    private void DoQuatEuler() => this.result.Value = Quaternion.Euler(this.eulerAngles.Value);
  }
}
