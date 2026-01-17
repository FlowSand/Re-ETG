// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.StringSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Sends an Event based on the value of a String Variable.")]
  [ActionCategory(ActionCategory.Logic)]
  public class StringSwitch : FsmStateAction
  {
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmString stringVariable;
    [CompoundArray("String Switches", "Compare String", "Send Event")]
    public FsmString[] compareTo;
    public FsmEvent[] sendEvent;
    public bool everyFrame;

    public override void Reset()
    {
      this.stringVariable = (FsmString) null;
      this.compareTo = new FsmString[1];
      this.sendEvent = new FsmEvent[1];
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoStringSwitch();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoStringSwitch();

    private void DoStringSwitch()
    {
      if (this.stringVariable.IsNone)
        return;
      for (int index = 0; index < this.compareTo.Length; ++index)
      {
        if (this.stringVariable.Value == this.compareTo[index].Value)
        {
          this.Fsm.Event(this.sendEvent[index]);
          break;
        }
      }
    }
  }
}
