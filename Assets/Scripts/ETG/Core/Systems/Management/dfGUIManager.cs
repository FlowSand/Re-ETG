// Decompiled with JetBrains decompiler
// Type: dfGUIManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Management
{
    [RequireComponent(typeof (BoxCollider))]
    [ExecuteInEditMode]
    [AddComponentMenu("Daikon Forge/User Interface/GUI Manager")]
    [RequireComponent(typeof (dfInputManager))]
    [Serializable]
    public class dfGUIManager : MonoBehaviour, IDFControlHost, IComparable<dfGUIManager>
    {
      [SerializeField]
      public CameraClearFlags overrideClearFlags = CameraClearFlags.Depth;
      [SerializeField]
      protected float uiScale = 1f;
      [SerializeField]
      public Vector2 InputOffsetScreenPercent;
      [SerializeField]
      protected bool uiScaleLegacy = true;
      [SerializeField]
      protected dfInputManager inputManager;
      [SerializeField]
      protected int fixedWidth = -1;
      [SerializeField]
      protected int fixedHeight = 600;
      [SerializeField]
      public bool FixedAspect;
      [SerializeField]
      protected dfAtlas atlas;
      [SerializeField]
      protected dfFontBase defaultFont;
      [SerializeField]
      protected bool mergeMaterials;
      [SerializeField]
      protected bool pixelPerfectMode = true;
      [SerializeField]
      protected Camera renderCamera;
      [SerializeField]
      protected bool generateNormals;
      [SerializeField]
      protected bool consumeMouseEvents;
      [SerializeField]
      protected bool overrideCamera;
      [SerializeField]
      protected int renderQueueBase = 3000;
      [SerializeField]
      public int renderQueueSecondDraw = -1;
      [SerializeField]
      public List<dfDesignGuide> guides = new List<dfDesignGuide>();
      private static List<dfGUIManager> activeInstances = new List<dfGUIManager>();
      private static dfControl activeControl = (dfControl) null;
      private static Stack<dfGUIManager.ModalControlReference> modalControlStack = new Stack<dfGUIManager.ModalControlReference>();
      private dfGUICamera guiCamera;
      private Mesh[] renderMesh;
      private MeshFilter renderFilter;
      private MeshRenderer meshRenderer;
      private int activeRenderMesh;
      private int cachedChildCount;
      private bool isDirty;
      private bool abortRendering;
      private Vector2 cachedScreenSize;
      private Vector3[] corners = new Vector3[4];
      private dfList<Rect> occluders = new dfList<Rect>(256 /*0x0100*/);
      private Stack<dfTriangleClippingRegion> clipStack = new Stack<dfTriangleClippingRegion>();
      private static dfRenderData masterBuffer = new dfRenderData(4096 /*0x1000*/);
      private dfList<dfRenderData> drawCallBuffers = new dfList<dfRenderData>();
      private dfList<dfRenderGroup> renderGroups = new dfList<dfRenderGroup>();
      private List<int> submeshes = new List<int>();
      private int drawCallCount;
      private Vector2 uiOffset = Vector2.zero;
      private static Plane[] clippingPlanes;
      private dfList<int> drawCallIndices = new dfList<int>();
      private dfList<dfControl> controlsRendered = new dfList<dfControl>();
      private bool shutdownInProcess;
      private int suspendCount;
      private bool? applyHalfPixelOffset;
      [NonSerialized]
      public bool ResolutionIsChanging;

      public static event dfGUIManager.RenderCallback BeforeRender;

      public static event dfGUIManager.RenderCallback AfterRender;

      public MeshRenderer MeshRenderer => this.meshRenderer;

      public static IEnumerable<dfGUIManager> ActiveManagers
      {
        get => (IEnumerable<dfGUIManager>) dfGUIManager.activeInstances;
      }

      public int TotalDrawCalls { get; private set; }

      public int TotalTriangles { get; private set; }

      public int NumControlsRendered { get; private set; }

      public int FramesRendered { get; private set; }

      public IList<dfControl> ControlsRendered => (IList<dfControl>) this.controlsRendered;

      public IList<int> DrawCallStartIndices => (IList<int>) this.drawCallIndices;

      public int RenderQueueBase
      {
        get => this.renderQueueBase;
        set
        {
          if (value == this.renderQueueBase)
            return;
          this.renderQueueBase = value;
          dfGUIManager.RefreshAll();
        }
      }

      public static dfControl ActiveControl => dfGUIManager.activeControl;

      public float UIScale
      {
        get => this.uiScale;
        set
        {
          if (Mathf.Approximately(value, this.uiScale))
            return;
          this.uiScale = value;
          this.onResolutionChanged();
        }
      }

      public bool UIScaleLegacyMode
      {
        get => this.uiScaleLegacy;
        set
        {
          if (value == this.uiScaleLegacy)
            return;
          this.uiScaleLegacy = value;
          this.onResolutionChanged();
        }
      }

      public Vector2 UIOffset
      {
        get => this.uiOffset;
        set
        {
          if (object.Equals((object) this.uiOffset, (object) value))
            return;
          this.uiOffset = value;
          this.Invalidate();
        }
      }

      public Camera RenderCamera
      {
        get => this.renderCamera;
        set
        {
          if (object.ReferenceEquals((object) this.renderCamera, (object) value))
            return;
          this.renderCamera = value;
          this.Invalidate();
          if ((UnityEngine.Object) value != (UnityEngine.Object) null && (UnityEngine.Object) value.gameObject.GetComponent<dfGUICamera>() == (UnityEngine.Object) null)
            value.gameObject.AddComponent<dfGUICamera>();
          if (!((UnityEngine.Object) this.inputManager != (UnityEngine.Object) null))
            return;
          this.inputManager.RenderCamera = value;
        }
      }

      public bool MergeMaterials
      {
        get => this.mergeMaterials;
        set
        {
          if (value == this.mergeMaterials)
            return;
          this.mergeMaterials = value;
          this.invalidateAllControls();
        }
      }

      public bool GenerateNormals
      {
        get => this.generateNormals;
        set
        {
          if (value == this.generateNormals)
            return;
          this.generateNormals = value;
          if (this.renderMesh != null)
          {
            this.renderMesh[0].Clear();
            this.renderMesh[1].Clear();
          }
          dfRenderData.FlushObjectPool();
          this.invalidateAllControls();
        }
      }

      public bool PixelPerfectMode
      {
        get => this.pixelPerfectMode;
        set
        {
          if (value == this.pixelPerfectMode)
            return;
          this.pixelPerfectMode = value;
          this.onResolutionChanged();
          this.Invalidate();
        }
      }

      public dfAtlas DefaultAtlas
      {
        get => this.atlas;
        set
        {
          if (dfAtlas.Equals(value, this.atlas))
            return;
          this.atlas = value;
          this.invalidateAllControls();
        }
      }

      public dfFontBase DefaultFont
      {
        get => this.defaultFont;
        set
        {
          if (!((UnityEngine.Object) value != (UnityEngine.Object) this.defaultFont))
            return;
          this.defaultFont = value;
          this.invalidateAllControls();
        }
      }

      public int FixedWidth
      {
        get => this.fixedWidth;
        set
        {
          if (value == this.fixedWidth)
            return;
          this.fixedWidth = value;
          this.onResolutionChanged();
        }
      }

      public int FixedHeight
      {
        get => this.fixedHeight;
        set
        {
          if (value == this.fixedHeight)
            return;
          int fixedHeight = this.fixedHeight;
          this.fixedHeight = value;
          this.onResolutionChanged(fixedHeight, value);
        }
      }

      public bool ConsumeMouseEvents
      {
        get => this.consumeMouseEvents;
        set => this.consumeMouseEvents = value;
      }

      public bool OverrideCamera
      {
        get => this.overrideCamera;
        set => this.overrideCamera = value;
      }

      public void OnApplicationQuit() => this.shutdownInProcess = true;

      public void Awake() => dfRenderData.FlushObjectPool();

      public void OnEnable()
      {
        foreach (Camera allCamera in Camera.allCameras)
          allCamera.eventMask &= ~(1 << this.gameObject.layer);
        if ((UnityEngine.Object) this.meshRenderer == (UnityEngine.Object) null)
          this.initialize();
        this.useGUILayout = !this.ConsumeMouseEvents;
        dfGUIManager.activeInstances.Add(this);
        this.FramesRendered = 0;
        this.TotalDrawCalls = 0;
        this.TotalTriangles = 0;
        if ((UnityEngine.Object) this.meshRenderer != (UnityEngine.Object) null)
          this.meshRenderer.enabled = true;
        this.inputManager = this.GetComponent<dfInputManager>() ?? this.gameObject.AddComponent<dfInputManager>();
        this.inputManager.RenderCamera = this.RenderCamera;
        this.FramesRendered = 0;
        if ((UnityEngine.Object) this.meshRenderer != (UnityEngine.Object) null)
          this.meshRenderer.enabled = true;
        if (Application.isPlaying)
          this.onResolutionChanged();
        this.Invalidate();
      }

      public void OnDisable()
      {
        dfGUIManager.activeInstances.Remove(this);
        if ((UnityEngine.Object) this.meshRenderer != (UnityEngine.Object) null)
          this.meshRenderer.enabled = false;
        this.resetDrawCalls();
      }

      public void OnDestroy()
      {
        if (dfGUIManager.activeInstances.Count == 0)
          dfMaterialCache.Clear();
        if (this.renderMesh == null || (UnityEngine.Object) this.renderFilter == (UnityEngine.Object) null)
          return;
        this.renderFilter.sharedMesh = (Mesh) null;
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.renderMesh[0]);
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.renderMesh[1]);
        this.renderMesh = (Mesh[]) null;
      }

      public void Start()
      {
        foreach (Camera allCamera in Camera.allCameras)
          allCamera.eventMask &= ~(1 << this.gameObject.layer);
      }

      public void Update()
      {
        dfGUIManager.activeInstances.Sort();
        if ((UnityEngine.Object) this.renderCamera == (UnityEngine.Object) null || !this.enabled)
        {
          if (!((UnityEngine.Object) this.meshRenderer != (UnityEngine.Object) null))
            return;
          this.meshRenderer.enabled = false;
        }
        else
        {
          if (this.renderMesh == null || this.renderMesh.Length == 0)
          {
            this.initialize();
            if (Application.isEditor && !Application.isPlaying)
              this.Render();
          }
          if (this.cachedChildCount != this.transform.childCount)
          {
            this.cachedChildCount = this.transform.childCount;
            this.Invalidate();
          }
          Vector2 screenSize = this.GetScreenSize();
          if ((double) (screenSize - this.cachedScreenSize).sqrMagnitude <= 1.4012984643248171E-45)
            return;
          this.onResolutionChanged(this.cachedScreenSize, screenSize);
          this.cachedScreenSize = screenSize;
        }
      }

      public void LateUpdate()
      {
        if (this.renderMesh == null || this.renderMesh.Length == 0)
          this.initialize();
        if (!Application.isPlaying)
        {
          BoxCollider component = this.GetComponent<Collider>() as BoxCollider;
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          {
            Vector2 vector2 = this.GetScreenSize() * this.PixelsToUnits();
            component.center = Vector3.zero;
            component.size = (Vector3) vector2;
          }
        }
        if (!((UnityEngine.Object) dfGUIManager.activeInstances[0] == (UnityEngine.Object) this))
          return;
        dfFontManager.RebuildDynamicFonts();
        bool flag = false;
        for (int index = 0; index < dfGUIManager.activeInstances.Count; ++index)
        {
          dfGUIManager activeInstance = dfGUIManager.activeInstances[index];
          if (activeInstance.isDirty && activeInstance.suspendCount <= 0)
          {
            flag = true;
            activeInstance.abortRendering = false;
            activeInstance.isDirty = false;
            activeInstance.Render();
          }
        }
        if (!flag)
          return;
        dfMaterialCache.Reset();
        this.updateDrawCalls();
        for (int index = 0; index < dfGUIManager.activeInstances.Count; ++index)
          dfGUIManager.activeInstances[index].updateDrawCalls();
      }

      public void SuspendRendering() => ++this.suspendCount;

      public void ResumeRendering()
      {
        if (this.suspendCount == 0 || --this.suspendCount != 0)
          return;
        this.Invalidate();
      }

      public static dfControl HitTestAll(Vector2 screenPosition)
      {
        dfControl dfControl1 = (dfControl) null;
        float num = float.MinValue;
        for (int index = 0; index < dfGUIManager.activeInstances.Count; ++index)
        {
          if (dfGUIManager.activeInstances[index].GetComponent<dfInputManager>().enabled)
          {
            dfGUIManager activeInstance = dfGUIManager.activeInstances[index];
            Camera renderCamera = activeInstance.RenderCamera;
            if ((double) renderCamera.depth >= (double) num)
            {
              dfControl dfControl2 = activeInstance.HitTest(screenPosition);
              if ((UnityEngine.Object) dfControl2 != (UnityEngine.Object) null)
              {
                dfControl1 = dfControl2;
                num = renderCamera.depth;
              }
            }
          }
        }
        return dfControl1;
      }

      public dfControl HitTest(Vector2 screenPosition)
      {
        Vector2 position = screenPosition;
        Ray ray = this.renderCamera.ScreenPointToRay((Vector3) position);
        float maxDistance = this.renderCamera.farClipPlane - this.renderCamera.nearClipPlane;
        dfControl modalControl = dfGUIManager.GetModalControl();
        dfList<dfControl> controlsRendered = this.controlsRendered;
        int count = controlsRendered.Count;
        dfControl[] items = controlsRendered.Items;
        if (this.occluders.Count != count)
        {
          Debug.LogWarning((object) "Occluder count does not match control count");
          return (dfControl) null;
        }
        position.y = (float) this.RenderCamera.pixelHeight / this.RenderCamera.rect.height - position.y;
        for (int index = count - 1; index >= 0; --index)
        {
          dfControl control = items[index];
          RaycastHit hitInfo;
          if (!((UnityEngine.Object) control == (UnityEngine.Object) null) && !((UnityEngine.Object) control.GetComponent<Collider>() == (UnityEngine.Object) null) && control.GetComponent<Collider>().Raycast(ray, out hitInfo, maxDistance) && (!((UnityEngine.Object) modalControl != (UnityEngine.Object) null) || control.transform.IsChildOf(modalControl.transform)) && control.IsInteractive && control.IsEnabled && dfGUIManager.isInsideClippingRegion(hitInfo.point, control))
            return control;
        }
        return (dfControl) null;
      }

      public Vector2 WorldPointToGUI(Vector3 worldPoint)
      {
        return this.ScreenToGui((Vector2) (Camera.main ?? this.renderCamera).WorldToScreenPoint(worldPoint));
      }

      public float PixelsToUnits() => 2f / (float) this.FixedHeight * this.UIScale;

      public Plane[] GetClippingPlanes()
      {
        Vector3[] corners = this.GetCorners();
        Vector3 inNormal1 = this.transform.TransformDirection(Vector3.right);
        Vector3 inNormal2 = this.transform.TransformDirection(Vector3.left);
        Vector3 inNormal3 = this.transform.TransformDirection(Vector3.up);
        Vector3 inNormal4 = this.transform.TransformDirection(Vector3.down);
        if (dfGUIManager.clippingPlanes == null)
          dfGUIManager.clippingPlanes = new Plane[4];
        dfGUIManager.clippingPlanes[0] = new Plane(inNormal1, corners[0]);
        dfGUIManager.clippingPlanes[1] = new Plane(inNormal2, corners[1]);
        dfGUIManager.clippingPlanes[2] = new Plane(inNormal3, corners[2]);
        dfGUIManager.clippingPlanes[3] = new Plane(inNormal4, corners[0]);
        return dfGUIManager.clippingPlanes;
      }

      public Vector3[] GetCorners()
      {
        float units = this.PixelsToUnits();
        Vector2 vector2 = this.GetScreenSize() * units;
        float x = vector2.x;
        float y = vector2.y;
        Vector3 point1 = new Vector3((float) (-(double) x * 0.5), y * 0.5f);
        Vector3 point2 = point1 + new Vector3(x, 0.0f);
        Vector3 point3 = point1 + new Vector3(0.0f, -y);
        Vector3 point4 = point2 + new Vector3(0.0f, -y);
        Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
        this.corners[0] = localToWorldMatrix.MultiplyPoint(point1);
        this.corners[1] = localToWorldMatrix.MultiplyPoint(point2);
        this.corners[2] = localToWorldMatrix.MultiplyPoint(point4);
        this.corners[3] = localToWorldMatrix.MultiplyPoint(point3);
        return this.corners;
      }

      public Vector2 GetScreenSize()
      {
        Camera renderCamera = this.RenderCamera;
        bool flag = Application.isPlaying && (UnityEngine.Object) renderCamera != (UnityEngine.Object) null;
        Vector2 screenSize = Vector2.zero;
        if (flag)
        {
          float num = !this.PixelPerfectMode ? (float) renderCamera.pixelHeight / (float) this.fixedHeight : 1f;
          if ((UnityEngine.Object) this.guiCamera == (UnityEngine.Object) null)
            this.guiCamera = renderCamera.GetComponent<dfGUICamera>();
          Vector2 vector2 = (new Vector2((float) renderCamera.pixelWidth, (float) renderCamera.pixelHeight) / num).CeilToInt();
          if (this.guiCamera.MaintainCameraAspect)
            ;
          screenSize = !this.uiScaleLegacy ? vector2 / this.uiScale : vector2 * this.uiScale;
        }
        else
        {
          screenSize = new Vector2((float) this.FixedWidth, (float) this.FixedHeight);
          if (!this.uiScaleLegacy)
            screenSize /= this.uiScale;
        }
        return screenSize;
      }

      public T AddControl<T>() where T : dfControl => (T) this.AddControl(typeof (T));

      public dfControl AddControl(System.Type controlType)
      {
        if (!typeof (dfControl).IsAssignableFrom(controlType))
          throw new InvalidCastException();
        dfControl dfControl = new GameObject(controlType.Name)
        {
          transform = {
            parent = this.transform
          },
          layer = this.gameObject.layer
        }.AddComponent(controlType) as dfControl;
        dfControl.ZOrder = this.getMaxZOrder() + 1;
        return dfControl;
      }

      public void AddControl(dfControl child) => child.transform.parent = this.transform;

      public dfControl AddPrefab(GameObject prefab)
      {
        GameObject gameObject = !((UnityEngine.Object) prefab.GetComponent<dfControl>() == (UnityEngine.Object) null) ? UnityEngine.Object.Instantiate<GameObject>(prefab) : throw new InvalidCastException();
        gameObject.transform.parent = this.transform;
        gameObject.layer = this.gameObject.layer;
        dfControl component = gameObject.GetComponent<dfControl>();
        component.transform.parent = this.transform;
        component.PerformLayout();
        this.BringToFront(component);
        return component;
      }

      public dfRenderData GetDrawCallBuffer(int drawCallNumber) => this.drawCallBuffers[drawCallNumber];

      public static dfControl GetModalControl()
      {
        return dfGUIManager.modalControlStack.Count > 0 ? dfGUIManager.modalControlStack.Peek().control : (dfControl) null;
      }

      public Vector2 ScreenToGui(Vector2 position)
      {
        Vector2 screenSize = this.GetScreenSize();
        Camera camera = GameManager.Instance.MainCameraController.Camera ?? this.renderCamera;
        position.x = (float) (camera.pixelWidth / Screen.width) * position.x;
        position.y = (float) (camera.pixelHeight / Screen.height) * position.y;
        position.y = screenSize.y - position.y;
        return position;
      }

      public static void PushModal(dfControl control)
      {
        dfGUIManager.PushModal(control, (dfGUIManager.ModalPoppedCallback) null);
      }

      public static void PushModal(dfControl control, dfGUIManager.ModalPoppedCallback callback)
      {
        if ((UnityEngine.Object) control == (UnityEngine.Object) null)
          throw new NullReferenceException("Cannot call PushModal() with a null reference");
        dfGUIManager.modalControlStack.Push(new dfGUIManager.ModalControlReference()
        {
          control = control,
          callback = callback
        });
      }

      public static void PopModal()
      {
        dfGUIManager.ModalControlReference controlReference = dfGUIManager.modalControlStack.Count != 0 ? dfGUIManager.modalControlStack.Pop() : throw new InvalidOperationException("Modal stack is empty");
        if (controlReference.callback == null)
          return;
        controlReference.callback(controlReference.control);
      }

      public static bool ModalStackContainsControl(dfControl control)
      {
        foreach (dfGUIManager.ModalControlReference controlReference in dfGUIManager.modalControlStack.ToArray())
        {
          if ((UnityEngine.Object) controlReference.control == (UnityEngine.Object) control)
            return true;
        }
        return false;
      }

      public static void PopModalToControl(dfControl control, bool includeControl)
      {
        while (dfGUIManager.modalControlStack.Count > 0)
        {
          if ((UnityEngine.Object) dfGUIManager.modalControlStack.Peek().control == (UnityEngine.Object) control)
          {
            if (!includeControl)
              break;
            dfGUIManager.modalControlStack.Pop();
            break;
          }
          dfGUIManager.modalControlStack.Pop();
        }
      }

      public static void SetFocus(dfControl control, bool allowScrolling = true)
      {
        if ((UnityEngine.Object) dfGUIManager.activeControl == (UnityEngine.Object) control || (UnityEngine.Object) control != (UnityEngine.Object) null && !control.CanFocus)
          return;
        dfControl activeControl = dfGUIManager.activeControl;
        dfGUIManager.activeControl = control;
        dfFocusEventArgs args = new dfFocusEventArgs(control, activeControl, allowScrolling);
        dfList<dfControl> prevFocusChain = dfList<dfControl>.Obtain();
        if ((UnityEngine.Object) activeControl != (UnityEngine.Object) null)
        {
          for (dfControl dfControl = activeControl; (UnityEngine.Object) dfControl != (UnityEngine.Object) null; dfControl = dfControl.Parent)
            prevFocusChain.Add(dfControl);
        }
        dfList<dfControl> newFocusChain = dfList<dfControl>.Obtain();
        if ((UnityEngine.Object) control != (UnityEngine.Object) null)
        {
          for (dfControl dfControl = control; (UnityEngine.Object) dfControl != (UnityEngine.Object) null; dfControl = dfControl.Parent)
            newFocusChain.Add(dfControl);
        }
        if ((UnityEngine.Object) activeControl != (UnityEngine.Object) null)
        {
          prevFocusChain.ForEach((Action<dfControl>) (c =>
          {
            if (newFocusChain.Contains(c))
              return;
            c.OnLeaveFocus(args);
          }));
          activeControl.OnLostFocus(args);
        }
        if ((UnityEngine.Object) control != (UnityEngine.Object) null)
        {
          newFocusChain.ForEach((Action<dfControl>) (c =>
          {
            if (prevFocusChain.Contains(c))
              return;
            c.OnEnterFocus(args);
          }));
          control.OnGotFocus(args);
        }
        newFocusChain.Release();
        prevFocusChain.Release();
      }

      public static bool HasFocus(dfControl control)
      {
        return !((UnityEngine.Object) control == (UnityEngine.Object) null) && (UnityEngine.Object) dfGUIManager.activeControl == (UnityEngine.Object) control;
      }

      public static bool ContainsFocus(dfControl control)
      {
        if ((UnityEngine.Object) dfGUIManager.activeControl == (UnityEngine.Object) control)
          return true;
        return (UnityEngine.Object) dfGUIManager.activeControl == (UnityEngine.Object) null || (UnityEngine.Object) control == (UnityEngine.Object) null ? object.ReferenceEquals((object) dfGUIManager.activeControl, (object) control) : dfGUIManager.activeControl.transform.IsChildOf(control.transform);
      }

      public void BringToFront(dfControl control)
      {
        if ((UnityEngine.Object) control.Parent != (UnityEngine.Object) null)
          control = control.GetRootContainer();
        using (dfList<dfControl> topLevelControls = this.getTopLevelControls())
        {
          int num = 0;
          for (int index = 0; index < topLevelControls.Count; ++index)
          {
            dfControl dfControl = topLevelControls[index];
            if ((UnityEngine.Object) dfControl != (UnityEngine.Object) control)
              dfControl.ZOrder = num++;
          }
          control.ZOrder = num;
          this.Invalidate();
        }
      }

      public void SendToBack(dfControl control)
      {
        if ((UnityEngine.Object) control.Parent != (UnityEngine.Object) null)
          control = control.GetRootContainer();
        using (dfList<dfControl> topLevelControls = this.getTopLevelControls())
        {
          int num = 1;
          for (int index = 0; index < topLevelControls.Count; ++index)
          {
            dfControl dfControl = topLevelControls[index];
            if ((UnityEngine.Object) dfControl != (UnityEngine.Object) control)
              dfControl.ZOrder = num++;
          }
          control.ZOrder = 0;
          this.Invalidate();
        }
      }

      public void Invalidate()
      {
        if (this.isDirty)
          return;
        this.isDirty = true;
        this.updateRenderSettings();
      }

      public static void InvalidateAll()
      {
        for (int index = 0; index < dfGUIManager.activeInstances.Count; ++index)
          dfGUIManager.activeInstances[index].Invalidate();
      }

      public static void RefreshAll() => dfGUIManager.RefreshAll(false);

      public static void RefreshAll(bool force)
      {
        List<dfGUIManager> activeInstances = dfGUIManager.activeInstances;
        for (int index = 0; index < activeInstances.Count; ++index)
        {
          dfGUIManager dfGuiManager = activeInstances[index];
          if (dfGuiManager.renderMesh != null && dfGuiManager.renderMesh.Length != 0)
          {
            dfGuiManager.invalidateAllControls();
            if (force || !Application.isPlaying)
              dfGuiManager.Render();
          }
        }
      }

      internal void AbortRender() => this.abortRendering = true;

      public void Render()
      {
        if ((UnityEngine.Object) this.meshRenderer == (UnityEngine.Object) null)
          return;
        this.meshRenderer.enabled = false;
        ++this.FramesRendered;
        if (dfGUIManager.BeforeRender != null)
          dfGUIManager.BeforeRender(this);
        try
        {
          this.occluders.Clear();
          this.occluders.EnsureCapacity(this.NumControlsRendered);
          this.NumControlsRendered = 0;
          this.controlsRendered.Clear();
          this.drawCallIndices.Clear();
          this.renderGroups.Clear();
          this.TotalDrawCalls = 0;
          this.TotalTriangles = 0;
          if ((UnityEngine.Object) this.RenderCamera == (UnityEngine.Object) null || !this.enabled)
          {
            if (!((UnityEngine.Object) this.meshRenderer != (UnityEngine.Object) null))
              return;
            this.meshRenderer.enabled = false;
          }
          else
          {
            if ((UnityEngine.Object) this.meshRenderer != (UnityEngine.Object) null && !this.meshRenderer.enabled)
              this.meshRenderer.enabled = true;
            if (this.renderMesh == null || this.renderMesh.Length == 0)
            {
              Debug.LogError((object) "GUI Manager not initialized before Render() called");
            }
            else
            {
              this.resetDrawCalls();
              dfRenderData buffer = (dfRenderData) null;
              this.clipStack.Clear();
              this.clipStack.Push(dfTriangleClippingRegion.Obtain());
              uint startValue = dfChecksumUtil.START_VALUE;
              using (dfList<dfControl> topLevelControls = this.getTopLevelControls())
              {
                this.updateRenderOrder(topLevelControls);
                for (int index = 0; index < topLevelControls.Count; ++index)
                {
                  if (!this.abortRendering)
                  {
                    dfControl control = topLevelControls[index];
                    this.renderControl(ref buffer, control, startValue, 1f);
                  }
                  else
                    break;
                }
              }
              if (this.abortRendering)
              {
                this.clipStack.Clear();
                throw new dfAbortRenderingException();
              }
              this.drawCallBuffers.RemoveAll(new Predicate<dfRenderData>(this.isEmptyBuffer));
              this.drawCallCount = this.drawCallBuffers.Count;
              this.TotalDrawCalls = this.drawCallCount;
              if (this.drawCallBuffers.Count == 0)
              {
                if ((UnityEngine.Object) this.renderFilter.sharedMesh != (UnityEngine.Object) null)
                  this.renderFilter.sharedMesh.Clear();
                if (this.clipStack.Count <= 0)
                  return;
                this.clipStack.Pop().Release();
                this.clipStack.Clear();
              }
              else
              {
                dfRenderData dfRenderData = this.compileMasterBuffer();
                this.TotalTriangles = dfRenderData.Triangles.Count / 3;
                Mesh renderMesh = this.getRenderMesh();
                this.renderFilter.sharedMesh = renderMesh;
                Mesh mesh = renderMesh;
                mesh.Clear(true);
                mesh.vertices = dfRenderData.Vertices.Items;
                mesh.uv = dfRenderData.UV.Items;
                mesh.colors32 = dfRenderData.Colors.Items;
                if (this.generateNormals && dfRenderData.Normals.Items.Length == dfRenderData.Vertices.Items.Length)
                {
                  mesh.normals = dfRenderData.Normals.Items;
                  mesh.tangents = dfRenderData.Tangents.Items;
                }
                mesh.subMeshCount = this.submeshes.Count;
                for (int index = 0; index < this.submeshes.Count; ++index)
                {
                  int submesh = this.submeshes[index];
                  int length = dfRenderData.Triangles.Count - submesh;
                  if (index < this.submeshes.Count - 1)
                    length = this.submeshes[index + 1] - submesh;
                  int[] numArray = dfTempArray<int>.Obtain(length);
                  dfRenderData.Triangles.CopyTo(submesh, numArray, 0, length);
                  mesh.SetTriangles(numArray, index);
                }
                if (this.clipStack.Count != 1)
                  Debug.LogError((object) "Clip stack not properly maintained");
                this.clipStack.Pop().Release();
                this.clipStack.Clear();
                this.updateRenderSettings();
              }
            }
          }
        }
        catch (dfAbortRenderingException ex)
        {
          this.isDirty = true;
          this.abortRendering = false;
        }
        finally
        {
          this.meshRenderer.enabled = true;
          if (dfGUIManager.AfterRender != null)
            dfGUIManager.AfterRender(this);
        }
      }

      private void updateDrawCalls()
      {
        if ((UnityEngine.Object) this.meshRenderer == (UnityEngine.Object) null)
          this.initialize();
        Material[] materialArray = this.gatherMaterials();
        this.meshRenderer.sharedMaterials = materialArray;
        int renderQueueBase = this.renderQueueBase + materialArray.Length;
        dfRenderGroup[] items = this.renderGroups.Items;
        int count = this.renderGroups.Count;
        for (int index = 0; index < count; ++index)
          items[index].UpdateRenderQueue(ref renderQueueBase);
      }

      private static bool isInsideClippingRegion(Vector3 point, dfControl control)
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

      private int getMaxZOrder()
      {
        int a = -1;
        using (dfList<dfControl> topLevelControls = this.getTopLevelControls())
        {
          for (int index = 0; index < topLevelControls.Count; ++index)
            a = Mathf.Max(a, topLevelControls[index].ZOrder);
        }
        return a;
      }

      private bool isEmptyBuffer(dfRenderData buffer) => buffer.Vertices.Count == 0;

      private dfList<dfControl> getTopLevelControls()
      {
        dfList<dfControl> topLevelControls = dfList<dfControl>.Obtain(this.transform.childCount);
        dfControl[] items = dfControl.ActiveInstances.Items;
        int count = dfControl.ActiveInstances.Count;
        for (int index = 0; index < count; ++index)
        {
          dfControl dfControl = items[index];
          if (dfControl.IsTopLevelControl(this))
            topLevelControls.Add(dfControl);
        }
        topLevelControls.Sort();
        return topLevelControls;
      }

      private void updateRenderSettings()
      {
        Camera renderCamera = this.RenderCamera;
        if ((UnityEngine.Object) renderCamera == (UnityEngine.Object) null)
          return;
        if (!this.overrideCamera)
          this.updateRenderCamera(renderCamera);
        if (this.transform.hasChanged)
        {
          Vector3 localScale = this.transform.localScale;
          if ((double) localScale.x < 1.4012984643248171E-45 || !Mathf.Approximately(localScale.x, localScale.y) || !Mathf.Approximately(localScale.x, localScale.z))
          {
            localScale.y = localScale.z = localScale.x = Mathf.Max(localScale.x, 1f / 1000f);
            this.transform.localScale = localScale;
          }
        }
        if (!this.overrideCamera)
        {
          float num1 = 1f;
          if (Application.isPlaying && this.PixelPerfectMode)
          {
            float num2 = (float) renderCamera.pixelHeight / (float) this.fixedHeight;
            renderCamera.orthographicSize = num2 / num1;
            renderCamera.fieldOfView = 60f * num2;
          }
          else
          {
            renderCamera.orthographicSize = 1f / num1;
            renderCamera.fieldOfView = 60f;
          }
        }
        renderCamera.transparencySortMode = TransparencySortMode.Orthographic;
        if ((double) this.cachedScreenSize.sqrMagnitude <= 1.4012984643248171E-45)
          this.cachedScreenSize = new Vector2((float) this.FixedWidth, (float) this.FixedHeight);
        this.transform.hasChanged = false;
      }

      private void updateRenderCamera(Camera camera)
      {
        if (Application.isPlaying && (UnityEngine.Object) camera.targetTexture != (UnityEngine.Object) null)
        {
          camera.clearFlags = CameraClearFlags.Color;
          camera.backgroundColor = Color.clear;
        }
        else
          camera.clearFlags = this.overrideClearFlags;
        dfGUICamera component = camera.GetComponent<dfGUICamera>();
        Vector3 vector3_1 = Vector3.zero;
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          vector3_1 = component.cameraPositionOffset;
        Vector3 vector3_2 = !Application.isPlaying ? vector3_1 : -(Vector3) this.uiOffset * this.PixelsToUnits() + vector3_1;
        if (camera.orthographic)
        {
          camera.nearClipPlane = Mathf.Min(camera.nearClipPlane, -1f);
          camera.farClipPlane = Mathf.Max(camera.farClipPlane, 1f);
        }
        else
        {
          float num1 = camera.fieldOfView * ((float) Math.PI / 180f);
          Vector3[] corners = this.GetCorners();
          float num2 = !this.uiScaleLegacy ? this.uiScale : 1f;
          float num3 = Vector3.Distance(corners[3], corners[0]) * num2 / (2f * Mathf.Tan(num1 / 2f));
          Vector3 vector3_3 = this.transform.TransformDirection(Vector3.back) * num3;
          camera.farClipPlane = Mathf.Max(num3 * 2f, camera.farClipPlane);
          vector3_2 += vector3_3 / this.uiScale;
        }
        int height = Screen.height;
        float num = (float) (2.0 / (double) height * ((double) height / (double) this.FixedHeight));
        if (Application.isPlaying && !component.ForceNoHalfPixelOffset && this.needHalfPixelOffset())
        {
          Vector3 vector3_4 = new Vector3(num * 0.5f, num * -0.5f, 0.0f);
          if (AmmonomiconController.GuiManagerIsPageRenderer(this))
            vector3_4.x /= 2f;
          vector3_2 += vector3_4;
        }
        if (this.overrideCamera)
          return;
        camera.renderingPath = RenderingPath.Forward;
        if (camera.pixelWidth % 2 != 0)
          vector3_2.x += num * 0.5f;
        if (camera.pixelHeight % 2 != 0)
          vector3_2.y += num * 0.5f;
        if ((double) Vector3.SqrMagnitude(camera.transform.localPosition - vector3_2) > 1.4012984643248171E-45)
          camera.transform.localPosition = vector3_2;
        camera.transform.hasChanged = false;
      }

      private dfRenderData compileMasterBuffer()
      {
        try
        {
          this.submeshes.Clear();
          dfGUIManager.masterBuffer.Clear();
          dfRenderData[] items = this.drawCallBuffers.Items;
          int capacity = 0;
          for (int index = 0; index < this.drawCallCount; ++index)
            capacity += items[index].Vertices.Count;
          dfGUIManager.masterBuffer.EnsureCapacity(capacity);
          for (int index = 0; index < this.drawCallCount; ++index)
          {
            this.submeshes.Add(dfGUIManager.masterBuffer.Triangles.Count);
            dfRenderData buffer = items[index];
            if (this.generateNormals && buffer.Normals.Count == 0)
              this.generateNormalsAndTangents(buffer);
            dfGUIManager.masterBuffer.Merge(buffer, false);
          }
          dfGUIManager.masterBuffer.ApplyTransform(this.transform.worldToLocalMatrix);
          return dfGUIManager.masterBuffer;
        }
        finally
        {
        }
      }

      private void generateNormalsAndTangents(dfRenderData buffer)
      {
        Vector3 normalized1 = buffer.Transform.MultiplyVector(Vector3.back).normalized;
        Vector4 normalized2 = (Vector4) buffer.Transform.MultiplyVector(Vector3.right).normalized with
        {
          w = -1f
        };
        for (int index = 0; index < buffer.Vertices.Count; ++index)
        {
          buffer.Normals.Add(normalized1);
          buffer.Tangents.Add(normalized2);
        }
      }

      private bool needHalfPixelOffset()
      {
        if (this.applyHalfPixelOffset.HasValue)
          return this.applyHalfPixelOffset.Value;
        RuntimePlatform platform = Application.platform;
        bool flag1 = this.pixelPerfectMode && (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor) && SystemInfo.graphicsDeviceVersion.ToLower().StartsWith("direct");
        bool flag2 = SystemInfo.graphicsShaderLevel >= 40;
        this.applyHalfPixelOffset = new bool?((Application.isEditor || flag1) && !flag2);
        return flag1;
      }

      private Material[] gatherMaterials()
      {
        try
        {
          int materialCount = this.getMaterialCount();
          int num = 0;
          int renderQueueBase = this.renderQueueBase;
          Material[] materialArray = dfTempArray<Material>.Obtain(materialCount);
          for (int index = 0; index < this.drawCallBuffers.Count; ++index)
          {
            dfRenderData drawCallBuffer = this.drawCallBuffers[index];
            if (!((UnityEngine.Object) drawCallBuffer.Material == (UnityEngine.Object) null))
            {
              Material material = dfMaterialCache.Lookup(drawCallBuffer.Material);
              material.mainTexture = drawCallBuffer.Material.mainTexture;
              material.shader = drawCallBuffer.Shader ?? material.shader;
              if (this.renderQueueSecondDraw > -1 && material.shader.renderQueue > 6000)
              {
                material.renderQueue = material.shader.renderQueue;
                ++renderQueueBase;
              }
              else
                material.renderQueue = renderQueueBase++;
              material.mainTextureOffset = Vector2.zero;
              material.mainTextureScale = Vector2.zero;
              materialArray[num++] = material;
            }
          }
          return materialArray;
        }
        finally
        {
        }
      }

      private int getMaterialCount()
      {
        int materialCount = 0;
        for (int index = 0; index < this.drawCallCount; ++index)
        {
          if (this.drawCallBuffers[index] != null && (UnityEngine.Object) this.drawCallBuffers[index].Material != (UnityEngine.Object) null)
            ++materialCount;
        }
        return materialCount;
      }

      private void resetDrawCalls()
      {
        this.drawCallCount = 0;
        for (int index = 0; index < this.drawCallBuffers.Count; ++index)
          this.drawCallBuffers[index].Release();
        this.drawCallBuffers.Clear();
      }

      private dfRenderData getDrawCallBuffer(Material material)
      {
        if (this.MergeMaterials && (UnityEngine.Object) material != (UnityEngine.Object) null)
        {
          dfRenderData bufferByMaterial = this.findDrawCallBufferByMaterial(material);
          if (bufferByMaterial != null)
            return bufferByMaterial;
        }
        dfRenderData drawCallBuffer = dfRenderData.Obtain();
        drawCallBuffer.Material = material;
        this.drawCallBuffers.Add(drawCallBuffer);
        ++this.drawCallCount;
        return drawCallBuffer;
      }

      private dfRenderData findDrawCallBufferByMaterial(Material material)
      {
        for (int index = 0; index < this.drawCallCount; ++index)
        {
          if ((UnityEngine.Object) this.drawCallBuffers[index].Material == (UnityEngine.Object) material)
            return this.drawCallBuffers[index];
        }
        return (dfRenderData) null;
      }

      private Mesh getRenderMesh()
      {
        this.activeRenderMesh = this.activeRenderMesh != 1 ? 1 : 0;
        return this.renderMesh[this.activeRenderMesh];
      }

      private void renderControl(
        ref dfRenderData buffer,
        dfControl control,
        uint checksum,
        float opacity)
      {
        if (!control.enabled || !control.gameObject.activeSelf)
          return;
        float opacity1 = opacity * control.Opacity;
        dfRenderGroup renderGroupForControl = dfRenderGroup.GetRenderGroupForControl(control, true);
        if ((UnityEngine.Object) renderGroupForControl != (UnityEngine.Object) null && renderGroupForControl.enabled)
        {
          this.renderGroups.Add(renderGroupForControl);
          renderGroupForControl.Render(this.renderCamera, control, this.occluders, this.controlsRendered, checksum, opacity1);
        }
        else
        {
          if ((double) opacity1 <= 1.0 / 1000.0 || !control.GetIsVisibleRaw())
            return;
          dfTriangleClippingRegion triangleClippingRegion = this.clipStack.Peek();
          checksum = dfChecksumUtil.Calculate(checksum, control.Version);
          Bounds bounds = control.GetBounds();
          bool wasClipped = false;
          if (!(control is IDFMultiRender))
          {
            dfRenderData controlData = control.Render();
            if (controlData != null)
              this.processRenderData(ref buffer, controlData, ref bounds, checksum, triangleClippingRegion, ref wasClipped);
          }
          else
          {
            dfList<dfRenderData> dfList = ((IDFMultiRender) control).RenderMultiple();
            if (dfList != null)
            {
              dfRenderData[] items = dfList.Items;
              int count = dfList.Count;
              for (int index = 0; index < count; ++index)
              {
                dfRenderData controlData = items[index];
                if (controlData != null)
                  this.processRenderData(ref buffer, controlData, ref bounds, checksum, triangleClippingRegion, ref wasClipped);
              }
            }
          }
          control.setClippingState(wasClipped);
          ++this.NumControlsRendered;
          this.occluders.Add(this.getControlOccluder(control));
          this.controlsRendered.Add(control);
          this.drawCallIndices.Add(this.drawCallBuffers.Count - 1);
          if (control.ClipChildren)
            this.clipStack.Push(dfTriangleClippingRegion.Obtain(triangleClippingRegion, control));
          dfControl[] items1 = control.Controls.Items;
          int count1 = control.Controls.Count;
          this.controlsRendered.EnsureCapacity(this.controlsRendered.Count + count1);
          this.occluders.EnsureCapacity(this.occluders.Count + count1);
          for (int index = 0; index < count1; ++index)
            this.renderControl(ref buffer, items1[index], checksum, opacity1);
          if (!control.ClipChildren)
            return;
          this.clipStack.Pop().Release();
        }
      }

      private Rect getControlOccluder(dfControl control)
      {
        if (!control.IsInteractive)
          return new Rect();
        Rect screenRect = control.GetScreenRect();
        Vector2 vector2_1 = new Vector2(screenRect.width * control.HotZoneScale.x, screenRect.height * control.HotZoneScale.y);
        Vector2 vector2_2 = new Vector2(vector2_1.x - screenRect.width, vector2_1.y - screenRect.height) * 0.5f;
        return new Rect(screenRect.x - vector2_2.x, screenRect.y - vector2_2.y, vector2_1.x, vector2_1.y);
      }

      private bool processRenderData(
        ref dfRenderData buffer,
        dfRenderData controlData,
        ref Bounds bounds,
        uint checksum,
        dfTriangleClippingRegion clipInfo,
        ref bool wasClipped)
      {
        wasClipped = false;
        if (controlData == null || (UnityEngine.Object) controlData.Material == (UnityEngine.Object) null || !controlData.IsValid())
          return false;
        bool flag = false;
        if (buffer == null)
          flag = true;
        else if (!object.Equals((object) controlData.Material, (object) buffer.Material))
          flag = true;
        else if (!this.textureEqual(controlData.Material.mainTexture, buffer.Material.mainTexture))
          flag = true;
        else if (!this.shaderEqual(buffer.Shader, controlData.Shader))
          flag = true;
        if (flag)
        {
          buffer = this.getDrawCallBuffer(controlData.Material);
          buffer.Material = controlData.Material;
          buffer.Material.mainTexture = controlData.Material.mainTexture;
          buffer.Material.shader = controlData.Shader ?? controlData.Material.shader;
        }
        if (clipInfo.PerformClipping(buffer, ref bounds, checksum, controlData))
          return true;
        wasClipped = true;
        return false;
      }

      private bool textureEqual(Texture lhs, Texture rhs) => object.Equals((object) lhs, (object) rhs);

      private bool shaderEqual(Shader lhs, Shader rhs)
      {
        return (UnityEngine.Object) lhs == (UnityEngine.Object) null || (UnityEngine.Object) rhs == (UnityEngine.Object) null ? object.ReferenceEquals((object) lhs, (object) rhs) : lhs.name.Equals(rhs.name);
      }

      private void initialize()
      {
        if (Application.isPlaying && (UnityEngine.Object) this.renderCamera == (UnityEngine.Object) null)
        {
          Debug.LogError((object) "No camera is assigned to the GUIManager");
        }
        else
        {
          this.meshRenderer = this.GetComponent<MeshRenderer>();
          if ((UnityEngine.Object) this.meshRenderer == (UnityEngine.Object) null)
            this.meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
          this.renderFilter = this.GetComponent<MeshFilter>();
          if ((UnityEngine.Object) this.renderFilter == (UnityEngine.Object) null)
            this.renderFilter = this.gameObject.AddComponent<MeshFilter>();
          Mesh[] meshArray = new Mesh[2];
          Mesh mesh1 = new Mesh();
          mesh1.hideFlags = HideFlags.DontSave;
          meshArray[0] = mesh1;
          Mesh mesh2 = new Mesh();
          mesh2.hideFlags = HideFlags.DontSave;
          meshArray[1] = mesh2;
          this.renderMesh = meshArray;
          this.renderMesh[0].MarkDynamic();
          this.renderMesh[1].MarkDynamic();
          if (this.fixedWidth >= 0)
            return;
          this.fixedWidth = Mathf.RoundToInt((float) this.fixedHeight * 1.33333f);
          foreach (dfControl componentsInChild in this.GetComponentsInChildren<dfControl>())
            componentsInChild.ResetLayout();
        }
      }

      private void onResolutionChanged()
      {
        this.onResolutionChanged(this.FixedHeight, !Application.isPlaying ? this.FixedHeight : this.renderCamera.pixelHeight);
      }

      private float RenderAspect => this.FixedAspect ? 1.77777779f : this.RenderCamera.aspect;

      private void onResolutionChanged(int oldSize, int currentSize)
      {
        float renderAspect = this.RenderAspect;
        float x1 = (float) oldSize * renderAspect;
        float x2 = (float) currentSize * renderAspect;
        this.onResolutionChanged(new Vector2(x1, (float) oldSize), new Vector2(x2, (float) currentSize));
      }

      public static void ForceResolutionUpdates()
      {
        for (int index = 0; index < dfGUIManager.activeInstances.Count; ++index)
          dfGUIManager.activeInstances[index].onResolutionChanged();
      }

      public void ResolutionChanged() => this.onResolutionChanged();

      private void onResolutionChanged(Vector2 oldSize, Vector2 currentSize)
      {
        if (this.shutdownInProcess)
          return;
        this.cachedScreenSize = currentSize;
        this.applyHalfPixelOffset = new bool?();
        float renderAspect = this.RenderAspect;
        float x1 = oldSize.y * renderAspect;
        float x2 = currentSize.y * renderAspect;
        Vector2 previousResolution = new Vector2(x1, oldSize.y);
        Vector2 currentResolution = new Vector2(x2, currentSize.y);
        dfControl[] componentsInChildren = this.GetComponentsInChildren<dfControl>();
        Array.Sort<dfControl>(componentsInChildren, new Comparison<dfControl>(this.renderSortFunc));
        this.ResolutionIsChanging = true;
        for (int index = componentsInChildren.Length - 1; index >= 0; --index)
        {
          if (this.pixelPerfectMode && (UnityEngine.Object) componentsInChildren[index].Parent == (UnityEngine.Object) null)
            componentsInChildren[index].MakePixelPerfect();
          componentsInChildren[index].OnResolutionChanged(previousResolution, currentResolution);
        }
        for (int index = 0; index < componentsInChildren.Length; ++index)
          componentsInChildren[index].PerformLayout();
        for (int index = 0; index < componentsInChildren.Length && this.pixelPerfectMode; ++index)
        {
          if ((UnityEngine.Object) componentsInChildren[index].Parent == (UnityEngine.Object) null)
            componentsInChildren[index].MakePixelPerfect();
        }
        this.ResolutionIsChanging = false;
        this.isDirty = true;
        this.updateRenderSettings();
      }

      private void invalidateAllControls()
      {
        foreach (dfControl componentsInChild in this.GetComponentsInChildren<dfControl>())
          componentsInChild.Invalidate();
        this.updateRenderOrder();
      }

      private int renderSortFunc(dfControl lhs, dfControl rhs)
      {
        return lhs.RenderOrder.CompareTo(rhs.RenderOrder);
      }

      private void updateRenderOrder() => this.updateRenderOrder((dfList<dfControl>) null);

      private void updateRenderOrder(dfList<dfControl> list)
      {
        dfList<dfControl> dfList = list;
        bool flag = false;
        if (list == null)
        {
          dfList = this.getTopLevelControls();
          flag = true;
        }
        else
          dfList.Sort();
        int order = 0;
        int count = dfList.Count;
        dfControl[] items = dfList.Items;
        for (int index = 0; index < count; ++index)
        {
          dfControl dfControl = items[index];
          if ((UnityEngine.Object) dfControl.Parent == (UnityEngine.Object) null)
            dfControl.setRenderOrder(ref order);
        }
        if (!flag)
          return;
        dfList.Release();
      }

      public int CompareTo(dfGUIManager other)
      {
        int num = this.renderQueueBase.CompareTo(other.renderQueueBase);
        return num == 0 && (UnityEngine.Object) this.RenderCamera != (UnityEngine.Object) null && (UnityEngine.Object) other.RenderCamera != (UnityEngine.Object) null ? this.RenderCamera.depth.CompareTo(other.RenderCamera.depth) : num;
      }

      [dfEventCategory("Modal Dialog")]
      public delegate void ModalPoppedCallback(dfControl control);

      [dfEventCategory("Global Callbacks")]
      public delegate void RenderCallback(dfGUIManager manager);

      private struct ModalControlReference
      {
        public dfControl control;
        public dfGUIManager.ModalPoppedCallback callback;
      }
    }

}
