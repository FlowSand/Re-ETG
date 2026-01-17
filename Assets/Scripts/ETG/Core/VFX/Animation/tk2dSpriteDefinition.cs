// Decompiled with JetBrains decompiler
// Type: tk2dSpriteDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    [Serializable]
    public class tk2dSpriteDefinition
    {
      public string name;
      public Vector3 boundsDataCenter;
      public Vector3 boundsDataExtents;
      public Vector3 untrimmedBoundsDataCenter;
      public Vector3 untrimmedBoundsDataExtents;
      public Vector2 texelSize;
      public Vector3 position0;
      public Vector3 position1;
      public Vector3 position2;
      public Vector3 position3;
      public static Vector3[] defaultNormals;
      public static Vector4[] defaultTangents;
      public Vector2[] uvs;
      private static int[] defaultIndices = new int[6]
      {
        0,
        3,
        1,
        2,
        3,
        0
      };
      public Material material;
      [NonSerialized]
      public Material materialInst;
      public int materialId;
      public bool extractRegion;
      public int regionX;
      public int regionY;
      public int regionW;
      public int regionH;
      public tk2dSpriteDefinition.FlipMode flipped;
      public bool complexGeometry;
      public tk2dSpriteDefinition.PhysicsEngine physicsEngine;
      public tk2dSpriteDefinition.ColliderType colliderType;
      public CollisionLayer collisionLayer;
      [SerializeField]
      public TilesetIndexMetadata metadata;
      public Vector3[] colliderVertices;
      public bool colliderConvex;
      public bool colliderSmoothSphereCollisions;
      private bool? m_cachedIsTileSquare;

      public Vector3[] ConstructExpensivePositions()
      {
        return new Vector3[4]
        {
          this.position0,
          this.position1,
          this.position2,
          this.position3
        };
      }

      public Vector3[] normals
      {
        get
        {
          if (tk2dSpriteDefinition.defaultNormals == null)
            tk2dSpriteDefinition.defaultNormals = new Vector3[4]
            {
              new Vector3(0.0f, 0.0f, -1f),
              new Vector3(0.0f, 0.0f, -1f),
              new Vector3(0.0f, 0.0f, -1f),
              new Vector3(0.0f, 0.0f, -1f)
            };
          return tk2dSpriteDefinition.defaultNormals;
        }
        set
        {
        }
      }

      public Vector4[] tangents
      {
        get
        {
          if (tk2dSpriteDefinition.defaultTangents == null)
            tk2dSpriteDefinition.defaultTangents = new Vector4[4]
            {
              new Vector4(1f, 0.0f, 0.0f, 1f),
              new Vector4(1f, 0.0f, 0.0f, 1f),
              new Vector4(1f, 0.0f, 0.0f, 1f),
              new Vector4(1f, 0.0f, 0.0f, 1f)
            };
          return tk2dSpriteDefinition.defaultTangents;
        }
        set
        {
        }
      }

      public int[] indices
      {
        get => tk2dSpriteDefinition.defaultIndices;
        set
        {
        }
      }

      public BagelCollider[] GetBagelColliders(tk2dSpriteCollectionData collection, int spriteId)
      {
        return collection.GetBagelColliders(spriteId);
      }

      public tk2dSpriteDefinition.AttachPoint[] GetAttachPoints(
        tk2dSpriteCollectionData collection,
        int spriteId)
      {
        return collection.GetAttachPoints(spriteId);
      }

      public bool Valid => this.name.Length != 0;

      public bool IsTileSquare
      {
        get
        {
          if (!this.m_cachedIsTileSquare.HasValue)
            this.m_cachedIsTileSquare = new bool?(this.CheckIsTileSquare());
          return this.m_cachedIsTileSquare.Value;
        }
      }

      private bool CheckIsTileSquare()
      {
        if (this.colliderVertices == null)
          return false;
        if (this.colliderVertices.Length == 2)
          return Mathf.Approximately(this.colliderVertices[0].x, 0.5f) && Mathf.Approximately(this.colliderVertices[0].y, 0.5f) && Mathf.Approximately(this.colliderVertices[1].x, 0.5f) && Mathf.Approximately(this.colliderVertices[1].y, 0.5f);
        if (this.colliderVertices.Length != 8)
          return false;
        for (int index = 0; index < 8; ++index)
        {
          if (!Mathf.Approximately(this.colliderVertices[index].x, 0.0f) && !Mathf.Approximately(this.colliderVertices[index].x, 1f))
            return false;
        }
        return true;
      }

      public Bounds GetBounds()
      {
        return new Bounds(new Vector3(this.boundsDataCenter.x, this.boundsDataCenter.y, this.boundsDataCenter.z), new Vector3(this.boundsDataExtents.x, this.boundsDataExtents.y, this.boundsDataExtents.z));
      }

      public Bounds GetUntrimmedBounds()
      {
        return new Bounds(new Vector3(this.untrimmedBoundsDataCenter.x, this.untrimmedBoundsDataCenter.y, this.untrimmedBoundsDataCenter.z), new Vector3(this.untrimmedBoundsDataExtents.x, this.untrimmedBoundsDataExtents.y, this.untrimmedBoundsDataExtents.z));
      }

      public enum ColliderType
      {
        Unset,
        None,
        Box,
        Mesh,
      }

      public enum PhysicsEngine
      {
        Physics3D,
        Physics2D,
      }

      public enum FlipMode
      {
        None,
        Tk2d,
        TPackerCW,
      }

      [Serializable]
      public class AttachPoint
      {
        public string name = string.Empty;
        public Vector3 position = Vector3.zero;
        public float angle;

        public void CopyFrom(tk2dSpriteDefinition.AttachPoint src)
        {
          this.name = src.name;
          this.position = src.position;
          this.angle = src.angle;
        }

        public bool CompareTo(tk2dSpriteDefinition.AttachPoint src)
        {
          return this.name == src.name && src.position == this.position && (double) src.angle == (double) this.angle;
        }
      }
    }

}
