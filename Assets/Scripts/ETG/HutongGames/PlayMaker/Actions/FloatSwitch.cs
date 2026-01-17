// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.FloatSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Logic)]
[Tooltip("Sends an Event based on the value of a Float Variable. The float could represent distance, angle to a target, health left... The array sets up float ranges that correspond to Events.")]
public class FloatSwitch : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  [Tooltip("The float variable to test.")]
  [RequiredField]
  public FsmFloat floatVariable;
  [CompoundArray("Float Switches", "Less Than", "Send Event")]
  public FsmFloat[] lessThan;
  public FsmEvent[] sendEvent;
  [Tooltip("Repeat every frame. Useful if the variable is changing.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.floatVariable = (FsmFloat) null;
    this.lessThan = new FsmFloat[1];
    this.sendEvent = new FsmEvent[1];
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoFloatSwitch();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoFloatSwitch();

  private void DoFloatSwitch()
  {
    if (this.floatVariable.IsNone)
      return;
    for (int index = 0; index < this.lessThan.Length; ++index)
    {
      if ((double) this.floatVariable.Value < (double) this.lessThan[index].Value)
      {
        this.Fsm.Event(this.sendEvent[index]);
        break;
      }
    }
  }
}
