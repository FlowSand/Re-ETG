// Decompiled with JetBrains decompiler
// Type: InControl.TouchTrackControl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace InControl
{
  public class TouchTrackControl : TouchControl
  {
    [Header("Dimensions")]
    [SerializeField]
    private TouchUnitType areaUnitType;
    [SerializeField]
    private Rect activeArea = new Rect(25f, 25f, 50f, 50f);
    [Header("Analog Target")]
    public TouchControl.AnalogTarget target = TouchControl.AnalogTarget.LeftStick;
    public float scale = 1f;
    [Header("Button Target")]
    public TouchControl.ButtonTarget tapTarget;
    public float maxTapDuration = 0.5f;
    public float maxTapMovement = 1f;
    private Rect worldActiveArea;
    private Vector3 lastPosition;
    private Vector3 thisPosition;
    private Touch currentTouch;
    private bool dirty;
    private bool fireButtonTarget;
    private float beganTime;
    private Vector3 beganPosition;

    public override void CreateControl() => this.ConfigureControl();

    public override void DestroyControl()
    {
      if (this.currentTouch == null)
        return;
      this.TouchEnded(this.currentTouch);
      this.currentTouch = (Touch) null;
    }

    public override void ConfigureControl()
    {
      this.worldActiveArea = TouchManager.ConvertToWorld(this.activeArea, this.areaUnitType);
    }

    public override void DrawGizmos() => Utility.DrawRectGizmo(this.worldActiveArea, Color.yellow);

    private void OnValidate()
    {
      if ((double) this.maxTapDuration >= 0.0)
        return;
      this.maxTapDuration = 0.0f;
    }

    private void Update()
    {
      if (!this.dirty)
        return;
      this.ConfigureControl();
      this.dirty = false;
    }

    public override void SubmitControlState(ulong updateTick, float deltaTime)
    {
      this.SubmitRawAnalogValue(this.target, (Vector2) ((this.thisPosition - this.lastPosition) * this.scale), updateTick, deltaTime);
      this.lastPosition = this.thisPosition;
      this.SubmitButtonState(this.tapTarget, this.fireButtonTarget, updateTick, deltaTime);
      this.fireButtonTarget = false;
    }

    public override void CommitControlState(ulong updateTick, float deltaTime)
    {
      this.CommitAnalog(this.target);
      this.CommitButton(this.tapTarget);
    }

    public override void TouchBegan(Touch touch)
    {
      if (this.currentTouch != null)
        return;
      this.beganPosition = TouchManager.ScreenToWorldPoint(touch.position);
      if (!this.worldActiveArea.Contains(this.beganPosition))
        return;
      this.thisPosition = TouchManager.ScreenToViewPoint(touch.position * 100f);
      this.lastPosition = this.thisPosition;
      this.currentTouch = touch;
      this.beganTime = Time.realtimeSinceStartup;
    }

    public override void TouchMoved(Touch touch)
    {
      if (this.currentTouch != touch)
        return;
      this.thisPosition = TouchManager.ScreenToViewPoint(touch.position * 100f);
    }

    public override void TouchEnded(Touch touch)
    {
      if (this.currentTouch != touch)
        return;
      Vector3 vector3 = TouchManager.ScreenToWorldPoint(touch.position) - this.beganPosition;
      float num = Time.realtimeSinceStartup - this.beganTime;
      if ((double) vector3.magnitude <= (double) this.maxTapMovement && (double) num <= (double) this.maxTapDuration && this.tapTarget != TouchControl.ButtonTarget.None)
        this.fireButtonTarget = true;
      this.thisPosition = Vector3.zero;
      this.lastPosition = Vector3.zero;
      this.currentTouch = (Touch) null;
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
}
