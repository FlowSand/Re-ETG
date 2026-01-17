// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.EnumCompare
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Logic)]
[Tooltip("Compares 2 Enum values and sends Events based on the result.")]
public class EnumCompare : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmEnum enumVariable;
  [MatchFieldType("enumVariable")]
  public FsmEnum compareTo;
  public FsmEvent equalEvent;
  public FsmEvent notEqualEvent;
  [Tooltip("Store the true/false result in a bool variable.")]
  [UIHint(UIHint.Variable)]
  public FsmBool storeResult;
  [Tooltip("Repeat every frame. Useful if the enum is changing over time.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.enumVariable = (FsmEnum) null;
    this.compareTo = (FsmEnum) null;
    this.equalEvent = (FsmEvent) null;
    this.notEqualEvent = (FsmEvent) null;
    this.storeResult = (FsmBool) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoEnumCompare();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoEnumCompare();

  private void DoEnumCompare()
  {
    if (this.enumVariable == null || this.compareTo == null)
      return;
    bool flag = object.Equals((object) this.enumVariable.Value, (object) this.compareTo.Value);
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
