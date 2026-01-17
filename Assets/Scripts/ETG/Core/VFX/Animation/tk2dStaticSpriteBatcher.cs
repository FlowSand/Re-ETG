// Decompiled with JetBrains decompiler
// Type: tk2dStaticSpriteBatcher
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using tk2dRuntime;
using UnityEngine;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    [RequireComponent(typeof (MeshFilter))]
    [ExecuteInEditMode]
    [AddComponentMenu("2D Toolkit/Sprite/tk2dStaticSpriteBatcher")]
    [RequireComponent(typeof (MeshRenderer))]
    public class tk2dStaticSpriteBatcher : MonoBehaviour, ISpriteCollectionForceBuild
    {
      public static int CURRENT_VERSION = 3;
      public int version;
      public tk2dBatchedSprite[] batchedSprites;
      public tk2dTextMeshData[] allTextMeshData;
      public tk2dSpriteCollectionData spriteCollection;
      [SerializeField]
      private tk2dStaticSpriteBatcher.Flags flags = tk2dStaticSpriteBatcher.Flags.GenerateCollider;
      private Mesh mesh;
      private Mesh colliderMesh;
      [SerializeField]
      private Vector3 _scale = new Vector3(1f, 1f, 1f);

      public bool CheckFlag(tk2dStaticSpriteBatcher.Flags mask)
      {
        return (this.flags & mask) != tk2dStaticSpriteBatcher.Flags.None;
      }

      public void SetFlag(tk2dStaticSpriteBatcher.Flags mask, bool value)
      {
        if (this.CheckFlag(mask) == value)
          return;
        if (value)
          this.flags |= mask;
        else
          this.flags &= ~mask;
        this.Build();
      }

      private void Awake() => this.Build();

      private bool UpgradeData()
      {
        if (this.version == tk2dStaticSpriteBatcher.CURRENT_VERSION)
          return false;
        if (this._scale == Vector3.zero)
          this._scale = Vector3.one;
        if (this.version < 2 && this.batchedSprites != null)
        {
          foreach (tk2dBatchedSprite batchedSprite in this.batchedSprites)
            batchedSprite.parentId = -1;
        }
        if (this.version < 3)
        {
          if (this.batchedSprites != null)
          {
            foreach (tk2dBatchedSprite batchedSprite in this.batchedSprites)
            {
              if (batchedSprite.spriteId == -1)
              {
                batchedSprite.type = tk2dBatchedSprite.Type.EmptyGameObject;
              }
              else
              {
                batchedSprite.type = tk2dBatchedSprite.Type.Sprite;
                if ((UnityEngine.Object) batchedSprite.spriteCollection == (UnityEngine.Object) null)
                  batchedSprite.spriteCollection = this.spriteCollection;
              }
            }
            this.UpdateMatrices();
          }
          this.spriteCollection = (tk2dSpriteCollectionData) null;
        }
        this.version = tk2dStaticSpriteBatcher.CURRENT_VERSION;
        return true;
      }

      protected void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.mesh)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.mesh);
        if (!(bool) (UnityEngine.Object) this.colliderMesh)
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.colliderMesh);
      }

      public void UpdateMatrices()
      {
        bool flag = false;
        foreach (tk2dBatchedSprite batchedSprite in this.batchedSprites)
        {
          if (batchedSprite.parentId != -1)
          {
            flag = true;
            break;
          }
        }
        if (flag)
        {
          Matrix4x4 matrix4x4 = new Matrix4x4();
          List<tk2dBatchedSprite> tk2dBatchedSpriteList = new List<tk2dBatchedSprite>((IEnumerable<tk2dBatchedSprite>) this.batchedSprites);
          tk2dBatchedSpriteList.Sort((Comparison<tk2dBatchedSprite>) ((a, b) => a.parentId.CompareTo(b.parentId)));
          foreach (tk2dBatchedSprite tk2dBatchedSprite in tk2dBatchedSpriteList)
          {
            matrix4x4.SetTRS(tk2dBatchedSprite.position, tk2dBatchedSprite.rotation, tk2dBatchedSprite.localScale);
            tk2dBatchedSprite.relativeMatrix = (tk2dBatchedSprite.parentId != -1 ? this.batchedSprites[tk2dBatchedSprite.parentId].relativeMatrix : Matrix4x4.identity) * matrix4x4;
          }
        }
        else
        {
          foreach (tk2dBatchedSprite batchedSprite in this.batchedSprites)
            batchedSprite.relativeMatrix.SetTRS(batchedSprite.position, batchedSprite.rotation, batchedSprite.localScale);
        }
      }

      public void Build()
      {
        this.UpgradeData();
        if ((UnityEngine.Object) this.mesh == (UnityEngine.Object) null)
        {
          this.mesh = new Mesh();
          this.mesh.hideFlags = HideFlags.DontSave;
          this.GetComponent<MeshFilter>().mesh = this.mesh;
        }
        else
          this.mesh.Clear();
        if ((bool) (UnityEngine.Object) this.colliderMesh)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) this.colliderMesh);
          this.colliderMesh = (Mesh) null;
        }
        if (this.batchedSprites == null || this.batchedSprites.Length == 0)
          return;
        this.SortBatchedSprites();
        this.BuildRenderMesh();
        this.BuildPhysicsMesh();
      }

      private void SortBatchedSprites()
      {
        List<tk2dBatchedSprite> collection1 = new List<tk2dBatchedSprite>();
        List<tk2dBatchedSprite> collection2 = new List<tk2dBatchedSprite>();
        List<tk2dBatchedSprite> collection3 = new List<tk2dBatchedSprite>();
        foreach (tk2dBatchedSprite batchedSprite in this.batchedSprites)
        {
          if (!batchedSprite.IsDrawn)
          {
            collection3.Add(batchedSprite);
          }
          else
          {
            Material material = this.GetMaterial(batchedSprite);
            if ((UnityEngine.Object) material != (UnityEngine.Object) null)
            {
              if (material.renderQueue == 2000)
                collection1.Add(batchedSprite);
              else
                collection2.Add(batchedSprite);
            }
            else
              collection1.Add(batchedSprite);
          }
        }
        List<tk2dBatchedSprite> tk2dBatchedSpriteList = new List<tk2dBatchedSprite>(collection1.Count + collection2.Count + collection3.Count);
        tk2dBatchedSpriteList.AddRange((IEnumerable<tk2dBatchedSprite>) collection1);
        tk2dBatchedSpriteList.AddRange((IEnumerable<tk2dBatchedSprite>) collection2);
        tk2dBatchedSpriteList.AddRange((IEnumerable<tk2dBatchedSprite>) collection3);
        Dictionary<tk2dBatchedSprite, int> dictionary = new Dictionary<tk2dBatchedSprite, int>();
        int num = 0;
        foreach (tk2dBatchedSprite key in tk2dBatchedSpriteList)
          dictionary[key] = num++;
        foreach (tk2dBatchedSprite tk2dBatchedSprite in tk2dBatchedSpriteList)
        {
          if (tk2dBatchedSprite.parentId != -1)
            tk2dBatchedSprite.parentId = dictionary[this.batchedSprites[tk2dBatchedSprite.parentId]];
        }
        this.batchedSprites = tk2dBatchedSpriteList.ToArray();
      }

      private Material GetMaterial(tk2dBatchedSprite bs)
      {
        switch (bs.type)
        {
          case tk2dBatchedSprite.Type.EmptyGameObject:
            return (Material) null;
          case tk2dBatchedSprite.Type.TextMesh:
            return this.allTextMeshData[bs.xRefId].font.materialInst;
          default:
            return bs.GetSpriteDefinition().materialInst;
        }
      }

      private void BuildRenderMesh()
      {
        List<Material> materialList = new List<Material>();
        List<List<int>> intListList = new List<List<int>>();
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        bool flag4 = this.CheckFlag(tk2dStaticSpriteBatcher.Flags.FlattenDepth);
        foreach (tk2dBatchedSprite batchedSprite in this.batchedSprites)
        {
          tk2dSpriteDefinition spriteDefinition = batchedSprite.GetSpriteDefinition();
          if (spriteDefinition != null)
          {
            flag1 = ((flag1 ? 1 : 0) | (spriteDefinition.normals == null ? 0 : (spriteDefinition.normals.Length > 0 ? 1 : 0))) != 0;
            flag2 = ((flag2 ? 1 : 0) | (spriteDefinition.tangents == null ? 0 : (spriteDefinition.tangents.Length > 0 ? 1 : 0))) != 0;
          }
          if (batchedSprite.type == tk2dBatchedSprite.Type.TextMesh)
          {
            tk2dTextMeshData tk2dTextMeshData = this.allTextMeshData[batchedSprite.xRefId];
            if ((UnityEngine.Object) tk2dTextMeshData.font != (UnityEngine.Object) null && tk2dTextMeshData.font.inst.textureGradients)
              flag3 = true;
          }
        }
        List<int> intList1 = new List<int>();
        List<int> intList2 = new List<int>();
        int length1 = 0;
        foreach (tk2dBatchedSprite batchedSprite in this.batchedSprites)
        {
          if (batchedSprite.IsDrawn)
          {
            tk2dSpriteDefinition spriteDefinition = batchedSprite.GetSpriteDefinition();
            int numVertices = 0;
            int numIndices = 0;
            switch (batchedSprite.type)
            {
              case tk2dBatchedSprite.Type.Sprite:
                if (spriteDefinition != null)
                {
                  tk2dSpriteGeomGen.GetSpriteGeomDesc(out numVertices, out numIndices, spriteDefinition);
                  break;
                }
                break;
              case tk2dBatchedSprite.Type.TiledSprite:
                if (spriteDefinition != null)
                {
                  tk2dSpriteGeomGen.GetTiledSpriteGeomDesc(out numVertices, out numIndices, spriteDefinition, batchedSprite.Dimensions);
                  break;
                }
                break;
              case tk2dBatchedSprite.Type.SlicedSprite:
                if (spriteDefinition != null)
                {
                  tk2dSpriteGeomGen.GetSlicedSpriteGeomDesc(out numVertices, out numIndices, spriteDefinition, batchedSprite.CheckFlag(tk2dBatchedSprite.Flags.SlicedSprite_BorderOnly), false, Vector2.zero, Vector2.zero, Vector2.zero, 0.0f);
                  break;
                }
                break;
              case tk2dBatchedSprite.Type.ClippedSprite:
                if (spriteDefinition != null)
                {
                  tk2dSpriteGeomGen.GetClippedSpriteGeomDesc(out numVertices, out numIndices, spriteDefinition);
                  break;
                }
                break;
              case tk2dBatchedSprite.Type.TextMesh:
                tk2dTextMeshData textMeshData = this.allTextMeshData[batchedSprite.xRefId];
                tk2dTextGeomGen.GetTextMeshGeomDesc(out numVertices, out numIndices, tk2dTextGeomGen.Data(textMeshData, textMeshData.font.inst, batchedSprite.FormattedText));
                break;
            }
            length1 += numVertices;
            intList1.Add(numVertices);
            intList2.Add(numIndices);
          }
          else
            break;
        }
        Vector3[] vector3Array1 = !flag1 ? (Vector3[]) null : new Vector3[length1];
        Vector4[] vector4Array = !flag2 ? (Vector4[]) null : new Vector4[length1];
        Vector3[] vector3Array2 = new Vector3[length1];
        Color32[] color32Array1 = new Color32[length1];
        Vector2[] vector2Array1 = new Vector2[length1];
        Vector2[] vector2Array2 = !flag3 ? (Vector2[]) null : new Vector2[length1];
        int vStart = 0;
        Material material1 = (Material) null;
        List<int> intList3 = (List<int>) null;
        Matrix4x4 identity = Matrix4x4.identity with
        {
          m00 = this._scale.x,
          m11 = this._scale.y,
          m22 = this._scale.z
        };
        int index1 = 0;
        foreach (tk2dBatchedSprite batchedSprite in this.batchedSprites)
        {
          if (batchedSprite.IsDrawn)
          {
            if (batchedSprite.type == tk2dBatchedSprite.Type.EmptyGameObject)
            {
              ++index1;
            }
            else
            {
              tk2dSpriteDefinition spriteDefinition = batchedSprite.GetSpriteDefinition();
              int numVertices = intList1[index1];
              int length2 = intList2[index1];
              Material material2 = this.GetMaterial(batchedSprite);
              if ((UnityEngine.Object) material2 != (UnityEngine.Object) material1)
              {
                if ((UnityEngine.Object) material1 != (UnityEngine.Object) null)
                {
                  materialList.Add(material1);
                  intListList.Add(intList3);
                }
                material1 = material2;
                intList3 = new List<int>();
              }
              Vector3[] pos = new Vector3[numVertices];
              Vector2[] uv = new Vector2[numVertices];
              Vector2[] uv2 = !flag3 ? (Vector2[]) null : new Vector2[numVertices];
              Color32[] color32Array2 = new Color32[numVertices];
              Vector3[] norm = !flag1 ? (Vector3[]) null : new Vector3[numVertices];
              Vector4[] tang = !flag2 ? (Vector4[]) null : new Vector4[numVertices];
              int[] numArray = new int[length2];
              Vector3 boundsCenter = Vector3.zero;
              Vector3 boundsExtents = Vector3.zero;
              switch (batchedSprite.type)
              {
                case tk2dBatchedSprite.Type.Sprite:
                  if (spriteDefinition != null)
                  {
                    tk2dSpriteGeomGen.SetSpriteGeom(pos, uv, norm, tang, 0, spriteDefinition, Vector3.one);
                    tk2dSpriteGeomGen.SetSpriteIndices(numArray, 0, vStart, spriteDefinition);
                    break;
                  }
                  break;
                case tk2dBatchedSprite.Type.TiledSprite:
                  if (spriteDefinition != null)
                  {
                    tk2dSpriteGeomGen.SetTiledSpriteGeom(pos, uv, 0, out boundsCenter, out boundsExtents, spriteDefinition, Vector3.one, batchedSprite.Dimensions, batchedSprite.anchor, batchedSprite.BoxColliderOffsetZ, batchedSprite.BoxColliderExtentZ);
                    tk2dSpriteGeomGen.SetTiledSpriteIndices(numArray, 0, vStart, spriteDefinition, batchedSprite.Dimensions);
                    break;
                  }
                  break;
                case tk2dBatchedSprite.Type.SlicedSprite:
                  if (spriteDefinition != null)
                  {
                    tk2dSpriteGeomGen.SetSlicedSpriteGeom(pos, uv, 0, out boundsCenter, out boundsExtents, spriteDefinition, batchedSprite.CheckFlag(tk2dBatchedSprite.Flags.SlicedSprite_BorderOnly), Vector3.one, batchedSprite.Dimensions, batchedSprite.SlicedSpriteBorderBottomLeft, batchedSprite.SlicedSpriteBorderTopRight, 0.0f, batchedSprite.anchor, batchedSprite.BoxColliderOffsetZ, batchedSprite.BoxColliderExtentZ, Vector2.zero, false);
                    tk2dSpriteGeomGen.SetSlicedSpriteIndices(numArray, 0, vStart, spriteDefinition, batchedSprite.CheckFlag(tk2dBatchedSprite.Flags.SlicedSprite_BorderOnly), false, Vector2.zero, Vector2.zero, Vector2.zero, 0.0f);
                    break;
                  }
                  break;
                case tk2dBatchedSprite.Type.ClippedSprite:
                  if (spriteDefinition != null)
                  {
                    tk2dSpriteGeomGen.SetClippedSpriteGeom(pos, uv, 0, out boundsCenter, out boundsExtents, spriteDefinition, Vector3.one, batchedSprite.ClippedSpriteRegionBottomLeft, batchedSprite.ClippedSpriteRegionTopRight, batchedSprite.BoxColliderOffsetZ, batchedSprite.BoxColliderExtentZ);
                    tk2dSpriteGeomGen.SetClippedSpriteIndices(numArray, 0, vStart, spriteDefinition);
                    break;
                  }
                  break;
                case tk2dBatchedSprite.Type.TextMesh:
                  tk2dTextMeshData textMeshData = this.allTextMeshData[batchedSprite.xRefId];
                  tk2dTextGeomGen.GeomData geomData = tk2dTextGeomGen.Data(textMeshData, textMeshData.font.inst, batchedSprite.FormattedText);
                  int target = tk2dTextGeomGen.SetTextMeshGeom(pos, uv, uv2, color32Array2, 0, geomData, int.MaxValue);
                  if (!geomData.fontInst.isPacked)
                  {
                    Color32 color = (Color32) textMeshData.color;
                    Color32 color32_1 = (Color32) (!textMeshData.useGradient ? textMeshData.color : textMeshData.color2);
                    for (int index2 = 0; index2 < color32Array2.Length; ++index2)
                    {
                      Color32 color32_2 = index2 % 4 >= 2 ? color32_1 : color;
                      byte r = (byte) ((int) color32Array2[index2].r * (int) color32_2.r / (int) byte.MaxValue);
                      byte g = (byte) ((int) color32Array2[index2].g * (int) color32_2.g / (int) byte.MaxValue);
                      byte b = (byte) ((int) color32Array2[index2].b * (int) color32_2.b / (int) byte.MaxValue);
                      byte a = (byte) ((int) color32Array2[index2].a * (int) color32_2.a / (int) byte.MaxValue);
                      if (geomData.fontInst.premultipliedAlpha)
                      {
                        r = (byte) ((int) r * (int) a / (int) byte.MaxValue);
                        g = (byte) ((int) g * (int) a / (int) byte.MaxValue);
                        b = (byte) ((int) b * (int) a / (int) byte.MaxValue);
                      }
                      color32Array2[index2] = new Color32(r, g, b, a);
                    }
                  }
                  tk2dTextGeomGen.SetTextMeshIndices(numArray, 0, vStart, geomData, target);
                  break;
              }
              batchedSprite.CachedBoundsCenter = boundsCenter;
              batchedSprite.CachedBoundsExtents = boundsExtents;
              if (numVertices > 0 && batchedSprite.type != tk2dBatchedSprite.Type.TextMesh)
              {
                bool premulAlpha = (UnityEngine.Object) batchedSprite.spriteCollection != (UnityEngine.Object) null && batchedSprite.spriteCollection.premultipliedAlpha;
                tk2dSpriteGeomGen.SetSpriteColors(color32Array2, 0, numVertices, batchedSprite.color, premulAlpha);
              }
              Matrix4x4 matrix4x4 = identity * batchedSprite.relativeMatrix;
              for (int index3 = 0; index3 < numVertices; ++index3)
              {
                Vector3 point = Vector3.Scale(pos[index3], batchedSprite.baseScale);
                Vector3 vector3_1 = matrix4x4.MultiplyPoint(point);
                if (flag4)
                  vector3_1.z = 0.0f;
                vector3Array2[vStart + index3] = vector3_1;
                vector2Array1[vStart + index3] = uv[index3];
                if (flag3)
                  vector2Array2[vStart + index3] = uv2[index3];
                color32Array1[vStart + index3] = color32Array2[index3];
                if (flag1)
                  vector3Array1[vStart + index3] = batchedSprite.rotation * norm[index3];
                if (flag2)
                {
                  Vector3 vector3_2 = new Vector3(tang[index3].x, tang[index3].y, tang[index3].z);
                  vector3_2 = batchedSprite.rotation * vector3_2;
                  vector4Array[vStart + index3] = new Vector4(vector3_2.x, vector3_2.y, vector3_2.z, tang[index3].w);
                }
              }
              intList3.AddRange((IEnumerable<int>) numArray);
              vStart += numVertices;
              ++index1;
            }
          }
          else
            break;
        }
        if (intList3 != null)
        {
          materialList.Add(material1);
          intListList.Add(intList3);
        }
        if ((bool) (UnityEngine.Object) this.mesh)
        {
          this.mesh.vertices = vector3Array2;
          this.mesh.uv = vector2Array1;
          if (flag3)
            this.mesh.uv2 = vector2Array2;
          this.mesh.colors32 = color32Array1;
          if (flag1)
            this.mesh.normals = vector3Array1;
          if (flag2)
            this.mesh.tangents = vector4Array;
          this.mesh.subMeshCount = intListList.Count;
          for (int index4 = 0; index4 < intListList.Count; ++index4)
            this.mesh.SetTriangles(intListList[index4].ToArray(), index4);
          this.mesh.RecalculateBounds();
        }
        this.GetComponent<Renderer>().sharedMaterials = materialList.ToArray();
      }

      private void BuildPhysicsMesh()
      {
        MeshCollider component = this.GetComponent<MeshCollider>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          if ((UnityEngine.Object) this.GetComponent<Collider>() != (UnityEngine.Object) component)
            return;
          if (!this.CheckFlag(tk2dStaticSpriteBatcher.Flags.GenerateCollider))
            UnityEngine.Object.Destroy((UnityEngine.Object) component);
        }
        EdgeCollider2D[] components = this.GetComponents<EdgeCollider2D>();
        if (!this.CheckFlag(tk2dStaticSpriteBatcher.Flags.GenerateCollider))
        {
          foreach (UnityEngine.Object @object in components)
            UnityEngine.Object.Destroy(@object);
        }
        if (!this.CheckFlag(tk2dStaticSpriteBatcher.Flags.GenerateCollider))
          return;
        bool flattenDepth = this.CheckFlag(tk2dStaticSpriteBatcher.Flags.FlattenDepth);
        int numIndices = 0;
        int numVertices = 0;
        int numEdgeColliders = 0;
        bool flag1 = true;
        foreach (tk2dBatchedSprite batchedSprite in this.batchedSprites)
        {
          if (batchedSprite.IsDrawn)
          {
            tk2dSpriteDefinition spriteDefinition = batchedSprite.GetSpriteDefinition();
            bool flag2 = false;
            bool flag3 = false;
            switch (batchedSprite.type)
            {
              case tk2dBatchedSprite.Type.Sprite:
                if (spriteDefinition != null && spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
                  flag2 = true;
                if (spriteDefinition != null && spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Box)
                {
                  flag3 = true;
                  break;
                }
                break;
              case tk2dBatchedSprite.Type.TiledSprite:
              case tk2dBatchedSprite.Type.SlicedSprite:
              case tk2dBatchedSprite.Type.ClippedSprite:
                flag3 = batchedSprite.CheckFlag(tk2dBatchedSprite.Flags.Sprite_CreateBoxCollider);
                break;
            }
            if (!flag2 && flag3)
            {
              numIndices += 36;
              numVertices += 8;
              ++numEdgeColliders;
            }
            if (spriteDefinition.physicsEngine == tk2dSpriteDefinition.PhysicsEngine.Physics2D)
              flag1 = false;
          }
          else
            break;
        }
        if (flag1 && numIndices == 0 || !flag1 && numEdgeColliders == 0)
        {
          if ((UnityEngine.Object) this.colliderMesh != (UnityEngine.Object) null)
          {
            UnityEngine.Object.Destroy((UnityEngine.Object) this.colliderMesh);
            this.colliderMesh = (Mesh) null;
          }
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) component);
          foreach (UnityEngine.Object @object in components)
            UnityEngine.Object.Destroy(@object);
        }
        else
        {
          if (flag1)
          {
            foreach (UnityEngine.Object @object in components)
              UnityEngine.Object.Destroy(@object);
          }
          else
          {
            if ((UnityEngine.Object) this.colliderMesh != (UnityEngine.Object) null)
              UnityEngine.Object.Destroy((UnityEngine.Object) this.colliderMesh);
            if ((UnityEngine.Object) component != (UnityEngine.Object) null)
              UnityEngine.Object.Destroy((UnityEngine.Object) component);
          }
          if (flag1)
            this.BuildPhysicsMesh3D(component, flattenDepth, numVertices, numIndices);
          else
            this.BuildPhysicsMesh2D(components, numEdgeColliders);
        }
      }

      private void BuildPhysicsMesh2D(EdgeCollider2D[] edgeColliders, int numEdgeColliders)
      {
        for (int index = numEdgeColliders; index < edgeColliders.Length; ++index)
          UnityEngine.Object.Destroy((UnityEngine.Object) edgeColliders[index]);
        Vector2[] vector2Array = new Vector2[5];
        if (numEdgeColliders > edgeColliders.Length)
        {
          EdgeCollider2D[] edgeCollider2DArray = new EdgeCollider2D[numEdgeColliders];
          int num = Mathf.Min(numEdgeColliders, edgeColliders.Length);
          for (int index = 0; index < num; ++index)
            edgeCollider2DArray[index] = edgeColliders[index];
          for (int index = num; index < numEdgeColliders; ++index)
            edgeCollider2DArray[index] = this.gameObject.AddComponent<EdgeCollider2D>();
          edgeColliders = edgeCollider2DArray;
        }
        Matrix4x4 identity = Matrix4x4.identity with
        {
          m00 = this._scale.x,
          m11 = this._scale.y,
          m22 = this._scale.z
        };
        int index1 = 0;
        foreach (tk2dBatchedSprite batchedSprite in this.batchedSprites)
        {
          if (!batchedSprite.IsDrawn)
            break;
          tk2dSpriteDefinition spriteDefinition = batchedSprite.GetSpriteDefinition();
          bool flag1 = false;
          bool flag2 = false;
          Vector3 vector3_1 = Vector3.zero;
          Vector3 vector3_2 = Vector3.zero;
          switch (batchedSprite.type)
          {
            case tk2dBatchedSprite.Type.Sprite:
              if (spriteDefinition != null && spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
                flag1 = true;
              if (spriteDefinition != null && spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Box)
              {
                flag2 = true;
                vector3_1 = spriteDefinition.colliderVertices[0];
                vector3_2 = spriteDefinition.colliderVertices[1];
                break;
              }
              break;
            case tk2dBatchedSprite.Type.TiledSprite:
            case tk2dBatchedSprite.Type.SlicedSprite:
            case tk2dBatchedSprite.Type.ClippedSprite:
              flag2 = batchedSprite.CheckFlag(tk2dBatchedSprite.Flags.Sprite_CreateBoxCollider);
              if (flag2)
              {
                vector3_1 = batchedSprite.CachedBoundsCenter;
                vector3_2 = batchedSprite.CachedBoundsExtents;
                break;
              }
              break;
          }
          Matrix4x4 matrix4x4 = identity * batchedSprite.relativeMatrix;
          if (!flag1 && flag2)
          {
            Vector3 vector3_3 = vector3_1 - vector3_2;
            Vector3 vector3_4 = vector3_1 + vector3_2;
            vector2Array[0] = (Vector2) matrix4x4.MultiplyPoint((Vector3) new Vector2(vector3_3.x, vector3_3.y));
            vector2Array[1] = (Vector2) matrix4x4.MultiplyPoint((Vector3) new Vector2(vector3_4.x, vector3_3.y));
            vector2Array[2] = (Vector2) matrix4x4.MultiplyPoint((Vector3) new Vector2(vector3_4.x, vector3_4.y));
            vector2Array[3] = (Vector2) matrix4x4.MultiplyPoint((Vector3) new Vector2(vector3_3.x, vector3_4.y));
            vector2Array[4] = vector2Array[0];
            edgeColliders[index1].points = vector2Array;
            ++index1;
          }
        }
      }

      private void BuildPhysicsMesh3D(
        MeshCollider meshCollider,
        bool flattenDepth,
        int numVertices,
        int numIndices)
      {
        if ((UnityEngine.Object) meshCollider == (UnityEngine.Object) null)
          meshCollider = this.gameObject.AddComponent<MeshCollider>();
        if ((UnityEngine.Object) this.colliderMesh == (UnityEngine.Object) null)
        {
          this.colliderMesh = new Mesh();
          this.colliderMesh.hideFlags = HideFlags.DontSave;
        }
        else
          this.colliderMesh.Clear();
        int num = 0;
        Vector3[] pos = new Vector3[numVertices];
        int indicesOffset = 0;
        int[] indices = new int[numIndices];
        Matrix4x4 identity = Matrix4x4.identity with
        {
          m00 = this._scale.x,
          m11 = this._scale.y,
          m22 = this._scale.z
        };
        foreach (tk2dBatchedSprite batchedSprite in this.batchedSprites)
        {
          if (batchedSprite.IsDrawn)
          {
            tk2dSpriteDefinition spriteDefinition = batchedSprite.GetSpriteDefinition();
            bool flag1 = false;
            bool flag2 = false;
            Vector3 origin = Vector3.zero;
            Vector3 extents = Vector3.zero;
            switch (batchedSprite.type)
            {
              case tk2dBatchedSprite.Type.Sprite:
                if (spriteDefinition != null && spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
                  flag1 = true;
                if (spriteDefinition != null && spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Box)
                {
                  flag2 = true;
                  origin = spriteDefinition.colliderVertices[0];
                  extents = spriteDefinition.colliderVertices[1];
                  break;
                }
                break;
              case tk2dBatchedSprite.Type.TiledSprite:
              case tk2dBatchedSprite.Type.SlicedSprite:
              case tk2dBatchedSprite.Type.ClippedSprite:
                flag2 = batchedSprite.CheckFlag(tk2dBatchedSprite.Flags.Sprite_CreateBoxCollider);
                if (flag2)
                {
                  origin = batchedSprite.CachedBoundsCenter;
                  extents = batchedSprite.CachedBoundsExtents;
                  break;
                }
                break;
            }
            Matrix4x4 mat = identity * batchedSprite.relativeMatrix;
            if (flattenDepth)
              mat.m23 = 0.0f;
            if (!flag1 && flag2)
            {
              tk2dSpriteGeomGen.SetBoxMeshData(pos, indices, num, indicesOffset, num, origin, extents, mat, batchedSprite.baseScale);
              num += 8;
              indicesOffset += 36;
            }
          }
          else
            break;
        }
        this.colliderMesh.vertices = pos;
        this.colliderMesh.triangles = indices;
        meshCollider.sharedMesh = this.colliderMesh;
      }

      public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
      {
        return (UnityEngine.Object) this.spriteCollection == (UnityEngine.Object) spriteCollection;
      }

      public void ForceBuild() => this.Build();

      public enum Flags
      {
        None = 0,
        GenerateCollider = 1,
        FlattenDepth = 2,
        SortToCamera = 4,
      }
    }

}
