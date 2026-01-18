// Decompiled with JetBrains decompiler
// Type: tk2dUIManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/UI/Core/tk2dUIManager")]
public class tk2dUIManager : MonoBehaviour
  {
    public static double version = 1.0;
    public static int releaseId = 0;
    private static tk2dUIManager instance;
    [SerializeField]
    private Camera uiCamera;
    private static List<tk2dUICamera> allCameras = new List<tk2dUICamera>();
    private List<tk2dUICamera> sortedCameras = new List<tk2dUICamera>();
    public LayerMask raycastLayerMask = (LayerMask) -1;
    private bool inputEnabled = true;
    public bool areHoverEventsTracked = true;
    private tk2dUIItem pressedUIItem;
    private tk2dUIItem overUIItem;
    private tk2dUITouch firstPressedUIItemTouch;
    private bool checkForHovers = true;
    [SerializeField]
    private bool useMultiTouch;
    private const int MAX_MULTI_TOUCH_COUNT = 5;
    private tk2dUITouch[] allTouches = new tk2dUITouch[5];
    private List<tk2dUIItem> prevPressedUIItemList = new List<tk2dUIItem>();
    private tk2dUIItem[] pressedUIItems = new tk2dUIItem[5];
    private int touchCounter;
    private Vector2 mouseDownFirstPos = Vector2.zero;
    private const string MOUSE_WHEEL_AXES_NAME = "Mouse ScrollWheel";
    private tk2dUITouch primaryTouch = new tk2dUITouch();
    private tk2dUITouch secondaryTouch = new tk2dUITouch();
    private tk2dUITouch resultTouch = new tk2dUITouch();
    private tk2dUIItem hitUIItem;
    private RaycastHit hit;
    private Ray ray;
    private tk2dUITouch currTouch;
    private tk2dUIItem currPressedItem;
    private tk2dUIItem prevPressedItem;

    public static tk2dUIManager Instance
    {
      get
      {
        if ((UnityEngine.Object) tk2dUIManager.instance == (UnityEngine.Object) null)
        {
          tk2dUIManager.instance = UnityEngine.Object.FindObjectOfType(typeof (tk2dUIManager)) as tk2dUIManager;
          if ((UnityEngine.Object) tk2dUIManager.instance == (UnityEngine.Object) null)
            tk2dUIManager.instance = new GameObject(nameof (tk2dUIManager)).AddComponent<tk2dUIManager>();
        }
        return tk2dUIManager.instance;
      }
    }

    public static tk2dUIManager Instance__NoCreate => tk2dUIManager.instance;

    public Camera UICamera
    {
      get => this.uiCamera;
      set => this.uiCamera = value;
    }

    public Camera GetUICameraForControl(GameObject go)
    {
      int num = 1 << go.layer;
      int count = tk2dUIManager.allCameras.Count;
      for (int index = 0; index < count; ++index)
      {
        tk2dUICamera allCamera = tk2dUIManager.allCameras[index];
        if (((int) allCamera.FilteredMask & num) != 0)
          return allCamera.HostCamera;
      }
      Debug.LogError((object) ("Unable to find UI camera for " + go.name));
      return (Camera) null;
    }

    public static void RegisterCamera(tk2dUICamera cam) => tk2dUIManager.allCameras.Add(cam);

    public static void UnregisterCamera(tk2dUICamera cam) => tk2dUIManager.allCameras.Remove(cam);

    public bool InputEnabled
    {
      get => this.inputEnabled;
      set
      {
        if (this.inputEnabled && !value)
        {
          this.SortCameras();
          this.inputEnabled = value;
          if (this.useMultiTouch)
            this.CheckMultiTouchInputs();
          else
            this.CheckInputs();
        }
        else
          this.inputEnabled = value;
      }
    }

    public tk2dUIItem PressedUIItem
    {
      get
      {
        if (!this.useMultiTouch)
          return this.pressedUIItem;
        return this.pressedUIItems.Length > 0 ? this.pressedUIItems[this.pressedUIItems.Length - 1] : (tk2dUIItem) null;
      }
    }

    public tk2dUIItem[] PressedUIItems => this.pressedUIItems;

    public bool UseMultiTouch
    {
      get => this.useMultiTouch;
      set
      {
        if (this.useMultiTouch != value && this.inputEnabled)
        {
          this.InputEnabled = false;
          this.useMultiTouch = value;
          this.InputEnabled = true;
        }
        else
          this.useMultiTouch = value;
      }
    }

    public event System.Action OnAnyPress;

    public event System.Action OnInputUpdate;

    public event Action<float> OnScrollWheelChange;

    private void SortCameras()
    {
      this.sortedCameras.Clear();
      int count = tk2dUIManager.allCameras.Count;
      for (int index = 0; index < count; ++index)
      {
        tk2dUICamera allCamera = tk2dUIManager.allCameras[index];
        if ((UnityEngine.Object) allCamera != (UnityEngine.Object) null)
          this.sortedCameras.Add(allCamera);
      }
      this.sortedCameras.Sort((Comparison<tk2dUICamera>) ((a, b) => b.GetComponent<Camera>().depth.CompareTo(a.GetComponent<Camera>().depth)));
    }

    private void Awake()
    {
      if ((UnityEngine.Object) tk2dUIManager.instance == (UnityEngine.Object) null)
      {
        tk2dUIManager.instance = this;
        if (tk2dUIManager.instance.transform.childCount != 0)
          Debug.LogError((object) "You should not attach anything to the tk2dUIManager object. The tk2dUIManager will not get destroyed between scene switches and any children will persist as well.");
        if (Application.isPlaying)
          UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
      }
      else if ((UnityEngine.Object) tk2dUIManager.instance != (UnityEngine.Object) this)
      {
        Debug.Log((object) "Discarding unnecessary tk2dUIManager instance.");
        if ((UnityEngine.Object) this.uiCamera != (UnityEngine.Object) null)
        {
          this.HookUpLegacyCamera(this.uiCamera);
          this.uiCamera = (Camera) null;
        }
        UnityEngine.Object.Destroy((UnityEngine.Object) this);
        return;
      }
      tk2dUITime.Init();
      this.Setup();
    }

    private void HookUpLegacyCamera(Camera cam)
    {
      if (!((UnityEngine.Object) cam.GetComponent<tk2dUICamera>() == (UnityEngine.Object) null))
        return;
      cam.gameObject.AddComponent<tk2dUICamera>().AssignRaycastLayerMask(this.raycastLayerMask);
    }

    private void Start()
    {
      if ((UnityEngine.Object) this.uiCamera != (UnityEngine.Object) null)
      {
        Debug.Log((object) "It is no longer necessary to hook up a camera to the tk2dUIManager. You can simply attach a tk2dUICamera script to the cameras that interact with UI.");
        this.HookUpLegacyCamera(this.uiCamera);
        this.uiCamera = (Camera) null;
      }
      if (tk2dUIManager.allCameras.Count != 0)
        return;
      Debug.LogError((object) "Unable to find any tk2dUICameras, and no cameras are connected to the tk2dUIManager. You will not be able to interact with the UI.");
    }

    private void Setup()
    {
      if (this.areHoverEventsTracked)
        return;
      this.checkForHovers = false;
    }

    private void Update()
    {
      tk2dUITime.Update();
      if (!this.inputEnabled)
        return;
      this.SortCameras();
      if (this.useMultiTouch)
        this.CheckMultiTouchInputs();
      else
        this.CheckInputs();
      if (this.OnInputUpdate != null)
        this.OnInputUpdate();
      if (this.OnScrollWheelChange == null)
        return;
      float axis = Input.GetAxis("Mouse ScrollWheel");
      if ((double) axis == 0.0)
        return;
      this.OnScrollWheelChange(axis);
    }

    private void CheckInputs()
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      this.primaryTouch = new tk2dUITouch();
      this.secondaryTouch = new tk2dUITouch();
      this.resultTouch = new tk2dUITouch();
      this.hitUIItem = (tk2dUIItem) null;
      if (this.inputEnabled)
      {
        if (Input.touchCount > 0)
        {
          foreach (Touch touch in Input.touches)
          {
            if (touch.phase == TouchPhase.Began)
            {
              this.primaryTouch = new tk2dUITouch(touch);
              flag1 = true;
              flag3 = true;
            }
            else if ((UnityEngine.Object) this.pressedUIItem != (UnityEngine.Object) null && touch.fingerId == this.firstPressedUIItemTouch.fingerId)
            {
              this.secondaryTouch = new tk2dUITouch(touch);
              flag2 = true;
            }
          }
          this.checkForHovers = false;
        }
        else if (Input.GetMouseButtonDown(0))
        {
          this.primaryTouch = new tk2dUITouch(TouchPhase.Began, 9999, (Vector2) Input.mousePosition, Vector2.zero, 0.0f);
          flag1 = true;
          flag3 = true;
        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
          Vector2 _deltaPosition = Vector2.zero;
          TouchPhase _phase = TouchPhase.Moved;
          if ((UnityEngine.Object) this.pressedUIItem != (UnityEngine.Object) null)
            _deltaPosition = this.firstPressedUIItemTouch.position - new Vector2(Input.mousePosition.x, Input.mousePosition.y);
          if (Input.GetMouseButtonUp(0))
            _phase = TouchPhase.Ended;
          else if (_deltaPosition == Vector2.zero)
            _phase = TouchPhase.Stationary;
          this.secondaryTouch = new tk2dUITouch(_phase, 9999, (Vector2) Input.mousePosition, _deltaPosition, tk2dUITime.deltaTime);
          flag2 = true;
        }
      }
      if (flag1)
        this.resultTouch = this.primaryTouch;
      else if (flag2)
        this.resultTouch = this.secondaryTouch;
      if (flag1 || flag2)
      {
        this.hitUIItem = this.RaycastForUIItem(this.resultTouch.position);
        if (this.resultTouch.phase == TouchPhase.Began)
        {
          if ((UnityEngine.Object) this.pressedUIItem != (UnityEngine.Object) null)
          {
            this.pressedUIItem.CurrentOverUIItem(this.hitUIItem);
            if ((UnityEngine.Object) this.pressedUIItem != (UnityEngine.Object) this.hitUIItem)
            {
              this.pressedUIItem.Release();
              this.pressedUIItem = (tk2dUIItem) null;
            }
            else
              this.firstPressedUIItemTouch = this.resultTouch;
          }
          if ((UnityEngine.Object) this.hitUIItem != (UnityEngine.Object) null)
            this.hitUIItem.Press(this.resultTouch);
          this.pressedUIItem = this.hitUIItem;
          this.firstPressedUIItemTouch = this.resultTouch;
        }
        else if (this.resultTouch.phase == TouchPhase.Ended)
        {
          if ((UnityEngine.Object) this.pressedUIItem != (UnityEngine.Object) null)
          {
            this.pressedUIItem.CurrentOverUIItem(this.hitUIItem);
            this.pressedUIItem.UpdateTouch(this.resultTouch);
            this.pressedUIItem.Release();
            this.pressedUIItem = (tk2dUIItem) null;
          }
        }
        else if ((UnityEngine.Object) this.pressedUIItem != (UnityEngine.Object) null)
        {
          this.pressedUIItem.CurrentOverUIItem(this.hitUIItem);
          this.pressedUIItem.UpdateTouch(this.resultTouch);
        }
      }
      else if ((UnityEngine.Object) this.pressedUIItem != (UnityEngine.Object) null)
      {
        this.pressedUIItem.CurrentOverUIItem((tk2dUIItem) null);
        this.pressedUIItem.Release();
        this.pressedUIItem = (tk2dUIItem) null;
      }
      if (this.checkForHovers)
      {
        if (this.inputEnabled)
        {
          if (!flag1 && !flag2 && (UnityEngine.Object) this.hitUIItem == (UnityEngine.Object) null && !Input.GetMouseButton(0))
            this.hitUIItem = this.RaycastForUIItem((Vector2) Input.mousePosition);
          else if (Input.GetMouseButton(0))
            this.hitUIItem = (tk2dUIItem) null;
        }
        if ((UnityEngine.Object) this.hitUIItem != (UnityEngine.Object) null)
        {
          if (this.hitUIItem.isHoverEnabled)
          {
            if (!this.hitUIItem.HoverOver(this.overUIItem) && (UnityEngine.Object) this.overUIItem != (UnityEngine.Object) null)
              this.overUIItem.HoverOut(this.hitUIItem);
            this.overUIItem = this.hitUIItem;
          }
          else if ((UnityEngine.Object) this.overUIItem != (UnityEngine.Object) null)
            this.overUIItem.HoverOut((tk2dUIItem) null);
        }
        else if ((UnityEngine.Object) this.overUIItem != (UnityEngine.Object) null)
          this.overUIItem.HoverOut((tk2dUIItem) null);
      }
      if (!flag3 || this.OnAnyPress == null)
        return;
      this.OnAnyPress();
    }

    private void CheckMultiTouchInputs()
    {
      bool flag1 = false;
      this.touchCounter = 0;
      if (this.inputEnabled)
      {
        if (Input.touchCount > 0)
        {
          foreach (Touch touch in Input.touches)
          {
            if (this.touchCounter < 5)
            {
              this.allTouches[this.touchCounter] = new tk2dUITouch(touch);
              ++this.touchCounter;
            }
            else
              break;
          }
        }
        else if (Input.GetMouseButtonDown(0))
        {
          this.allTouches[this.touchCounter] = new tk2dUITouch(TouchPhase.Began, 9999, (Vector2) Input.mousePosition, Vector2.zero, 0.0f);
          this.mouseDownFirstPos = (Vector2) Input.mousePosition;
          ++this.touchCounter;
        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
          Vector2 _deltaPosition = this.mouseDownFirstPos - new Vector2(Input.mousePosition.x, Input.mousePosition.y);
          TouchPhase _phase = TouchPhase.Moved;
          if (Input.GetMouseButtonUp(0))
            _phase = TouchPhase.Ended;
          else if (_deltaPosition == Vector2.zero)
            _phase = TouchPhase.Stationary;
          this.allTouches[this.touchCounter] = new tk2dUITouch(_phase, 9999, (Vector2) Input.mousePosition, _deltaPosition, tk2dUITime.deltaTime);
          ++this.touchCounter;
        }
      }
      for (int index = 0; index < this.touchCounter; ++index)
        this.pressedUIItems[index] = this.RaycastForUIItem(this.allTouches[index].position);
      for (int index1 = 0; index1 < this.prevPressedUIItemList.Count; ++index1)
      {
        this.prevPressedItem = this.prevPressedUIItemList[index1];
        if ((UnityEngine.Object) this.prevPressedItem != (UnityEngine.Object) null)
        {
          int fingerId = this.prevPressedItem.Touch.fingerId;
          bool flag2 = false;
          for (int index2 = 0; index2 < this.touchCounter; ++index2)
          {
            this.currTouch = this.allTouches[index2];
            if (this.currTouch.fingerId == fingerId)
            {
              flag2 = true;
              this.currPressedItem = this.pressedUIItems[index2];
              if (this.currTouch.phase == TouchPhase.Began)
              {
                this.prevPressedItem.CurrentOverUIItem(this.currPressedItem);
                if ((UnityEngine.Object) this.prevPressedItem != (UnityEngine.Object) this.currPressedItem)
                {
                  this.prevPressedItem.Release();
                  this.prevPressedUIItemList.RemoveAt(index1);
                  --index1;
                  break;
                }
                break;
              }
              if (this.currTouch.phase == TouchPhase.Ended)
              {
                this.prevPressedItem.CurrentOverUIItem(this.currPressedItem);
                this.prevPressedItem.UpdateTouch(this.currTouch);
                this.prevPressedItem.Release();
                this.prevPressedUIItemList.RemoveAt(index1);
                --index1;
                break;
              }
              this.prevPressedItem.CurrentOverUIItem(this.currPressedItem);
              this.prevPressedItem.UpdateTouch(this.currTouch);
              break;
            }
          }
          if (!flag2)
          {
            this.prevPressedItem.CurrentOverUIItem((tk2dUIItem) null);
            this.prevPressedItem.Release();
            this.prevPressedUIItemList.RemoveAt(index1);
            --index1;
          }
        }
      }
      for (int index = 0; index < this.touchCounter; ++index)
      {
        this.currPressedItem = this.pressedUIItems[index];
        this.currTouch = this.allTouches[index];
        if (this.currTouch.phase == TouchPhase.Began)
        {
          if ((UnityEngine.Object) this.currPressedItem != (UnityEngine.Object) null && this.currPressedItem.Press(this.currTouch))
            this.prevPressedUIItemList.Add(this.currPressedItem);
          flag1 = true;
        }
      }
      if (!flag1 || this.OnAnyPress == null)
        return;
      this.OnAnyPress();
    }

    private tk2dUIItem RaycastForUIItem(Vector2 screenPos)
    {
      int count = this.sortedCameras.Count;
      for (int index = 0; index < count; ++index)
      {
        tk2dUICamera sortedCamera = this.sortedCameras[index];
        this.ray = sortedCamera.HostCamera.ScreenPointToRay((Vector3) screenPos);
        if (Physics.Raycast(this.ray, out this.hit, sortedCamera.HostCamera.farClipPlane, (int) sortedCamera.FilteredMask))
          return this.hit.collider.GetComponent<tk2dUIItem>();
      }
      return (tk2dUIItem) null;
    }

    public void OverrideClearAllChildrenPresses(tk2dUIItem item)
    {
      if (this.useMultiTouch)
      {
        for (int index = 0; index < this.pressedUIItems.Length; ++index)
        {
          tk2dUIItem pressedUiItem = this.pressedUIItems[index];
          if ((UnityEngine.Object) pressedUiItem != (UnityEngine.Object) null && item.CheckIsUIItemChildOfMe(pressedUiItem))
            pressedUiItem.CurrentOverUIItem(item);
        }
      }
      else
      {
        if (!((UnityEngine.Object) this.pressedUIItem != (UnityEngine.Object) null) || !item.CheckIsUIItemChildOfMe(this.pressedUIItem))
          return;
        this.pressedUIItem.CurrentOverUIItem(item);
      }
    }
  }

