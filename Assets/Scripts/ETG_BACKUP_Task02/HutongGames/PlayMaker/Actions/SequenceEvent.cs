// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SequenceEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Sends the next event on the state each time the state is entered.")]
[ActionCategory(ActionCategory.StateMachine)]
public class SequenceEvent : FsmStateAction
{
  [HasFloatSlider(0.0f, 10f)]
  public FsmFloat delay;
  private DelayedEvent delayedEvent;
  private int eventIndex;

  public override void Reset() => this.delay = (FsmFloat) null;

  public override void OnEnter()
  {
    int length = this.State.Transitions.Length;
    if (length <= 0)
      return;
    FsmEvent fsmEvent = this.State.Transitions[this.eventIndex].FsmEvent;
    if ((double) this.delay.Value < 1.0 / 1000.0)
    {
      this.Fsm.Event(fsmEvent);
      this.Finish();
    }
    else
      this.delayedEvent = this.Fsm.DelayedEvent(fsmEvent, this.delay.Value);
    ++this.eventIndex;
    if (this.eventIndex != length)
      return;
    this.eventIndex = 0;
  }

  public override void OnUpdate()
  {
    if (!DelayedEvent.WasSent(this.delayedEvent))
      return;
    this.Finish();
  }
}
