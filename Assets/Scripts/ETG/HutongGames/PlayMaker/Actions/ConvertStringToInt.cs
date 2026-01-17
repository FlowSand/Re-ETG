// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ConvertStringToInt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Convert)]
  [Tooltip("Converts an String value to an Int value.")]
  public class ConvertStringToInt : FsmStateAction
  {
    [Tooltip("The String variable to convert to an integer.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmString stringVariable;
    [Tooltip("Store the result in an Int variable.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmInt intVariable;
    [Tooltip("Repeat every frame. Useful if the String variable is changing.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.intVariable = (FsmInt) null;
      this.stringVariable = (FsmString) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoConvertStringToInt();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoConvertStringToInt();

    private void DoConvertStringToInt()
    {
      this.intVariable.Value = int.Parse(this.stringVariable.Value);
    }
  }
}
