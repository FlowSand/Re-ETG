// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.FloatChanged
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Logic)]
  [Tooltip("Tests if the value of a Float variable changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
  public class FloatChanged : FsmStateAction
  {
    [Tooltip("The Float variable to watch for a change.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmFloat floatVariable;
    [Tooltip("Event to send if the float variable changes.")]
    public FsmEvent changedEvent;
    [Tooltip("Set to True if the float variable changes.")]
    [UIHint(UIHint.Variable)]
    public FsmBool storeResult;
    private float previousValue;

    public override void Reset()
    {
      this.floatVariable = (FsmFloat) null;
      this.changedEvent = (FsmEvent) null;
      this.storeResult = (FsmBool) null;
    }

    public override void OnEnter()
    {
      if (this.floatVariable.IsNone)
        this.Finish();
      else
        this.previousValue = this.floatVariable.Value;
    }

    public override void OnUpdate()
    {
      this.storeResult.Value = false;
      if ((double) this.floatVariable.Value == (double) this.previousValue)
        return;
      this.previousValue = this.floatVariable.Value;
      this.storeResult.Value = true;
      this.Fsm.Event(this.changedEvent);
    }
  }
}
