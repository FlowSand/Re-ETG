// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAtan2FromVector3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Get the Arc Tangent 2 as in atan2(y,x) from a vector 3, where you pick which is x and y from the vector 3. You can get the result in degrees, simply check on the RadToDeg conversion")]
[ActionCategory(ActionCategory.Trigonometry)]
public class GetAtan2FromVector3 : FsmStateAction
{
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The vector3 definition of the tan")]
  public FsmVector3 vector3;
  [HutongGames.PlayMaker.Tooltip("which axis in the vector3 to use as the x value of the tan")]
  [RequiredField]
  public GetAtan2FromVector3.aTan2EnumAxis xAxis;
  [HutongGames.PlayMaker.Tooltip("which axis in the vector3 to use as the y value of the tan")]
  [RequiredField]
  public GetAtan2FromVector3.aTan2EnumAxis yAxis;
  [HutongGames.PlayMaker.Tooltip("The resulting angle. Note:If you want degrees, simply check RadToDeg")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmFloat angle;
  [HutongGames.PlayMaker.Tooltip("Check on if you want the angle expressed in degrees.")]
  public FsmBool RadToDeg;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.vector3 = (FsmVector3) null;
    this.xAxis = GetAtan2FromVector3.aTan2EnumAxis.x;
    this.yAxis = GetAtan2FromVector3.aTan2EnumAxis.y;
    this.RadToDeg = (FsmBool) true;
    this.everyFrame = false;
    this.angle = (FsmFloat) null;
  }

  public override void OnEnter()
  {
    this.DoATan();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoATan();

  private void DoATan()
  {
    float x = this.vector3.Value.x;
    if (this.xAxis == GetAtan2FromVector3.aTan2EnumAxis.y)
      x = this.vector3.Value.y;
    else if (this.xAxis == GetAtan2FromVector3.aTan2EnumAxis.z)
      x = this.vector3.Value.z;
    float y = this.vector3.Value.y;
    if (this.yAxis == GetAtan2FromVector3.aTan2EnumAxis.x)
      y = this.vector3.Value.x;
    else if (this.yAxis == GetAtan2FromVector3.aTan2EnumAxis.z)
      y = this.vector3.Value.z;
    float num = Mathf.Atan2(y, x);
    if (this.RadToDeg.Value)
      num *= 57.29578f;
    this.angle.Value = num;
  }

  public enum aTan2EnumAxis
  {
    x,
    y,
    z,
  }
}
