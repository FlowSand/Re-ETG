// Decompiled with JetBrains decompiler
// Type: InControl.Touch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace InControl
{
  public class Touch
  {
    public static readonly int FingerID_None = -1;
    public static readonly int FingerID_Mouse = -2;
    public int fingerId;
    public TouchPhase phase;
    public int tapCount;
    public Vector2 position;
    public Vector2 deltaPosition;
    public Vector2 lastPosition;
    public float deltaTime;
    public ulong updateTick;
    public TouchType type;
    public float altitudeAngle;
    public float azimuthAngle;
    public float maximumPossiblePressure;
    public float pressure;
    public float radius;
    public float radiusVariance;

    internal Touch()
    {
      this.fingerId = Touch.FingerID_None;
      this.phase = TouchPhase.Ended;
    }

    internal void Reset()
    {
      this.fingerId = Touch.FingerID_None;
      this.phase = TouchPhase.Ended;
      this.tapCount = 0;
      this.position = Vector2.zero;
      this.deltaPosition = Vector2.zero;
      this.lastPosition = Vector2.zero;
      this.deltaTime = 0.0f;
      this.updateTick = 0UL;
      this.type = TouchType.Direct;
      this.altitudeAngle = 0.0f;
      this.azimuthAngle = 0.0f;
      this.maximumPossiblePressure = 1f;
      this.pressure = 0.0f;
      this.radius = 0.0f;
      this.radiusVariance = 0.0f;
    }

    public float normalizedPressure
    {
      get => Mathf.Clamp(this.pressure / this.maximumPossiblePressure, 1f / 1000f, 1f);
    }

    internal void SetWithTouchData(UnityEngine.Touch touch, ulong updateTick, float deltaTime)
    {
      this.phase = touch.phase;
      this.tapCount = touch.tapCount;
      this.altitudeAngle = touch.altitudeAngle;
      this.azimuthAngle = touch.azimuthAngle;
      this.maximumPossiblePressure = touch.maximumPossiblePressure;
      this.pressure = touch.pressure;
      this.radius = touch.radius;
      this.radiusVariance = touch.radiusVariance;
      Vector2 position = touch.position;
      if ((double) position.x < 0.0)
        position.x = (float) Screen.width + position.x;
      if (this.phase == TouchPhase.Began)
      {
        this.deltaPosition = Vector2.zero;
        this.lastPosition = position;
        this.position = position;
      }
      else
      {
        if (this.phase == TouchPhase.Stationary)
          this.phase = TouchPhase.Moved;
        this.deltaPosition = position - this.lastPosition;
        this.lastPosition = this.position;
        this.position = position;
      }
      this.deltaTime = deltaTime;
      this.updateTick = updateTick;
    }

    internal bool SetWithMouseData(ulong updateTick, float deltaTime)
    {
      if (Input.touchCount > 0)
        return false;
      Vector2 vector2 = new Vector2(Mathf.Round(Input.mousePosition.x), Mathf.Round(Input.mousePosition.y));
      if (Input.GetMouseButtonDown(0))
      {
        this.phase = TouchPhase.Began;
        this.pressure = 1f;
        this.maximumPossiblePressure = 1f;
        this.tapCount = 1;
        this.type = TouchType.Mouse;
        this.deltaPosition = Vector2.zero;
        this.lastPosition = vector2;
        this.position = vector2;
        this.deltaTime = deltaTime;
        this.updateTick = updateTick;
        return true;
      }
      if (Input.GetMouseButtonUp(0))
      {
        this.phase = TouchPhase.Ended;
        this.pressure = 0.0f;
        this.maximumPossiblePressure = 1f;
        this.tapCount = 1;
        this.type = TouchType.Mouse;
        this.deltaPosition = vector2 - this.lastPosition;
        this.lastPosition = this.position;
        this.position = vector2;
        this.deltaTime = deltaTime;
        this.updateTick = updateTick;
        return true;
      }
      if (!Input.GetMouseButton(0))
        return false;
      this.phase = TouchPhase.Moved;
      this.pressure = 1f;
      this.maximumPossiblePressure = 1f;
      this.tapCount = 1;
      this.type = TouchType.Mouse;
      this.deltaPosition = vector2 - this.lastPosition;
      this.lastPosition = this.position;
      this.position = vector2;
      this.deltaTime = deltaTime;
      this.updateTick = updateTick;
      return true;
    }
  }
}
