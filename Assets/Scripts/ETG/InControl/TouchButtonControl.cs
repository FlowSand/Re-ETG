using UnityEngine;

#nullable disable
namespace InControl
{
  public class TouchButtonControl : TouchControl
  {
    [Header("Position")]
    [SerializeField]
    private TouchControlAnchor anchor = TouchControlAnchor.BottomRight;
    [SerializeField]
    private TouchUnitType offsetUnitType;
    [SerializeField]
    private Vector2 offset = new Vector2(-10f, 10f);
    [SerializeField]
    private bool lockAspectRatio = true;
    [Header("Options")]
    public TouchControl.ButtonTarget target = TouchControl.ButtonTarget.Action1;
    public bool allowSlideToggle = true;
    public bool toggleOnLeave;
    public bool pressureSensitive;
    [Header("Sprites")]
    public TouchSprite button = new TouchSprite(15f);
    private bool buttonState;
    private Touch currentTouch;
    private bool dirty;

    public override void CreateControl() => this.button.Create("Button", this.transform, 1000);

    public override void DestroyControl()
    {
      this.button.Delete();
      if (this.currentTouch == null)
        return;
      this.TouchEnded(this.currentTouch);
      this.currentTouch = (Touch) null;
    }

    public override void ConfigureControl()
    {
      this.transform.position = this.OffsetToWorldPosition(this.anchor, this.offset, this.offsetUnitType, this.lockAspectRatio);
      this.button.Update(true);
    }

    public override void DrawGizmos() => this.button.DrawGizmos(this.ButtonPosition, Color.yellow);

    private void Update()
    {
      if (this.dirty)
      {
        this.ConfigureControl();
        this.dirty = false;
      }
      else
        this.button.Update();
    }

    public override void SubmitControlState(ulong updateTick, float deltaTime)
    {
      if (this.pressureSensitive)
      {
        float v0 = 0.0f;
        if (this.currentTouch == null)
        {
          if (this.allowSlideToggle)
          {
            int touchCount = TouchManager.TouchCount;
            for (int touchIndex = 0; touchIndex < touchCount; ++touchIndex)
            {
              Touch touch = TouchManager.GetTouch(touchIndex);
              if (this.button.Contains(touch))
                v0 = Utility.Max(v0, touch.normalizedPressure);
            }
          }
        }
        else
          v0 = this.currentTouch.normalizedPressure;
        this.ButtonState = (double) v0 > 0.0;
        this.SubmitButtonValue(this.target, v0, updateTick, deltaTime);
      }
      else
      {
        if (this.currentTouch == null && this.allowSlideToggle)
        {
          this.ButtonState = false;
          int touchCount = TouchManager.TouchCount;
          for (int touchIndex = 0; touchIndex < touchCount; ++touchIndex)
            this.ButtonState = this.ButtonState || this.button.Contains(TouchManager.GetTouch(touchIndex));
        }
        this.SubmitButtonState(this.target, this.ButtonState, updateTick, deltaTime);
      }
    }

    public override void CommitControlState(ulong updateTick, float deltaTime)
    {
      this.CommitButton(this.target);
    }

    public override void TouchBegan(Touch touch)
    {
      if (this.currentTouch != null || !this.button.Contains(touch))
        return;
      this.ButtonState = true;
      this.currentTouch = touch;
    }

    public override void TouchMoved(Touch touch)
    {
      if (this.currentTouch != touch || !this.toggleOnLeave || this.button.Contains(touch))
        return;
      this.ButtonState = false;
      this.currentTouch = (Touch) null;
    }

    public override void TouchEnded(Touch touch)
    {
      if (this.currentTouch != touch)
        return;
      this.ButtonState = false;
      this.currentTouch = (Touch) null;
    }

    private bool ButtonState
    {
      get => this.buttonState;
      set
      {
        if (this.buttonState == value)
          return;
        this.buttonState = value;
        this.button.State = value;
      }
    }

    public Vector3 ButtonPosition
    {
      get => this.button.Ready ? this.button.Position : this.transform.position;
      set
      {
        if (!this.button.Ready)
          return;
        this.button.Position = value;
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
  }
}
