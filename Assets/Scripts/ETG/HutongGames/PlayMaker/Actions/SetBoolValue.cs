// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetBoolValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Sets the value of a Bool Variable.")]
  [ActionCategory(ActionCategory.Math)]
  public class SetBoolValue : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmBool boolVariable;
    [RequiredField]
    public FsmBool boolValue;
    public bool everyFrame;

    public override void Reset()
    {
      this.boolVariable = (FsmBool) null;
      this.boolValue = (FsmBool) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.boolVariable.Value = this.boolValue.Value;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.boolVariable.Value = this.boolValue.Value;
  }
}
