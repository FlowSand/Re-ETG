// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetQuaternionMultipliedByQuaternion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Quaternion)]
  [Tooltip("Get the quaternion from a quaternion multiplied by a quaternion.")]
  public class GetQuaternionMultipliedByQuaternion : QuaternionBaseAction
  {
    [Tooltip("The first quaternion to multiply")]
    [RequiredField]
    public FsmQuaternion quaternionA;
    [Tooltip("The second quaternion to multiply")]
    [RequiredField]
    public FsmQuaternion quaternionB;
    [RequiredField]
    [UIHint(UIHint.Variable)]
    [Tooltip("The resulting quaternion")]
    public FsmQuaternion result;

    public override void Reset()
    {
      this.quaternionA = (FsmQuaternion) null;
      this.quaternionB = (FsmQuaternion) null;
      this.result = (FsmQuaternion) null;
      this.everyFrame = false;
      this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
    }

    public override void OnEnter()
    {
      this.DoQuatMult();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.Update)
        return;
      this.DoQuatMult();
    }

    public override void OnLateUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.LateUpdate)
        return;
      this.DoQuatMult();
    }

    public override void OnFixedUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.FixedUpdate)
        return;
      this.DoQuatMult();
    }

    private void DoQuatMult() => this.result.Value = this.quaternionA.Value * this.quaternionB.Value;
  }
}
