using System;
using System.Collections.Generic;
using tk2dRuntime;
using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/Backend/tk2dBaseSprite")]
  public abstract class tk2dBaseSprite : PersistentVFXBehaviour, ISpriteCollectionForceBuild
  {
    public bool automaticallyManagesDepth = true;
    public bool ignoresTiltworldDepth;
    public bool depthUsesTrimmedBounds;
    public bool allowDefaultLayer;
    public tk2dBaseSprite attachParent;
    public tk2dBaseSprite.SpriteMaterialOverrideMode OverrideMaterialMode;
[HideInInspector]
    public bool independentOrientation;
[Header("Decorator Data")]
    public bool autodetectFootprint = true;
    public IntVector2 customFootprintOrigin;
    public IntVector2 customFootprint;
    protected List<tk2dBaseSprite> attachedRenderers;
    protected MeshRenderer m_renderer;
    private Quaternion m_cachedRotation;
    protected float m_cachedYPosition;
    protected int m_cachedStartingSpriteID;
    public bool hasOffScreenCachedUpdate;
    public tk2dSpriteCollectionData offScreenCachedCollection;
    public int offScreenCachedID = -1;
[SerializeField]
    private tk2dSpriteCollectionData collection;
    protected tk2dSpriteCollectionData collectionInst;
[SerializeField]
    protected Color _color = Color.white;
[SerializeField]
    protected Vector3 _scale = new Vector3(1f, 1f, 1f);
[SerializeField]
    protected int _spriteId;
    public BoxCollider2D boxCollider2D;
    public BoxCollider boxCollider;
    public MeshCollider meshCollider;
    public Vector3[] meshColliderPositions;
    public Mesh meshColliderMesh;
    private Renderer _cachedRenderer;
    protected tk2dSpriteAnimator m_cachedAnimator;
    protected Transform m_transform;
    protected bool m_forceNoTilt;
[NonSerialized]
    public float AdditionalFlatForwardPercentage;
[NonSerialized]
    public float AdditionalPerpForwardPercentage;
    public tk2dBaseSprite.PerpendicularState CachedPerpState;
[HideInInspector]
[SerializeField]
    protected float m_heightOffGround;
[SerializeField]
    protected int renderLayer;
[NonSerialized]
    public bool IsOutlineSprite;
    public bool IsBraveOutlineSprite;
    private Vector2 m_cachedScale;
    public bool IsZDepthDirty;

    public bool usesOverrideMaterial
    {
      get => this.OverrideMaterialMode != tk2dBaseSprite.SpriteMaterialOverrideMode.NONE;
      set
      {
        if (value)
        {
          if (this.OverrideMaterialMode != tk2dBaseSprite.SpriteMaterialOverrideMode.NONE)
            return;
          this.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE;
        }
        else
          this.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.NONE;
      }
    }

    public tk2dSpriteCollectionData Collection
    {
      get
      {
        if ((UnityEngine.Object) this.m_cachedAnimator != (UnityEngine.Object) null)
          this.m_cachedAnimator.ForceInvisibleSpriteUpdate();
        return this.collection;
      }
      set
      {
        this.collection = value;
        this.collectionInst = this.collection.inst;
      }
    }

    public event Action<tk2dBaseSprite> SpriteChanged;

    private void InitInstance()
    {
      if (!((UnityEngine.Object) this.collectionInst == (UnityEngine.Object) null) || !((UnityEngine.Object) this.collection != (UnityEngine.Object) null))
        return;
      this.collectionInst = this.collection.inst;
    }

    public Color color
    {
      get => this._color;
      set
      {
        if (!(value != this._color))
          return;
        this._color = value;
        this.InitInstance();
        this.UpdateColors();
      }
    }

    public Vector3 scale
    {
      get => this._scale;
      set
      {
        if (!(value != this._scale))
          return;
        this._scale = value;
        this.InitInstance();
        this.UpdateVertices();
        this.UpdateCollider();
        if (this.SpriteChanged == null)
          return;
        this.SpriteChanged(this);
      }
    }

    private Renderer CachedRenderer
    {
      get
      {
        if ((UnityEngine.Object) this._cachedRenderer == (UnityEngine.Object) null)
          this._cachedRenderer = this.renderer;
        return this._cachedRenderer;
      }
    }

    public bool ShouldDoTilt
    {
      get
      {
        if (this.m_forceNoTilt)
          return false;
        if (this.CachedPerpState != tk2dBaseSprite.PerpendicularState.UNDEFINED)
          return true;
        return (UnityEngine.Object) this.renderer != (UnityEngine.Object) null && (UnityEngine.Object) this.renderer.sharedMaterial != (UnityEngine.Object) null && this.renderer.sharedMaterial.HasProperty("_Perpendicular");
      }
      set => this.m_forceNoTilt = !value;
    }

    public bool IsPerpendicular
    {
      get
      {
        if ((UnityEngine.Object) this.renderer == (UnityEngine.Object) null || (UnityEngine.Object) this.renderer.sharedMaterial == (UnityEngine.Object) null)
          return false;
        if (this.CachedPerpState != tk2dBaseSprite.PerpendicularState.UNDEFINED)
          return this.CachedPerpState == tk2dBaseSprite.PerpendicularState.PERPENDICULAR;
        if (this.renderer.sharedMaterial.HasProperty("_Perpendicular"))
        {
          if (!Application.isPlaying)
            return (double) this.renderer.sharedMaterial.GetFloat("_Perpendicular") == 1.0;
          this.CachedPerpState = (double) this.renderer.sharedMaterial.GetFloat("_Perpendicular") != 1.0 ? tk2dBaseSprite.PerpendicularState.FLAT : tk2dBaseSprite.PerpendicularState.PERPENDICULAR;
          return this.CachedPerpState == tk2dBaseSprite.PerpendicularState.PERPENDICULAR;
        }
        Debug.LogWarning((object) (this.name + " <- failed to get perp"));
        return true;
      }
      set
      {
        this.CachedPerpState = !value ? tk2dBaseSprite.PerpendicularState.FLAT : tk2dBaseSprite.PerpendicularState.PERPENDICULAR;
        this.ForceBuild();
      }
    }

    public float HeightOffGround
    {
      get => this.m_heightOffGround;
      set => this.m_heightOffGround = value;
    }

    public int SortingOrder
    {
      get => this.CachedRenderer.sortingOrder;
      set
      {
        if (this.CachedRenderer.sortingOrder == value)
          return;
        this.renderLayer = value;
        this.CachedRenderer.sortingOrder = value;
      }
    }

    public bool FlipX
    {
      get => (double) this._scale.x < 0.0;
      set
      {
        this.scale = new Vector3(Mathf.Abs(this._scale.x) * (!value ? 1f : -1f), this._scale.y, this._scale.z);
      }
    }

    public bool FlipY
    {
      get => (double) this._scale.y < 0.0;
      set
      {
        this.scale = new Vector3(this._scale.x, Mathf.Abs(this._scale.y) * (!value ? 1f : -1f), this._scale.z);
      }
    }

    public int spriteId
    {
      get
      {
        if ((UnityEngine.Object) this.m_cachedAnimator != (UnityEngine.Object) null)
          this.m_cachedAnimator.ForceInvisibleSpriteUpdate();
        return this._spriteId;
      }
      set
      {
        this.hasOffScreenCachedUpdate = false;
        this.offScreenCachedCollection = (tk2dSpriteCollectionData) null;
        this.offScreenCachedID = -1;
        if (value == this._spriteId)
          return;
        this.InitInstance();
        value = Mathf.Clamp(value, 0, this.collectionInst.spriteDefinitions.Length - 1);
        if (this._spriteId < 0 || this._spriteId >= this.collectionInst.spriteDefinitions.Length || this.GetCurrentVertexCount() != 4 || this.collectionInst.spriteDefinitions[this._spriteId].complexGeometry != this.collectionInst.spriteDefinitions[value].complexGeometry)
        {
          this._spriteId = value;
          this.UpdateGeometry();
        }
        else
        {
          this._spriteId = value;
          this.UpdateVertices();
        }
        this.UpdateMaterial();
        this.UpdateCollider();
        if (this.SpriteChanged == null)
          return;
        this.SpriteChanged(this);
      }
    }

    public void SetSprite(int newSpriteId) => this.spriteId = newSpriteId;

    public bool SetSprite(string spriteName)
    {
      int spriteIdByName = this.collection.GetSpriteIdByName(spriteName, -1);
      if (spriteIdByName != -1)
        this.SetSprite(spriteIdByName);
      else
        Debug.LogError((object) ("SetSprite - Sprite not found in collection: " + spriteName));
      return spriteIdByName != -1;
    }

    public void SetSprite(tk2dSpriteCollectionData newCollection, int newSpriteId)
    {
      bool flag = false;
      if ((UnityEngine.Object) this.Collection != (UnityEngine.Object) newCollection)
      {
        this.collection = newCollection;
        this.collectionInst = this.collection.inst;
        this._spriteId = -1;
        flag = true;
      }
      this.spriteId = newSpriteId;
      if (!flag)
        return;
      this.UpdateMaterial();
    }

    public bool SetSprite(tk2dSpriteCollectionData newCollection, string spriteName)
    {
      int spriteIdByName = newCollection.GetSpriteIdByName(spriteName, -1);
      if (spriteIdByName != -1)
        this.SetSprite(newCollection, spriteIdByName);
      else
        Debug.LogError((object) ("SetSprite - Sprite not found in collection: " + spriteName));
      return spriteIdByName != -1;
    }

    public void MakePixelPerfect()
    {
      float num1 = 1f;
      tk2dCamera tk2dCamera = tk2dCamera.CameraForLayer(this.gameObject.layer);
      if ((UnityEngine.Object) tk2dCamera != (UnityEngine.Object) null)
      {
        if (this.Collection.version < 2)
          Debug.LogError((object) "Need to rebuild sprite collection.");
        float distance = this.transform.position.z - tk2dCamera.transform.position.z;
        float num2 = this.Collection.invOrthoSize * this.Collection.halfTargetHeight;
        num1 = tk2dCamera.GetSizeAtDistance(distance) * num2;
      }
      else if ((bool) (UnityEngine.Object) Camera.main)
        num1 = (!Camera.main.orthographic ? tk2dPixelPerfectHelper.CalculateScaleForPerspectiveCamera(Camera.main.fieldOfView, this.transform.position.z - Camera.main.transform.position.z) : Camera.main.orthographicSize) * this.Collection.invOrthoSize;
      else
        Debug.LogError((object) "Main camera not found.");
      this.scale = new Vector3(Mathf.Sign(this.scale.x) * num1, Mathf.Sign(this.scale.y) * num1, Mathf.Sign(this.scale.z) * num1);
    }

    public void ForceRotationRebuild()
    {
      if ((bool) (UnityEngine.Object) this.m_transform && this.m_transform.rotation != this.m_cachedRotation)
      {
        this.Build();
        this.m_cachedRotation = this.m_transform.rotation;
      }
      if (this.attachedRenderers == null)
        return;
      for (int index = 0; index < this.attachedRenderers.Count; ++index)
        this.attachedRenderers[index].ForceRotationRebuild();
    }

    protected abstract void UpdateMaterial();

    protected abstract void UpdateColors();

    protected abstract void UpdateVertices();

    protected abstract void UpdateGeometry();

    protected abstract int GetCurrentVertexCount();

    public void ForceUpdateMaterial()
    {
      if ((UnityEngine.Object) this.renderer == (UnityEngine.Object) null || (UnityEngine.Object) this.collectionInst == (UnityEngine.Object) null || !((UnityEngine.Object) this.renderer.sharedMaterial != (UnityEngine.Object) this.collectionInst.spriteDefinitions[this.spriteId].materialInst))
        return;
      this.renderer.material = this.collectionInst.spriteDefinitions[this.spriteId].materialInst;
    }

    public abstract void Build();

    public int GetSpriteIdByName(string name)
    {
      this.InitInstance();
      return this.collectionInst.GetSpriteIdByName(name);
    }

    public static T AddComponent<T>(
      GameObject go,
      tk2dSpriteCollectionData spriteCollection,
      int spriteId)
      where T : tk2dBaseSprite
    {
      T obj = go.AddComponent<T>();
      obj._spriteId = -1;
      obj.SetSprite(spriteCollection, spriteId);
      obj.Build();
      return obj;
    }

    public static T AddComponent<T>(
      GameObject go,
      tk2dSpriteCollectionData spriteCollection,
      string spriteName)
      where T : tk2dBaseSprite
    {
      int spriteIdByName = spriteCollection.GetSpriteIdByName(spriteName, -1);
      if (spriteIdByName != -1)
        return tk2dBaseSprite.AddComponent<T>(go, spriteCollection, spriteIdByName);
      Debug.LogError((object) $"Unable to find sprite named {spriteName} in sprite collection {spriteCollection.spriteCollectionName}");
      return (T) null;
    }

    protected int GetNumVertices()
    {
      this.InitInstance();
      return 4;
    }

    protected int GetNumIndices()
    {
      this.InitInstance();
      return this.collectionInst.spriteDefinitions[this.spriteId].indices.Length;
    }

    protected void SetPositions(Vector3[] positions, Vector3[] normals, Vector4[] tangents)
    {
      if ((UnityEngine.Object) this.m_transform == (UnityEngine.Object) null)
        this.m_transform = this.transform;
      tk2dSpriteDefinition spriteDefinition = this.collectionInst.spriteDefinitions[this.spriteId];
      int numVertices = this.GetNumVertices();
      positions[0] = Vector3.Scale(spriteDefinition.position0, this._scale);
      positions[1] = Vector3.Scale(spriteDefinition.position1, this._scale);
      positions[2] = Vector3.Scale(spriteDefinition.position2, this._scale);
      positions[3] = Vector3.Scale(spriteDefinition.position3, this._scale);
      if (!this.ShouldDoTilt)
        return;
      float num = 0.0f;
      for (int index = 0; index < numVertices; ++index)
      {
        float y = (this.m_transform.rotation * Vector3.Scale(positions[index], this.m_transform.lossyScale)).y;
        if (this.IsPerpendicular)
        {
          positions[index].z -= y;
          if ((double) this.AdditionalPerpForwardPercentage > 0.0)
            positions[index].z -= y * this.AdditionalPerpForwardPercentage;
        }
        else
        {
          positions[index].z += y;
          if ((double) this.AdditionalFlatForwardPercentage > 0.0)
          {
            num = Mathf.Max(y * this.AdditionalFlatForwardPercentage, num);
            positions[index].z -= y * this.AdditionalFlatForwardPercentage;
          }
        }
      }
      if ((double) this.AdditionalFlatForwardPercentage <= 0.0)
        return;
      for (int index = 0; index < numVertices; ++index)
        positions[index] = positions[index] + new Vector3(0.0f, 0.0f, num);
    }

    protected void SetColors(Color32[] dest)
    {
      Color color = this._color;
      if (this.collectionInst.premultipliedAlpha)
      {
        color.r *= color.a;
        color.g *= color.a;
        color.b *= color.a;
      }
      Color32 color32 = (Color32) color;
      int numVertices = this.GetNumVertices();
      for (int index = 0; index < numVertices; ++index)
        dest[index] = color32;
    }

    public Bounds GetBounds()
    {
      this.InitInstance();
      tk2dSpriteDefinition spriteDefinition = this.collectionInst.spriteDefinitions[this._spriteId];
      return new Bounds(new Vector3(spriteDefinition.boundsDataCenter.x * this._scale.x, spriteDefinition.boundsDataCenter.y * this._scale.y, spriteDefinition.boundsDataCenter.z * this._scale.z), new Vector3(spriteDefinition.boundsDataExtents.x * Mathf.Abs(this._scale.x), spriteDefinition.boundsDataExtents.y * Mathf.Abs(this._scale.y), spriteDefinition.boundsDataExtents.z * Mathf.Abs(this._scale.z)));
    }

    public Bounds GetUntrimmedBounds()
    {
      this.InitInstance();
      tk2dSpriteDefinition spriteDefinition = this.collectionInst.spriteDefinitions[this._spriteId];
      return new Bounds(new Vector3(spriteDefinition.untrimmedBoundsDataCenter.x * this._scale.x, spriteDefinition.untrimmedBoundsDataCenter.y * this._scale.y, spriteDefinition.untrimmedBoundsDataCenter.z * this._scale.z), new Vector3(spriteDefinition.untrimmedBoundsDataExtents.x * Mathf.Abs(this._scale.x), spriteDefinition.untrimmedBoundsDataExtents.y * Mathf.Abs(this._scale.y), spriteDefinition.untrimmedBoundsDataExtents.z * Mathf.Abs(this._scale.z)));
    }

    public static Bounds AdjustedMeshBounds(Bounds bounds, int renderLayer)
    {
      Vector3 center = bounds.center with
      {
        z = (float) -renderLayer * 0.01f
      };
      bounds.center = center;
      return bounds;
    }

    public tk2dSpriteDefinition GetCurrentSpriteDef()
    {
      this.InitInstance();
      return (UnityEngine.Object) this.collectionInst == (UnityEngine.Object) null ? (tk2dSpriteDefinition) null : this.collectionInst.spriteDefinitions[this._spriteId];
    }

    public tk2dSpriteDefinition GetTrueCurrentSpriteDef()
    {
      return this.hasOffScreenCachedUpdate ? this.offScreenCachedCollection.spriteDefinitions[this.offScreenCachedID] : this.GetCurrentSpriteDef();
    }

    public tk2dSpriteDefinition CurrentSprite
    {
      get
      {
        this.InitInstance();
        return (UnityEngine.Object) this.collectionInst == (UnityEngine.Object) null ? (tk2dSpriteDefinition) null : this.collectionInst.spriteDefinitions[this._spriteId];
      }
    }

    public virtual void ReshapeBounds(Vector3 dMin, Vector3 dMax)
    {
    }

    protected virtual bool NeedBoxCollider() => false;

    protected virtual void UpdateCollider()
    {
      tk2dSpriteDefinition spriteDefinition = this.collectionInst.spriteDefinitions[this._spriteId];
      if (spriteDefinition.physicsEngine == tk2dSpriteDefinition.PhysicsEngine.Physics3D || spriteDefinition.physicsEngine != tk2dSpriteDefinition.PhysicsEngine.Physics2D)
        return;
      if (spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Box)
      {
        if ((UnityEngine.Object) this.boxCollider2D == (UnityEngine.Object) null)
        {
          this.boxCollider2D = this.gameObject.GetComponent<BoxCollider2D>();
          if ((UnityEngine.Object) this.boxCollider2D == (UnityEngine.Object) null)
            this.boxCollider2D = this.gameObject.AddComponent<BoxCollider2D>();
        }
        if (!this.boxCollider2D.enabled)
          this.boxCollider2D.enabled = true;
        this.boxCollider2D.offset = new Vector2(spriteDefinition.colliderVertices[0].x * this._scale.x, spriteDefinition.colliderVertices[0].y * this._scale.y);
        this.boxCollider2D.size = new Vector2(Mathf.Abs(2f * spriteDefinition.colliderVertices[1].x * this._scale.x), Mathf.Abs(2f * spriteDefinition.colliderVertices[1].y * this._scale.y));
      }
      else if (spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
      {
        Debug.LogError((object) "BraveTK2D does not support mesh colliders.");
      }
      else
      {
        if (spriteDefinition.colliderType != tk2dSpriteDefinition.ColliderType.None || !((UnityEngine.Object) this.boxCollider2D != (UnityEngine.Object) null) || !this.boxCollider2D.enabled)
          return;
        this.boxCollider2D.enabled = false;
      }
    }

    protected virtual void CreateCollider()
    {
      tk2dSpriteDefinition spriteDefinition = this.collectionInst.spriteDefinitions[this._spriteId];
      if (spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Unset || spriteDefinition.physicsEngine == tk2dSpriteDefinition.PhysicsEngine.Physics3D || spriteDefinition.physicsEngine != tk2dSpriteDefinition.PhysicsEngine.Physics2D)
        return;
      this.UpdateCollider();
    }

    protected void Awake()
    {
      if ((UnityEngine.Object) this.collection != (UnityEngine.Object) null)
        this.collectionInst = this.collection.inst;
      this.CachedRenderer.sortingOrder = this.renderLayer;
      this.m_cachedStartingSpriteID = this._spriteId;
      this.m_transform = this.transform;
      this.m_renderer = this.GetComponent<MeshRenderer>();
      this.m_cachedYPosition = this.m_transform.position.y;
      this.m_cachedAnimator = this.GetComponent<tk2dSpriteAnimator>();
      if ((UnityEngine.Object) this.attachParent != (UnityEngine.Object) null)
      {
        this.automaticallyManagesDepth = false;
        this.attachParent.AttachRenderer(this);
      }
      bool flag = this.gameObject.layer == 28;
      if (!this.allowDefaultLayer)
      {
        if (this.m_renderer.sortingLayerName == "Default" || this.m_renderer.sortingLayerID == 0)
        {
          this.renderLayer = 2;
          DepthLookupManager.ProcessRenderer((Renderer) this.m_renderer);
        }
        if (this.gameObject.layer < 13 || this.gameObject.layer > 26)
          this.gameObject.layer = 22;
      }
      if (flag || Pixelator.IsValidReflectionObject(this))
        this.gameObject.layer = 28;
      this.m_cachedScale = (Vector2) this.scale;
      if (this.automaticallyManagesDepth)
        this.UpdateZDepth();
      this.m_cachedRotation = this.m_transform.rotation;
    }

    public void OnSpawned()
    {
      this.m_transform = this.transform;
      this.m_cachedYPosition = this.m_transform.position.y;
      if ((UnityEngine.Object) this.attachParent != (UnityEngine.Object) null)
      {
        this.automaticallyManagesDepth = false;
        this.attachParent.AttachRenderer(this);
      }
      if (this.automaticallyManagesDepth)
        this.UpdateZDepth();
      this.m_cachedRotation = this.m_transform.rotation;
    }

    public void OnDespawned() => this.scale = (Vector3) this.m_cachedScale;

    public void CreateSimpleBoxCollider()
    {
      if (this.CurrentSprite == null)
        return;
      if (this.CurrentSprite.physicsEngine == tk2dSpriteDefinition.PhysicsEngine.Physics3D)
      {
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        if ((UnityEngine.Object) this.boxCollider2D != (UnityEngine.Object) null)
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.boxCollider2D, true);
        this.boxCollider = this.GetComponent<BoxCollider>();
        if (!((UnityEngine.Object) this.boxCollider == (UnityEngine.Object) null))
          return;
        this.boxCollider = this.gameObject.AddComponent<BoxCollider>();
      }
      else
      {
        if (this.CurrentSprite.physicsEngine != tk2dSpriteDefinition.PhysicsEngine.Physics2D)
          return;
        this.boxCollider = this.GetComponent<BoxCollider>();
        if ((UnityEngine.Object) this.boxCollider != (UnityEngine.Object) null)
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.boxCollider, true);
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        if (!((UnityEngine.Object) this.boxCollider2D == (UnityEngine.Object) null))
          return;
        this.boxCollider2D = this.gameObject.AddComponent<BoxCollider2D>();
      }
    }

    public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
    {
      return (UnityEngine.Object) this.Collection == (UnityEngine.Object) spriteCollection;
    }

    public virtual void ForceBuild()
    {
      if (!(bool) (UnityEngine.Object) this || (UnityEngine.Object) this.collection == (UnityEngine.Object) null)
        return;
      this.collectionInst = this.collection.inst;
      if (this.spriteId < 0 || this.spriteId >= this.collectionInst.spriteDefinitions.Length)
        this.spriteId = 0;
      this.Build();
      if (this.SpriteChanged == null)
        return;
      this.SpriteChanged(this);
    }

    public static GameObject CreateFromTexture<T>(
      Texture texture,
      tk2dSpriteCollectionSize size,
      Rect region,
      Vector2 anchor)
      where T : tk2dBaseSprite
    {
      tk2dSpriteCollectionData fromTexture = SpriteCollectionGenerator.CreateFromTexture(texture, size, region, anchor);
      if ((UnityEngine.Object) fromTexture == (UnityEngine.Object) null)
        return (GameObject) null;
      GameObject go = new GameObject();
      tk2dBaseSprite.AddComponent<T>(go, fromTexture, 0);
      return go;
    }

    public IntVector2 GetAnchorPixelOffset()
    {
      return -PhysicsEngine.UnitToPixel((Vector2) this.GetUntrimmedBounds().min);
    }

    public Vector2 GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor anchor)
    {
      Bounds bounds = this.GetBounds();
      Vector2 min = (Vector2) bounds.min;
      switch (anchor)
      {
        case tk2dBaseSprite.Anchor.LowerCenter:
        case tk2dBaseSprite.Anchor.MiddleCenter:
        case tk2dBaseSprite.Anchor.UpperCenter:
          min.x += bounds.extents.x;
          break;
        case tk2dBaseSprite.Anchor.LowerRight:
        case tk2dBaseSprite.Anchor.MiddleRight:
        case tk2dBaseSprite.Anchor.UpperRight:
          min.x += bounds.extents.x * 2f;
          break;
      }
      switch (anchor)
      {
        case tk2dBaseSprite.Anchor.MiddleLeft:
        case tk2dBaseSprite.Anchor.MiddleCenter:
        case tk2dBaseSprite.Anchor.MiddleRight:
          min.y += bounds.extents.y;
          break;
        case tk2dBaseSprite.Anchor.UpperLeft:
        case tk2dBaseSprite.Anchor.UpperCenter:
        case tk2dBaseSprite.Anchor.UpperRight:
          min.y += bounds.extents.y * 2f;
          break;
      }
      return min;
    }

    public Vector2 WorldCenter
    {
      get
      {
        return this.transform.position.XY() + this.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter).Rotate(this.transform.eulerAngles.z);
      }
    }

    public Vector2 WorldTopCenter
    {
      get
      {
        return this.transform.position.XY() + this.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.UpperCenter).Rotate(this.transform.eulerAngles.z);
      }
    }

    public Vector2 WorldTopLeft
    {
      get
      {
        return this.transform.position.XY() + this.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.UpperLeft).Rotate(this.transform.eulerAngles.z);
      }
    }

    public Vector2 WorldTopRight
    {
      get
      {
        return this.transform.position.XY() + this.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.UpperRight).Rotate(this.transform.eulerAngles.z);
      }
    }

    public Vector2 WorldBottomLeft
    {
      get
      {
        return this.transform.position.XY() + this.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.LowerLeft).Rotate(this.transform.eulerAngles.z);
      }
    }

    public Vector2 WorldBottomCenter
    {
      get
      {
        return this.transform.position.XY() + this.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.LowerCenter).Rotate(this.transform.eulerAngles.z);
      }
    }

    public Vector2 WorldBottomRight
    {
      get
      {
        return this.transform.position.XY() + this.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.LowerRight).Rotate(this.transform.eulerAngles.z);
      }
    }

    public void PlayEffectOnSprite(GameObject effect, Vector3 offset, bool attached = true)
    {
      if ((UnityEngine.Object) effect == (UnityEngine.Object) null)
        return;
      GameObject gameObject = SpawnManager.SpawnVFX(effect);
      tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
      component.PlaceAtPositionByAnchor(this.WorldCenter.ToVector3ZUp() + offset, tk2dBaseSprite.Anchor.MiddleCenter);
      if (!attached)
        return;
      gameObject.transform.parent = this.transform;
      component.HeightOffGround = 0.2f;
      this.AttachRenderer(component);
    }

    public void PlaceAtPositionByAnchor(Vector3 position, tk2dBaseSprite.Anchor anchor)
    {
      Vector2 positionFromAnchor = this.GetRelativePositionFromAnchor(anchor);
      this.m_transform.position = position - positionFromAnchor.ToVector3ZUp();
    }

    public void PlaceAtLocalPositionByAnchor(Vector3 position, tk2dBaseSprite.Anchor anchor)
    {
      Vector2 positionFromAnchor = this.GetRelativePositionFromAnchor(anchor);
      this.m_transform.localPosition = position - positionFromAnchor.ToVector3ZUp();
    }

    public void AttachRenderer(tk2dBaseSprite attachment)
    {
      if (this.attachedRenderers == null)
        this.attachedRenderers = new List<tk2dBaseSprite>();
      if (this.attachedRenderers.Contains(attachment))
        return;
      attachment.attachParent = this;
      if (!attachment.independentOrientation)
        attachment.IsPerpendicular = this.IsPerpendicular;
      this.attachedRenderers.Add(attachment);
    }

    public void DetachRenderer(tk2dBaseSprite attachment)
    {
      if (this.attachedRenderers == null || !this.attachedRenderers.Contains(attachment))
        return;
      if (attachment is tk2dSprite)
        (attachment as tk2dSprite).attachParent = (tk2dBaseSprite) null;
      this.attachedRenderers.Remove(attachment);
    }

    public void ForceBuildWithAttached()
    {
      this.ForceBuild();
      if (this.attachedRenderers == null || this.attachedRenderers.Count <= 0)
        return;
      for (int index = 0; index < this.attachedRenderers.Count; ++index)
      {
        if (this.attachedRenderers[index] is tk2dSprite)
          (this.attachedRenderers[index] as tk2dSprite).ForceBuildWithAttached();
        else
          this.attachedRenderers[index].ForceBuild();
      }
    }

    public void UpdateZDepthAttached(float parentDepth, float parentWorldY, bool parentPerpendicular)
    {
      float num1 = parentDepth - this.HeightOffGround;
      float num2 = this.m_transform.position.y - parentWorldY;
      this.UpdateZDepthInternal(!parentPerpendicular ? num1 + num2 : num1 - num2, this.m_transform.position.y);
    }

    public void StackTraceAttachment()
    {
      if (this.attachedRenderers == null)
      {
        Debug.Log((object) (this.name + " has no children."));
      }
      else
      {
        string message = this.name + " parent of: ";
        for (int index = 0; index < this.attachedRenderers.Count; ++index)
          message = $"{message}{this.attachedRenderers[index].name} ";
        Debug.Log((object) message);
        for (int index = 0; index < this.attachedRenderers.Count; ++index)
          this.attachedRenderers[index].StackTraceAttachment();
      }
    }

    public void UpdateZDepthLater() => this.IsZDepthDirty = true;

    public void UpdateZDepth()
    {
      this.IsZDepthDirty = false;
      if (this.ignoresTiltworldDepth)
        return;
      if ((UnityEngine.Object) this.attachParent != (UnityEngine.Object) null)
      {
        this.attachParent.UpdateZDepth();
      }
      else
      {
        if ((UnityEngine.Object) this.m_transform == (UnityEngine.Object) null && (bool) (UnityEngine.Object) this)
          this.m_transform = this.transform;
        if ((UnityEngine.Object) this.collectionInst == (UnityEngine.Object) null || this.collectionInst.spriteDefinitions == null || !(bool) (UnityEngine.Object) this.m_transform)
          return;
        float y1 = this.m_transform.position.y;
        float num;
        if (this.depthUsesTrimmedBounds)
        {
          float y2 = this.GetBounds().min.y;
          num = (float) ((double) y1 + (double) y2 + (!this.IsPerpendicular ? -(double) y2 : (double) y2));
        }
        else
          num = y1;
        this.UpdateZDepthInternal(num - this.HeightOffGround, y1);
      }
    }

    protected void UpdateZDepthInternal(float targetZValue, float currentYValue)
    {
      this.IsZDepthDirty = false;
      Vector3 position = this.m_transform.position;
      if ((double) position.z != (double) targetZValue)
      {
        position.z = targetZValue;
        this.m_transform.position = position;
      }
      if (this.attachedRenderers == null || this.attachedRenderers.Count <= 0)
        return;
      for (int index = 0; index < this.attachedRenderers.Count; ++index)
      {
        tk2dBaseSprite attachedRenderer = this.attachedRenderers[index];
        if (!(bool) (UnityEngine.Object) attachedRenderer || (UnityEngine.Object) attachedRenderer.attachParent != (UnityEngine.Object) this)
        {
          this.attachedRenderers.RemoveAt(index);
          --index;
        }
        else
        {
          attachedRenderer?.UpdateZDepthAttached(targetZValue, currentYValue, this.IsPerpendicular);
          if (!attachedRenderer.independentOrientation)
          {
            bool isPerpendicular = this.IsPerpendicular;
            if (isPerpendicular && !attachedRenderer.IsPerpendicular)
              attachedRenderer.IsPerpendicular = true;
            if (!isPerpendicular && attachedRenderer.IsPerpendicular)
              attachedRenderer.IsPerpendicular = false;
          }
        }
      }
    }

    protected override void OnDestroy() => base.OnDestroy();

public enum SpriteMaterialOverrideMode
    {
      NONE,
      OVERRIDE_MATERIAL_SIMPLE,
      OVERRIDE_MATERIAL_COMPLEX,
    }

    public enum Anchor
    {
      LowerLeft,
      LowerCenter,
      LowerRight,
      MiddleLeft,
      MiddleCenter,
      MiddleRight,
      UpperLeft,
      UpperCenter,
      UpperRight,
    }

    public enum PerpendicularState
    {
      UNDEFINED,
      PERPENDICULAR,
      FLAT,
    }
  }

