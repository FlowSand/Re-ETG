// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetQuaternionEulerAngles
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Quaternion)]
  [Tooltip("Gets a quaternion as euler angles.")]
  public class GetQuaternionEulerAngles : QuaternionBaseAction
  {
    [Tooltip("The rotation")]
    [RequiredField]
    public FsmQuaternion quaternion;
    [Tooltip("The euler angles of the quaternion.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmVector3 eulerAngles;

    public override void Reset()
    {
      this.quaternion = (FsmQuaternion) null;
      this.eulerAngles = (FsmVector3) null;
      this.everyFrame = true;
      this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
    }

    public override void OnEnter()
    {
      this.GetQuatEuler();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.Update)
        return;
      this.GetQuatEuler();
    }

    public override void OnLateUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.LateUpdate)
        return;
      this.GetQuatEuler();
    }

    public override void OnFixedUpdate()
    {
      if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.FixedUpdate)
        return;
      this.GetQuatEuler();
    }

    private void GetQuatEuler() => this.eulerAngles.Value = this.quaternion.Value.eulerAngles;
  }
}
