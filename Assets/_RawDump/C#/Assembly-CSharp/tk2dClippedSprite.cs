// Decompiled with JetBrains decompiler
// Type: tk2dClippedSprite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[RequireComponent(typeof (MeshFilter))]
[AddComponentMenu("2D Toolkit/Sprite/tk2dClippedSprite")]
[RequireComponent(typeof (MeshRenderer))]
public class tk2dClippedSprite : tk2dBaseSprite
{
  private Mesh mesh;
  private Vector2[] meshUvs;
  private Vector3[] meshVertices;
  private Color32[] meshColors;
  private Vector3[] meshNormals;
  private Vector4[] meshTangents;
  private int[] meshIndices;
  public Vector2 _clipBottomLeft = new Vector2(0.0f, 0.0f);
  public Vector2 _clipTopRight = new Vector2(1f, 1f);
  private Rect _clipRect = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
  [SerializeField]
  protected bool _createBoxCollider;
  private Vector3 boundsCenter = Vector3.zero;
  private Vector3 boundsExtents = Vector3.zero;

  public Rect ClipRect
  {
    get
    {
      this._clipRect.Set(this._clipBottomLeft.x, this._clipBottomLeft.y, this._clipTopRight.x - this._clipBottomLeft.x, this._clipTopRight.y - this._clipBottomLeft.y);
      return this._clipRect;
    }
    set
    {
      Vector2 vector2 = new Vector2(value.x, value.y);
      this.clipBottomLeft = vector2;
      vector2.x += value.width;
      vector2.y += value.height;
      this.clipTopRight = vector2;
    }
  }

  public Vector2 clipBottomLeft
  {
    get => this._clipBottomLeft;
    set
    {
      if (!(value != this._clipBottomLeft))
        return;
      this._clipBottomLeft = new Vector2(value.x, value.y);
      this.Build();
      this.UpdateCollider();
    }
  }

  public Vector2 clipTopRight
  {
    get => this._clipTopRight;
    set
    {
      if (!(value != this._clipTopRight))
        return;
      this._clipTopRight = new Vector2(value.x, value.y);
      this.Build();
      this.UpdateCollider();
    }
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
    tk2dSpriteGeomGen.SetSpriteColors(dest, 0, 4, this._color, this.collectionInst.premultipliedAlpha);
  }

  protected void SetGeometry(Vector3[] vertices, Vector2[] uvs)
  {
    tk2dSpriteDefinition currentSprite = this.CurrentSprite;
    float colliderOffsetZ = !((Object) this.boxCollider != (Object) null) ? 0.0f : this.boxCollider.center.z;
    float colliderExtentZ = !((Object) this.boxCollider != (Object) null) ? 0.5f : this.boxCollider.size.z * 0.5f;
    tk2dSpriteGeomGen.SetClippedSpriteGeom(this.meshVertices, this.meshUvs, 0, out this.boundsCenter, out this.boundsExtents, currentSprite, this._scale, this._clipBottomLeft, this._clipTopRight, colliderOffsetZ, colliderExtentZ);
    if (this.meshNormals.Length > 0 || this.meshTangents.Length > 0)
      tk2dSpriteGeomGen.SetSpriteVertexNormals(this.meshVertices, this.meshVertices[0], this.meshVertices[3], currentSprite.normals, currentSprite.tangents, this.meshNormals, this.meshTangents);
    if (!currentSprite.complexGeometry)
      return;
    for (int index = 0; index < vertices.Length; ++index)
      vertices[index] = Vector3.zero;
  }

  public override void Build()
  {
    tk2dSpriteDefinition currentSprite = this.CurrentSprite;
    this.meshUvs = new Vector2[4];
    this.meshVertices = new Vector3[4];
    this.meshColors = new Color32[4];
    this.meshNormals = new Vector3[0];
    this.meshTangents = new Vector4[0];
    if (currentSprite.normals != null && currentSprite.normals.Length > 0)
      this.meshNormals = new Vector3[4];
    if (currentSprite.tangents != null && currentSprite.tangents.Length > 0)
      this.meshTangents = new Vector4[4];
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
    int[] indices = new int[6];
    tk2dSpriteGeomGen.SetClippedSpriteIndices(indices, 0, 0, this.CurrentSprite);
    this.mesh.triangles = indices;
    this.mesh.RecalculateBounds();
    this.mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(this.mesh.bounds, this.renderLayer);
    this.GetComponent<MeshFilter>().mesh = this.mesh;
    this.UpdateCollider();
    this.UpdateMaterial();
  }

  protected override void UpdateGeometry() => this.UpdateGeometryImpl();

  protected override void UpdateColors() => this.UpdateColorsImpl();

  protected override void UpdateVertices() => this.UpdateGeometryImpl();

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
    if (this.meshVertices == null || this.meshVertices.Length == 0)
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
    if (this.OverrideMaterialMode != tk2dBaseSprite.SpriteMaterialOverrideMode.NONE && (Object) this.renderer.sharedMaterial != (Object) null)
    {
      if (this.OverrideMaterialMode == tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE)
      {
        if (!((Object) this.renderer.sharedMaterial != (Object) this.collectionInst.spriteDefinitions[this.spriteId].materialInst))
          return;
        this.renderer.sharedMaterial.mainTexture = this.collectionInst.spriteDefinitions[this.spriteId].materialInst.mainTexture;
        return;
      }
      if (this.OverrideMaterialMode == tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_COMPLEX)
        return;
    }
    if (!((Object) this.renderer.sharedMaterial != (Object) this.collectionInst.spriteDefinitions[this.spriteId].materialInst))
      return;
    this.renderer.material = this.collectionInst.spriteDefinitions[this.spriteId].materialInst;
  }

  protected override int GetCurrentVertexCount() => 4;

  public override void ReshapeBounds(Vector3 dMin, Vector3 dMax)
  {
    tk2dSpriteDefinition currentSprite = this.CurrentSprite;
    Vector3 vector3_1 = Vector3.Scale(currentSprite.untrimmedBoundsDataCenter - 0.5f * currentSprite.untrimmedBoundsDataExtents, this._scale);
    Vector3 vector3_2 = Vector3.Scale(currentSprite.untrimmedBoundsDataExtents, this._scale) + dMax - dMin;
    vector3_2.x /= currentSprite.untrimmedBoundsDataExtents.x;
    vector3_2.y /= currentSprite.untrimmedBoundsDataExtents.y;
    Vector3 vector3_3 = new Vector3(!Mathf.Approximately(this._scale.x, 0.0f) ? vector3_1.x * vector3_2.x / this._scale.x : 0.0f, !Mathf.Approximately(this._scale.y, 0.0f) ? vector3_1.y * vector3_2.y / this._scale.y : 0.0f);
    this.transform.position = this.transform.TransformPoint((vector3_1 + dMin - vector3_3) with
    {
      z = 0.0f
    });
    this.scale = new Vector3(vector3_2.x, vector3_2.y, this._scale.z);
  }
}
