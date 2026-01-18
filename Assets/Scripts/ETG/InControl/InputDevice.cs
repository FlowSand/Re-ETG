using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

#nullable disable
namespace InControl
{
  public class InputDevice
  {
    public static readonly InputDevice Null = new InputDevice("None");
    private GameOptions.ControllerSymbology m_controllerSymbology = GameOptions.ControllerSymbology.Xbox;
    private List<InputControl> controls;
    public bool Passive;
    private InputControl cachedLeftStickUp;
    private InputControl cachedLeftStickDown;
    private InputControl cachedLeftStickLeft;
    private InputControl cachedLeftStickRight;
    private InputControl cachedRightStickUp;
    private InputControl cachedRightStickDown;
    private InputControl cachedRightStickLeft;
    private InputControl cachedRightStickRight;
    private InputControl cachedDPadUp;
    private InputControl cachedDPadDown;
    private InputControl cachedDPadLeft;
    private InputControl cachedDPadRight;
    private InputControl cachedAction1;
    private InputControl cachedAction2;
    private InputControl cachedAction3;
    private InputControl cachedAction4;
    private InputControl cachedLeftTrigger;
    private InputControl cachedRightTrigger;
    private InputControl cachedLeftBumper;
    private InputControl cachedRightBumper;
    private InputControl cachedLeftStickButton;
    private InputControl cachedRightStickButton;
    private InputControl cachedLeftStickX;
    private InputControl cachedLeftStickY;
    private InputControl cachedRightStickX;
    private InputControl cachedRightStickY;
    private InputControl cachedDPadX;
    private InputControl cachedDPadY;
    private InputControl cachedCommand;

    public InputDevice()
      : this(string.Empty)
    {
    }

    public InputDevice(string name)
      : this(name, false)
    {
    }

    public InputDevice(string name, bool rawSticks)
    {
      this.Name = name;
      this.RawSticks = rawSticks;
      this.Meta = string.Empty;
      this.GUID = Guid.NewGuid();
      this.LastChangeTick = 0UL;
      this.LastChangeTime = 0.0f;
      this.SortOrder = int.MaxValue;
      this.DeviceClass = InputDeviceClass.Unknown;
      this.DeviceStyle = InputDeviceStyle.Unknown;
      this.Passive = false;
      this.ControlsByTarget = new InputControl[521];
      this.controls = new List<InputControl>(32 /*0x20*/);
      this.Controls = new ReadOnlyCollection<InputControl>((IList<InputControl>) this.controls);
      this.RemoveAliasControls();
    }

    public string Name { get; protected set; }

    public string Meta { get; protected set; }

    public int SortOrder { get; protected set; }

    public InputDeviceClass DeviceClass { get; protected set; }

    public InputDeviceStyle DeviceStyle { get; protected set; }

    public Guid GUID { get; private set; }

    public GameOptions.ControllerSymbology ControllerSymbology
    {
      get => this.m_controllerSymbology;
      protected set => this.m_controllerSymbology = value;
    }

    public ulong LastChangeTick { get; protected set; }

    public float LastChangeTime { get; protected set; }

    public bool IsAttached { get; private set; }

    protected bool RawSticks { get; private set; }

    public ReadOnlyCollection<InputControl> Controls { get; protected set; }

    protected InputControl[] ControlsByTarget { get; private set; }

    public TwoAxisInputControl LeftStick { get; private set; }

    public TwoAxisInputControl RightStick { get; private set; }

    public TwoAxisInputControl DPad { get; private set; }

    protected InputDevice.AnalogSnapshotEntry[] AnalogSnapshot { get; set; }

    internal void OnAttached()
    {
      this.IsAttached = true;
      this.AddAliasControls();
    }

    internal void OnDetached()
    {
      this.IsAttached = false;
      this.StopVibration();
      this.RemoveAliasControls();
    }

    private void AddAliasControls()
    {
      this.RemoveAliasControls();
      if (!this.IsKnown)
        return;
      this.LeftStick = new TwoAxisInputControl();
      this.RightStick = new TwoAxisInputControl();
      this.DPad = new TwoAxisInputControl();
      this.AddControl(InputControlType.LeftStickX, "Left Stick X");
      this.AddControl(InputControlType.LeftStickY, "Left Stick Y");
      this.AddControl(InputControlType.RightStickX, "Right Stick X");
      this.AddControl(InputControlType.RightStickY, "Right Stick Y");
      this.AddControl(InputControlType.DPadX, "DPad X");
      this.AddControl(InputControlType.DPadY, "DPad Y");
      this.AddControl(InputControlType.Command, "Command");
      this.ExpireControlCache();
    }

    private void RemoveAliasControls()
    {
      this.LeftStick = TwoAxisInputControl.Null;
      this.RightStick = TwoAxisInputControl.Null;
      this.DPad = TwoAxisInputControl.Null;
      this.RemoveControl(InputControlType.LeftStickX);
      this.RemoveControl(InputControlType.LeftStickY);
      this.RemoveControl(InputControlType.RightStickX);
      this.RemoveControl(InputControlType.RightStickY);
      this.RemoveControl(InputControlType.DPadX);
      this.RemoveControl(InputControlType.DPadY);
      this.RemoveControl(InputControlType.Command);
      this.ExpireControlCache();
    }

    protected void ClearControls()
    {
      Array.Clear((Array) this.ControlsByTarget, 0, this.ControlsByTarget.Length);
      this.controls.Clear();
      this.ExpireControlCache();
    }

    public bool HasControl(InputControlType controlType)
    {
      return this.ControlsByTarget[(int) controlType] != null;
    }

    public InputControl GetControl(InputControlType controlType)
    {
      return this.ControlsByTarget[(int) controlType] ?? InputControl.Null;
    }

    public InputControl this[InputControlType controlType] => this.GetControl(controlType);

    public static InputControlType GetInputControlTypeByName(string inputControlName)
    {
      return (InputControlType) Enum.Parse(typeof (InputControlType), inputControlName);
    }

    public InputControl GetControlByName(string controlName)
    {
      return this.GetControl(InputDevice.GetInputControlTypeByName(controlName));
    }

    public InputControl AddControl(InputControlType controlType, string handle)
    {
      InputControl inputControl = this.ControlsByTarget[(int) controlType];
      if (inputControl == null)
      {
        inputControl = new InputControl(handle, controlType);
        this.ControlsByTarget[(int) controlType] = inputControl;
        this.controls.Add(inputControl);
        this.ExpireControlCache();
      }
      return inputControl;
    }

    public InputControl AddControl(
      InputControlType controlType,
      string handle,
      float lowerDeadZone,
      float upperDeadZone)
    {
      InputControl inputControl = this.AddControl(controlType, handle);
      inputControl.LowerDeadZone = lowerDeadZone;
      inputControl.UpperDeadZone = upperDeadZone;
      return inputControl;
    }

    private void RemoveControl(InputControlType controlType)
    {
      InputControl inputControl = this.ControlsByTarget[(int) controlType];
      if (inputControl == null)
        return;
      this.ControlsByTarget[(int) controlType] = (InputControl) null;
      this.controls.Remove(inputControl);
      this.ExpireControlCache();
    }

    public void ClearInputState()
    {
      this.LeftStick.ClearInputState();
      this.RightStick.ClearInputState();
      this.DPad.ClearInputState();
      int count = this.Controls.Count;
      for (int index = 0; index < count; ++index)
        this.Controls[index]?.ClearInputState();
    }

    protected void UpdateWithState(
      InputControlType controlType,
      bool state,
      ulong updateTick,
      float deltaTime)
    {
      this.GetControl(controlType).UpdateWithState(state, updateTick, deltaTime);
    }

    protected void UpdateWithValue(
      InputControlType controlType,
      float value,
      ulong updateTick,
      float deltaTime)
    {
      this.GetControl(controlType).UpdateWithValue(value, updateTick, deltaTime);
    }

    internal void UpdateLeftStickWithValue(Vector2 value, ulong updateTick, float deltaTime)
    {
      this.LeftStickLeft.UpdateWithValue(Mathf.Max(0.0f, -value.x), updateTick, deltaTime);
      this.LeftStickRight.UpdateWithValue(Mathf.Max(0.0f, value.x), updateTick, deltaTime);
      if (InputManager.InvertYAxis)
      {
        this.LeftStickUp.UpdateWithValue(Mathf.Max(0.0f, -value.y), updateTick, deltaTime);
        this.LeftStickDown.UpdateWithValue(Mathf.Max(0.0f, value.y), updateTick, deltaTime);
      }
      else
      {
        this.LeftStickUp.UpdateWithValue(Mathf.Max(0.0f, value.y), updateTick, deltaTime);
        this.LeftStickDown.UpdateWithValue(Mathf.Max(0.0f, -value.y), updateTick, deltaTime);
      }
    }

    internal void UpdateLeftStickWithRawValue(Vector2 value, ulong updateTick, float deltaTime)
    {
      this.LeftStickLeft.UpdateWithRawValue(Mathf.Max(0.0f, -value.x), updateTick, deltaTime);
      this.LeftStickRight.UpdateWithRawValue(Mathf.Max(0.0f, value.x), updateTick, deltaTime);
      if (InputManager.InvertYAxis)
      {
        this.LeftStickUp.UpdateWithRawValue(Mathf.Max(0.0f, -value.y), updateTick, deltaTime);
        this.LeftStickDown.UpdateWithRawValue(Mathf.Max(0.0f, value.y), updateTick, deltaTime);
      }
      else
      {
        this.LeftStickUp.UpdateWithRawValue(Mathf.Max(0.0f, value.y), updateTick, deltaTime);
        this.LeftStickDown.UpdateWithRawValue(Mathf.Max(0.0f, -value.y), updateTick, deltaTime);
      }
    }

    internal void CommitLeftStick()
    {
      this.LeftStickUp.Commit();
      this.LeftStickDown.Commit();
      this.LeftStickLeft.Commit();
      this.LeftStickRight.Commit();
    }

    internal void UpdateRightStickWithValue(Vector2 value, ulong updateTick, float deltaTime)
    {
      this.RightStickLeft.UpdateWithValue(Mathf.Max(0.0f, -value.x), updateTick, deltaTime);
      this.RightStickRight.UpdateWithValue(Mathf.Max(0.0f, value.x), updateTick, deltaTime);
      if (InputManager.InvertYAxis)
      {
        this.RightStickUp.UpdateWithValue(Mathf.Max(0.0f, -value.y), updateTick, deltaTime);
        this.RightStickDown.UpdateWithValue(Mathf.Max(0.0f, value.y), updateTick, deltaTime);
      }
      else
      {
        this.RightStickUp.UpdateWithValue(Mathf.Max(0.0f, value.y), updateTick, deltaTime);
        this.RightStickDown.UpdateWithValue(Mathf.Max(0.0f, -value.y), updateTick, deltaTime);
      }
    }

    internal void UpdateRightStickWithRawValue(Vector2 value, ulong updateTick, float deltaTime)
    {
      this.RightStickLeft.UpdateWithRawValue(Mathf.Max(0.0f, -value.x), updateTick, deltaTime);
      this.RightStickRight.UpdateWithRawValue(Mathf.Max(0.0f, value.x), updateTick, deltaTime);
      if (InputManager.InvertYAxis)
      {
        this.RightStickUp.UpdateWithRawValue(Mathf.Max(0.0f, -value.y), updateTick, deltaTime);
        this.RightStickDown.UpdateWithRawValue(Mathf.Max(0.0f, value.y), updateTick, deltaTime);
      }
      else
      {
        this.RightStickUp.UpdateWithRawValue(Mathf.Max(0.0f, value.y), updateTick, deltaTime);
        this.RightStickDown.UpdateWithRawValue(Mathf.Max(0.0f, -value.y), updateTick, deltaTime);
      }
    }

    internal void CommitRightStick()
    {
      this.RightStickUp.Commit();
      this.RightStickDown.Commit();
      this.RightStickLeft.Commit();
      this.RightStickRight.Commit();
    }

    public virtual void Update(ulong updateTick, float deltaTime)
    {
    }

    private bool AnyCommandControlIsPressed()
    {
      for (int index = 100; index <= 113; ++index)
      {
        InputControl inputControl = this.ControlsByTarget[index];
        if (inputControl != null && inputControl.IsPressed)
          return true;
      }
      return false;
    }

    private void ProcessLeftStick(ulong updateTick, float deltaTime)
    {
      float x = Utility.ValueFromSides(this.LeftStickLeft.NextRawValue, this.LeftStickRight.NextRawValue);
      float y = Utility.ValueFromSides(this.LeftStickDown.NextRawValue, this.LeftStickUp.NextRawValue, InputManager.InvertYAxis);
      Vector2 vector2;
      if (this.RawSticks || this.LeftStickLeft.Raw || this.LeftStickRight.Raw || this.LeftStickUp.Raw || this.LeftStickDown.Raw)
      {
        vector2 = new Vector2(x, y);
      }
      else
      {
        float lowerDeadZone = Utility.Max(this.LeftStickLeft.LowerDeadZone, this.LeftStickRight.LowerDeadZone, this.LeftStickUp.LowerDeadZone, this.LeftStickDown.LowerDeadZone);
        float upperDeadZone = Utility.Min(this.LeftStickLeft.UpperDeadZone, this.LeftStickRight.UpperDeadZone, this.LeftStickUp.UpperDeadZone, this.LeftStickDown.UpperDeadZone);
        vector2 = Utility.ApplyCircularDeadZone(x, y, lowerDeadZone, upperDeadZone);
      }
      this.LeftStick.Raw = true;
      this.LeftStick.UpdateWithAxes(vector2.x, vector2.y, updateTick, deltaTime);
      this.LeftStickX.Raw = true;
      this.LeftStickX.CommitWithValue(vector2.x, updateTick, deltaTime);
      this.LeftStickY.Raw = true;
      this.LeftStickY.CommitWithValue(vector2.y, updateTick, deltaTime);
      this.LeftStickLeft.SetValue(this.LeftStick.Left.Value, updateTick);
      this.LeftStickRight.SetValue(this.LeftStick.Right.Value, updateTick);
      this.LeftStickUp.SetValue(this.LeftStick.Up.Value, updateTick);
      this.LeftStickDown.SetValue(this.LeftStick.Down.Value, updateTick);
    }

    private void ProcessRightStick(ulong updateTick, float deltaTime)
    {
      float x = Utility.ValueFromSides(this.RightStickLeft.NextRawValue, this.RightStickRight.NextRawValue);
      float y = Utility.ValueFromSides(this.RightStickDown.NextRawValue, this.RightStickUp.NextRawValue, InputManager.InvertYAxis);
      Vector2 vector2;
      if (this.RawSticks || this.RightStickLeft.Raw || this.RightStickRight.Raw || this.RightStickUp.Raw || this.RightStickDown.Raw)
      {
        vector2 = new Vector2(x, y);
      }
      else
      {
        float lowerDeadZone = Utility.Max(this.RightStickLeft.LowerDeadZone, this.RightStickRight.LowerDeadZone, this.RightStickUp.LowerDeadZone, this.RightStickDown.LowerDeadZone);
        float upperDeadZone = Utility.Min(this.RightStickLeft.UpperDeadZone, this.RightStickRight.UpperDeadZone, this.RightStickUp.UpperDeadZone, this.RightStickDown.UpperDeadZone);
        vector2 = Utility.ApplyCircularDeadZone(x, y, lowerDeadZone, upperDeadZone);
      }
      this.RightStick.Raw = true;
      this.RightStick.UpdateWithAxes(vector2.x, vector2.y, updateTick, deltaTime);
      this.RightStickX.Raw = true;
      this.RightStickX.CommitWithValue(vector2.x, updateTick, deltaTime);
      this.RightStickY.Raw = true;
      this.RightStickY.CommitWithValue(vector2.y, updateTick, deltaTime);
      this.RightStickLeft.SetValue(this.RightStick.Left.Value, updateTick);
      this.RightStickRight.SetValue(this.RightStick.Right.Value, updateTick);
      this.RightStickUp.SetValue(this.RightStick.Up.Value, updateTick);
      this.RightStickDown.SetValue(this.RightStick.Down.Value, updateTick);
    }

    private void ProcessDPad(ulong updateTick, float deltaTime)
    {
      float x = Utility.ValueFromSides(this.DPadLeft.NextRawValue, this.DPadRight.NextRawValue);
      float y = Utility.ValueFromSides(this.DPadDown.NextRawValue, this.DPadUp.NextRawValue, InputManager.InvertYAxis);
      Vector2 vector2;
      if (this.RawSticks || this.DPadLeft.Raw || this.DPadRight.Raw || this.DPadUp.Raw || this.DPadDown.Raw)
      {
        vector2 = new Vector2(x, y);
      }
      else
      {
        float lowerDeadZone = Utility.Max(this.DPadLeft.LowerDeadZone, this.DPadRight.LowerDeadZone, this.DPadUp.LowerDeadZone, this.DPadDown.LowerDeadZone);
        float upperDeadZone = Utility.Min(this.DPadLeft.UpperDeadZone, this.DPadRight.UpperDeadZone, this.DPadUp.UpperDeadZone, this.DPadDown.UpperDeadZone);
        vector2 = Utility.ApplySeparateDeadZone(x, y, lowerDeadZone, upperDeadZone);
      }
      this.DPad.Raw = true;
      this.DPad.UpdateWithAxes(vector2.x, vector2.y, updateTick, deltaTime);
      this.DPadX.Raw = true;
      this.DPadX.CommitWithValue(vector2.x, updateTick, deltaTime);
      this.DPadY.Raw = true;
      this.DPadY.CommitWithValue(vector2.y, updateTick, deltaTime);
      this.DPadLeft.SetValue(this.DPad.Left.Value, updateTick);
      this.DPadRight.SetValue(this.DPad.Right.Value, updateTick);
      this.DPadUp.SetValue(this.DPad.Up.Value, updateTick);
      this.DPadDown.SetValue(this.DPad.Down.Value, updateTick);
    }

    public void Commit(ulong updateTick, float deltaTime)
    {
      if (this.IsKnown)
      {
        this.ProcessLeftStick(updateTick, deltaTime);
        this.ProcessRightStick(updateTick, deltaTime);
        this.ProcessDPad(updateTick, deltaTime);
      }
      int count = this.Controls.Count;
      for (int index = 0; index < count; ++index)
      {
        InputControl control = this.Controls[index];
        if (control != null)
        {
          control.Commit();
          if (control.HasChanged && !control.Passive)
          {
            this.LastChangeTick = updateTick;
            this.LastChangeTime = UnityEngine.Time.realtimeSinceStartup;
          }
        }
      }
      if (!this.IsKnown)
        return;
      this.Command.CommitWithState(this.AnyCommandControlIsPressed(), updateTick, deltaTime);
    }

    public bool LastChangeAfterTime(InputDevice device)
    {
      return (double) this.LastChangeTime > (double) device.LastChangeTime + 0.5;
    }

    public bool LastChangedAfter(InputDevice device)
    {
      if (Application.platform != RuntimePlatform.PS4 && Application.platform != RuntimePlatform.XboxOne)
        return this.LastChangeTick > device.LastChangeTick + 1UL;
      return device == null || this.LastChangeTick > device.LastChangeTick;
    }

    internal void RequestActivation()
    {
      this.LastChangeTick = InputManager.CurrentTick;
      this.LastChangeTime = (float) InputManager.CurrentTick;
    }

    public virtual void Vibrate(float leftMotor, float rightMotor)
    {
    }

    public void Vibrate(float intensity) => this.Vibrate(intensity, intensity);

    public void StopVibration() => this.Vibrate(0.0f);

    public virtual void SetLightColor(float red, float green, float blue)
    {
    }

    public void SetLightColor(Color color)
    {
      this.SetLightColor(color.r * color.a, color.g * color.a, color.b * color.a);
    }

    public virtual void SetLightFlash(float flashOnDuration, float flashOffDuration)
    {
    }

    public void StopLightFlash() => this.SetLightFlash(1f, 0.0f);

    public virtual bool IsSupportedOnThisPlatform => true;

    public virtual bool IsKnown => true;

    public bool IsUnknown => !this.IsKnown;

    [Obsolete("Use InputDevice.CommandIsPressed instead.", false)]
    public bool MenuIsPressed => this.IsKnown && this.Command.IsPressed;

    [Obsolete("Use InputDevice.CommandWasPressed instead.", false)]
    public bool MenuWasPressed => this.IsKnown && this.Command.WasPressed;

    [Obsolete("Use InputDevice.CommandWasReleased instead.", false)]
    public bool MenuWasReleased => this.IsKnown && this.Command.WasReleased;

    public bool CommandIsPressed => this.IsKnown && this.Command.IsPressed;

    public bool CommandWasPressed => this.IsKnown && this.Command.WasPressed;

    public bool CommandWasReleased => this.IsKnown && this.Command.WasReleased;

    public InputControl AnyButton
    {
      get
      {
        int count = this.Controls.Count;
        for (int index = 0; index < count; ++index)
        {
          InputControl control = this.Controls[index];
          if (control != null && control.IsButton && control.IsPressed)
            return control;
        }
        return InputControl.Null;
      }
    }

    public bool AnyButtonIsPressed
    {
      get
      {
        int count = this.Controls.Count;
        for (int index = 0; index < count; ++index)
        {
          InputControl control = this.Controls[index];
          if (control != null && control.IsButton && control.IsPressed)
            return true;
        }
        return false;
      }
    }

    public bool AnyButtonWasPressed
    {
      get
      {
        int count = this.Controls.Count;
        for (int index = 0; index < count; ++index)
        {
          InputControl control = this.Controls[index];
          if (control != null && control.IsButton && control.WasPressed)
            return true;
        }
        return false;
      }
    }

    public bool AnyButtonWasReleased
    {
      get
      {
        int count = this.Controls.Count;
        for (int index = 0; index < count; ++index)
        {
          InputControl control = this.Controls[index];
          if (control != null && control.IsButton && control.WasReleased)
            return true;
        }
        return false;
      }
    }

    public TwoAxisInputControl Direction
    {
      get => this.DPad.UpdateTick > this.LeftStick.UpdateTick ? this.DPad : this.LeftStick;
    }

    public InputControl LeftStickUp
    {
      get
      {
        return this.cachedLeftStickUp ?? (this.cachedLeftStickUp = this.GetControl(InputControlType.LeftStickUp));
      }
    }

    public InputControl LeftStickDown
    {
      get
      {
        return this.cachedLeftStickDown ?? (this.cachedLeftStickDown = this.GetControl(InputControlType.LeftStickDown));
      }
    }

    public InputControl LeftStickLeft
    {
      get
      {
        return this.cachedLeftStickLeft ?? (this.cachedLeftStickLeft = this.GetControl(InputControlType.LeftStickLeft));
      }
    }

    public InputControl LeftStickRight
    {
      get
      {
        return this.cachedLeftStickRight ?? (this.cachedLeftStickRight = this.GetControl(InputControlType.LeftStickRight));
      }
    }

    public InputControl RightStickUp
    {
      get
      {
        return this.cachedRightStickUp ?? (this.cachedRightStickUp = this.GetControl(InputControlType.RightStickUp));
      }
    }

    public InputControl RightStickDown
    {
      get
      {
        return this.cachedRightStickDown ?? (this.cachedRightStickDown = this.GetControl(InputControlType.RightStickDown));
      }
    }

    public InputControl RightStickLeft
    {
      get
      {
        return this.cachedRightStickLeft ?? (this.cachedRightStickLeft = this.GetControl(InputControlType.RightStickLeft));
      }
    }

    public InputControl RightStickRight
    {
      get
      {
        return this.cachedRightStickRight ?? (this.cachedRightStickRight = this.GetControl(InputControlType.RightStickRight));
      }
    }

    public InputControl DPadUp
    {
      get => this.cachedDPadUp ?? (this.cachedDPadUp = this.GetControl(InputControlType.DPadUp));
    }

    public InputControl DPadDown
    {
      get
      {
        return this.cachedDPadDown ?? (this.cachedDPadDown = this.GetControl(InputControlType.DPadDown));
      }
    }

    public InputControl DPadLeft
    {
      get
      {
        return this.cachedDPadLeft ?? (this.cachedDPadLeft = this.GetControl(InputControlType.DPadLeft));
      }
    }

    public InputControl DPadRight
    {
      get
      {
        return this.cachedDPadRight ?? (this.cachedDPadRight = this.GetControl(InputControlType.DPadRight));
      }
    }

    public InputControl Action1
    {
      get => this.cachedAction1 ?? (this.cachedAction1 = this.GetControl(InputControlType.Action1));
    }

    public InputControl Action2
    {
      get => this.cachedAction2 ?? (this.cachedAction2 = this.GetControl(InputControlType.Action2));
    }

    public InputControl Action3
    {
      get => this.cachedAction3 ?? (this.cachedAction3 = this.GetControl(InputControlType.Action3));
    }

    public InputControl Action4
    {
      get => this.cachedAction4 ?? (this.cachedAction4 = this.GetControl(InputControlType.Action4));
    }

    public InputControl LeftTrigger
    {
      get
      {
        return this.cachedLeftTrigger ?? (this.cachedLeftTrigger = this.GetControl(InputControlType.LeftTrigger));
      }
    }

    public InputControl RightTrigger
    {
      get
      {
        return this.cachedRightTrigger ?? (this.cachedRightTrigger = this.GetControl(InputControlType.RightTrigger));
      }
    }

    public InputControl LeftBumper
    {
      get
      {
        return this.cachedLeftBumper ?? (this.cachedLeftBumper = this.GetControl(InputControlType.LeftBumper));
      }
    }

    public InputControl RightBumper
    {
      get
      {
        return this.cachedRightBumper ?? (this.cachedRightBumper = this.GetControl(InputControlType.RightBumper));
      }
    }

    public InputControl LeftStickButton
    {
      get
      {
        return this.cachedLeftStickButton ?? (this.cachedLeftStickButton = this.GetControl(InputControlType.LeftStickButton));
      }
    }

    public InputControl RightStickButton
    {
      get
      {
        return this.cachedRightStickButton ?? (this.cachedRightStickButton = this.GetControl(InputControlType.RightStickButton));
      }
    }

    public InputControl LeftStickX
    {
      get
      {
        return this.cachedLeftStickX ?? (this.cachedLeftStickX = this.GetControl(InputControlType.LeftStickX));
      }
    }

    public InputControl LeftStickY
    {
      get
      {
        return this.cachedLeftStickY ?? (this.cachedLeftStickY = this.GetControl(InputControlType.LeftStickY));
      }
    }

    public InputControl RightStickX
    {
      get
      {
        return this.cachedRightStickX ?? (this.cachedRightStickX = this.GetControl(InputControlType.RightStickX));
      }
    }

    public InputControl RightStickY
    {
      get
      {
        return this.cachedRightStickY ?? (this.cachedRightStickY = this.GetControl(InputControlType.RightStickY));
      }
    }

    public InputControl DPadX
    {
      get => this.cachedDPadX ?? (this.cachedDPadX = this.GetControl(InputControlType.DPadX));
    }

    public InputControl DPadY
    {
      get => this.cachedDPadY ?? (this.cachedDPadY = this.GetControl(InputControlType.DPadY));
    }

    public InputControl Command
    {
      get => this.cachedCommand ?? (this.cachedCommand = this.GetControl(InputControlType.Command));
    }

    private void ExpireControlCache()
    {
      this.cachedLeftStickUp = (InputControl) null;
      this.cachedLeftStickDown = (InputControl) null;
      this.cachedLeftStickLeft = (InputControl) null;
      this.cachedLeftStickRight = (InputControl) null;
      this.cachedRightStickUp = (InputControl) null;
      this.cachedRightStickDown = (InputControl) null;
      this.cachedRightStickLeft = (InputControl) null;
      this.cachedRightStickRight = (InputControl) null;
      this.cachedDPadUp = (InputControl) null;
      this.cachedDPadDown = (InputControl) null;
      this.cachedDPadLeft = (InputControl) null;
      this.cachedDPadRight = (InputControl) null;
      this.cachedAction1 = (InputControl) null;
      this.cachedAction2 = (InputControl) null;
      this.cachedAction3 = (InputControl) null;
      this.cachedAction4 = (InputControl) null;
      this.cachedLeftTrigger = (InputControl) null;
      this.cachedRightTrigger = (InputControl) null;
      this.cachedLeftBumper = (InputControl) null;
      this.cachedRightBumper = (InputControl) null;
      this.cachedLeftStickButton = (InputControl) null;
      this.cachedRightStickButton = (InputControl) null;
      this.cachedLeftStickX = (InputControl) null;
      this.cachedLeftStickY = (InputControl) null;
      this.cachedRightStickX = (InputControl) null;
      this.cachedRightStickY = (InputControl) null;
      this.cachedDPadX = (InputControl) null;
      this.cachedDPadY = (InputControl) null;
      this.cachedCommand = (InputControl) null;
    }

    internal virtual int NumUnknownAnalogs => 0;

    internal virtual int NumUnknownButtons => 0;

    internal virtual bool ReadRawButtonState(int index) => false;

    internal virtual float ReadRawAnalogValue(int index) => 0.0f;

    internal void TakeSnapshot()
    {
      if (this.AnalogSnapshot == null)
        this.AnalogSnapshot = new InputDevice.AnalogSnapshotEntry[this.NumUnknownAnalogs];
      for (int index = 0; index < this.NumUnknownAnalogs; ++index)
      {
        float num = Utility.ApplySnapping(this.ReadRawAnalogValue(index), 0.5f);
        this.AnalogSnapshot[index].value = num;
      }
    }

    internal UnknownDeviceControl GetFirstPressedAnalog()
    {
      if (this.AnalogSnapshot != null)
      {
        for (int index = 0; index < this.NumUnknownAnalogs; ++index)
        {
          InputControlType control = (InputControlType) (400 + index);
          float currentValue = Utility.ApplySnapping(this.ReadRawAnalogValue(index), 0.5f);
          float num = currentValue - this.AnalogSnapshot[index].value;
          this.AnalogSnapshot[index].TrackMinMaxValue(currentValue);
          if ((double) num > 0.10000000149011612)
            num = this.AnalogSnapshot[index].maxValue - this.AnalogSnapshot[index].value;
          if ((double) num < -0.10000000149011612)
            num = this.AnalogSnapshot[index].minValue - this.AnalogSnapshot[index].value;
          if ((double) num > 1.8999999761581421)
            return new UnknownDeviceControl(control, InputRangeType.MinusOneToOne);
          if ((double) num < -0.89999997615814209)
            return new UnknownDeviceControl(control, InputRangeType.ZeroToMinusOne);
          if ((double) num > 0.89999997615814209)
            return new UnknownDeviceControl(control, InputRangeType.ZeroToOne);
        }
      }
      return UnknownDeviceControl.None;
    }

    internal UnknownDeviceControl GetFirstPressedButton()
    {
      for (int index = 0; index < this.NumUnknownButtons; ++index)
      {
        if (this.ReadRawButtonState(index))
          return new UnknownDeviceControl((InputControlType) (500 + index), InputRangeType.ZeroToOne);
      }
      return UnknownDeviceControl.None;
    }

    protected struct AnalogSnapshotEntry
    {
      public float value;
      public float maxValue;
      public float minValue;

      public void TrackMinMaxValue(float currentValue)
      {
        this.maxValue = Mathf.Max(this.maxValue, currentValue);
        this.minValue = Mathf.Min(this.minValue, currentValue);
      }
    }
  }
}
