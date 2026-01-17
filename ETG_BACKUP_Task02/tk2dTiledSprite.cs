// Decompiled with JetBrains decompiler
// Type: tk2dTiledSprite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[RequireComponent(typeof (MeshRenderer))]
[AddComponentMenu("2D Toolkit/Sprite/tk2dTiledSprite")]
[RequireComponent(typeof (MeshFilter))]
public class tk2dTiledSprite : tk2dBaseSprite
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
  protected bool _createBoxCollider;
  private Vector3 boundsCenter = Vector3.zero;
  private Vector3 boundsExtents = Vector3.zero;
  public tk2dTiledSprite.OverrideGetTiledSpriteGeomDescDelegate OverrideGetTiledSpriteGeomDesc;
  public tk2dTiledSprite.OverrideSetTiledSpriteGeomDelegate OverrideSetTiledSpriteGeom;

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
    if ((Object) this.boxCollider == (Object) null)
      this.boxCollider = this.GetComponent<BoxCollider>();
    if (!((Object) this.boxCollider2D == (Object) null))
      return;
    this.boxCollider2D = this.GetComponent<BoxCollider2D>();
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
    int numVertices;
    int numIndices;
    if (this.OverrideGetTiledSpriteGeomDesc != null)
      this.OverrideGetTiledSpriteGeomDesc(out numVertices, out numIndices, this.CurrentSprite, this.dimensions);
    else
      tk2dSpriteGeomGen.GetTiledSpriteGeomDesc(out numVertices, out numIndices, this.CurrentSprite, this.dimensions);
    tk2dSpriteGeomGen.SetSpriteColors(dest, 0, numVertices, this._color, this.collectionInst.premultipliedAlpha);
  }

  public override void Build()
  {
    tk2dSpriteDefinition currentSprite = this.CurrentSprite;
    int numVertices;
    int numIndices;
    if (this.OverrideGetTiledSpriteGeomDesc != null)
      this.OverrideGetTiledSpriteGeomDesc(out numVertices, out numIndices, currentSprite, this.dimensions);
    else
      tk2dSpriteGeomGen.GetTiledSpriteGeomDesc(out numVertices, out numIndices, currentSprite, this.dimensions);
    int length1 = numVertices;
    if (this.meshUvs == null || this.meshUvs.Length < numVertices)
    {
      length1 = BraveUtility.SmartListResizer(this.meshUvs != null ? this.meshUvs.Length : 0, numVertices);
      this.meshUvs = new Vector2[length1];
      this.meshVertices = new Vector3[length1];
      this.meshColors = new Color32[length1];
    }
    if (this.meshIndices == null || this.meshIndices.Length < numIndices)
      this.meshIndices = new int[BraveUtility.SmartListResizer(this.meshIndices != null ? this.meshIndices.Length : 0, numIndices, forceMultipleOf: 3)];
    int length2 = 0;
    if (currentSprite != null && currentSprite.normals != null && currentSprite.normals.Length > 0)
      length2 = length1;
    if (this.meshNormals == null || this.meshNormals.Length < length2)
      this.meshNormals = new Vector3[length2];
    int length3 = 0;
    if (currentSprite != null && currentSprite.tangents != null && currentSprite.tangents.Length > 0)
      length3 = length1;
    if (this.meshTangents == null || this.meshTangents.Length < length3)
      this.meshTangents = new Vector4[length3];
    float colliderOffsetZ = !((Object) this.boxCollider != (Object) null) ? 0.0f : this.boxCollider.center.z;
    float colliderExtentZ = !((Object) this.boxCollider != (Object) null) ? 0.5f : this.boxCollider.size.z * 0.5f;
    if (this.OverrideSetTiledSpriteGeom != null)
      this.OverrideSetTiledSpriteGeom(this.meshVertices, this.meshUvs, 0, out this.boundsCenter, out this.boundsExtents, currentSprite, this._scale, this.dimensions, this.anchor, colliderOffsetZ, colliderExtentZ);
    else
      tk2dSpriteGeomGen.SetTiledSpriteGeom(this.meshVertices, this.meshUvs, 0, out this.boundsCenter, out this.boundsExtents, currentSprite, this._scale, this.dimensions, this.anchor, colliderOffsetZ, colliderExtentZ);
    tk2dSpriteGeomGen.SetTiledSpriteIndices(this.meshIndices, 0, 0, currentSprite, this.dimensions, this.OverrideGetTiledSpriteGeomDesc);
    if (this.meshNormals.Length > 0 || this.meshTangents.Length > 0)
      tk2dSpriteGeomGen.SetSpriteVertexNormalsFast(this.meshVertices, this.meshNormals, this.meshTangents);
    this.SetColors(this.meshColors);
    if (this.ShouldDoTilt)
    {
      bool isPerpendicular = this.IsPerpendicular;
      for (int index = 0; index < numVertices; ++index)
      {
        float y = (this.m_transform.rotation * Vector3.Scale(this.meshVertices[index], this.m_transform.lossyScale)).y;
        if (isPerpendicular)
          this.meshVertices[index].z -= y;
        else
          this.meshVertices[index].z += y;
      }
    }
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

  protected void UpdateGeometryImpl() => this.Build();

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
    if (this.OverrideMaterialMode != tk2dBaseSprite.SpriteMaterialOverrideMode.NONE || !((Object) this.renderer.sharedMaterial != (Object) this.collectionInst.spriteDefinitions[this.spriteId].materialInst))
      return;
    this.renderer.material = this.collectionInst.spriteDefinitions[this.spriteId].materialInst;
  }

  protected override int GetCurrentVertexCount() => 16 /*0x10*/;

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

  public delegate void OverrideGetTiledSpriteGeomDescDelegate(
    out int numVertices,
    out int numIndices,
    tk2dSpriteDefinition spriteDef,
    Vector2 dimensions);

  public delegate void OverrideSetTiledSpriteGeomDelegate(
    Vector3[] pos,
    Vector2[] uv,
    int offset,
    out Vector3 boundsCenter,
    out Vector3 boundsExtents,
    tk2dSpriteDefinition spriteDef,
    Vector3 scale,
    Vector2 dimensions,
    tk2dBaseSprite.Anchor anchor,
    float colliderOffsetZ,
    float colliderExtentZ);
}
