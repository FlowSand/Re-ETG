using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Time)]
  [HutongGames.PlayMaker.Tooltip("Delays a State from finishing by a random time. NOTE: Other actions continue, but FINISHED can't happen before Time.")]
  public class RandomWait : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Minimum amount of time to wait.")]
    [RequiredField]
    public FsmFloat min;
    [HutongGames.PlayMaker.Tooltip("Maximum amount of time to wait.")]
    [RequiredField]
    public FsmFloat max;
    [HutongGames.PlayMaker.Tooltip("Event to send when timer is finished.")]
    public FsmEvent finishEvent;
    [HutongGames.PlayMaker.Tooltip("Ignore time scale.")]
    public bool realTime;
    private float startTime;
    private float timer;
    private float time;

    public override void Reset()
    {
      this.min = (FsmFloat) 0.0f;
      this.max = (FsmFloat) 1f;
      this.finishEvent = (FsmEvent) null;
      this.realTime = false;
    }

    public override void OnEnter()
    {
      this.time = Random.Range(this.min.Value, this.max.Value);
      if ((double) this.time <= 0.0)
      {
        this.Fsm.Event(this.finishEvent);
        this.Finish();
      }
      else
      {
        this.startTime = FsmTime.RealtimeSinceStartup;
        this.timer = 0.0f;
      }
    }

    public override void OnUpdate()
    {
      if (this.realTime)
        this.timer = FsmTime.RealtimeSinceStartup - this.startTime;
      else
        this.timer += Time.deltaTime;
      if ((double) this.timer < (double) this.time)
        return;
      this.Finish();
      if (this.finishEvent == null)
        return;
      this.Fsm.Event(this.finishEvent);
    }
  }
}
