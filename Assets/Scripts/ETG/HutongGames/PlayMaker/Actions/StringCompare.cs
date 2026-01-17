// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.StringCompare
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Logic)]
  [Tooltip("Compares 2 Strings and sends Events based on the result.")]
  public class StringCompare : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmString stringVariable;
    public FsmString compareTo;
    public FsmEvent equalEvent;
    public FsmEvent notEqualEvent;
    [UIHint(UIHint.Variable)]
    [Tooltip("Store the true/false result in a bool variable.")]
    public FsmBool storeResult;
    [Tooltip("Repeat every frame. Useful if any of the strings are changing over time.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.stringVariable = (FsmString) null;
      this.compareTo = (FsmString) string.Empty;
      this.equalEvent = (FsmEvent) null;
      this.notEqualEvent = (FsmEvent) null;
      this.storeResult = (FsmBool) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoStringCompare();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoStringCompare();

    private void DoStringCompare()
    {
      if (this.stringVariable == null || this.compareTo == null)
        return;
      bool flag = this.stringVariable.Value == this.compareTo.Value;
      if (this.storeResult != null)
        this.storeResult.Value = flag;
      if (flag && this.equalEvent != null)
      {
        this.Fsm.Event(this.equalEvent);
      }
      else
      {
        if (flag || this.notEqualEvent == null)
          return;
        this.Fsm.Event(this.notEqualEvent);
      }
    }
  }
}
