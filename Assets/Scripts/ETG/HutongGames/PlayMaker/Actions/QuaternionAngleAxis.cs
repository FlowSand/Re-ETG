// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.QuaternionAngleAxis
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Creates a rotation which rotates angle degrees around axis.")]
  [ActionCategory(ActionCategory.Quaternion)]
  public class QuaternionAngleAxis : QuaternionBaseAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The angle.")]
    public FsmFloat angle;
    [HutongGames.PlayMaker.Tooltip("The rotation axis.")]
    [RequiredField]
    public FsmVector3 axis;
    [HutongGames.PlayMaker.Tooltip("Store the rotation of this quaternion variable.")]
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmQuaternion result;

    public override void Reset()
    {
      this.angle = (FsmFloat) null;
      this.axis = (FsmVector3) null;
      this.result = (FsmQuaternion) null;
      this.everyFrame = true;
      this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
    }

    public override void OnEnter()
    {
      this.DoQuatAngleAxis();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.Update)
        return;
      this.DoQuatAngleAxis();
    }

    public override void OnLateUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.LateUpdate)
        return;
      this.DoQuatAngleAxis();
    }

    public override void OnFixedUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.FixedUpdate)
        return;
      this.DoQuatAngleAxis();
    }

    private void DoQuatAngleAxis()
    {
      this.result.Value = Quaternion.AngleAxis(this.angle.Value, this.axis.Value);
    }
  }
}
