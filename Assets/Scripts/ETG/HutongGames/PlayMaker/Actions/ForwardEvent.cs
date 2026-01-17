// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ForwardEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Forward an event received by this FSM to another target.")]
  [ActionCategory(ActionCategory.StateMachine)]
  public class ForwardEvent : FsmStateAction
  {
    [Tooltip("Forward to this target.")]
    public FsmEventTarget forwardTo;
    [Tooltip("The events to forward.")]
    public FsmEvent[] eventsToForward;
    [Tooltip("Should this action eat the events or pass them on.")]
    public bool eatEvents;

    public override void Reset()
    {
      this.forwardTo = new FsmEventTarget()
      {
        target = FsmEventTarget.EventTarget.FSMComponent
      };
      this.eventsToForward = (FsmEvent[]) null;
      this.eatEvents = true;
    }

    public override bool Event(FsmEvent fsmEvent)
    {
      if (this.eventsToForward != null)
      {
        foreach (FsmEvent fsmEvent1 in this.eventsToForward)
        {
          if (fsmEvent1 == fsmEvent)
          {
            this.Fsm.Event(this.forwardTo, fsmEvent);
            return this.eatEvents;
          }
        }
      }
      return false;
    }
  }
}
