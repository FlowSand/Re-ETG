// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.IntAdd
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Math)]
  [Tooltip("Adds a value to an Integer Variable.")]
  public class IntAdd : FsmStateAction
  {
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmInt intVariable;
    [RequiredField]
    public FsmInt add;
    public bool everyFrame;

    public override void Reset()
    {
      this.intVariable = (FsmInt) null;
      this.add = (FsmInt) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.intVariable.Value += this.add.Value;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.intVariable.Value += this.add.Value;
  }
}
