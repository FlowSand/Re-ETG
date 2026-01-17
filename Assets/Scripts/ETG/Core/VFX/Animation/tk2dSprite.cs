// Decompiled with JetBrains decompiler
// Type: tk2dSprite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    [RequireComponent(typeof (MeshFilter))]
    [ExecuteInEditMode]
    [AddComponentMenu("2D Toolkit/Sprite/tk2dSprite")]
    [RequireComponent(typeof (MeshRenderer))]
    public class tk2dSprite : tk2dBaseSprite
    {
      private Mesh mesh;
      private Vector3[] meshVertices;
      private Vector3[] meshNormals;
      private Vector4[] meshTangents;
      private Color32[] meshColors;
      private MeshFilter m_filter;
      public bool ApplyEmissivePropertyBlock;
      public bool GenerateUV2;
      public bool LockUV2OnFrameOne;
      public bool StaticPositions;
      [NonSerialized]
      private bool m_hasGeneratedLockedUV2;
      private static Vector3[] m_defaultNormalArray = new Vector3[4]
      {
        new Vector3(-1f, -1f, -1f),
        new Vector3(1f, -1f, -1f),
        new Vector3(-1f, 1f, -1f),
        new Vector3(1f, 1f, -1f)
      };
      private static Vector4[] m_defaultTangentArray = new Vector4[4]
      {
        new Vector4(1f, 0.0f, 0.0f, 1f),
        new Vector4(1f, 0.0f, 0.0f, 1f),
        new Vector4(1f, 0.0f, 0.0f, 1f),
        new Vector4(1f, 0.0f, 0.0f, 1f)
      };
      private bool hasSetPositions;
      private static Vector2[] m_defaultUvs = new Vector2[4]
      {
        Vector2.zero,
        Vector2.right,
        Vector2.up,
        Vector2.one
      };
      private static int m_shaderEmissivePowerID = -1;
      private static int m_shaderEmissiveColorPowerID = -1;
      private static int m_shaderEmissiveColorID = -1;
      private static int m_shaderThresholdID = -1;

      private new void Awake()
      {
        base.Awake();
        this.mesh = new Mesh();
        this.mesh.MarkDynamic();
        this.mesh.hideFlags = HideFlags.DontSave;
        this.m_filter = this.GetComponent<MeshFilter>();
        this.m_filter.mesh = this.mesh;
        if (!(bool) (UnityEngine.Object) this.Collection)
          return;
        if (this._spriteId < 0 || this._spriteId >= this.Collection.Count)
          this._spriteId = 0;
        this.Build();
      }

      protected override void OnDestroy()
      {
        base.OnDestroy();
        if ((bool) (UnityEngine.Object) this.mesh)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.mesh);
        if (!(bool) (UnityEngine.Object) this.meshColliderMesh)
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.meshColliderMesh);
      }

      public override void Build()
      {
        tk2dSpriteDefinition spriteDefinition = this.collectionInst.spriteDefinitions[this.spriteId];
        if (this.meshVertices == null || this.meshVertices.Length != 4 || this.meshColors == null || this.meshColors.Length != 4)
        {
          this.meshVertices = new Vector3[4];
          this.meshColors = new Color32[4];
        }
        this.meshNormals = tk2dSprite.m_defaultNormalArray;
        this.meshTangents = tk2dSprite.m_defaultTangentArray;
        this.SetPositions(this.meshVertices, this.meshNormals, this.meshTangents);
        this.SetColors(this.meshColors);
        if ((UnityEngine.Object) this.mesh == (UnityEngine.Object) null)
        {
          this.mesh = new Mesh();
          this.mesh.MarkDynamic();
          this.mesh.hideFlags = HideFlags.DontSave;
          this.GetComponent<MeshFilter>().mesh = this.mesh;
        }
        this.mesh.Clear();
        this.mesh.vertices = this.meshVertices;
        this.mesh.normals = this.meshNormals;
        this.mesh.tangents = this.meshTangents;
        this.mesh.colors32 = this.meshColors;
        this.mesh.uv = spriteDefinition.uvs;
        if (this.GenerateUV2)
        {
          if (this.LockUV2OnFrameOne)
          {
            if (!this.m_hasGeneratedLockedUV2)
            {
              this.m_hasGeneratedLockedUV2 = true;
              this.mesh.uv2 = spriteDefinition.uvs;
            }
          }
          else
            this.mesh.uv2 = !((UnityEngine.Object) this.spriteAnimator != (UnityEngine.Object) null) || !this.spriteAnimator.IsFrameBlendedAnimation ? tk2dSprite.m_defaultUvs : this.spriteAnimator.GetNextFrameUVs();
        }
        this.mesh.triangles = spriteDefinition.indices;
        this.mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(this.GetBounds(), this.renderLayer);
        this.UpdateMaterial();
        this.CreateCollider();
      }

      public static tk2dSprite AddComponent(
        GameObject go,
        tk2dSpriteCollectionData spriteCollection,
        int spriteId)
      {
        return tk2dBaseSprite.AddComponent<tk2dSprite>(go, spriteCollection, spriteId);
      }

      public static tk2dSprite AddComponent(
        GameObject go,
        tk2dSpriteCollectionData spriteCollection,
        string spriteName)
      {
        return tk2dBaseSprite.AddComponent<tk2dSprite>(go, spriteCollection, spriteName);
      }

      public static GameObject CreateFromTexture(
        Texture texture,
        tk2dSpriteCollectionSize size,
        Rect region,
        Vector2 anchor)
      {
        return tk2dBaseSprite.CreateFromTexture<tk2dSprite>(texture, size, region, anchor);
      }

      protected override void UpdateGeometry() => this.UpdateGeometryImpl();

      protected override void UpdateColors() => this.UpdateColorsImpl();

      protected override void UpdateVertices() => this.UpdateVerticesImpl();

      protected void UpdateColorsImpl()
      {
        if ((UnityEngine.Object) this.mesh == (UnityEngine.Object) null || this.meshColors == null || this.meshColors.Length == 0)
          return;
        this.SetColors(this.meshColors);
        this.mesh.colors32 = this.meshColors;
      }

      protected void UpdateVerticesImpl()
      {
        if ((UnityEngine.Object) this.mesh == (UnityEngine.Object) null || this.meshVertices == null || this.meshVertices.Length == 0 || !(bool) (UnityEngine.Object) this.collectionInst || this.collectionInst.spriteDefinitions == null)
          return;
        tk2dSpriteDefinition spriteDefinition = this.collectionInst.spriteDefinitions[this.spriteId];
        this.meshNormals = tk2dSprite.m_defaultNormalArray;
        this.meshTangents = tk2dSprite.m_defaultTangentArray;
        if (!this.StaticPositions || !this.hasSetPositions)
        {
          this.SetPositions(this.meshVertices, this.meshNormals, this.meshTangents);
          this.hasSetPositions = true;
        }
        this.mesh.vertices = this.meshVertices;
        this.mesh.normals = this.meshNormals;
        this.mesh.tangents = this.meshTangents;
        this.mesh.uv = spriteDefinition.uvs;
        if (this.GenerateUV2)
        {
          if (this.LockUV2OnFrameOne)
          {
            if (!this.m_hasGeneratedLockedUV2)
            {
              this.m_hasGeneratedLockedUV2 = true;
              this.mesh.uv2 = spriteDefinition.uvs;
            }
          }
          else
            this.mesh.uv2 = !(bool) (UnityEngine.Object) this.spriteAnimator || !this.spriteAnimator.IsFrameBlendedAnimation ? tk2dSprite.m_defaultUvs : this.spriteAnimator.GetNextFrameUVs();
        }
        this.mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(this.GetBounds(), this.renderLayer);
      }

      protected void UpdateGeometryImpl()
      {
        if ((UnityEngine.Object) this.mesh == (UnityEngine.Object) null)
          return;
        tk2dSpriteDefinition spriteDefinition = this.collectionInst.spriteDefinitions[this.spriteId];
        if (this.meshVertices == null || this.meshVertices.Length != 4)
        {
          this.meshVertices = new Vector3[4];
          this.meshNormals = tk2dSprite.m_defaultNormalArray;
          this.meshTangents = tk2dSprite.m_defaultTangentArray;
          this.meshColors = new Color32[4];
        }
        this.SetPositions(this.meshVertices, this.meshNormals, this.meshTangents);
        this.SetColors(this.meshColors);
        this.mesh.Clear();
        this.mesh.vertices = this.meshVertices;
        this.mesh.normals = this.meshNormals;
        this.mesh.tangents = this.meshTangents;
        this.mesh.colors32 = this.meshColors;
        this.mesh.uv = spriteDefinition.uvs;
        if (this.GenerateUV2)
        {
          if (this.LockUV2OnFrameOne)
          {
            if (!this.m_hasGeneratedLockedUV2)
            {
              this.m_hasGeneratedLockedUV2 = true;
              this.mesh.uv2 = spriteDefinition.uvs;
            }
          }
          else
            this.mesh.uv2 = !this.spriteAnimator.IsFrameBlendedAnimation ? tk2dSprite.m_defaultUvs : this.spriteAnimator.GetNextFrameUVs();
        }
        this.mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(this.GetBounds(), this.renderLayer);
        this.mesh.triangles = spriteDefinition.indices;
      }

      protected void CopyPropertyBlock(Material source, Material dest)
      {
        if (dest.HasProperty(tk2dSprite.m_shaderEmissivePowerID) && source.HasProperty(tk2dSprite.m_shaderEmissivePowerID))
          dest.SetFloat(tk2dSprite.m_shaderEmissivePowerID, source.GetFloat(tk2dSprite.m_shaderEmissivePowerID));
        if (dest.HasProperty(tk2dSprite.m_shaderEmissiveColorPowerID) && source.HasProperty(tk2dSprite.m_shaderEmissiveColorPowerID))
          dest.SetFloat(tk2dSprite.m_shaderEmissiveColorPowerID, source.GetFloat(tk2dSprite.m_shaderEmissiveColorPowerID));
        if (dest.HasProperty(tk2dSprite.m_shaderEmissiveColorID) && source.HasProperty(tk2dSprite.m_shaderEmissiveColorID))
          dest.SetColor(tk2dSprite.m_shaderEmissiveColorID, source.GetColor(tk2dSprite.m_shaderEmissiveColorID));
        if (!dest.HasProperty(tk2dSprite.m_shaderThresholdID) || !source.HasProperty(tk2dSprite.m_shaderThresholdID))
          return;
        dest.SetFloat(tk2dSprite.m_shaderThresholdID, source.GetFloat(tk2dSprite.m_shaderThresholdID));
      }

      protected override void UpdateMaterial()
      {
        if (!(bool) (UnityEngine.Object) this.renderer)
          return;
        if (tk2dSprite.m_shaderEmissiveColorID == -1)
        {
          tk2dSprite.m_shaderEmissivePowerID = Shader.PropertyToID("_EmissivePower");
          tk2dSprite.m_shaderEmissiveColorPowerID = Shader.PropertyToID("_EmissiveColorPower");
          tk2dSprite.m_shaderEmissiveColorID = Shader.PropertyToID("_EmissiveColor");
          tk2dSprite.m_shaderThresholdID = Shader.PropertyToID("_EmissiveThresholdSensitivity");
        }
        if (this.OverrideMaterialMode != tk2dBaseSprite.SpriteMaterialOverrideMode.NONE && (UnityEngine.Object) this.renderer.sharedMaterial != (UnityEngine.Object) null)
        {
          if (this.OverrideMaterialMode == tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE)
          {
            Material materialInst = this.collectionInst.spriteDefinitions[this.spriteId].materialInst;
            Material sharedMaterial = this.renderer.sharedMaterial;
            if (!((UnityEngine.Object) sharedMaterial != (UnityEngine.Object) materialInst))
              return;
            sharedMaterial.mainTexture = materialInst.mainTexture;
            if (!this.ApplyEmissivePropertyBlock)
              return;
            this.CopyPropertyBlock(materialInst, sharedMaterial);
            return;
          }
          if (this.OverrideMaterialMode == tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_COMPLEX)
            return;
        }
        if (!((UnityEngine.Object) this.renderer.sharedMaterial != (UnityEngine.Object) this.collectionInst.spriteDefinitions[this.spriteId].materialInst))
          return;
        this.renderer.material = this.collectionInst.spriteDefinitions[this.spriteId].materialInst;
      }

      protected override int GetCurrentVertexCount()
      {
        return this.meshVertices == null ? 0 : this.meshVertices.Length;
      }

      public override void ForceBuild()
      {
        if (!(bool) (UnityEngine.Object) this)
          return;
        base.ForceBuild();
        this.GetComponent<MeshFilter>().mesh = this.mesh;
      }

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

}
