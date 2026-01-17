// Decompiled with JetBrains decompiler
// Type: InControl.InControlInputModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

#nullable disable
namespace InControl;

[AddComponentMenu("Event/InControl Input Module")]
public class InControlInputModule : PointerInputModule
{
  public InControlInputModule.Button submitButton = InControlInputModule.Button.Action1;
  public InControlInputModule.Button cancelButton = InControlInputModule.Button.Action2;
  [Range(0.1f, 0.9f)]
  public float analogMoveThreshold = 0.5f;
  public float moveRepeatFirstDuration = 0.8f;
  public float moveRepeatDelayDuration = 0.1f;
  [FormerlySerializedAs("allowMobileDevice")]
  public bool forceModuleActive;
  public bool allowMouseInput = true;
  public bool focusOnMouseHover;
  private InputDevice inputDevice;
  private Vector3 thisMousePosition;
  private Vector3 lastMousePosition;
  private Vector2 thisVectorState;
  private Vector2 lastVectorState;
  private bool thisSubmitState;
  private bool lastSubmitState;
  private bool thisCancelState;
  private bool lastCancelState;
  private float nextMoveRepeatTime;
  private float lastVectorPressedTime;
  private TwoAxisInputControl direction;

  protected InControlInputModule()
  {
    this.direction = new TwoAxisInputControl();
    this.direction.StateThreshold = this.analogMoveThreshold;
  }

  public PlayerAction SubmitAction { get; set; }

  public PlayerAction CancelAction { get; set; }

  public PlayerTwoAxisAction MoveAction { get; set; }

  public override void UpdateModule()
  {
    this.lastMousePosition = this.thisMousePosition;
    this.thisMousePosition = Input.mousePosition;
  }

  public override bool IsModuleSupported()
  {
    return this.forceModuleActive || Input.mousePresent || Input.touchSupported;
  }

  public override bool ShouldActivateModule()
  {
    if (!this.enabled || !this.gameObject.activeInHierarchy)
      return false;
    this.UpdateInputState();
    bool flag = false | this.SubmitWasPressed | this.CancelWasPressed | this.VectorWasPressed;
    if (this.allowMouseInput)
      flag = flag | this.MouseHasMoved | this.MouseButtonIsPressed;
    if (Input.touchCount > 0)
      flag = true;
    return flag;
  }

  public override void ActivateModule()
  {
    base.ActivateModule();
    this.thisMousePosition = Input.mousePosition;
    this.lastMousePosition = Input.mousePosition;
    GameObject selectedGameObject = this.eventSystem.currentSelectedGameObject;
    if ((Object) selectedGameObject == (Object) null)
      selectedGameObject = this.eventSystem.firstSelectedGameObject;
    this.eventSystem.SetSelectedGameObject(selectedGameObject, this.GetBaseEventData());
  }

  public override void Process()
  {
    bool selectedObject = this.SendUpdateEventToSelectedObject();
    if (this.eventSystem.sendNavigationEvents)
    {
      if (!selectedObject)
        selectedObject = this.SendVectorEventToSelectedObject();
      if (!selectedObject)
        this.SendButtonEventToSelectedObject();
    }
    if (this.ProcessTouchEvents() || !this.allowMouseInput)
      return;
    this.ProcessMouseEvent();
  }

  private bool ProcessTouchEvents()
  {
    int touchCount = Input.touchCount;
    for (int index = 0; index < touchCount; ++index)
    {
      UnityEngine.Touch touch = Input.GetTouch(index);
      if (touch.type != UnityEngine.TouchType.Indirect)
      {
        bool pressed;
        bool released;
        PointerEventData pointerEventData = this.GetTouchPointerEventData(touch, out pressed, out released);
        this.ProcessTouchPress(pointerEventData, pressed, released);
        if (!released)
        {
          this.ProcessMove(pointerEventData);
          this.ProcessDrag(pointerEventData);
        }
        else
          this.RemovePointerData(pointerEventData);
      }
    }
    return touchCount > 0;
  }

  private bool SendButtonEventToSelectedObject()
  {
    if ((Object) this.eventSystem.currentSelectedGameObject == (Object) null)
      return false;
    BaseEventData baseEventData = this.GetBaseEventData();
    if (this.SubmitWasPressed)
      ExecuteEvents.Execute<ISubmitHandler>(this.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
    else if (!this.SubmitWasReleased)
      ;
    if (this.CancelWasPressed)
      ExecuteEvents.Execute<ICancelHandler>(this.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
    return baseEventData.used;
  }

  private bool SendVectorEventToSelectedObject()
  {
    if (!this.VectorWasPressed)
      return false;
    AxisEventData axisEventData = this.GetAxisEventData(this.thisVectorState.x, this.thisVectorState.y, 0.5f);
    if (axisEventData.moveDir != MoveDirection.None)
    {
      if ((Object) this.eventSystem.currentSelectedGameObject == (Object) null)
        this.eventSystem.SetSelectedGameObject(this.eventSystem.firstSelectedGameObject, this.GetBaseEventData());
      else
        ExecuteEvents.Execute<IMoveHandler>(this.eventSystem.currentSelectedGameObject, (BaseEventData) axisEventData, ExecuteEvents.moveHandler);
      this.SetVectorRepeatTimer();
    }
    return axisEventData.used;
  }

  protected override void ProcessMove(PointerEventData pointerEvent)
  {
    GameObject pointerEnter = pointerEvent.pointerEnter;
    base.ProcessMove(pointerEvent);
    if (!this.focusOnMouseHover || !((Object) pointerEnter != (Object) pointerEvent.pointerEnter))
      return;
    this.eventSystem.SetSelectedGameObject(ExecuteEvents.GetEventHandler<ISelectHandler>(pointerEvent.pointerEnter), (BaseEventData) pointerEvent);
  }

  private void Update() => this.direction.Filter(this.Device.Direction, Time.deltaTime);

  private void UpdateInputState()
  {
    this.lastVectorState = this.thisVectorState;
    this.thisVectorState = Vector2.zero;
    TwoAxisInputControl axisInputControl = (TwoAxisInputControl) this.MoveAction ?? this.direction;
    if (Utility.AbsoluteIsOverThreshold(axisInputControl.X, this.analogMoveThreshold))
      this.thisVectorState.x = Mathf.Sign(axisInputControl.X);
    if (Utility.AbsoluteIsOverThreshold(axisInputControl.Y, this.analogMoveThreshold))
      this.thisVectorState.y = Mathf.Sign(axisInputControl.Y);
    if (this.VectorIsReleased)
      this.nextMoveRepeatTime = 0.0f;
    if (this.VectorIsPressed)
    {
      if (this.lastVectorState == Vector2.zero)
        this.nextMoveRepeatTime = (double) Time.realtimeSinceStartup <= (double) this.lastVectorPressedTime + 0.10000000149011612 ? Time.realtimeSinceStartup + this.moveRepeatDelayDuration : Time.realtimeSinceStartup + this.moveRepeatFirstDuration;
      this.lastVectorPressedTime = Time.realtimeSinceStartup;
    }
    this.lastSubmitState = this.thisSubmitState;
    this.thisSubmitState = this.SubmitAction != null ? this.SubmitAction.IsPressed : this.SubmitButton.IsPressed;
    this.lastCancelState = this.thisCancelState;
    this.thisCancelState = this.CancelAction != null ? this.CancelAction.IsPressed : this.CancelButton.IsPressed;
  }

  public InputDevice Device
  {
    set => this.inputDevice = value;
    get => this.inputDevice ?? InputManager.ActiveDevice;
  }

  private InputControl SubmitButton => this.Device.GetControl((InputControlType) this.submitButton);

  private InputControl CancelButton => this.Device.GetControl((InputControlType) this.cancelButton);

  private void SetVectorRepeatTimer()
  {
    this.nextMoveRepeatTime = Mathf.Max(this.nextMoveRepeatTime, Time.realtimeSinceStartup + this.moveRepeatDelayDuration);
  }

  private bool VectorIsPressed => this.thisVectorState != Vector2.zero;

  private bool VectorIsReleased => this.thisVectorState == Vector2.zero;

  private bool VectorHasChanged => this.thisVectorState != this.lastVectorState;

  private bool VectorWasPressed
  {
    get
    {
      if (this.VectorIsPressed && (double) Time.realtimeSinceStartup > (double) this.nextMoveRepeatTime)
        return true;
      return this.VectorIsPressed && this.lastVectorState == Vector2.zero;
    }
  }

  private bool SubmitWasPressed
  {
    get => this.thisSubmitState && this.thisSubmitState != this.lastSubmitState;
  }

  private bool SubmitWasReleased
  {
    get => !this.thisSubmitState && this.thisSubmitState != this.lastSubmitState;
  }

  private bool CancelWasPressed
  {
    get => this.thisCancelState && this.thisCancelState != this.lastCancelState;
  }

  private bool MouseHasMoved
  {
    get => (double) (this.thisMousePosition - this.lastMousePosition).sqrMagnitude > 0.0;
  }

  private bool MouseButtonIsPressed => Input.GetMouseButtonDown(0);

  protected bool SendUpdateEventToSelectedObject()
  {
    if ((Object) this.eventSystem.currentSelectedGameObject == (Object) null)
      return false;
    BaseEventData baseEventData = this.GetBaseEventData();
    ExecuteEvents.Execute<IUpdateSelectedHandler>(this.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
    return baseEventData.used;
  }

  protected void ProcessMouseEvent() => this.ProcessMouseEvent(0);

  protected void ProcessMouseEvent(int id)
  {
    PointerInputModule.MouseState pointerEventData = this.GetMousePointerEventData(id);
    PointerInputModule.MouseButtonEventData eventData = pointerEventData.GetButtonState(PointerEventData.InputButton.Left).eventData;
    this.ProcessMousePress(eventData);
    this.ProcessMove(eventData.buttonData);
    this.ProcessDrag(eventData.buttonData);
    this.ProcessMousePress(pointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData);
    this.ProcessDrag(pointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
    this.ProcessMousePress(pointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
    this.ProcessDrag(pointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
    if (Mathf.Approximately(eventData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
      return;
    ExecuteEvents.ExecuteHierarchy<IScrollHandler>(ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject), (BaseEventData) eventData.buttonData, ExecuteEvents.scrollHandler);
  }

  protected void ProcessMousePress(PointerInputModule.MouseButtonEventData data)
  {
    PointerEventData buttonData = data.buttonData;
    GameObject gameObject1 = buttonData.pointerCurrentRaycast.gameObject;
    if (data.PressedThisFrame())
    {
      buttonData.eligibleForClick = true;
      buttonData.delta = Vector2.zero;
      buttonData.dragging = false;
      buttonData.useDragThreshold = true;
      buttonData.pressPosition = buttonData.position;
      buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
      this.DeselectIfSelectionChanged(gameObject1, (BaseEventData) buttonData);
      GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject1, (BaseEventData) buttonData, ExecuteEvents.pointerDownHandler);
      if ((Object) gameObject2 == (Object) null)
        gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
      float unscaledTime = Time.unscaledTime;
      if ((Object) gameObject2 == (Object) buttonData.lastPress)
      {
        if ((double) (unscaledTime - buttonData.clickTime) < 0.30000001192092896)
          ++buttonData.clickCount;
        else
          buttonData.clickCount = 1;
        buttonData.clickTime = unscaledTime;
      }
      else
        buttonData.clickCount = 1;
      buttonData.pointerPress = gameObject2;
      buttonData.rawPointerPress = gameObject1;
      buttonData.clickTime = unscaledTime;
      buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject1);
      if ((Object) buttonData.pointerDrag != (Object) null)
        ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, (BaseEventData) buttonData, ExecuteEvents.initializePotentialDrag);
    }
    if (!data.ReleasedThisFrame())
      return;
    ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, (BaseEventData) buttonData, ExecuteEvents.pointerUpHandler);
    GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
    if ((Object) buttonData.pointerPress == (Object) eventHandler && buttonData.eligibleForClick)
      ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, (BaseEventData) buttonData, ExecuteEvents.pointerClickHandler);
    else if ((Object) buttonData.pointerDrag != (Object) null && buttonData.dragging)
      ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject1, (BaseEventData) buttonData, ExecuteEvents.dropHandler);
    buttonData.eligibleForClick = false;
    buttonData.pointerPress = (GameObject) null;
    buttonData.rawPointerPress = (GameObject) null;
    if ((Object) buttonData.pointerDrag != (Object) null && buttonData.dragging)
      ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, (BaseEventData) buttonData, ExecuteEvents.endDragHandler);
    buttonData.dragging = false;
    buttonData.pointerDrag = (GameObject) null;
    if (!((Object) gameObject1 != (Object) buttonData.pointerEnter))
      return;
    this.HandlePointerExitAndEnter(buttonData, (GameObject) null);
    this.HandlePointerExitAndEnter(buttonData, gameObject1);
  }

  protected void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
  {
    GameObject gameObject1 = pointerEvent.pointerCurrentRaycast.gameObject;
    if (pressed)
    {
      pointerEvent.eligibleForClick = true;
      pointerEvent.delta = Vector2.zero;
      pointerEvent.dragging = false;
      pointerEvent.useDragThreshold = true;
      pointerEvent.pressPosition = pointerEvent.position;
      pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
      this.DeselectIfSelectionChanged(gameObject1, (BaseEventData) pointerEvent);
      if ((Object) pointerEvent.pointerEnter != (Object) gameObject1)
      {
        this.HandlePointerExitAndEnter(pointerEvent, gameObject1);
        pointerEvent.pointerEnter = gameObject1;
      }
      GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject1, (BaseEventData) pointerEvent, ExecuteEvents.pointerDownHandler);
      if ((Object) gameObject2 == (Object) null)
        gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
      float unscaledTime = Time.unscaledTime;
      if ((Object) gameObject2 == (Object) pointerEvent.lastPress)
      {
        if ((double) (unscaledTime - pointerEvent.clickTime) < 0.30000001192092896)
          ++pointerEvent.clickCount;
        else
          pointerEvent.clickCount = 1;
        pointerEvent.clickTime = unscaledTime;
      }
      else
        pointerEvent.clickCount = 1;
      pointerEvent.pointerPress = gameObject2;
      pointerEvent.rawPointerPress = gameObject1;
      pointerEvent.clickTime = unscaledTime;
      pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject1);
      if ((Object) pointerEvent.pointerDrag != (Object) null)
        ExecuteEvents.Execute<IInitializePotentialDragHandler>(pointerEvent.pointerDrag, (BaseEventData) pointerEvent, ExecuteEvents.initializePotentialDrag);
    }
    if (!released)
      return;
    ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, (BaseEventData) pointerEvent, ExecuteEvents.pointerUpHandler);
    GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
    if ((Object) pointerEvent.pointerPress == (Object) eventHandler && pointerEvent.eligibleForClick)
      ExecuteEvents.Execute<IPointerClickHandler>(pointerEvent.pointerPress, (BaseEventData) pointerEvent, ExecuteEvents.pointerClickHandler);
    else if ((Object) pointerEvent.pointerDrag != (Object) null && pointerEvent.dragging)
      ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject1, (BaseEventData) pointerEvent, ExecuteEvents.dropHandler);
    pointerEvent.eligibleForClick = false;
    pointerEvent.pointerPress = (GameObject) null;
    pointerEvent.rawPointerPress = (GameObject) null;
    if ((Object) pointerEvent.pointerDrag != (Object) null && pointerEvent.dragging)
      ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, (BaseEventData) pointerEvent, ExecuteEvents.endDragHandler);
    pointerEvent.dragging = false;
    pointerEvent.pointerDrag = (GameObject) null;
    if ((Object) pointerEvent.pointerDrag != (Object) null)
      ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, (BaseEventData) pointerEvent, ExecuteEvents.endDragHandler);
    pointerEvent.pointerDrag = (GameObject) null;
    ExecuteEvents.ExecuteHierarchy<IPointerExitHandler>(pointerEvent.pointerEnter, (BaseEventData) pointerEvent, ExecuteEvents.pointerExitHandler);
    pointerEvent.pointerEnter = (GameObject) null;
  }

  public enum Button
  {
    Action1 = 19, // 0x00000013
    Action2 = 20, // 0x00000014
    Action3 = 21, // 0x00000015
    Action4 = 22, // 0x00000016
  }
}
