// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.QuaternionLerp
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Interpolates between from and to by t and normalizes the result afterwards.")]
[ActionCategory(ActionCategory.Quaternion)]
public class QuaternionLerp : QuaternionBaseAction
{
  [HutongGames.PlayMaker.Tooltip("From Quaternion.")]
  [RequiredField]
  public FsmQuaternion fromQuaternion;
  [HutongGames.PlayMaker.Tooltip("To Quaternion.")]
  [RequiredField]
  public FsmQuaternion toQuaternion;
  [HasFloatSlider(0.0f, 1f)]
  [HutongGames.PlayMaker.Tooltip("Interpolate between fromQuaternion and toQuaternion by this amount. Value is clamped to 0-1 range. 0 = fromQuaternion; 1 = toQuaternion; 0.5 = half way between.")]
  [RequiredField]
  public FsmFloat amount;
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
    this.amount = (FsmFloat) 0.5f;
    this.storeResult = (FsmQuaternion) null;
    this.everyFrame = true;
    this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
  }

  public override void OnEnter()
  {
    this.DoQuatLerp();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate()
  {
    if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.Update)
      return;
    this.DoQuatLerp();
  }

  public override void OnLateUpdate()
  {
    if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.LateUpdate)
      return;
    this.DoQuatLerp();
  }

  public override void OnFixedUpdate()
  {
    if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.FixedUpdate)
      return;
    this.DoQuatLerp();
  }

  private void DoQuatLerp()
  {
    this.storeResult.Value = Quaternion.Lerp(this.fromQuaternion.Value, this.toQuaternion.Value, this.amount.Value);
  }
}
