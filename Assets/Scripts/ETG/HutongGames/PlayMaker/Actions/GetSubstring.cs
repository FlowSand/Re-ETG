// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetSubstring
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.String)]
  [Tooltip("Gets a sub-string from a String Variable.")]
  public class GetSubstring : FsmStateAction
  {
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmString stringVariable;
    [RequiredField]
    public FsmInt startIndex;
    [RequiredField]
    public FsmInt length;
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmString storeResult;
    public bool everyFrame;

    public override void Reset()
    {
      this.stringVariable = (FsmString) null;
      this.startIndex = (FsmInt) 0;
      this.length = (FsmInt) 1;
      this.storeResult = (FsmString) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoGetSubstring();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetSubstring();

    private void DoGetSubstring()
    {
      if (this.stringVariable == null || this.storeResult == null)
        return;
      this.storeResult.Value = this.stringVariable.Value.Substring(this.startIndex.Value, this.length.Value);
    }
  }
}
