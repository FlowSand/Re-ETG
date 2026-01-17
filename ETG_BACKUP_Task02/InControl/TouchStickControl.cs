// Decompiled with JetBrains decompiler
// Type: InControl.TouchStickControl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace InControl;

public class TouchStickControl : TouchControl
{
  [SerializeField]
  [Header("Position")]
  private TouchControlAnchor anchor = TouchControlAnchor.BottomLeft;
  [SerializeField]
  private TouchUnitType offsetUnitType;
  [SerializeField]
  private Vector2 offset = new Vector2(20f, 20f);
  [SerializeField]
  private TouchUnitType areaUnitType;
  [SerializeField]
  private Rect activeArea = new Rect(0.0f, 0.0f, 50f, 100f);
  [Header("Options")]
  public TouchControl.AnalogTarget target = TouchControl.AnalogTarget.LeftStick;
  public TouchControl.SnapAngles snapAngles;
  public LockAxis lockToAxis;
  [Range(0.0f, 1f)]
  public float lowerDeadZone = 0.1f;
  [Range(0.0f, 1f)]
  public float upperDeadZone = 0.9f;
  public AnimationCurve inputCurve = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  public bool allowDragging;
  public DragAxis allowDraggingAxis;
  public bool snapToInitialTouch = true;
  public bool resetWhenDone = true;
  public float resetDuration = 0.1f;
  [Header("Sprites")]
  public TouchSprite ring = new TouchSprite(20f);
  public TouchSprite knob = new TouchSprite(10f);
  public float knobRange = 7.5f;
  private Vector3 resetPosition;
  private Vector3 beganPosition;
  private Vector3 movedPosition;
  private float ringResetSpeed;
  private float knobResetSpeed;
  private Rect worldActiveArea;
  private float worldKnobRange;
  private Vector3 value;
  private Touch currentTouch;
  private bool dirty;

  public override void CreateControl()
  {
    this.ring.Create("Ring", this.transform, 1000);
    this.knob.Create("Knob", this.transform, 1001);
  }

  public override void DestroyControl()
  {
    this.ring.Delete();
    this.knob.Delete();
    if (this.currentTouch == null)
      return;
    this.TouchEnded(this.currentTouch);
    this.currentTouch = (Touch) null;
  }

  public override void ConfigureControl()
  {
    this.resetPosition = this.OffsetToWorldPosition(this.anchor, this.offset, this.offsetUnitType, true);
    this.transform.position = this.resetPosition;
    this.ring.Update(true);
    this.knob.Update(true);
    this.worldActiveArea = TouchManager.ConvertToWorld(this.activeArea, this.areaUnitType);
    this.worldKnobRange = TouchManager.ConvertToWorld(this.knobRange, this.knob.SizeUnitType);
  }

  public override void DrawGizmos()
  {
    this.ring.DrawGizmos(this.RingPosition, Color.yellow);
    this.knob.DrawGizmos(this.KnobPosition, Color.yellow);
    Utility.DrawCircleGizmo((Vector2) this.RingPosition, this.worldKnobRange, Color.red);
    Utility.DrawRectGizmo(this.worldActiveArea, Color.green);
  }

  private void Update()
  {
    if (this.dirty)
    {
      this.ConfigureControl();
      this.dirty = false;
    }
    else
    {
      this.ring.Update();
      this.knob.Update();
    }
    if (!this.IsNotActive)
      return;
    if (this.resetWhenDone && this.KnobPosition != this.resetPosition)
    {
      Vector3 vector3 = this.KnobPosition - this.RingPosition;
      this.RingPosition = Vector3.MoveTowards(this.RingPosition, this.resetPosition, this.ringResetSpeed * Time.deltaTime);
      this.KnobPosition = this.RingPosition + vector3;
    }
    if (!(this.KnobPosition != this.RingPosition))
      return;
    this.KnobPosition = Vector3.MoveTowards(this.KnobPosition, this.RingPosition, this.knobResetSpeed * Time.deltaTime);
  }

  public override void SubmitControlState(ulong updateTick, float deltaTime)
  {
    this.SubmitAnalogValue(this.target, (Vector2) this.value, this.lowerDeadZone, this.upperDeadZone, updateTick, deltaTime);
  }

  public override void CommitControlState(ulong updateTick, float deltaTime)
  {
    this.CommitAnalog(this.target);
  }

  public override void TouchBegan(Touch touch)
  {
    if (this.IsActive)
      return;
    this.beganPosition = TouchManager.ScreenToWorldPoint(touch.position);
    bool flag1 = this.worldActiveArea.Contains(this.beganPosition);
    bool flag2 = this.ring.Contains((Vector2) this.beganPosition);
    if (this.snapToInitialTouch && (flag1 || flag2))
    {
      this.RingPosition = this.beganPosition;
      this.KnobPosition = this.beganPosition;
      this.currentTouch = touch;
    }
    else if (flag2)
    {
      this.KnobPosition = this.beganPosition;
      this.beganPosition = this.RingPosition;
      this.currentTouch = touch;
    }
    if (!this.IsActive)
      return;
    this.TouchMoved(touch);
    this.ring.State = true;
    this.knob.State = true;
  }

  public override void TouchMoved(Touch touch)
  {
    if (this.currentTouch != touch)
      return;
    this.movedPosition = TouchManager.ScreenToWorldPoint(touch.position);
    if (this.lockToAxis == LockAxis.Horizontal && this.allowDraggingAxis == DragAxis.Horizontal)
      this.movedPosition.y = this.beganPosition.y;
    else if (this.lockToAxis == LockAxis.Vertical && this.allowDraggingAxis == DragAxis.Vertical)
      this.movedPosition.x = this.beganPosition.x;
    Vector3 vector3_1 = this.movedPosition - this.beganPosition;
    Vector3 normalized = vector3_1.normalized;
    float magnitude = vector3_1.magnitude;
    if (this.allowDragging)
    {
      float num = magnitude - this.worldKnobRange;
      if ((double) num < 0.0)
        num = 0.0f;
      Vector3 vector3_2 = num * normalized;
      if (this.allowDraggingAxis == DragAxis.Horizontal)
        vector3_2.y = 0.0f;
      else if (this.allowDraggingAxis == DragAxis.Vertical)
        vector3_2.x = 0.0f;
      this.beganPosition += vector3_2;
      this.RingPosition = this.beganPosition;
    }
    this.movedPosition = this.beganPosition + Mathf.Clamp(magnitude, 0.0f, this.worldKnobRange) * normalized;
    if (this.lockToAxis == LockAxis.Horizontal)
      this.movedPosition.y = this.beganPosition.y;
    else if (this.lockToAxis == LockAxis.Vertical)
      this.movedPosition.x = this.beganPosition.x;
    if (this.snapAngles != TouchControl.SnapAngles.None)
      this.movedPosition = TouchControl.SnapTo((Vector2) (this.movedPosition - this.beganPosition), this.snapAngles) + this.beganPosition;
    this.RingPosition = this.beganPosition;
    this.KnobPosition = this.movedPosition;
    this.value = (this.movedPosition - this.beganPosition) / this.worldKnobRange;
    this.value.x = this.inputCurve.Evaluate(Utility.Abs(this.value.x)) * Mathf.Sign(this.value.x);
    this.value.y = this.inputCurve.Evaluate(Utility.Abs(this.value.y)) * Mathf.Sign(this.value.y);
  }

  public override void TouchEnded(Touch touch)
  {
    if (this.currentTouch != touch)
      return;
    this.value = Vector3.zero;
    float magnitude1 = (this.resetPosition - this.RingPosition).magnitude;
    this.ringResetSpeed = !Utility.IsZero(this.resetDuration) ? magnitude1 / this.resetDuration : magnitude1;
    float magnitude2 = (this.RingPosition - this.KnobPosition).magnitude;
    this.knobResetSpeed = !Utility.IsZero(this.resetDuration) ? magnitude2 / this.resetDuration : this.knobRange;
    this.currentTouch = (Touch) null;
    this.ring.State = false;
    this.knob.State = false;
  }

  public bool IsActive => this.currentTouch != null;

  public bool IsNotActive => this.currentTouch == null;

  public Vector3 RingPosition
  {
    get => this.ring.Ready ? this.ring.Position : this.transform.position;
    set
    {
      if (!this.ring.Ready)
        return;
      this.ring.Position = value;
    }
  }

  public Vector3 KnobPosition
  {
    get => this.knob.Ready ? this.knob.Position : this.transform.position;
    set
    {
      if (!this.knob.Ready)
        return;
      this.knob.Position = value;
    }
  }

  public TouchControlAnchor Anchor
  {
    get => this.anchor;
    set
    {
      if (this.anchor == value)
        return;
      this.anchor = value;
      this.dirty = true;
    }
  }

  public Vector2 Offset
  {
    get => this.offset;
    set
    {
      if (!(this.offset != value))
        return;
      this.offset = value;
      this.dirty = true;
    }
  }

  public TouchUnitType OffsetUnitType
  {
    get => this.offsetUnitType;
    set
    {
      if (this.offsetUnitType == value)
        return;
      this.offsetUnitType = value;
      this.dirty = true;
    }
  }

  public Rect ActiveArea
  {
    get => this.activeArea;
    set
    {
      if (!(this.activeArea != value))
        return;
      this.activeArea = value;
      this.dirty = true;
    }
  }

  public TouchUnitType AreaUnitType
  {
    get => this.areaUnitType;
    set
    {
      if (this.areaUnitType == value)
        return;
      this.areaUnitType = value;
      this.dirty = true;
    }
  }
}
