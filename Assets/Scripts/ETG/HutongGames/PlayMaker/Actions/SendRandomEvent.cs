// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SendRandomEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.StateMachine)]
  [Tooltip("Sends a Random Event picked from an array of Events. Optionally set the relative weight of each event.")]
  public class SendRandomEvent : FsmStateAction
  {
    [CompoundArray("Events", "Event", "Weight")]
    public FsmEvent[] events;
    [HasFloatSlider(0.0f, 1f)]
    public FsmFloat[] weights;
    public FsmFloat delay;
    private DelayedEvent delayedEvent;

    public override void Reset()
    {
      this.events = new FsmEvent[3];
      this.weights = new FsmFloat[3]
      {
        (FsmFloat) 1f,
        (FsmFloat) 1f,
        (FsmFloat) 1f
      };
      this.delay = (FsmFloat) null;
    }

    public override void OnEnter()
    {
      if (this.events.Length > 0)
      {
        int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
        if (randomWeightedIndex != -1)
        {
          if ((double) this.delay.Value < 1.0 / 1000.0)
          {
            this.Fsm.Event(this.events[randomWeightedIndex]);
            this.Finish();
            return;
          }
          this.delayedEvent = this.Fsm.DelayedEvent(this.events[randomWeightedIndex], this.delay.Value);
          return;
        }
      }
      this.Finish();
    }

    public override void OnUpdate()
    {
      if (!DelayedEvent.WasSent(this.delayedEvent))
        return;
      this.Finish();
    }
  }
}
