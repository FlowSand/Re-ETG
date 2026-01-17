// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.FloatClamp
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Math)]
[HutongGames.PlayMaker.Tooltip("Clamps the value of Float Variable to a Min/Max range.")]
public class FloatClamp : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Float variable to clamp.")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmFloat floatVariable;
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The minimum value.")]
  public FsmFloat minValue;
  [HutongGames.PlayMaker.Tooltip("The maximum value.")]
  [RequiredField]
  public FsmFloat maxValue;
  [HutongGames.PlayMaker.Tooltip("Repeate every frame. Useful if the float variable is changing.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.floatVariable = (FsmFloat) null;
    this.minValue = (FsmFloat) null;
    this.maxValue = (FsmFloat) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoClamp();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoClamp();

  private void DoClamp()
  {
    this.floatVariable.Value = Mathf.Clamp(this.floatVariable.Value, this.minValue.Value, this.maxValue.Value);
  }
}
