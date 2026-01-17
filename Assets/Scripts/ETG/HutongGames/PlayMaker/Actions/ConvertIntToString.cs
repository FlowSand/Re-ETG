// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ConvertIntToString
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Converts an Integer value to a String value with an optional format.")]
  [ActionCategory(ActionCategory.Convert)]
  public class ConvertIntToString : FsmStateAction
  {
    [Tooltip("The Int variable to convert.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmInt intVariable;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    [Tooltip("A String variable to store the converted value.")]
    public FsmString stringVariable;
    [Tooltip("Optional Format, allows for leading zeroes. E.g., 0000")]
    public FsmString format;
    [Tooltip("Repeat every frame. Useful if the Int variable is changing.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.intVariable = (FsmInt) null;
      this.stringVariable = (FsmString) null;
      this.everyFrame = false;
      this.format = (FsmString) null;
    }

    public override void OnEnter()
    {
      this.DoConvertIntToString();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoConvertIntToString();

    private void DoConvertIntToString()
    {
      if (this.format.IsNone || string.IsNullOrEmpty(this.format.Value))
        this.stringVariable.Value = this.intVariable.Value.ToString();
      else
        this.stringVariable.Value = this.intVariable.Value.ToString(this.format.Value);
    }
  }
}
