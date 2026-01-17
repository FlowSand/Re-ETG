// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetQuaternionFromRotation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Creates a rotation which rotates from fromDirection to toDirection. Usually you use this to rotate a transform so that one of its axes eg. the y-axis - follows a target direction toDirection in world space.")]
  [ActionCategory(ActionCategory.Quaternion)]
  public class GetQuaternionFromRotation : QuaternionBaseAction
  {
    [HutongGames.PlayMaker.Tooltip("the 'from' direction")]
    [RequiredField]
    public FsmVector3 fromDirection;
    [HutongGames.PlayMaker.Tooltip("the 'to' direction")]
    [RequiredField]
    public FsmVector3 toDirection;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("the resulting quaternion")]
    [UIHint(UIHint.Variable)]
    public FsmQuaternion result;

    public override void Reset()
    {
      this.fromDirection = (FsmVector3) null;
      this.toDirection = (FsmVector3) null;
      this.result = (FsmQuaternion) null;
      this.everyFrame = false;
      this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
    }

    public override void OnEnter()
    {
      this.DoQuatFromRotation();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.Update)
        return;
      this.DoQuatFromRotation();
    }

    public override void OnLateUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.LateUpdate)
        return;
      this.DoQuatFromRotation();
    }

    public override void OnFixedUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.FixedUpdate)
        return;
      this.DoQuatFromRotation();
    }

    private void DoQuatFromRotation()
    {
      this.result.Value = Quaternion.FromToRotation(this.fromDirection.Value, this.toDirection.Value);
    }
  }
}
