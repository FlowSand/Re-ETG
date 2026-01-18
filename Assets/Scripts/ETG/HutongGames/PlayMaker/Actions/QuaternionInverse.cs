using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Inverse a quaternion")]
  [ActionCategory(ActionCategory.Quaternion)]
  public class QuaternionInverse : QuaternionBaseAction
  {
    [HutongGames.PlayMaker.Tooltip("the rotation")]
    [RequiredField]
    public FsmQuaternion rotation;
    [HutongGames.PlayMaker.Tooltip("Store the inverse of the rotation variable.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmQuaternion result;

    public override void Reset()
    {
      this.rotation = (FsmQuaternion) null;
      this.result = (FsmQuaternion) null;
      this.everyFrame = true;
      this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
    }

    public override void OnEnter()
    {
      this.DoQuatInverse();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.Update)
        return;
      this.DoQuatInverse();
    }

    public override void OnLateUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.LateUpdate)
        return;
      this.DoQuatInverse();
    }

    public override void OnFixedUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.FixedUpdate)
        return;
      this.DoQuatInverse();
    }

    private void DoQuatInverse() => this.result.Value = Quaternion.Inverse(this.rotation.Value);
  }
}
