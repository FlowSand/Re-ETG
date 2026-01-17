// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.BoolFlip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Flips the value of a Bool Variable.")]
[ActionCategory(ActionCategory.Math)]
public class BoolFlip : FsmStateAction
{
  [Tooltip("Bool variable to flip.")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmBool boolVariable;

  public override void Reset() => this.boolVariable = (FsmBool) null;

  public override void OnEnter()
  {
    this.boolVariable.Value = !this.boolVariable.Value;
    this.Finish();
  }
}
