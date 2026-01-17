// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.StringReplace
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.String)]
  [Tooltip("Replace a substring with a new String.")]
  public class StringReplace : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmString stringVariable;
    public FsmString replace;
    public FsmString with;
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmString storeResult;
    public bool everyFrame;

    public override void Reset()
    {
      this.stringVariable = (FsmString) null;
      this.replace = (FsmString) string.Empty;
      this.with = (FsmString) string.Empty;
      this.storeResult = (FsmString) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoReplace();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoReplace();

    private void DoReplace()
    {
      if (this.stringVariable == null || this.storeResult == null)
        return;
      this.storeResult.Value = this.stringVariable.Value.Replace(this.replace.Value, this.with.Value);
    }
  }
}
