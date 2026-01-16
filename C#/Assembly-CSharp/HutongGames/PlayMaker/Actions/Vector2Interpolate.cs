// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Vector2Interpolate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Interpolates between 2 Vector2 values over a specified Time.")]
[ActionCategory(ActionCategory.Vector2)]
public class Vector2Interpolate : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The interpolation type")]
  public InterpolationType mode;
  [HutongGames.PlayMaker.Tooltip("The vector to interpolate from")]
  [RequiredField]
  public FsmVector2 fromVector;
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The vector to interpolate to")]
  public FsmVector2 toVector;
  [HutongGames.PlayMaker.Tooltip("the interpolate time")]
  [RequiredField]
  public FsmFloat time;
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("the interpolated result")]
  [UIHint(UIHint.Variable)]
  public FsmVector2 storeResult;
  [HutongGames.PlayMaker.Tooltip("This event is fired when the interpolation is done.")]
  public FsmEvent finishEvent;
  [HutongGames.PlayMaker.Tooltip("Ignore TimeScale")]
  public bool realTime;
  private float startTime;
  private float currentTime;

  public override void Reset()
  {
    this.mode = InterpolationType.Linear;
    FsmVector2 fsmVector2_1 = new FsmVector2();
    fsmVector2_1.UseVariable = true;
    this.fromVector = fsmVector2_1;
    FsmVector2 fsmVector2_2 = new FsmVector2();
    fsmVector2_2.UseVariable = true;
    this.toVector = fsmVector2_2;
    this.time = (FsmFloat) 1f;
    this.storeResult = (FsmVector2) null;
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
    this.storeResult.Value = Vector2.Lerp(this.fromVector.Value, this.toVector.Value, t);
    if ((double) t <= 1.0)
      return;
    if (this.finishEvent != null)
      this.Fsm.Event(this.finishEvent);
    this.Finish();
  }
}
