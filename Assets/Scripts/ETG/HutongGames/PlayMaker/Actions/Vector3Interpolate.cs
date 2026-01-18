using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Vector3)]
  [HutongGames.PlayMaker.Tooltip("Interpolates between 2 Vector3 values over a specified Time.")]
  public class Vector3Interpolate : FsmStateAction
  {
    public InterpolationType mode;
    [RequiredField]
    public FsmVector3 fromVector;
    [RequiredField]
    public FsmVector3 toVector;
    [RequiredField]
    public FsmFloat time;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmVector3 storeResult;
    public FsmEvent finishEvent;
    [HutongGames.PlayMaker.Tooltip("Ignore TimeScale")]
    public bool realTime;
    private float startTime;
    private float currentTime;

    public override void Reset()
    {
      this.mode = InterpolationType.Linear;
      FsmVector3 fsmVector3_1 = new FsmVector3();
      fsmVector3_1.UseVariable = true;
      this.fromVector = fsmVector3_1;
      FsmVector3 fsmVector3_2 = new FsmVector3();
      fsmVector3_2.UseVariable = true;
      this.toVector = fsmVector3_2;
      this.time = (FsmFloat) 1f;
      this.storeResult = (FsmVector3) null;
      this.finishEvent = (FsmEvent) null;
      this.realTime = false;
    }

    public override void OnEnter()
    {
      this.startTime = FsmTime.RealtimeSinceStartup;
      this.currentTime = 0.0f;
      if (this.storeResult == null)
        this.Finish();
      else
        this.storeResult.Value = this.fromVector.Value;
    }

    public override void OnUpdate()
    {
      if (this.realTime)
        this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
      else
        this.currentTime += Time.deltaTime;
      float t = this.currentTime / this.time.Value;
      switch (this.mode)
      {
        case InterpolationType.EaseInOut:
          t = Mathf.SmoothStep(0.0f, 1f, t);
          break;
      }
      this.storeResult.Value = Vector3.Lerp(this.fromVector.Value, this.toVector.Value, t);
      if ((double) t < 1.0)
        return;
      if (this.finishEvent != null)
        this.Fsm.Event(this.finishEvent);
      this.Finish();
    }
  }
}
