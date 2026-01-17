// Decompiled with JetBrains decompiler
// Type: dfInputManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/User Interface/Input Manager")]
[Serializable]
public class dfInputManager : MonoBehaviour
{
  private static dfControl controlUnderMouse = (dfControl) null;
  private static dfList<dfInputManager> activeInstances = new dfList<dfInputManager>();
  [SerializeField]
  protected Camera renderCamera;
  [SerializeField]
  protected bool useTouch = true;
  [SerializeField]
  protected bool useMouse = true;
  [SerializeField]
  protected bool useJoystick;
  [SerializeField]
  protected KeyCode joystickClickButton = KeyCode.Joystick1Button1;
  [SerializeField]
  protected string horizontalAxis = "Horizontal";
  [SerializeField]
  protected string verticalAxis = "Vertical";
  [SerializeField]
  protected float axisPollingInterval = 0.15f;
  [SerializeField]
  protected bool retainFocus;
  [SerializeField]
  [HideInInspector]
  protected int touchClickRadius = 125;
  [SerializeField]
  protected float hoverStartDelay = 0.25f;
  [SerializeField]
  protected float hoverNotifactionFrequency = 0.1f;
  private IDFTouchInputSource touchInputSource;
  private dfInputManager.TouchInputManager touchHandler;
  private dfInputManager.MouseInputManager mouseHandler;
  private dfGUIManager guiManager;
  private dfControl buttonDownTarget;
  private IInputAdapter adapter;
  private float lastAxisCheck;
  private Vector2 mousePositionLastFrame;

  public static IList<dfInputManager> ActiveInstances
  {
    get => (IList<dfInputManager>) dfInputManager.activeInstances;
  }

  public static dfControl ControlUnderMouse => dfInputManager.controlUnderMouse;

  public Camera RenderCamera
  {
    get => this.renderCamera;
    set => this.renderCamera = value;
  }

  public bool UseTouch
  {
    get => this.useTouch;
    set => this.useTouch = value;
  }

  public bool UseMouse
  {
    get => this.useMouse;
    set => this.useMouse = value;
  }

  public bool UseJoystick
  {
    get => this.useJoystick;
    set => this.useJoystick = value;
  }

  public KeyCode JoystickClickButton
  {
    get => this.joystickClickButton;
    set => this.joystickClickButton = value;
  }

  public string HorizontalAxis
  {
    get => this.horizontalAxis;
    set => this.horizontalAxis = value;
  }

  public string VerticalAxis
  {
    get => this.verticalAxis;
    set => this.verticalAxis = value;
  }

  public IInputAdapter Adapter
  {
    get => this.adapter;
    set
    {
      this.adapter = value ?? (IInputAdapter) new dfInputManager.DefaultInput(this.renderCamera);
    }
  }

  public bool RetainFocus
  {
    get => this.retainFocus;
    set => this.retainFocus = value;
  }

  public IDFTouchInputSource TouchInputSource
  {
    get => this.touchInputSource;
    set => this.touchInputSource = value;
  }

  public float HoverStartDelay
  {
    get => this.hoverStartDelay;
    set => this.hoverStartDelay = value;
  }

  public float HoverNotificationFrequency
  {
    get => this.hoverNotifactionFrequency;
    set => this.hoverNotifactionFrequency = value;
  }

  public void Awake() => this.useGUILayout = false;

  public void Start()
  {
  }

  public void OnDisable()
  {
    dfInputManager.activeInstances.Remove(this);
    dfControl activeControl = dfGUIManager.ActiveControl;
    if (!((UnityEngine.Object) activeControl != (UnityEngine.Object) null) || !activeControl.transform.IsChildOf(this.transform))
      return;
    dfGUIManager.SetFocus((dfControl) null);
  }

  public void OnEnable()
  {
    dfInputManager.activeInstances.Add(this);
    this.mouseHandler = new dfInputManager.MouseInputManager();
    if (this.useTouch)
      this.touchHandler = new dfInputManager.TouchInputManager(this);
    if (this.adapter == null)
      this.adapter = (IInputAdapter) ((IEnumerable<Component>) this.GetComponents(typeof (MonoBehaviour))).Where<Component>((Func<Component, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) null && c.GetType() != null && typeof (IInputAdapter).IsAssignableFrom(c.GetType()))).FirstOrDefault<Component>() ?? (IInputAdapter) new dfInputManager.DefaultInput(this.renderCamera);
    Input.simulateMouseWithTouches = !this.useTouch;
  }

  public void Update()
  {
    if (!Application.isPlaying)
      return;
    if ((UnityEngine.Object) this.guiManager == (UnityEngine.Object) null)
    {
      this.guiManager = this.GetComponent<dfGUIManager>();
      if ((UnityEngine.Object) this.guiManager == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "No associated dfGUIManager instance", (UnityEngine.Object) this);
        this.enabled = false;
        return;
      }
    }
    dfControl activeControl = dfGUIManager.ActiveControl;
    if (this.useTouch && this.processTouchInput())
      return;
    if (this.useMouse)
    {
      if ((UnityEngine.Object) BraveInput.PrimaryPlayerInstance != (UnityEngine.Object) null && BraveInput.PrimaryPlayerInstance.ActiveActions != null)
      {
        Vector2 vector2 = this.adapter.GetMousePosition() - this.mousePositionLastFrame;
        if (BraveInput.PrimaryPlayerInstance.ActiveActions.LastInputType == BindingSourceType.MouseBindingSource || BraveInput.PrimaryPlayerInstance.ActiveActions.LastInputType == BindingSourceType.KeyBindingSource && (double) vector2.magnitude > 1.4012984643248171E-45 || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
          this.processMouseInput();
      }
      else
        this.processMouseInput();
    }
    if ((UnityEngine.Object) activeControl == (UnityEngine.Object) null || this.processKeyboard())
      return;
    this.mousePositionLastFrame = this.adapter.GetMousePosition();
  }

  private void processJoystick()
  {
    try
    {
      dfControl activeControl = dfGUIManager.ActiveControl;
      if ((UnityEngine.Object) activeControl == (UnityEngine.Object) null || !activeControl.transform.IsChildOf(this.transform))
        return;
      float axis1 = this.adapter.GetAxis(this.horizontalAxis);
      float axis2 = this.adapter.GetAxis(this.verticalAxis);
      if ((double) Mathf.Abs(axis1) < 0.5 && (double) Mathf.Abs(axis2) <= 0.5)
        this.lastAxisCheck = BraveTime.DeltaTime - this.axisPollingInterval;
      if ((double) UnityEngine.Time.realtimeSinceStartup - (double) this.lastAxisCheck > (double) this.axisPollingInterval)
      {
        if ((double) Mathf.Abs(axis1) >= 0.5)
        {
          this.lastAxisCheck = UnityEngine.Time.realtimeSinceStartup;
          KeyCode Key = (double) axis1 <= 0.0 ? KeyCode.LeftArrow : KeyCode.RightArrow;
          activeControl.OnKeyDown(new dfKeyEventArgs(activeControl, Key, false, false, false));
        }
        if ((double) Mathf.Abs(axis2) >= 0.5)
        {
          this.lastAxisCheck = UnityEngine.Time.realtimeSinceStartup;
          KeyCode Key = (double) axis2 <= 0.0 ? KeyCode.DownArrow : KeyCode.UpArrow;
          activeControl.OnKeyDown(new dfKeyEventArgs(activeControl, Key, false, false, false));
        }
      }
      if (this.joystickClickButton != KeyCode.None)
      {
        if (this.adapter.GetKeyDown(this.joystickClickButton))
        {
          Vector3 center = activeControl.GetCenter();
          Camera camera = activeControl.GetCamera();
          Ray ray = camera.ScreenPointToRay(camera.WorldToScreenPoint(center));
          dfMouseEventArgs args = new dfMouseEventArgs(activeControl, dfMouseButtons.Left, 0, ray, (Vector2) center, 0.0f);
          activeControl.OnMouseDown(args);
          this.buttonDownTarget = activeControl;
        }
        if (this.adapter.GetKeyUp(this.joystickClickButton))
        {
          if ((UnityEngine.Object) this.buttonDownTarget == (UnityEngine.Object) activeControl)
            activeControl.DoClick();
          Vector3 center = activeControl.GetCenter();
          Camera camera = activeControl.GetCamera();
          Ray ray = camera.ScreenPointToRay(camera.WorldToScreenPoint(center));
          dfMouseEventArgs args = new dfMouseEventArgs(activeControl, dfMouseButtons.Left, 0, ray, (Vector2) center, 0.0f);
          activeControl.OnMouseUp(args);
          this.buttonDownTarget = (dfControl) null;
        }
      }
      for (KeyCode keyCode = KeyCode.Joystick1Button0; keyCode <= KeyCode.Joystick1Button19; ++keyCode)
      {
        if (this.adapter.GetKeyDown(keyCode))
          activeControl.OnKeyDown(new dfKeyEventArgs(activeControl, keyCode, false, false, false));
      }
    }
    catch (UnityException ex)
    {
      Debug.LogError((object) ex.ToString(), (UnityEngine.Object) this);
      this.useJoystick = false;
    }
  }

  private void processKeyEvent(EventType eventType, KeyCode keyCode, EventModifiers modifiers)
  {
    dfControl activeControl = dfGUIManager.ActiveControl;
    if ((UnityEngine.Object) activeControl == (UnityEngine.Object) null || !activeControl.IsEnabled || !activeControl.transform.IsChildOf(this.transform))
      return;
    bool Control = (modifiers & EventModifiers.Control) == EventModifiers.Control || (modifiers & EventModifiers.Command) == EventModifiers.Command;
    bool Shift = (modifiers & EventModifiers.Shift) == EventModifiers.Shift;
    bool Alt = (modifiers & EventModifiers.Alt) == EventModifiers.Alt;
    dfKeyEventArgs args = new dfKeyEventArgs(activeControl, keyCode, Control, Shift, Alt);
    if (keyCode >= KeyCode.Space && keyCode <= KeyCode.Z)
    {
      char c = (char) keyCode;
      args.Character = !Shift ? char.ToLower(c) : char.ToUpper(c);
    }
    switch (eventType)
    {
      case EventType.KeyDown:
        activeControl.OnKeyDown(args);
        break;
      case EventType.KeyUp:
        activeControl.OnKeyUp(args);
        break;
    }
    if (args.Used || eventType != EventType.KeyUp)
      ;
  }

  private bool processKeyboard()
  {
    dfControl activeControl = dfGUIManager.ActiveControl;
    if ((UnityEngine.Object) activeControl == (UnityEngine.Object) null || string.IsNullOrEmpty(Input.inputString) || !activeControl.transform.IsChildOf(this.transform))
      return false;
    foreach (char ch in Input.inputString)
    {
      switch (ch)
      {
        case '\b':
        case '\n':
          continue;
        default:
          KeyCode Key = (KeyCode) ch;
          activeControl.OnKeyPress(new dfKeyEventArgs(activeControl, Key, false, false, false)
          {
            Character = ch
          });
          continue;
      }
    }
    return true;
  }

  private bool processTouchInput()
  {
    if (this.touchInputSource == null)
    {
      foreach (dfTouchInputSourceComponent inputSourceComponent in ((IEnumerable<dfTouchInputSourceComponent>) this.GetComponents<dfTouchInputSourceComponent>()).OrderByDescending<dfTouchInputSourceComponent, int>((Func<dfTouchInputSourceComponent, int>) (x => x.Priority)).ToArray<dfTouchInputSourceComponent>())
      {
        if (inputSourceComponent.enabled)
        {
          this.touchInputSource = inputSourceComponent.Source;
          if (this.touchInputSource != null)
            break;
        }
      }
      if (this.touchInputSource == null)
        this.touchInputSource = (IDFTouchInputSource) dfMobileTouchInputSource.Instance;
    }
    this.touchInputSource.Update();
    if (this.touchInputSource.TouchCount == 0)
      return false;
    this.touchHandler.Process(this.transform, this.renderCamera, this.touchInputSource, this.retainFocus);
    return true;
  }

  public bool TestRectUnderMouseEquipment(Bounds b)
  {
    if (!this.renderCamera.enabled)
      return false;
    Vector2 mousePosition = this.adapter.GetMousePosition();
    Vector2 rectSize = BraveCameraUtility.GetRectSize();
    float num1 = (float) ((double) Screen.width * (double) rectSize.x / 2.0);
    float num2 = (float) ((double) Screen.height * (double) rectSize.y / 2.0);
    Vector2 vector = (mousePosition - new Vector2((float) Screen.width / 2f, (float) Screen.height / 2f)) / ((float) Screen.height / 2f) * this.renderCamera.orthographicSize;
    Vector3 vector3 = new Vector3(num1 / 2f / num2 * this.renderCamera.orthographicSize, 0.0f, 0.0f);
    Ray ray = new Ray(this.renderCamera.transform.position + vector.ToVector3ZUp() + vector3, this.renderCamera.transform.forward);
    Debug.DrawRay(ray.origin, ray.direction, Color.yellow, 5f);
    return b.IntersectRay(ray);
  }

  private Vector2 PreprocessUltrawideAmmonomiconMousePosition(Vector2 inPosition)
  {
    Vector2 vector2 = inPosition;
    Rect rect = BraveCameraUtility.GetRect();
    float num1 = (float) Screen.height * rect.height;
    float num2 = (float) Screen.width * rect.width;
    float aspect = BraveCameraUtility.ASPECT;
    vector2.x -= (float) ((double) Screen.width * (1.0 - (double) rect.width) / 2.0);
    vector2.y -= (float) ((double) Screen.height * (1.0 - (double) rect.height) / 2.0);
    if ((double) aspect > 1.7777777910232544)
    {
      float num3 = num1 * 1.77777779f;
      vector2.x -= (float) (((double) num2 - (double) num3) / 2.0);
      vector2.x = Mathf.Lerp(0.0f, 1920f, Mathf.Clamp01(vector2.x / num3));
      vector2.y = Mathf.Lerp(0.0f, 1080f, vector2.y / num1);
    }
    else
    {
      float num4 = num2 * (9f / 16f);
      vector2.y -= (float) (((double) num1 - (double) num4) / 2.0);
      vector2.x = Mathf.Lerp(0.0f, 1920f, vector2.x / num2);
      vector2.y = Mathf.Lerp(0.0f, 1080f, Mathf.Clamp01(vector2.y / num4));
    }
    return vector2;
  }

  private void processMouseInput()
  {
    if ((UnityEngine.Object) this.guiManager == (UnityEngine.Object) null)
      return;
    Vector2 vector2 = this.adapter.GetMousePosition();
    if (AmmonomiconController.HasInstance)
    {
      if ((UnityEngine.Object) AmmonomiconController.Instance.CurrentRightPageRenderer != (UnityEngine.Object) null && (UnityEngine.Object) AmmonomiconController.Instance.CurrentRightPageRenderer.guiManager == (UnityEngine.Object) this.guiManager)
      {
        vector2 = this.PreprocessUltrawideAmmonomiconMousePosition(vector2);
        vector2.x -= 960f;
      }
      else if ((UnityEngine.Object) AmmonomiconController.Instance.CurrentLeftPageRenderer != (UnityEngine.Object) null && (UnityEngine.Object) AmmonomiconController.Instance.CurrentLeftPageRenderer.guiManager == (UnityEngine.Object) this.guiManager)
        vector2 = this.PreprocessUltrawideAmmonomiconMousePosition(vector2);
    }
    Ray ray = this.renderCamera.ScreenPointToRay((Vector3) vector2);
    dfInputManager.controlUnderMouse = dfGUIManager.HitTestAll(vector2);
    if ((UnityEngine.Object) dfInputManager.controlUnderMouse != (UnityEngine.Object) null && !dfInputManager.controlUnderMouse.transform.IsChildOf(this.transform))
      dfInputManager.controlUnderMouse = (dfControl) null;
    this.mouseHandler.ProcessInput(this, this.adapter, ray, dfInputManager.controlUnderMouse, this.retainFocus);
  }

  internal static int raycastHitSorter(RaycastHit lhs, RaycastHit rhs)
  {
    return lhs.distance.CompareTo(rhs.distance);
  }

  internal dfControl clipCast(RaycastHit[] hits)
  {
    if (hits == null || hits.Length == 0)
      return (dfControl) null;
    dfControl dfControl = (dfControl) null;
    dfControl modalControl = dfGUIManager.GetModalControl();
    for (int index = hits.Length - 1; index >= 0; --index)
    {
      RaycastHit hit = hits[index];
      dfControl component = hit.transform.GetComponent<dfControl>();
      if (!((UnityEngine.Object) component == (UnityEngine.Object) null) && (!((UnityEngine.Object) modalControl != (UnityEngine.Object) null) || component.transform.IsChildOf(modalControl.transform)) && component.enabled && (double) dfInputManager.combinedOpacity(component) > 0.0099999997764825821 && component.IsEnabled && component.IsVisible && component.transform.IsChildOf(this.transform) && dfInputManager.isInsideClippingRegion(hit.point, component) && ((UnityEngine.Object) dfControl == (UnityEngine.Object) null || component.RenderOrder > dfControl.RenderOrder))
        dfControl = component;
    }
    return dfControl;
  }

  internal static bool isInsideClippingRegion(Vector3 point, dfControl control)
  {
    for (; (UnityEngine.Object) control != (UnityEngine.Object) null; control = control.Parent)
    {
      Plane[] clippingPlanes = !control.ClipChildren ? (Plane[]) null : control.GetClippingPlanes();
      if (clippingPlanes != null && clippingPlanes.Length > 0)
      {
        for (int index = 0; index < clippingPlanes.Length; ++index)
        {
          if (!clippingPlanes[index].GetSide(point))
            return false;
        }
      }
    }
    return true;
  }

  private static float combinedOpacity(dfControl control)
  {
    float num = 1f;
    for (; (UnityEngine.Object) control != (UnityEngine.Object) null; control = control.Parent)
      num *= control.Opacity;
    return num;
  }

  private class TouchInputManager
  {
    private List<dfInputManager.TouchInputManager.ControlTouchTracker> tracked = new List<dfInputManager.TouchInputManager.ControlTouchTracker>();
    private List<int> untracked = new List<int>();
    private dfInputManager manager;

    private TouchInputManager()
    {
    }

    public TouchInputManager(dfInputManager manager) => this.manager = manager;

    internal void Process(
      Transform transform,
      Camera renderCamera,
      IDFTouchInputSource input,
      bool retainFocusSetting)
    {
      dfInputManager.controlUnderMouse = (dfControl) null;
      IList<dfTouchInfo> touches = input.Touches;
      for (int index1 = 0; index1 < touches.Count; ++index1)
      {
        dfTouchInfo touch = touches[index1];
        dfControl dfControl = dfGUIManager.HitTestAll(touch.position);
        if ((UnityEngine.Object) dfControl != (UnityEngine.Object) null && dfControl.transform.IsChildOf(this.manager.transform))
          dfInputManager.controlUnderMouse = dfControl;
        if ((UnityEngine.Object) dfInputManager.controlUnderMouse == (UnityEngine.Object) null && touch.phase == TouchPhase.Began)
        {
          if (!retainFocusSetting && this.untracked.Count == 0)
          {
            dfControl activeControl = dfGUIManager.ActiveControl;
            if ((UnityEngine.Object) activeControl != (UnityEngine.Object) null && activeControl.transform.IsChildOf(this.manager.transform))
              activeControl.Unfocus();
          }
          this.untracked.Add(touch.fingerId);
        }
        else if (this.untracked.Contains(touch.fingerId))
        {
          if (touch.phase == TouchPhase.Ended)
            this.untracked.Remove(touch.fingerId);
        }
        else
        {
          Ray ray = renderCamera.ScreenPointToRay((Vector3) touch.position);
          dfInputManager.TouchInputManager.TouchRaycast info = new dfInputManager.TouchInputManager.TouchRaycast(dfInputManager.controlUnderMouse, touch, ray);
          dfInputManager.TouchInputManager.ControlTouchTracker controlTouchTracker1 = this.tracked.FirstOrDefault<dfInputManager.TouchInputManager.ControlTouchTracker>((Func<dfInputManager.TouchInputManager.ControlTouchTracker, bool>) (x => x.IsTrackingFinger(info.FingerID)));
          if (controlTouchTracker1 != null)
          {
            controlTouchTracker1.Process(info);
          }
          else
          {
            bool flag = false;
            for (int index2 = 0; index2 < this.tracked.Count; ++index2)
            {
              if (this.tracked[index2].Process(info))
              {
                flag = true;
                break;
              }
            }
            if (!flag && (UnityEngine.Object) dfInputManager.controlUnderMouse != (UnityEngine.Object) null && !this.tracked.Any<dfInputManager.TouchInputManager.ControlTouchTracker>((Func<dfInputManager.TouchInputManager.ControlTouchTracker, bool>) (x => (UnityEngine.Object) x.control == (UnityEngine.Object) dfInputManager.controlUnderMouse)))
            {
              dfInputManager.TouchInputManager.ControlTouchTracker controlTouchTracker2 = new dfInputManager.TouchInputManager.ControlTouchTracker(this.manager, dfInputManager.controlUnderMouse);
              this.tracked.Add(controlTouchTracker2);
              controlTouchTracker2.Process(info);
            }
          }
        }
      }
    }

    private dfControl clipCast(Transform transform, RaycastHit[] hits)
    {
      if (hits == null || hits.Length == 0)
        return (dfControl) null;
      dfControl dfControl = (dfControl) null;
      dfControl modalControl = dfGUIManager.GetModalControl();
      for (int index = hits.Length - 1; index >= 0; --index)
      {
        RaycastHit hit = hits[index];
        dfControl component = hit.transform.GetComponent<dfControl>();
        if (!((UnityEngine.Object) component == (UnityEngine.Object) null) && (!((UnityEngine.Object) modalControl != (UnityEngine.Object) null) || component.transform.IsChildOf(modalControl.transform)) && component.enabled && component.IsEnabled && component.IsVisible && component.transform.IsChildOf(transform) && this.isInsideClippingRegion(hit, component) && ((UnityEngine.Object) dfControl == (UnityEngine.Object) null || component.RenderOrder > dfControl.RenderOrder))
          dfControl = component;
      }
      return dfControl;
    }

    private bool isInsideClippingRegion(RaycastHit hit, dfControl control)
    {
      Vector3 point = hit.point;
      for (; (UnityEngine.Object) control != (UnityEngine.Object) null; control = control.Parent)
      {
        Plane[] clippingPlanes = !control.ClipChildren ? (Plane[]) null : control.GetClippingPlanes();
        if (clippingPlanes != null && clippingPlanes.Length > 0)
        {
          for (int index = 0; index < clippingPlanes.Length; ++index)
          {
            if (!clippingPlanes[index].GetSide(point))
              return false;
          }
        }
      }
      return true;
    }

    private class ControlTouchTracker
    {
      public readonly dfControl control;
      public readonly Dictionary<int, dfInputManager.TouchInputManager.TouchRaycast> touches = new Dictionary<int, dfInputManager.TouchInputManager.TouchRaycast>();
      public readonly List<int> capture = new List<int>();
      private dfInputManager manager;
      private Vector3 controlStartPosition;
      private dfDragDropState dragState;
      private object dragData;

      public ControlTouchTracker(dfInputManager manager, dfControl control)
      {
        this.manager = manager;
        this.control = control;
        this.controlStartPosition = control.transform.position;
      }

      public bool IsDragging => this.dragState == dfDragDropState.Dragging;

      public int TouchCount => this.touches.Count;

      public bool IsTrackingFinger(int fingerID) => this.touches.ContainsKey(fingerID);

      public bool Process(dfInputManager.TouchInputManager.TouchRaycast info)
      {
        if (this.IsDragging)
        {
          if (!this.capture.Contains(info.FingerID))
            return false;
          if (info.Phase == TouchPhase.Stationary)
            return true;
          if (info.Phase == TouchPhase.Canceled)
          {
            this.control.OnDragEnd(new dfDragEventArgs(this.control, dfDragDropState.Cancelled, this.dragData, info.ray, info.position));
            this.dragState = dfDragDropState.None;
            this.touches.Clear();
            this.capture.Clear();
            return true;
          }
          if (info.Phase != TouchPhase.Ended)
            return true;
          if ((UnityEngine.Object) info.control == (UnityEngine.Object) null || (UnityEngine.Object) info.control == (UnityEngine.Object) this.control)
          {
            this.control.OnDragEnd(new dfDragEventArgs(this.control, dfDragDropState.CancelledNoTarget, this.dragData, info.ray, info.position));
            this.dragState = dfDragDropState.None;
            this.touches.Clear();
            this.capture.Clear();
            return true;
          }
          dfDragEventArgs args = new dfDragEventArgs(info.control, dfDragDropState.Dragging, this.dragData, info.ray, info.position);
          info.control.OnDragDrop(args);
          if (!args.Used || args.State != dfDragDropState.Dropped)
            args.State = dfDragDropState.Cancelled;
          this.control.OnDragEnd(new dfDragEventArgs(this.control, args.State, this.dragData, info.ray, info.position)
          {
            Target = info.control
          });
          this.dragState = dfDragDropState.None;
          this.touches.Clear();
          this.capture.Clear();
          return true;
        }
        if (!this.touches.ContainsKey(info.FingerID))
        {
          if ((UnityEngine.Object) info.control != (UnityEngine.Object) this.control)
            return false;
          this.touches[info.FingerID] = info;
          if (this.touches.Count == 1)
          {
            this.control.OnMouseEnter((dfMouseEventArgs) (dfTouchEventArgs) info);
            if (info.Phase == TouchPhase.Began)
            {
              this.capture.Add(info.FingerID);
              this.controlStartPosition = this.control.transform.position;
              this.control.OnMouseDown((dfMouseEventArgs) (dfTouchEventArgs) info);
              if (Event.current != null)
                Event.current.Use();
            }
            return true;
          }
          if (info.Phase == TouchPhase.Began)
            this.control.OnMultiTouch(new dfTouchEventArgs(this.control, this.getActiveTouches(), info.ray));
          return true;
        }
        if (info.Phase == TouchPhase.Canceled || info.Phase == TouchPhase.Ended)
        {
          TouchPhase phase = info.Phase;
          dfInputManager.TouchInputManager.TouchRaycast touch = this.touches[info.FingerID];
          this.touches.Remove(info.FingerID);
          if (this.touches.Count == 0 && phase != TouchPhase.Canceled)
          {
            if (this.capture.Contains(info.FingerID))
            {
              if (this.canFireClickEvent(info, touch) && (UnityEngine.Object) info.control == (UnityEngine.Object) this.control)
              {
                if (info.touch.tapCount > 1)
                  this.control.OnDoubleClick((dfMouseEventArgs) (dfTouchEventArgs) info);
                else
                  this.control.OnClick((dfMouseEventArgs) (dfTouchEventArgs) info);
              }
              info.control = this.control;
              if ((bool) (UnityEngine.Object) this.control)
                this.control.OnMouseUp((dfMouseEventArgs) (dfTouchEventArgs) info);
            }
            this.capture.Remove(info.FingerID);
            return true;
          }
          this.capture.Remove(info.FingerID);
          if (this.touches.Count == 1)
          {
            this.control.OnMultiTouchEnd();
            return true;
          }
        }
        if (this.touches.Count > 1)
        {
          this.control.OnMultiTouch(new dfTouchEventArgs(this.control, this.getActiveTouches(), info.ray));
          return true;
        }
        if (!this.IsDragging && info.Phase == TouchPhase.Stationary)
        {
          if (!((UnityEngine.Object) info.control == (UnityEngine.Object) this.control))
            return false;
          this.control.OnMouseHover((dfMouseEventArgs) (dfTouchEventArgs) info);
          return true;
        }
        if (this.capture.Contains(info.FingerID) && this.dragState == dfDragDropState.None && info.Phase == TouchPhase.Moved)
        {
          dfDragEventArgs args = (dfDragEventArgs) info;
          this.control.OnDragStart(args);
          if (args.State == dfDragDropState.Dragging && args.Used)
          {
            this.dragState = dfDragDropState.Dragging;
            this.dragData = args.Data;
            return true;
          }
          this.dragState = dfDragDropState.Denied;
        }
        if ((UnityEngine.Object) info.control != (UnityEngine.Object) this.control && !this.capture.Contains(info.FingerID))
        {
          info.control = this.control;
          this.control.OnMouseLeave((dfMouseEventArgs) (dfTouchEventArgs) info);
          this.touches.Remove(info.FingerID);
          return true;
        }
        info.control = this.control;
        this.control.OnMouseMove((dfMouseEventArgs) (dfTouchEventArgs) info);
        return true;
      }

      private bool canFireClickEvent(
        dfInputManager.TouchInputManager.TouchRaycast info,
        dfInputManager.TouchInputManager.TouchRaycast touch)
      {
        if ((UnityEngine.Object) this.control == (UnityEngine.Object) null)
          return false;
        float units = this.control.PixelsToUnits();
        return (double) Vector3.Distance(this.controlStartPosition / units, this.control.transform.position / units) <= 1.0;
      }

      private List<dfTouchInfo> getActiveTouches()
      {
        IList<dfTouchInfo> touches = this.manager.touchInputSource.Touches;
        List<dfTouchInfo> result = this.touches.Select<KeyValuePair<int, dfInputManager.TouchInputManager.TouchRaycast>, dfTouchInfo>((Func<KeyValuePair<int, dfInputManager.TouchInputManager.TouchRaycast>, dfTouchInfo>) (x => x.Value.touch)).ToList<dfTouchInfo>();
        int i = 0;
        while (i < result.Count)
        {
          bool flag = false;
          int index = 0;
          while (i < touches.Count)
          {
            if (touches[index].fingerId == result[i].fingerId)
            {
              flag = true;
              break;
            }
            ++index;
          }
          if (flag)
          {
            result[i] = touches.First<dfTouchInfo>((Func<dfTouchInfo, bool>) (x => x.fingerId == result[i].fingerId));
            ++i;
          }
          else
            result.RemoveAt(i);
        }
        return result;
      }
    }

    private class TouchRaycast
    {
      public dfControl control;
      public dfTouchInfo touch;
      public Ray ray;
      public Vector2 position;

      public TouchRaycast(dfControl control, dfTouchInfo touch, Ray ray)
      {
        this.control = control;
        this.touch = touch;
        this.ray = ray;
        this.position = touch.position;
      }

      public int FingerID => this.touch.fingerId;

      public TouchPhase Phase => this.touch.phase;

      public static implicit operator dfTouchEventArgs(
        dfInputManager.TouchInputManager.TouchRaycast touch)
      {
        return new dfTouchEventArgs(touch.control, touch.touch, touch.ray);
      }

      public static implicit operator dfDragEventArgs(
        dfInputManager.TouchInputManager.TouchRaycast touch)
      {
        return new dfDragEventArgs(touch.control, dfDragDropState.None, (object) null, touch.ray, touch.position);
      }
    }
  }

  private class MouseInputManager
  {
    private const string scrollAxisName = "Mouse ScrollWheel";
    private const float DOUBLECLICK_TIME = 0.25f;
    private const int DRAG_START_DELTA = 2;
    private dfControl activeControl;
    private Vector3 activeControlPosition;
    private Vector2 lastPosition = Vector2.one * (float) int.MinValue;
    private Vector2 mouseMoveDelta = Vector2.zero;
    private float lastClickTime;
    private float lastHoverTime;
    private dfDragDropState dragState;
    private object dragData;
    private dfControl lastDragControl;
    private dfMouseButtons buttonsDown;
    private dfMouseButtons buttonsReleased;
    private dfMouseButtons buttonsPressed;

    public void ProcessInput(
      dfInputManager manager,
      IInputAdapter adapter,
      Ray ray,
      dfControl control,
      bool retainFocusSetting)
    {
      Vector2 mousePosition = adapter.GetMousePosition();
      this.buttonsDown = dfMouseButtons.None;
      this.buttonsReleased = dfMouseButtons.None;
      this.buttonsPressed = dfMouseButtons.None;
      dfInputManager.MouseInputManager.getMouseButtonInfo(adapter, ref this.buttonsDown, ref this.buttonsReleased, ref this.buttonsPressed);
      float num = adapter.GetAxis("Mouse ScrollWheel");
      if (!Mathf.Approximately(num, 0.0f))
        num = Mathf.Sign(num) * Mathf.Max(1f, Mathf.Abs(num));
      this.mouseMoveDelta = mousePosition - this.lastPosition;
      this.lastPosition = mousePosition;
      if (this.dragState == dfDragDropState.Dragging)
      {
        if (this.buttonsReleased == dfMouseButtons.None)
        {
          if ((UnityEngine.Object) control == (UnityEngine.Object) this.activeControl)
            return;
          if ((UnityEngine.Object) control != (UnityEngine.Object) this.lastDragControl)
          {
            if ((UnityEngine.Object) this.lastDragControl != (UnityEngine.Object) null)
              this.lastDragControl.OnDragLeave(new dfDragEventArgs(this.lastDragControl, this.dragState, this.dragData, ray, mousePosition));
            if ((UnityEngine.Object) control != (UnityEngine.Object) null)
            {
              dfDragEventArgs args = new dfDragEventArgs(control, this.dragState, this.dragData, ray, mousePosition);
              control.OnDragEnter(args);
            }
            this.lastDragControl = control;
          }
          else
          {
            if (!((UnityEngine.Object) control != (UnityEngine.Object) null) || (double) this.mouseMoveDelta.magnitude <= 1.0)
              return;
            dfDragEventArgs args = new dfDragEventArgs(control, this.dragState, this.dragData, ray, mousePosition);
            control.OnDragOver(args);
          }
        }
        else
        {
          if ((UnityEngine.Object) control != (UnityEngine.Object) null && (UnityEngine.Object) control != (UnityEngine.Object) this.activeControl)
          {
            dfDragEventArgs args = new dfDragEventArgs(control, dfDragDropState.Dragging, this.dragData, ray, mousePosition);
            control.OnDragDrop(args);
            if (!args.Used || args.State == dfDragDropState.Dragging)
              args.State = dfDragDropState.Cancelled;
            this.activeControl.OnDragEnd(new dfDragEventArgs(this.activeControl, args.State, args.Data, ray, mousePosition)
            {
              Target = control
            });
          }
          else
            this.activeControl.OnDragEnd(new dfDragEventArgs(this.activeControl, !((UnityEngine.Object) control == (UnityEngine.Object) null) ? dfDragDropState.Cancelled : dfDragDropState.CancelledNoTarget, this.dragData, ray, mousePosition));
          this.dragState = dfDragDropState.None;
          this.lastDragControl = (dfControl) null;
          this.activeControl = (dfControl) null;
          this.lastClickTime = 0.0f;
          this.lastHoverTime = 0.0f;
          this.lastPosition = mousePosition;
        }
      }
      else
      {
        if (this.buttonsPressed != dfMouseButtons.None)
        {
          this.lastHoverTime = UnityEngine.Time.realtimeSinceStartup + manager.hoverStartDelay;
          if ((UnityEngine.Object) this.activeControl != (UnityEngine.Object) null)
          {
            if (this.activeControl.transform.IsChildOf(manager.transform))
              this.activeControl.OnMouseDown(new dfMouseEventArgs(this.activeControl, this.buttonsPressed, 0, ray, mousePosition, num));
          }
          else if ((UnityEngine.Object) control == (UnityEngine.Object) null || control.transform.IsChildOf(manager.transform))
          {
            this.setActive(manager, control, mousePosition, ray);
            if ((UnityEngine.Object) control != (UnityEngine.Object) null)
            {
              dfGUIManager.SetFocus(control);
              control.OnMouseDown(new dfMouseEventArgs(control, this.buttonsPressed, 0, ray, mousePosition, num));
            }
            else if (!retainFocusSetting)
            {
              dfControl activeControl = dfGUIManager.ActiveControl;
              if ((UnityEngine.Object) activeControl != (UnityEngine.Object) null && activeControl.transform.IsChildOf(manager.transform))
                activeControl.Unfocus();
            }
          }
          if (this.buttonsReleased == dfMouseButtons.None)
            return;
        }
        if (this.buttonsReleased != dfMouseButtons.None)
        {
          this.lastHoverTime = UnityEngine.Time.realtimeSinceStartup + manager.hoverStartDelay;
          if ((UnityEngine.Object) this.activeControl == (UnityEngine.Object) null)
          {
            this.setActive(manager, control, mousePosition, ray);
          }
          else
          {
            if ((UnityEngine.Object) this.activeControl == (UnityEngine.Object) control && this.buttonsDown == dfMouseButtons.None)
            {
              float units = this.activeControl.PixelsToUnits();
              if ((double) Vector3.Distance(this.activeControlPosition / units, this.activeControl.transform.position / units) <= 1.0)
              {
                if ((double) UnityEngine.Time.realtimeSinceStartup - (double) this.lastClickTime < 0.25)
                {
                  this.lastClickTime = 0.0f;
                  this.activeControl.OnDoubleClick(new dfMouseEventArgs(this.activeControl, this.buttonsReleased, 1, ray, mousePosition, num));
                }
                else
                {
                  this.lastClickTime = UnityEngine.Time.realtimeSinceStartup;
                  this.activeControl.OnClick(new dfMouseEventArgs(this.activeControl, this.buttonsReleased, 1, ray, mousePosition, num));
                }
              }
            }
            this.activeControl.OnMouseUp(new dfMouseEventArgs(this.activeControl, this.buttonsReleased, 0, ray, mousePosition, num));
            if (this.buttonsDown != dfMouseButtons.None || !((UnityEngine.Object) this.activeControl != (UnityEngine.Object) control))
              return;
            this.setActive(manager, (dfControl) null, mousePosition, ray);
          }
        }
        else
        {
          if ((UnityEngine.Object) this.activeControl != (UnityEngine.Object) null && (UnityEngine.Object) this.activeControl == (UnityEngine.Object) control && (double) this.mouseMoveDelta.magnitude == 0.0 && (double) UnityEngine.Time.realtimeSinceStartup - (double) this.lastHoverTime > (double) manager.hoverNotifactionFrequency)
          {
            this.activeControl.OnMouseHover(new dfMouseEventArgs(this.activeControl, this.buttonsDown, 0, ray, mousePosition, num));
            this.lastHoverTime = UnityEngine.Time.realtimeSinceStartup;
          }
          if (this.buttonsDown == dfMouseButtons.None)
          {
            if ((double) num != 0.0 && (UnityEngine.Object) control != (UnityEngine.Object) null)
            {
              this.setActive(manager, control, mousePosition, ray);
              control.OnMouseWheel(new dfMouseEventArgs(control, this.buttonsDown, 0, ray, mousePosition, num));
              return;
            }
            this.setActive(manager, control, mousePosition, ray);
          }
          else if (this.buttonsDown != dfMouseButtons.None && (UnityEngine.Object) this.activeControl != (UnityEngine.Object) null)
          {
            if (!((UnityEngine.Object) control != (UnityEngine.Object) null) || control.RenderOrder <= this.activeControl.RenderOrder)
              ;
            if ((double) this.mouseMoveDelta.magnitude >= 2.0 && (this.buttonsDown & (dfMouseButtons.Left | dfMouseButtons.Right)) != dfMouseButtons.None && this.dragState != dfDragDropState.Denied)
            {
              dfDragEventArgs args = new dfDragEventArgs(this.activeControl)
              {
                Position = mousePosition
              };
              this.activeControl.OnDragStart(args);
              if (args.State == dfDragDropState.Dragging)
              {
                this.dragState = dfDragDropState.Dragging;
                this.dragData = args.Data;
                return;
              }
              this.dragState = dfDragDropState.Denied;
            }
          }
          if (!((UnityEngine.Object) this.activeControl != (UnityEngine.Object) null) || (double) this.mouseMoveDelta.magnitude < 1.0)
            return;
          this.activeControl.OnMouseMove(new dfMouseEventArgs(this.activeControl, this.buttonsDown, 0, ray, mousePosition, num)
          {
            MoveDelta = this.mouseMoveDelta
          });
        }
      }
    }

    private static void getMouseButtonInfo(
      IInputAdapter adapter,
      ref dfMouseButtons buttonsDown,
      ref dfMouseButtons buttonsReleased,
      ref dfMouseButtons buttonsPressed)
    {
      for (int button = 0; button < 3; ++button)
      {
        if (adapter.GetMouseButton(button))
          buttonsDown |= (dfMouseButtons) (1 << button);
        if (adapter.GetMouseButtonUp(button))
          buttonsReleased |= (dfMouseButtons) (1 << button);
        if (adapter.GetMouseButtonDown(button))
          buttonsPressed |= (dfMouseButtons) (1 << button);
      }
    }

    private void setActive(dfInputManager manager, dfControl control, Vector2 position, Ray ray)
    {
      if ((UnityEngine.Object) this.activeControl != (UnityEngine.Object) null && (UnityEngine.Object) this.activeControl != (UnityEngine.Object) control)
        this.activeControl.OnMouseLeave(new dfMouseEventArgs(this.activeControl)
        {
          Position = position,
          Ray = ray
        });
      if ((UnityEngine.Object) control != (UnityEngine.Object) null && (UnityEngine.Object) control != (UnityEngine.Object) this.activeControl)
      {
        this.lastClickTime = 0.0f;
        this.lastHoverTime = UnityEngine.Time.realtimeSinceStartup + manager.hoverStartDelay;
        control.OnMouseEnter(new dfMouseEventArgs(control)
        {
          Position = position,
          Ray = ray
        });
      }
      this.activeControl = control;
      this.activeControlPosition = !((UnityEngine.Object) control != (UnityEngine.Object) null) ? Vector3.one * float.MinValue : control.transform.position;
      this.lastPosition = position;
      this.dragState = dfDragDropState.None;
    }
  }

  private class DefaultInput : IInputAdapter
  {
    public DefaultInput(Camera renderCam)
    {
    }

    public bool GetKeyDown(KeyCode key) => Input.GetKeyDown(key);

    public bool GetKeyUp(KeyCode key) => Input.GetKeyUp(key);

    public float GetAxis(string axisName) => Input.GetAxis(axisName);

    public Vector2 GetMousePosition() => (Vector2) Input.mousePosition;

    public bool GetMouseButton(int button) => Input.GetMouseButton(button);

    public bool GetMouseButtonDown(int button) => Input.GetMouseButtonDown(button);

    public bool GetMouseButtonUp(int button) => Input.GetMouseButtonUp(button);
  }
}
