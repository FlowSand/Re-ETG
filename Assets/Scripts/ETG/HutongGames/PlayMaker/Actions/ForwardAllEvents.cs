// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ForwardAllEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Forwards all event received by this FSM to another target. Optionally specify a list of events to ignore.")]
  [ActionCategory(ActionCategory.StateMachine)]
  public class ForwardAllEvents : FsmStateAction
  {
    [Tooltip("Forward to this target.")]
    public FsmEventTarget forwardTo;
    [Tooltip("Don't forward these events.")]
    public FsmEvent[] exceptThese;
    [Tooltip("Should this action eat the events or pass them on.")]
    public bool eatEvents;

    public override void Reset()
    {
      this.forwardTo = new FsmEventTarget()
      {
        target = FsmEventTarget.EventTarget.FSMComponent
      };
      this.exceptThese = new FsmEvent[1]{ FsmEvent.Finished };
      this.eatEvents = true;
    }

    public override bool Event(FsmEvent fsmEvent)
    {
      if (this.exceptThese != null)
      {
        foreach (FsmEvent fsmEvent1 in this.exceptThese)
        {
          if (fsmEvent1 == fsmEvent)
            return false;
        }
      }
      this.Fsm.Event(this.forwardTo, fsmEvent);
      return this.eatEvents;
    }
  }
}
