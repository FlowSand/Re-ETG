// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.FloatSubtract
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Subtracts a value from a Float Variable.")]
[ActionCategory(ActionCategory.Math)]
public class FloatSubtract : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The float variable to subtract from.")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmFloat floatVariable;
  [HutongGames.PlayMaker.Tooltip("Value to subtract from the float variable.")]
  [RequiredField]
  public FsmFloat subtract;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
  public bool everyFrame;
  [HutongGames.PlayMaker.Tooltip("Used with Every Frame. Adds the value over one second to make the operation frame rate independent.")]
  public bool perSecond;

  public override void Reset()
  {
    this.floatVariable = (FsmFloat) null;
    this.subtract = (FsmFloat) null;
    this.everyFrame = false;
    this.perSecond = false;
  }

  public override void OnEnter()
  {
    this.DoFloatSubtract();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoFloatSubtract();

  private void DoFloatSubtract()
  {
    if (!this.perSecond)
      this.floatVariable.Value -= this.subtract.Value;
    else
      this.floatVariable.Value -= this.subtract.Value * Time.deltaTime;
  }
}
