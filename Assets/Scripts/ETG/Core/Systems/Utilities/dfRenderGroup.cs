using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[AddComponentMenu("Daikon Forge/User Interface/Render Group")]
[Serializable]
internal class dfRenderGroup : MonoBehaviour
  {
    private static List<dfRenderGroup> activeInstances = new List<dfRenderGroup>();
    [SerializeField]
    protected dfClippingMethod clipType;
    private Mesh renderMesh;
    private MeshFilter renderFilter;
    private MeshRenderer meshRenderer;
    private Camera renderCamera;
    private dfControl attachedControl;
    private static dfRenderData masterBuffer = new dfRenderData(4096 /*0x1000*/);
    private dfList<dfRenderData> drawCallBuffers = new dfList<dfRenderData>();
    private List<int> submeshes = new List<int>();
    private Stack<dfTriangleClippingRegion> clipStack = new Stack<dfTriangleClippingRegion>();
    private dfList<Rect> groupOccluders = new dfList<Rect>();
    private dfList<dfControl> groupControls = new dfList<dfControl>();
    private dfList<dfRenderGroup> renderGroups = new dfList<dfRenderGroup>();
    private dfRenderGroup.ClipRegionInfo clipInfo = new dfRenderGroup.ClipRegionInfo();
    private Rect clipRect = new Rect();
    private Rect containerRect = new Rect();
    private int drawCallCount;
    private bool isDirty;

    public dfClippingMethod ClipType
    {
      get => this.clipType;
      set
      {
        if (value == this.clipType)
          return;
        this.clipType = value;
        if (!((UnityEngine.Object) this.attachedControl != (UnityEngine.Object) null))
          return;
        this.attachedControl.Invalidate();
      }
    }

    public void OnEnable()
    {
      dfRenderGroup.activeInstances.Add(this);
      this.isDirty = true;
      if ((UnityEngine.Object) this.meshRenderer == (UnityEngine.Object) null)
        this.initialize();
      this.meshRenderer.enabled = true;
      if ((UnityEngine.Object) this.attachedControl != (UnityEngine.Object) null)
        this.attachedControl.Invalidate();
      else
        dfGUIManager.InvalidateAll();
      this.attachedControl = this.GetComponent<dfControl>();
    }

    public void OnDisable()
    {
      dfRenderGroup.activeInstances.Remove(this);
      if ((UnityEngine.Object) this.meshRenderer != (UnityEngine.Object) null)
        this.meshRenderer.enabled = false;
      if (!((UnityEngine.Object) this.attachedControl != (UnityEngine.Object) null))
        return;
      this.attachedControl.Invalidate();
    }

    public void OnDestroy()
    {
      if ((UnityEngine.Object) this.renderFilter != (UnityEngine.Object) null)
        this.renderFilter.sharedMesh = (Mesh) null;
      this.renderFilter = (MeshFilter) null;
      this.meshRenderer = (MeshRenderer) null;
      if ((UnityEngine.Object) this.renderMesh != (UnityEngine.Object) null)
      {
        this.renderMesh.Clear();
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.renderMesh);
        this.renderMesh = (Mesh) null;
      }
      dfGUIManager.InvalidateAll();
    }

    internal static dfRenderGroup GetRenderGroupForControl(dfControl control)
    {
      return dfRenderGroup.GetRenderGroupForControl(control, false);
    }

    internal static dfRenderGroup GetRenderGroupForControl(
      dfControl control,
      bool directlyAttachedOnly)
    {
      Transform transform = control.transform;
      for (int index = 0; index < dfRenderGroup.activeInstances.Count; ++index)
      {
        dfRenderGroup activeInstance = dfRenderGroup.activeInstances[index];
        if ((UnityEngine.Object) activeInstance.attachedControl == (UnityEngine.Object) control || !directlyAttachedOnly && transform.IsChildOf(activeInstance.transform))
          return activeInstance;
      }
      return (dfRenderGroup) null;
    }

    internal static void InvalidateGroupForControl(dfControl control)
    {
      Transform transform = control.transform;
      for (int index = 0; index < dfRenderGroup.activeInstances.Count; ++index)
      {
        dfRenderGroup activeInstance = dfRenderGroup.activeInstances[index];
        if (transform.IsChildOf(activeInstance.transform))
          activeInstance.isDirty = true;
      }
    }

    internal void Render(
      Camera renderCamera,
      dfControl control,
      dfList<Rect> occluders,
      dfList<dfControl> controlsRendered,
      uint checksum,
      float opacity)
    {
      if ((UnityEngine.Object) this.meshRenderer == (UnityEngine.Object) null)
        this.initialize();
      this.renderCamera = renderCamera;
      this.attachedControl = control;
      if (!this.isDirty)
      {
        occluders.AddRange(this.groupOccluders);
        controlsRendered.AddRange(this.groupControls);
      }
      else
      {
        this.groupOccluders.Clear();
        this.groupControls.Clear();
        this.renderGroups.Clear();
        this.resetDrawCalls();
        this.clipInfo = new dfRenderGroup.ClipRegionInfo();
        this.clipRect = new Rect();
        dfRenderData buffer = (dfRenderData) null;
        using (dfTriangleClippingRegion triangleClippingRegion = dfTriangleClippingRegion.Obtain())
        {
          this.clipStack.Clear();
          this.clipStack.Push(triangleClippingRegion);
          this.renderControl(ref buffer, control, checksum, opacity);
          this.clipStack.Pop();
        }
        this.drawCallBuffers.RemoveAll(new Predicate<dfRenderData>(this.isEmptyBuffer));
        this.drawCallCount = this.drawCallBuffers.Count;
        if (this.drawCallBuffers.Count == 0)
        {
          this.meshRenderer.enabled = false;
        }
        else
        {
          this.meshRenderer.enabled = true;
          dfRenderData dfRenderData = this.compileMasterBuffer();
          Mesh renderMesh = this.renderMesh;
          renderMesh.Clear(true);
          renderMesh.vertices = dfRenderData.Vertices.Items;
          renderMesh.uv = dfRenderData.UV.Items;
          renderMesh.colors32 = dfRenderData.Colors.Items;
          renderMesh.subMeshCount = this.submeshes.Count;
          for (int index = 0; index < this.submeshes.Count; ++index)
          {
            int submesh = this.submeshes[index];
            int length = dfRenderData.Triangles.Count - submesh;
            if (index < this.submeshes.Count - 1)
              length = this.submeshes[index + 1] - submesh;
            int[] numArray = dfTempArray<int>.Obtain(length);
            dfRenderData.Triangles.CopyTo(submesh, numArray, 0, length);
            renderMesh.SetTriangles(numArray, index);
          }
          this.isDirty = false;
          occluders.AddRange(this.groupOccluders);
          controlsRendered.AddRange(this.groupControls);
        }
      }
    }

    internal void UpdateRenderQueue(ref int renderQueueBase)
    {
      int materialCount = this.getMaterialCount();
      int num1 = 0;
      Material[] materialArray = dfTempArray<Material>.Obtain(materialCount);
      for (int index = 0; index < this.drawCallBuffers.Count; ++index)
      {
        if (!((UnityEngine.Object) this.drawCallBuffers[index].Material == (UnityEngine.Object) null))
        {
          Material material = dfMaterialCache.Lookup(this.drawCallBuffers[index].Material);
          material.mainTexture = this.drawCallBuffers[index].Material.mainTexture;
          material.shader = this.drawCallBuffers[index].Shader ?? material.shader;
          material.mainTextureScale = Vector2.zero;
          material.mainTextureOffset = Vector2.zero;
          material.renderQueue = ++renderQueueBase;
          if (Application.isPlaying && this.clipType == dfClippingMethod.Shader && !this.clipInfo.IsEmpty && index > 0)
          {
            Vector3 center = this.attachedControl.Pivot.TransformToCenter(this.attachedControl.Size);
            float num2 = center.x + this.clipInfo.Offset.x;
            float num3 = center.y + this.clipInfo.Offset.y;
            float units = this.attachedControl.PixelsToUnits();
            material.mainTextureScale = new Vector2((float) (1.0 / (-(double) this.clipInfo.Size.x * 0.5 * (double) units)), (float) (1.0 / (-(double) this.clipInfo.Size.y * 0.5 * (double) units)));
            material.mainTextureOffset = new Vector2((float) ((double) num2 / (double) this.clipInfo.Size.x * 2.0), (float) ((double) num3 / (double) this.clipInfo.Size.y * 2.0));
          }
          materialArray[num1++] = material;
        }
      }
      this.meshRenderer.sharedMaterials = materialArray;
      dfRenderGroup[] items = this.renderGroups.Items;
      int count = this.renderGroups.Count;
      for (int index = 0; index < count; ++index)
        items[index].UpdateRenderQueue(ref renderQueueBase);
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
      if ((double) opacity1 <= 1.0 / 1000.0)
        return;
      dfRenderGroup renderGroupForControl = dfRenderGroup.GetRenderGroupForControl(control, true);
      if ((UnityEngine.Object) renderGroupForControl != (UnityEngine.Object) null && (UnityEngine.Object) renderGroupForControl != (UnityEngine.Object) this && renderGroupForControl.enabled)
      {
        this.renderGroups.Add(renderGroupForControl);
        renderGroupForControl.Render(this.renderCamera, control, this.groupOccluders, this.groupControls, checksum, opacity1);
      }
      else
      {
        if (!control.GetIsVisibleRaw())
          return;
        dfTriangleClippingRegion triangleClippingRegion = this.clipStack.Peek();
        checksum = dfChecksumUtil.Calculate(checksum, control.Version);
        Bounds bounds = control.GetBounds();
        Rect screenRect = control.GetScreenRect();
        Rect controlOccluder = this.getControlOccluder(ref screenRect, control);
        bool wasClipped = false;
        if (!(control is IDFMultiRender))
        {
          dfRenderData controlData = control.Render();
          if (controlData != null)
            this.processRenderData(ref buffer, controlData, ref bounds, ref screenRect, checksum, triangleClippingRegion, ref wasClipped);
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
                this.processRenderData(ref buffer, controlData, ref bounds, ref screenRect, checksum, triangleClippingRegion, ref wasClipped);
            }
          }
        }
        control.setClippingState(wasClipped);
        this.groupOccluders.Add(controlOccluder);
        this.groupControls.Add(control);
        if (control.ClipChildren)
        {
          if (!Application.isPlaying || this.clipType == dfClippingMethod.Software)
            this.clipStack.Push(dfTriangleClippingRegion.Obtain(triangleClippingRegion, control));
          else if (this.clipInfo.IsEmpty)
            this.setClipRegion(control, ref screenRect);
        }
        dfControl[] items1 = control.Controls.Items;
        int count1 = control.Controls.Count;
        this.groupControls.EnsureCapacity(this.groupControls.Count + count1);
        this.groupOccluders.EnsureCapacity(this.groupOccluders.Count + count1);
        for (int index = 0; index < count1; ++index)
          this.renderControl(ref buffer, items1[index], checksum, opacity1);
        if (!control.ClipChildren || Application.isPlaying && this.clipType != dfClippingMethod.Software)
          return;
        this.clipStack.Pop().Release();
      }
    }

    private void setClipRegion(dfControl control, ref Rect screenRect)
    {
      Vector2 size = control.Size;
      RectOffset clipPadding = control.GetClipPadding();
      float num1 = Mathf.Min(Mathf.Max(0.0f, Mathf.Min(size.x, (float) clipPadding.horizontal)), size.x);
      float num2 = Mathf.Min(Mathf.Max(0.0f, Mathf.Min(size.y, (float) clipPadding.vertical)), size.y);
      this.clipInfo = new dfRenderGroup.ClipRegionInfo();
      this.clipInfo.Size = Vector2.Max(new Vector2(size.x - num1, size.y - num2), (Vector2) Vector3.zero);
      this.clipInfo.Offset = (Vector2) (new Vector3((float) (clipPadding.left - clipPadding.right), (float) -(clipPadding.top - clipPadding.bottom)) * 0.5f);
      this.clipRect = !this.containerRect.IsEmpty() ? this.containerRect.Intersection(screenRect) : screenRect;
    }

    private bool processRenderData(
      ref dfRenderData buffer,
      dfRenderData controlData,
      ref Bounds bounds,
      ref Rect screenRect,
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
      else if (!this.clipInfo.IsEmpty && this.drawCallBuffers.Count == 1)
        flag = true;
      if (flag)
      {
        buffer = this.getDrawCallBuffer(controlData.Material);
        buffer.Material = controlData.Material;
        buffer.Material.mainTexture = controlData.Material.mainTexture;
        buffer.Material.shader = controlData.Shader ?? controlData.Material.shader;
      }
      if (!Application.isPlaying || this.clipType == dfClippingMethod.Software)
      {
        if (clipInfo.PerformClipping(buffer, ref bounds, checksum, controlData))
          return true;
        wasClipped = true;
      }
      else if (this.clipRect.IsEmpty() || screenRect.Intersects(this.clipRect))
        buffer.Merge(controlData);
      else
        wasClipped = true;
      return false;
    }

    private dfRenderData compileMasterBuffer()
    {
      this.submeshes.Clear();
      dfRenderGroup.masterBuffer.Clear();
      dfRenderData[] items = this.drawCallBuffers.Items;
      int capacity = 0;
      for (int index = 0; index < this.drawCallCount; ++index)
        capacity += items[index].Vertices.Count;
      dfRenderGroup.masterBuffer.EnsureCapacity(capacity);
      for (int index = 0; index < this.drawCallCount; ++index)
      {
        this.submeshes.Add(dfRenderGroup.masterBuffer.Triangles.Count);
        dfRenderData buffer = items[index];
        dfRenderGroup.masterBuffer.Merge(buffer, false);
      }
      dfRenderGroup.masterBuffer.ApplyTransform(this.transform.worldToLocalMatrix);
      return dfRenderGroup.masterBuffer;
    }

    private bool isEmptyBuffer(dfRenderData buffer) => buffer.Vertices.Count == 0;

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
      dfRenderData drawCallBuffer = dfRenderData.Obtain();
      drawCallBuffer.Material = material;
      this.drawCallBuffers.Add(drawCallBuffer);
      ++this.drawCallCount;
      return drawCallBuffer;
    }

    private Rect getControlOccluder(ref Rect screenRect, dfControl control)
    {
      if (!control.IsInteractive)
        return new Rect();
      Vector2 vector2_1 = new Vector2(screenRect.width * control.HotZoneScale.x, screenRect.height * control.HotZoneScale.y);
      Vector2 vector2_2 = new Vector2(vector2_1.x - screenRect.width, vector2_1.y - screenRect.height) * 0.5f;
      return new Rect(screenRect.x - vector2_2.x, screenRect.y - vector2_2.y, vector2_1.x, vector2_1.y);
    }

    private bool textureEqual(Texture lhs, Texture rhs) => object.Equals((object) lhs, (object) rhs);

    private bool shaderEqual(Shader lhs, Shader rhs)
    {
      return (UnityEngine.Object) lhs == (UnityEngine.Object) null || (UnityEngine.Object) rhs == (UnityEngine.Object) null ? object.ReferenceEquals((object) lhs, (object) rhs) : lhs.name.Equals(rhs.name);
    }

    private void initialize()
    {
      this.meshRenderer = this.GetComponent<MeshRenderer>();
      if ((UnityEngine.Object) this.meshRenderer == (UnityEngine.Object) null)
        this.meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
      this.meshRenderer.hideFlags = HideFlags.HideInInspector;
      this.renderFilter = this.GetComponent<MeshFilter>();
      if ((UnityEngine.Object) this.renderFilter == (UnityEngine.Object) null)
        this.renderFilter = this.gameObject.AddComponent<MeshFilter>();
      this.renderFilter.hideFlags = HideFlags.HideInInspector;
      Mesh mesh = new Mesh();
      mesh.hideFlags = HideFlags.DontSave;
      this.renderMesh = mesh;
      this.renderMesh.MarkDynamic();
      this.renderFilter.sharedMesh = this.renderMesh;
    }

    private struct ClipRegionInfo
    {
      public Vector2 Offset;
      public Vector2 Size;

      public bool IsEmpty => this.Offset == Vector2.zero && this.Size == Vector2.zero;
    }
  }

