// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.StringContains
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Logic)]
[Tooltip("Tests if a String contains another String.")]
public class StringContains : FsmStateAction
{
  [Tooltip("The String variable to test.")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmString stringVariable;
  [Tooltip("Test if the String variable contains this string.")]
  [RequiredField]
  public FsmString containsString;
  [Tooltip("Event to send if true.")]
  public FsmEvent trueEvent;
  [Tooltip("Event to send if false.")]
  public FsmEvent falseEvent;
  [UIHint(UIHint.Variable)]
  [Tooltip("Store the true/false result in a bool variable.")]
  public FsmBool storeResult;
  [Tooltip("Repeat every frame. Useful if any of the strings are changing over time.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.stringVariable = (FsmString) null;
    this.containsString = (FsmString) string.Empty;
    this.trueEvent = (FsmEvent) null;
    this.falseEvent = (FsmEvent) null;
    this.storeResult = (FsmBool) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoStringContains();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoStringContains();

  private void DoStringContains()
  {
    if (this.stringVariable.IsNone || this.containsString.IsNone)
      return;
    bool flag = this.stringVariable.Value.Contains(this.containsString.Value);
    if (this.storeResult != null)
      this.storeResult.Value = flag;
    if (flag && this.trueEvent != null)
    {
      this.Fsm.Event(this.trueEvent);
    }
    else
    {
      if (flag || this.falseEvent == null)
        return;
      this.Fsm.Event(this.falseEvent);
    }
  }
}
