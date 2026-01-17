// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.IntChanged
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Logic)]
[Tooltip("Tests if the value of an integer variable changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
public class IntChanged : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmInt intVariable;
  public FsmEvent changedEvent;
  [UIHint(UIHint.Variable)]
  public FsmBool storeResult;
  private int previousValue;

  public override void Reset()
  {
    this.intVariable = (FsmInt) null;
    this.changedEvent = (FsmEvent) null;
    this.storeResult = (FsmBool) null;
  }

  public override void OnEnter()
  {
    if (this.intVariable.IsNone)
      this.Finish();
    else
      this.previousValue = this.intVariable.Value;
  }

  public override void OnUpdate()
  {
    this.storeResult.Value = false;
    if (this.intVariable.Value == this.previousValue)
      return;
    this.previousValue = this.intVariable.Value;
    this.storeResult.Value = true;
    this.Fsm.Event(this.changedEvent);
  }
}
