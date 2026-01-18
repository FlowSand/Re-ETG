// Decompiled with JetBrains decompiler
// Type: tk2dSlicedSprite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/Sprite/tk2dSlicedSprite")]
[RequireComponent(typeof (MeshRenderer))]
[RequireComponent(typeof (MeshFilter))]
public class tk2dSlicedSprite : tk2dBaseSprite
  {
    private Mesh mesh;
    private Vector2[] meshUvs;
    private Vector3[] meshVertices;
    private Color32[] meshColors;
    private Vector3[] meshNormals;
    private Vector4[] meshTangents;
    private int[] meshIndices;
    [SerializeField]
    private Vector2 _dimensions = new Vector2(50f, 50f);
    [SerializeField]
    private tk2dBaseSprite.Anchor _anchor;
    [SerializeField]
    private bool _borderOnly;
    [SerializeField]
    private bool legacyMode;
    [SerializeField]
    private Vector2 _anchorOffset;
    [SerializeField]
    private bool _tileStretchedSprites;
    public float borderTop = 0.2f;
    public float borderBottom = 0.2f;
    public float borderLeft = 0.2f;
    public float borderRight = 0.2f;
    public float borderCornerBottom;
    [SerializeField]
    protected bool _createBoxCollider;
    private Vector3 boundsCenter = Vector3.zero;
    private Vector3 boundsExtents = Vector3.zero;

    public bool BorderOnly
    {
      get => this._borderOnly;
      set
      {
        if (value == this._borderOnly)
          return;
        this._borderOnly = value;
        if (this._tileStretchedSprites)
          this.UpdateGeometryImpl();
        else
          this.UpdateIndices();
      }
    }

    public Vector2 dimensions
    {
      get => this._dimensions;
      set
      {
        if (!(value != this._dimensions))
          return;
        this._dimensions = value;
        this.UpdateVertices();
        this.UpdateCollider();
      }
    }

    public tk2dBaseSprite.Anchor anchor
    {
      get => this._anchor;
      set
      {
        if (value == this._anchor)
          return;
        this._anchor = value;
        this.UpdateVertices();
        this.UpdateCollider();
      }
    }

    public Vector2 anchorOffset
    {
      get => this._anchorOffset;
      set
      {
        if (!(value != this._anchorOffset))
          return;
        this._anchorOffset = value;
        this.UpdateVertices();
        this.UpdateCollider();
      }
    }

    public bool TileStretchedSprites
    {
      get => this._tileStretchedSprites;
      set
      {
        if (value == this._tileStretchedSprites)
          return;
        this._tileStretchedSprites = value;
        this.meshVertices = (Vector3[]) null;
        this.UpdateGeometryImpl();
      }
    }

    public void SetBorder(float left, float bottom, float right, float top)
    {
      if ((double) this.borderLeft == (double) left && (double) this.borderBottom == (double) bottom && (double) this.borderRight == (double) right && (double) this.borderTop == (double) top)
        return;
      this.borderLeft = left;
      this.borderBottom = bottom;
      this.borderRight = right;
      this.borderTop = top;
      this.UpdateVertices();
    }

    public bool CreateBoxCollider
    {
      get => this._createBoxCollider;
      set
      {
        if (this._createBoxCollider == value)
          return;
        this._createBoxCollider = value;
        this.UpdateCollider();
      }
    }

    private new void Awake()
    {
      base.Awake();
      this.mesh = new Mesh();
      this.mesh.hideFlags = HideFlags.DontSave;
      this.GetComponent<MeshFilter>().mesh = this.mesh;
      if ((Object) this.boxCollider == (Object) null)
        this.boxCollider = this.GetComponent<BoxCollider>();
      if ((Object) this.boxCollider2D == (Object) null)
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
      if (!(bool) (Object) this.Collection)
        return;
      if (this._spriteId < 0 || this._spriteId >= this.Collection.Count)
        this._spriteId = 0;
      this.Build();
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      if (!(bool) (Object) this.mesh)
        return;
      Object.Destroy((Object) this.mesh);
    }

    protected new void SetColors(Color32[] dest)
    {
      tk2dSpriteGeomGen.SetSpriteColors(dest, 0, this.meshVertices.Length, this._color, this.collectionInst.premultipliedAlpha);
    }

    protected void SetGeometry(Vector3[] vertices, Vector2[] uvs)
    {
      tk2dSpriteDefinition currentSprite = this.CurrentSprite;
      float colliderOffsetZ = !((Object) this.boxCollider != (Object) null) ? 0.0f : this.boxCollider.center.z;
      float colliderExtentZ = !((Object) this.boxCollider != (Object) null) ? 0.5f : this.boxCollider.size.z * 0.5f;
      tk2dSpriteGeomGen.SetSlicedSpriteGeom(this.meshVertices, this.meshUvs, 0, out this.boundsCenter, out this.boundsExtents, currentSprite, this._borderOnly, this._scale, this.dimensions, new Vector2(this.borderLeft, this.borderBottom), new Vector2(this.borderRight, this.borderTop), this.borderCornerBottom, this.anchor, colliderOffsetZ, colliderExtentZ, this._anchorOffset, this._tileStretchedSprites);
      if (this.ShouldDoTilt)
      {
        for (int index = 0; index < this.meshVertices.Length; ++index)
        {
          float y = (this.m_transform.rotation * Vector3.Scale(this.meshVertices[index], this.m_transform.lossyScale)).y;
          if (this.IsPerpendicular)
            this.meshVertices[index].z -= y;
          else
            this.meshVertices[index].z += y;
        }
      }
      if (this.meshNormals.Length > 0 || this.meshTangents.Length > 0)
        tk2dSpriteGeomGen.SetSpriteVertexNormals(this.meshVertices, this.meshVertices[0], this.meshVertices[this.meshVertices.Length - 1], currentSprite.normals, currentSprite.tangents, this.meshNormals, this.meshTangents);
      if (!currentSprite.complexGeometry)
        return;
      for (int index = 0; index < vertices.Length; ++index)
        vertices[index] = Vector3.zero;
    }

    private void SetIndices()
    {
      int numIndices;
      tk2dSpriteGeomGen.GetSlicedSpriteGeomDesc(out int _, out numIndices, this.CurrentSprite, this._borderOnly, this._tileStretchedSprites, this.dimensions, new Vector2(this.borderLeft, this.borderBottom), new Vector2(this.borderRight, this.borderTop), this.borderCornerBottom);
      if (this.meshIndices == null || this.meshIndices.Length != numIndices)
        this.meshIndices = new int[numIndices];
      tk2dSpriteGeomGen.SetSlicedSpriteIndices(this.meshIndices, 0, 0, this.CurrentSprite, this._borderOnly, this._tileStretchedSprites, this.dimensions, new Vector2(this.borderLeft, this.borderBottom), new Vector2(this.borderRight, this.borderTop), this.borderCornerBottom);
    }

    private bool NearEnough(float value, float compValue, float scale)
    {
      return (double) Mathf.Abs(Mathf.Abs(value - compValue) / scale) < 0.0099999997764825821;
    }

    private void PermanentUpgradeLegacyMode()
    {
      tk2dSpriteDefinition currentSprite = this.CurrentSprite;
      float x1 = currentSprite.untrimmedBoundsDataCenter.x;
      float y1 = currentSprite.untrimmedBoundsDataCenter.y;
      float x2 = currentSprite.untrimmedBoundsDataExtents.x;
      float y2 = currentSprite.untrimmedBoundsDataExtents.y;
      if (this.NearEnough(x1, 0.0f, x2) && this.NearEnough(y1, (float) (-(double) y2 / 2.0), y2))
        this._anchor = tk2dBaseSprite.Anchor.UpperCenter;
      else if (this.NearEnough(x1, 0.0f, x2) && this.NearEnough(y1, 0.0f, y2))
        this._anchor = tk2dBaseSprite.Anchor.MiddleCenter;
      else if (this.NearEnough(x1, 0.0f, x2) && this.NearEnough(y1, y2 / 2f, y2))
        this._anchor = tk2dBaseSprite.Anchor.LowerCenter;
      else if (this.NearEnough(x1, (float) (-(double) x2 / 2.0), x2) && this.NearEnough(y1, (float) (-(double) y2 / 2.0), y2))
        this._anchor = tk2dBaseSprite.Anchor.UpperRight;
      else if (this.NearEnough(x1, (float) (-(double) x2 / 2.0), x2) && this.NearEnough(y1, 0.0f, y2))
        this._anchor = tk2dBaseSprite.Anchor.MiddleRight;
      else if (this.NearEnough(x1, (float) (-(double) x2 / 2.0), x2) && this.NearEnough(y1, y2 / 2f, y2))
        this._anchor = tk2dBaseSprite.Anchor.LowerRight;
      else if (this.NearEnough(x1, x2 / 2f, x2) && this.NearEnough(y1, (float) (-(double) y2 / 2.0), y2))
        this._anchor = tk2dBaseSprite.Anchor.UpperLeft;
      else if (this.NearEnough(x1, x2 / 2f, x2) && this.NearEnough(y1, 0.0f, y2))
        this._anchor = tk2dBaseSprite.Anchor.MiddleLeft;
      else if (this.NearEnough(x1, x2 / 2f, x2) && this.NearEnough(y1, y2 / 2f, y2))
      {
        this._anchor = tk2dBaseSprite.Anchor.LowerLeft;
      }
      else
      {
        Debug.LogError((object) $"tk2dSlicedSprite ({this.name}) error - Unable to determine anchor upgrading from legacy mode. Please fix this manually.");
        this._anchor = tk2dBaseSprite.Anchor.MiddleCenter;
      }
      float num1 = x2 / currentSprite.texelSize.x;
      float num2 = y2 / currentSprite.texelSize.y;
      this._dimensions.x = this._scale.x * num1;
      this._dimensions.y = this._scale.y * num2;
      this._scale.Set(1f, 1f, 1f);
      this.legacyMode = false;
    }

    public override void Build()
    {
      if (this.legacyMode)
        this.PermanentUpgradeLegacyMode();
      tk2dSpriteDefinition currentSprite = this.CurrentSprite;
      int numVertices;
      tk2dSpriteGeomGen.GetSlicedSpriteGeomDesc(out numVertices, out int _, currentSprite, this._borderOnly, this._tileStretchedSprites, this.dimensions, new Vector2(this.borderLeft, this.borderBottom), new Vector2(this.borderRight, this.borderTop), this.borderCornerBottom);
      int length1 = 0;
      int length2 = 0;
      if (currentSprite.normals != null && currentSprite.normals.Length > 0)
        length1 = numVertices;
      if (currentSprite.tangents != null && currentSprite.tangents.Length > 0)
        length2 = numVertices;
      if (this.meshUvs == null || this.meshUvs.Length != numVertices)
        this.meshUvs = new Vector2[numVertices];
      if (this.meshVertices == null || this.meshVertices.Length != numVertices)
        this.meshVertices = new Vector3[numVertices];
      if (this.meshColors == null || this.meshColors.Length != numVertices)
        this.meshColors = new Color32[numVertices];
      if (this.meshNormals == null || this.meshNormals.Length != length1)
        this.meshNormals = new Vector3[length1];
      if (this.meshTangents == null || this.meshTangents.Length != length2)
        this.meshTangents = new Vector4[length2];
      this.SetIndices();
      this.SetGeometry(this.meshVertices, this.meshUvs);
      this.SetColors(this.meshColors);
      if ((Object) this.mesh == (Object) null)
      {
        this.mesh = new Mesh();
        this.mesh.hideFlags = HideFlags.DontSave;
      }
      else
        this.mesh.Clear();
      this.mesh.vertices = this.meshVertices;
      this.mesh.colors32 = this.meshColors;
      this.mesh.uv = this.meshUvs;
      this.mesh.normals = this.meshNormals;
      this.mesh.tangents = this.meshTangents;
      this.mesh.triangles = this.meshIndices;
      this.mesh.RecalculateBounds();
      this.mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(this.mesh.bounds, this.renderLayer);
      this.GetComponent<MeshFilter>().mesh = this.mesh;
      this.UpdateCollider();
      this.UpdateMaterial();
    }

    protected override void UpdateGeometry() => this.UpdateGeometryImpl();

    protected override void UpdateColors() => this.UpdateColorsImpl();

    protected override void UpdateVertices() => this.UpdateGeometryImpl();

    private void UpdateIndices()
    {
      if (!((Object) this.mesh != (Object) null))
        return;
      this.SetIndices();
      this.mesh.triangles = this.meshIndices;
    }

    protected void UpdateColorsImpl()
    {
      if (this.meshColors == null || this.meshColors.Length == 0)
      {
        this.Build();
      }
      else
      {
        this.SetColors(this.meshColors);
        this.mesh.colors32 = this.meshColors;
      }
    }

    protected void UpdateGeometryImpl()
    {
      if (this.meshVertices == null || this.meshVertices.Length == 0 || this.TileStretchedSprites)
      {
        this.Build();
      }
      else
      {
        this.SetGeometry(this.meshVertices, this.meshUvs);
        this.mesh.vertices = this.meshVertices;
        this.mesh.uv = this.meshUvs;
        this.mesh.normals = this.meshNormals;
        this.mesh.tangents = this.meshTangents;
        this.mesh.RecalculateBounds();
        this.mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(this.mesh.bounds, this.renderLayer);
        this.UpdateCollider();
      }
    }

    protected override void UpdateCollider()
    {
      if (!this.CreateBoxCollider)
        return;
      if (this.CurrentSprite.physicsEngine == tk2dSpriteDefinition.PhysicsEngine.Physics3D)
      {
        if (!((Object) this.boxCollider != (Object) null))
          return;
        this.boxCollider.size = 2f * this.boundsExtents;
        this.boxCollider.center = this.boundsCenter;
      }
      else
      {
        if (this.CurrentSprite.physicsEngine != tk2dSpriteDefinition.PhysicsEngine.Physics2D || !((Object) this.boxCollider2D != (Object) null))
          return;
        this.boxCollider2D.size = (Vector2) (2f * this.boundsExtents);
        this.boxCollider2D.offset = (Vector2) this.boundsCenter;
      }
    }

    protected override void CreateCollider() => this.UpdateCollider();

    protected override void UpdateMaterial()
    {
      if (!((Object) this.renderer.sharedMaterial != (Object) this.collectionInst.spriteDefinitions[this.spriteId].materialInst))
        return;
      this.renderer.material = this.collectionInst.spriteDefinitions[this.spriteId].materialInst;
    }

    protected override int GetCurrentVertexCount()
    {
      return this.TileStretchedSprites && this._spriteId != -1 && this._spriteId < this.collectionInst.spriteDefinitions.Length ? 4 : 16 /*0x10*/;
    }

    public override void ReshapeBounds(Vector3 dMin, Vector3 dMax)
    {
      tk2dSpriteDefinition currentSprite = this.CurrentSprite;
      Vector3 b = new Vector3(this._dimensions.x * currentSprite.texelSize.x * this._scale.x, this._dimensions.y * currentSprite.texelSize.y * this._scale.y);
      Vector3 zero = Vector3.zero;
      switch (this._anchor)
      {
        case tk2dBaseSprite.Anchor.LowerLeft:
          zero.Set(0.0f, 0.0f, 0.0f);
          break;
        case tk2dBaseSprite.Anchor.LowerCenter:
          zero.Set(0.5f, 0.0f, 0.0f);
          break;
        case tk2dBaseSprite.Anchor.LowerRight:
          zero.Set(1f, 0.0f, 0.0f);
          break;
        case tk2dBaseSprite.Anchor.MiddleLeft:
          zero.Set(0.0f, 0.5f, 0.0f);
          break;
        case tk2dBaseSprite.Anchor.MiddleCenter:
          zero.Set(0.5f, 0.5f, 0.0f);
          break;
        case tk2dBaseSprite.Anchor.MiddleRight:
          zero.Set(1f, 0.5f, 0.0f);
          break;
        case tk2dBaseSprite.Anchor.UpperLeft:
          zero.Set(0.0f, 1f, 0.0f);
          break;
        case tk2dBaseSprite.Anchor.UpperCenter:
          zero.Set(0.5f, 1f, 0.0f);
          break;
        case tk2dBaseSprite.Anchor.UpperRight:
          zero.Set(1f, 1f, 0.0f);
          break;
      }
      Vector3 vector3_1 = Vector3.Scale(zero, b) * -1f;
      Vector3 vector3_2 = b + dMax - dMin;
      vector3_2.x /= currentSprite.texelSize.x * this._scale.x;
      vector3_2.y /= currentSprite.texelSize.y * this._scale.y;
      Vector3 vector3_3 = new Vector3(!Mathf.Approximately(this._dimensions.x, 0.0f) ? vector3_1.x * vector3_2.x / this._dimensions.x : 0.0f, !Mathf.Approximately(this._dimensions.y, 0.0f) ? vector3_1.y * vector3_2.y / this._dimensions.y : 0.0f);
      this.transform.position = this.transform.TransformPoint((vector3_1 + dMin - vector3_3) with
      {
        z = 0.0f
      });
      this.dimensions = new Vector2(vector3_2.x, vector3_2.y);
    }
  }

