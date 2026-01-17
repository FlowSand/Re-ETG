// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.FloatInterpolate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Interpolates between 2 Float values over a specified Time.")]
[ActionCategory(ActionCategory.Math)]
public class FloatInterpolate : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Interpolation mode: Linear or EaseInOut.")]
  public InterpolationType mode;
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("Interpolate from this value.")]
  public FsmFloat fromFloat;
  [HutongGames.PlayMaker.Tooltip("Interpolate to this value.")]
  [RequiredField]
  public FsmFloat toFloat;
  [HutongGames.PlayMaker.Tooltip("Interpolate over this amount of time in seconds.")]
  [RequiredField]
  public FsmFloat time;
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("Store the current value in a float variable.")]
  [UIHint(UIHint.Variable)]
  public FsmFloat storeResult;
  [HutongGames.PlayMaker.Tooltip("Event to send when the interpolation is finished.")]
  public FsmEvent finishEvent;
  [HutongGames.PlayMaker.Tooltip("Ignore TimeScale. Useful if the game is paused (Time scaled to 0).")]
  public bool realTime;
  private float startTime;
  private float currentTime;

  public override void Reset()
  {
    this.mode = InterpolationType.Linear;
    this.fromFloat = (FsmFloat) null;
    this.toFloat = (FsmFloat) null;
    this.time = (FsmFloat) 1f;
    this.storeResult = (FsmFloat) null;
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
      this.storeResult.Value = this.fromFloat.Value;
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
      case InterpolationType.Linear:
        this.storeResult.Value = Mathf.Lerp(this.fromFloat.Value, this.toFloat.Value, t);
        break;
      case InterpolationType.EaseInOut:
        this.storeResult.Value = Mathf.SmoothStep(this.fromFloat.Value, this.toFloat.Value, t);
        break;
    }
    if ((double) t <= 1.0)
      return;
    if (this.finishEvent != null)
      this.Fsm.Event(this.finishEvent);
    this.Finish();
  }
}
