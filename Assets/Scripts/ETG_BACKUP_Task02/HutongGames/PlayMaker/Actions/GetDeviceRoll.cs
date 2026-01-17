// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetDeviceRoll
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Device)]
[HutongGames.PlayMaker.Tooltip("Gets the rotation of the device around its z axis (into the screen). For example when you steer with the iPhone in a driving game.")]
public class GetDeviceRoll : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("How the user is expected to hold the device (where angle will be zero).")]
  public GetDeviceRoll.BaseOrientation baseOrientation;
  [UIHint(UIHint.Variable)]
  public FsmFloat storeAngle;
  public FsmFloat limitAngle;
  public FsmFloat smoothing;
  public bool everyFrame;
  private float lastZAngle;

  public override void Reset()
  {
    this.baseOrientation = GetDeviceRoll.BaseOrientation.LandscapeLeft;
    this.storeAngle = (FsmFloat) null;
    FsmFloat fsmFloat = new FsmFloat();
    fsmFloat.UseVariable = true;
    this.limitAngle = fsmFloat;
    this.smoothing = (FsmFloat) 5f;
    this.everyFrame = true;
  }

  public override void OnEnter()
  {
    this.DoGetDeviceRoll();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetDeviceRoll();

  private void DoGetDeviceRoll()
  {
    float x = Input.acceleration.x;
    float y = Input.acceleration.y;
    float b = 0.0f;
    switch (this.baseOrientation)
    {
      case GetDeviceRoll.BaseOrientation.Portrait:
        b = -Mathf.Atan2(x, -y);
        break;
      case GetDeviceRoll.BaseOrientation.LandscapeLeft:
        b = Mathf.Atan2(y, -x);
        break;
      case GetDeviceRoll.BaseOrientation.LandscapeRight:
        b = -Mathf.Atan2(y, x);
        break;
    }
    if (!this.limitAngle.IsNone)
      b = Mathf.Clamp(57.29578f * b, -this.limitAngle.Value, this.limitAngle.Value);
    if ((double) this.smoothing.Value > 0.0)
      b = Mathf.LerpAngle(this.lastZAngle, b, this.smoothing.Value * Time.deltaTime);
    this.lastZAngle = b;
    this.storeAngle.Value = b;
  }

  public enum BaseOrientation
  {
    Portrait,
    LandscapeLeft,
    LandscapeRight,
  }
}
