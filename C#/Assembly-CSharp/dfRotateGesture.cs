// Decompiled with JetBrains decompiler
// Type: dfRotateGesture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Input/Gestures/Rotate")]
public class dfRotateGesture : dfGestureBase
{
  [SerializeField]
  protected float thresholdAngle = 10f;
  private float accumulatedDelta;

  public event dfGestureEventHandler<dfRotateGesture> RotateGestureStart;

  public event dfGestureEventHandler<dfRotateGesture> RotateGestureUpdate;

  public event dfGestureEventHandler<dfRotateGesture> RotateGestureEnd;

  public float AngleDelta { get; protected set; }

  protected void Start()
  {
  }

  public void OnMultiTouchEnd() => this.endGesture();

  public void OnMultiTouch(dfControl sender, dfTouchEventArgs args)
  {
    List<dfTouchInfo> touches = args.Touches;
    if (this.State == dfGestureState.None || this.State == dfGestureState.Cancelled || this.State == dfGestureState.Ended)
    {
      this.State = dfGestureState.Possible;
      this.accumulatedDelta = 0.0f;
    }
    else if (this.State == dfGestureState.Possible)
    {
      if (!this.isRotateMovement(args.Touches))
        return;
      float f = this.getAngleDelta(touches) + this.accumulatedDelta;
      if ((double) Mathf.Abs(f) < (double) this.thresholdAngle)
      {
        this.accumulatedDelta = f;
      }
      else
      {
        this.State = dfGestureState.Began;
        Vector2 center = this.getCenter(touches);
        this.CurrentPosition = center;
        this.StartPosition = center;
        this.AngleDelta = f;
        if (this.RotateGestureStart != null)
          this.RotateGestureStart(this);
        this.gameObject.Signal("OnRotateGestureStart", (object) this);
      }
    }
    else
    {
      if (this.State != dfGestureState.Began && this.State != dfGestureState.Changed)
        return;
      float angleDelta = this.getAngleDelta(touches);
      if ((double) Mathf.Abs(angleDelta) <= 1.4012984643248171E-45 || (double) Mathf.Abs(angleDelta) > 22.5)
        return;
      this.State = dfGestureState.Changed;
      this.AngleDelta = angleDelta;
      this.CurrentPosition = this.getCenter(touches);
      if (this.RotateGestureUpdate != null)
        this.RotateGestureUpdate(this);
      this.gameObject.Signal("OnRotateGestureUpdate", (object) this);
    }
  }

  private float getAngleDelta(List<dfTouchInfo> touches)
  {
    if (touches.Count < 2)
      return 0.0f;
    dfTouchInfo touch1 = touches[0];
    dfTouchInfo touch2 = touches[1];
    if ((double) Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition) <= 1.4012984643248171E-45)
      return 0.0f;
    Vector2 vector2_1 = touch1.deltaPosition * (BraveTime.DeltaTime / touch1.deltaTime);
    Vector2 vector2_2 = touch2.deltaPosition * (BraveTime.DeltaTime / touch2.deltaTime);
    float f = this.deltaAngle((touch1.position - vector2_1 - (touch2.position - vector2_2)).normalized, (touch1.position - touch2.position).normalized);
    if (float.IsNaN(f))
      return 0.0f;
    if (touch1.phase == TouchPhase.Stationary || touch2.phase == TouchPhase.Stationary)
      f *= 0.5f;
    return f;
  }

  private float deltaAngle(Vector2 start, Vector2 end)
  {
    return 57.29578f * Mathf.Atan2((float) ((double) start.x * (double) end.y - (double) start.y * (double) end.x), Vector2.Dot(start, end));
  }

  private Vector2 getCenter(List<dfTouchInfo> list)
  {
    Vector2 zero = Vector2.zero;
    for (int index = 0; index < list.Count; ++index)
      zero += list[index].position;
    return zero / (float) list.Count;
  }

  private bool isRotateMovement(List<dfTouchInfo> list)
  {
    return (double) Mathf.Abs(this.getAngleDelta(list)) >= 0.10000000149011612;
  }

  private void endGesture()
  {
    this.AngleDelta = 0.0f;
    this.accumulatedDelta = 0.0f;
    if (this.State == dfGestureState.Began || this.State == dfGestureState.Changed)
    {
      this.State = dfGestureState.Ended;
      if (this.RotateGestureEnd != null)
        this.RotateGestureEnd(this);
      this.gameObject.Signal("OnRotateGestureEnd", (object) this);
    }
    else if (this.State == dfGestureState.Possible)
      this.State = dfGestureState.Cancelled;
    else
      this.State = dfGestureState.None;
  }
}
