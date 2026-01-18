using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Device)]
  [HutongGames.PlayMaker.Tooltip("Gets the last measured linear acceleration of a device and stores it in a Vector3 Variable.")]
  public class GetDeviceAcceleration : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    public FsmVector3 storeVector;
    [UIHint(UIHint.Variable)]
    public FsmFloat storeX;
    [UIHint(UIHint.Variable)]
    public FsmFloat storeY;
    [UIHint(UIHint.Variable)]
    public FsmFloat storeZ;
    public FsmFloat multiplier;
    public bool everyFrame;

    public override void Reset()
    {
      this.storeVector = (FsmVector3) null;
      this.storeX = (FsmFloat) null;
      this.storeY = (FsmFloat) null;
      this.storeZ = (FsmFloat) null;
      this.multiplier = (FsmFloat) 1f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoGetDeviceAcceleration();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetDeviceAcceleration();

    private void DoGetDeviceAcceleration()
    {
      Vector3 vector3 = new Vector3(Input.acceleration.x, Input.acceleration.y, Input.acceleration.z);
      if (!this.multiplier.IsNone)
        vector3 *= this.multiplier.Value;
      this.storeVector.Value = vector3;
      this.storeX.Value = vector3.x;
      this.storeY.Value = vector3.y;
      this.storeZ.Value = vector3.z;
    }
  }
}
