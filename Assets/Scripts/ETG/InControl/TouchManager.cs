// Decompiled with JetBrains decompiler
// Type: InControl.TouchManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace InControl;

[ExecuteInEditMode]
public class TouchManager : SingletonMonoBehavior<TouchManager, InControlManager>
{
  [Space(10f)]
  public Camera touchCamera;
  public TouchManager.GizmoShowOption controlsShowGizmos = TouchManager.GizmoShowOption.Always;
  [HideInInspector]
  public bool enableControlsOnTouch;
  [HideInInspector]
  [SerializeField]
  private bool _controlsEnabled = true;
  [HideInInspector]
  public int controlsLayer = 5;
  private InputDevice device;
  private Vector3 viewSize;
  private Vector2 screenSize;
  private Vector2 halfScreenSize;
  private float percentToWorld;
  private float halfPercentToWorld;
  private float pixelToWorld;
  private float halfPixelToWorld;
  private TouchControl[] touchControls;
  private TouchPool cachedTouches;
  private List<Touch> activeTouches;
  private ReadOnlyCollection<Touch> readOnlyActiveTouches;
  private Vector2 lastMousePosition;
  private bool isReady;
  private Touch mouseTouch;

  protected TouchManager()
  {
  }

  public static event Action OnSetup;

  private void OnEnable()
  {
    if ((UnityEngine.Object) this.GetComponent<InControlManager>() == (UnityEngine.Object) null)
    {
      UnityEngine.Debug.LogError((object) "Touch Manager component can only be added to the InControl Manager object.");
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this);
    }
    else if (!this.EnforceSingletonComponent())
    {
      UnityEngine.Debug.LogWarning((object) "There is already a Touch Manager component on this game object.");
    }
    else
    {
      this.touchControls = this.GetComponentsInChildren<TouchControl>(true);
      if (!Application.isPlaying)
        return;
      InputManager.OnSetup += new Action(this.Setup);
      InputManager.OnUpdateDevices += new Action<ulong, float>(this.UpdateDevice);
      InputManager.OnCommitDevices += new Action<ulong, float>(this.CommitDevice);
    }
  }

  private void OnDisable()
  {
    if (Application.isPlaying)
    {
      InputManager.OnSetup -= new Action(this.Setup);
      InputManager.OnUpdateDevices -= new Action<ulong, float>(this.UpdateDevice);
      InputManager.OnCommitDevices -= new Action<ulong, float>(this.CommitDevice);
    }
    this.Reset();
  }

  private void Setup()
  {
    this.UpdateScreenSize(this.GetCurrentScreenSize());
    this.CreateDevice();
    this.CreateTouches();
    if (TouchManager.OnSetup == null)
      return;
    TouchManager.OnSetup();
    TouchManager.OnSetup = (Action) null;
  }

  private void Reset()
  {
    this.device = (InputDevice) null;
    this.mouseTouch = (Touch) null;
    this.cachedTouches = (TouchPool) null;
    this.activeTouches = (List<Touch>) null;
    this.readOnlyActiveTouches = (ReadOnlyCollection<Touch>) null;
    this.touchControls = (TouchControl[]) null;
    TouchManager.OnSetup = (Action) null;
  }

  [DebuggerHidden]
  private IEnumerator UpdateScreenSizeAtEndOfFrame()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new TouchManager__UpdateScreenSizeAtEndOfFramec__Iterator0()
    {
      _this = this
    };
  }

  private void Update()
  {
    Vector2 currentScreenSize = this.GetCurrentScreenSize();
    if (!this.isReady)
    {
      this.StartCoroutine(this.UpdateScreenSizeAtEndOfFrame());
      this.UpdateScreenSize(currentScreenSize);
      this.isReady = true;
    }
    else
    {
      if (this.screenSize != currentScreenSize)
        this.UpdateScreenSize(currentScreenSize);
      if (TouchManager.OnSetup == null)
        return;
      TouchManager.OnSetup();
      TouchManager.OnSetup = (Action) null;
    }
  }

  private void CreateDevice()
  {
    this.device = (InputDevice) new TouchInputDevice();
    this.device.AddControl(InputControlType.LeftStickLeft, "LeftStickLeft");
    this.device.AddControl(InputControlType.LeftStickRight, "LeftStickRight");
    this.device.AddControl(InputControlType.LeftStickUp, "LeftStickUp");
    this.device.AddControl(InputControlType.LeftStickDown, "LeftStickDown");
    this.device.AddControl(InputControlType.RightStickLeft, "RightStickLeft");
    this.device.AddControl(InputControlType.RightStickRight, "RightStickRight");
    this.device.AddControl(InputControlType.RightStickUp, "RightStickUp");
    this.device.AddControl(InputControlType.RightStickDown, "RightStickDown");
    this.device.AddControl(InputControlType.DPadUp, "DPadUp");
    this.device.AddControl(InputControlType.DPadDown, "DPadDown");
    this.device.AddControl(InputControlType.DPadLeft, "DPadLeft");
    this.device.AddControl(InputControlType.DPadRight, "DPadRight");
    this.device.AddControl(InputControlType.LeftTrigger, "LeftTrigger");
    this.device.AddControl(InputControlType.RightTrigger, "RightTrigger");
    this.device.AddControl(InputControlType.LeftBumper, "LeftBumper");
    this.device.AddControl(InputControlType.RightBumper, "RightBumper");
    for (InputControlType controlType = InputControlType.Action1; controlType <= InputControlType.Action4; ++controlType)
      this.device.AddControl(controlType, controlType.ToString());
    this.device.AddControl(InputControlType.Menu, "Menu");
    for (InputControlType controlType = InputControlType.Button0; controlType <= InputControlType.Button19; ++controlType)
      this.device.AddControl(controlType, controlType.ToString());
    InputManager.AttachDevice(this.device);
  }

  private void UpdateDevice(ulong updateTick, float deltaTime)
  {
    this.UpdateTouches(updateTick, deltaTime);
    this.SubmitControlStates(updateTick, deltaTime);
  }

  private void CommitDevice(ulong updateTick, float deltaTime)
  {
    this.CommitControlStates(updateTick, deltaTime);
  }

  private void SubmitControlStates(ulong updateTick, float deltaTime)
  {
    int length = this.touchControls.Length;
    for (int index = 0; index < length; ++index)
    {
      TouchControl touchControl = this.touchControls[index];
      if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
        touchControl.SubmitControlState(updateTick, deltaTime);
    }
  }

  private void CommitControlStates(ulong updateTick, float deltaTime)
  {
    int length = this.touchControls.Length;
    for (int index = 0; index < length; ++index)
    {
      TouchControl touchControl = this.touchControls[index];
      if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
        touchControl.CommitControlState(updateTick, deltaTime);
    }
  }

  private void UpdateScreenSize(Vector2 currentScreenSize)
  {
    this.touchCamera.rect = new Rect(0.0f, 0.0f, 0.99f, 1f);
    this.touchCamera.rect = new Rect(0.0f, 0.0f, 1f, 1f);
    this.screenSize = currentScreenSize;
    this.halfScreenSize = this.screenSize / 2f;
    this.viewSize = this.ConvertViewToWorldPoint(Vector2.one) * 0.02f;
    this.percentToWorld = Mathf.Min(this.viewSize.x, this.viewSize.y);
    this.halfPercentToWorld = this.percentToWorld / 2f;
    if ((UnityEngine.Object) this.touchCamera != (UnityEngine.Object) null)
    {
      this.halfPixelToWorld = this.touchCamera.orthographicSize / this.screenSize.y;
      this.pixelToWorld = this.halfPixelToWorld * 2f;
    }
    if (this.touchControls == null)
      return;
    int length = this.touchControls.Length;
    for (int index = 0; index < length; ++index)
      this.touchControls[index].ConfigureControl();
  }

  private void CreateTouches()
  {
    this.cachedTouches = new TouchPool();
    this.mouseTouch = new Touch();
    this.mouseTouch.fingerId = Touch.FingerID_Mouse;
    this.activeTouches = new List<Touch>(32 /*0x20*/);
    this.readOnlyActiveTouches = new ReadOnlyCollection<Touch>((IList<Touch>) this.activeTouches);
  }

  private void UpdateTouches(ulong updateTick, float deltaTime)
  {
    this.activeTouches.Clear();
    this.cachedTouches.FreeEndedTouches();
    if (this.mouseTouch.SetWithMouseData(updateTick, deltaTime))
      this.activeTouches.Add(this.mouseTouch);
    for (int index = 0; index < Input.touchCount; ++index)
    {
      UnityEngine.Touch touch = Input.GetTouch(index);
      Touch orCreateTouch = this.cachedTouches.FindOrCreateTouch(touch.fingerId);
      orCreateTouch.SetWithTouchData(touch, updateTick, deltaTime);
      this.activeTouches.Add(orCreateTouch);
    }
    int count = this.cachedTouches.Touches.Count;
    for (int index = 0; index < count; ++index)
    {
      Touch touch = this.cachedTouches.Touches[index];
      if (touch.phase != TouchPhase.Ended && (long) touch.updateTick != (long) updateTick)
      {
        touch.phase = TouchPhase.Ended;
        this.activeTouches.Add(touch);
      }
    }
    this.InvokeTouchEvents();
  }

  private void SendTouchBegan(Touch touch)
  {
    int length = this.touchControls.Length;
    for (int index = 0; index < length; ++index)
    {
      TouchControl touchControl = this.touchControls[index];
      if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
        touchControl.TouchBegan(touch);
    }
  }

  private void SendTouchMoved(Touch touch)
  {
    int length = this.touchControls.Length;
    for (int index = 0; index < length; ++index)
    {
      TouchControl touchControl = this.touchControls[index];
      if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
        touchControl.TouchMoved(touch);
    }
  }

  private void SendTouchEnded(Touch touch)
  {
    int length = this.touchControls.Length;
    for (int index = 0; index < length; ++index)
    {
      TouchControl touchControl = this.touchControls[index];
      if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
        touchControl.TouchEnded(touch);
    }
  }

  private void InvokeTouchEvents()
  {
    int count = this.activeTouches.Count;
    if (this.enableControlsOnTouch && count > 0 && !this.controlsEnabled)
    {
      TouchManager.Device.RequestActivation();
      this.controlsEnabled = true;
    }
    for (int index = 0; index < count; ++index)
    {
      Touch activeTouch = this.activeTouches[index];
      switch (activeTouch.phase)
      {
        case TouchPhase.Began:
          this.SendTouchBegan(activeTouch);
          break;
        case TouchPhase.Moved:
          this.SendTouchMoved(activeTouch);
          break;
        case TouchPhase.Ended:
          this.SendTouchEnded(activeTouch);
          break;
        case TouchPhase.Canceled:
          this.SendTouchEnded(activeTouch);
          break;
      }
    }
  }

  private bool TouchCameraIsValid()
  {
    return !((UnityEngine.Object) this.touchCamera == (UnityEngine.Object) null) && !Utility.IsZero(this.touchCamera.orthographicSize) && (!Utility.IsZero(this.touchCamera.rect.width) || !Utility.IsZero(this.touchCamera.rect.height)) && (!Utility.IsZero(this.touchCamera.pixelRect.width) || !Utility.IsZero(this.touchCamera.pixelRect.height));
  }

  private Vector3 ConvertScreenToWorldPoint(Vector2 point)
  {
    return this.TouchCameraIsValid() ? this.touchCamera.ScreenToWorldPoint(new Vector3(point.x, point.y, -this.touchCamera.transform.position.z)) : Vector3.zero;
  }

  private Vector3 ConvertViewToWorldPoint(Vector2 point)
  {
    return this.TouchCameraIsValid() ? this.touchCamera.ViewportToWorldPoint(new Vector3(point.x, point.y, -this.touchCamera.transform.position.z)) : Vector3.zero;
  }

  private Vector3 ConvertScreenToViewPoint(Vector2 point)
  {
    return this.TouchCameraIsValid() ? this.touchCamera.ScreenToViewportPoint(new Vector3(point.x, point.y, -this.touchCamera.transform.position.z)) : Vector3.zero;
  }

  private Vector2 GetCurrentScreenSize()
  {
    return this.TouchCameraIsValid() ? new Vector2((float) this.touchCamera.pixelWidth, (float) this.touchCamera.pixelHeight) : new Vector2((float) Screen.width, (float) Screen.height);
  }

  public bool controlsEnabled
  {
    get => this._controlsEnabled;
    set
    {
      if (this._controlsEnabled == value)
        return;
      int length = this.touchControls.Length;
      for (int index = 0; index < length; ++index)
        this.touchControls[index].enabled = value;
      this._controlsEnabled = value;
    }
  }

  public static ReadOnlyCollection<Touch> Touches
  {
    get => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.readOnlyActiveTouches;
  }

  public static int TouchCount
  {
    get => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.activeTouches.Count;
  }

  public static Touch GetTouch(int touchIndex)
  {
    return SingletonMonoBehavior<TouchManager, InControlManager>.Instance.activeTouches[touchIndex];
  }

  public static Touch GetTouchByFingerId(int fingerId)
  {
    return SingletonMonoBehavior<TouchManager, InControlManager>.Instance.cachedTouches.FindTouch(fingerId);
  }

  public static Vector3 ScreenToWorldPoint(Vector2 point)
  {
    return SingletonMonoBehavior<TouchManager, InControlManager>.Instance.ConvertScreenToWorldPoint(point);
  }

  public static Vector3 ViewToWorldPoint(Vector2 point)
  {
    return SingletonMonoBehavior<TouchManager, InControlManager>.Instance.ConvertViewToWorldPoint(point);
  }

  public static Vector3 ScreenToViewPoint(Vector2 point)
  {
    return SingletonMonoBehavior<TouchManager, InControlManager>.Instance.ConvertScreenToViewPoint(point);
  }

  public static float ConvertToWorld(float value, TouchUnitType unitType)
  {
    return value * (unitType != TouchUnitType.Pixels ? TouchManager.PercentToWorld : TouchManager.PixelToWorld);
  }

  public static Rect PercentToWorldRect(Rect rect)
  {
    return new Rect((rect.xMin - 50f) * TouchManager.ViewSize.x, (rect.yMin - 50f) * TouchManager.ViewSize.y, rect.width * TouchManager.ViewSize.x, rect.height * TouchManager.ViewSize.y);
  }

  public static Rect PixelToWorldRect(Rect rect)
  {
    return new Rect(Mathf.Round(rect.xMin - TouchManager.HalfScreenSize.x) * TouchManager.PixelToWorld, Mathf.Round(rect.yMin - TouchManager.HalfScreenSize.y) * TouchManager.PixelToWorld, Mathf.Round(rect.width) * TouchManager.PixelToWorld, Mathf.Round(rect.height) * TouchManager.PixelToWorld);
  }

  public static Rect ConvertToWorld(Rect rect, TouchUnitType unitType)
  {
    return unitType == TouchUnitType.Pixels ? TouchManager.PixelToWorldRect(rect) : TouchManager.PercentToWorldRect(rect);
  }

  public static Camera Camera
  {
    get => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.touchCamera;
  }

  public static InputDevice Device
  {
    get => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.device;
  }

  public static Vector3 ViewSize
  {
    get => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.viewSize;
  }

  public static float PercentToWorld
  {
    get => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.percentToWorld;
  }

  public static float HalfPercentToWorld
  {
    get => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.halfPercentToWorld;
  }

  public static float PixelToWorld
  {
    get => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.pixelToWorld;
  }

  public static float HalfPixelToWorld
  {
    get => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.halfPixelToWorld;
  }

  public static Vector2 ScreenSize
  {
    get => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.screenSize;
  }

  public static Vector2 HalfScreenSize
  {
    get => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.halfScreenSize;
  }

  public static TouchManager.GizmoShowOption ControlsShowGizmos
  {
    get => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.controlsShowGizmos;
  }

  public static bool ControlsEnabled
  {
    get => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.controlsEnabled;
    set => SingletonMonoBehavior<TouchManager, InControlManager>.Instance.controlsEnabled = value;
  }

  public static implicit operator bool(TouchManager instance) => (UnityEngine.Object) instance != (UnityEngine.Object) null;

  public enum GizmoShowOption
  {
    Never,
    WhenSelected,
    UnlessPlaying,
    Always,
  }
}
