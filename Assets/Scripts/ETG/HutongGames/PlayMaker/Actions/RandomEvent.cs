// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.RandomEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sends a Random State Event after an optional delay. Use this to transition to a random state from the current state.")]
  [ActionCategory(ActionCategory.StateMachine)]
  public class RandomEvent : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Delay before sending the event.")]
    [HasFloatSlider(0.0f, 10f)]
    public FsmFloat delay;
    [HutongGames.PlayMaker.Tooltip("Don't repeat the same event twice in a row.")]
    public FsmBool noRepeat;
    private DelayedEvent delayedEvent;
    private int randomEventIndex;
    private int lastEventIndex = -1;

    public override void Reset() => this.delay = (FsmFloat) null;

    public override void OnEnter()
    {
      if (this.State.Transitions.Length == 0)
        return;
      if (this.lastEventIndex == -1)
        this.lastEventIndex = Random.Range(0, this.State.Transitions.Length);
      if ((double) this.delay.Value < 1.0 / 1000.0)
      {
        this.Fsm.Event(this.GetRandomEvent());
        this.Finish();
      }
      else
        this.delayedEvent = this.Fsm.DelayedEvent(this.GetRandomEvent(), this.delay.Value);
    }

    public override void OnUpdate()
    {
      if (!DelayedEvent.WasSent(this.delayedEvent))
        return;
      this.Finish();
    }

    private FsmEvent GetRandomEvent()
    {
      do
      {
        this.randomEventIndex = Random.Range(0, this.State.Transitions.Length);
      }
      while (this.noRepeat.Value && this.State.Transitions.Length > 1 && this.randomEventIndex == this.lastEventIndex);
      this.lastEventIndex = this.randomEventIndex;
      return this.State.Transitions[this.randomEventIndex].FsmEvent;
    }
  }
}
