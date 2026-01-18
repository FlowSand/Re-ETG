// Decompiled with JetBrains decompiler
// Type: SpriteShadow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class SpriteShadow
  {
    public bool hasChanged;
    public List<Vector3> vertices;
    public tk2dSprite shadowedSprite;
    private SpriteShadowCaster m_caster;
    private Transform m_casterTransform;
    private tk2dSpriteAnimator spriteAnimator;
    private Transform spriteTransform;
    private Vector3 cachedPosition;
    private tk2dSpriteAnimationClip cachedClip;
    private int cachedFrame;
    private float shadowDepth;
    private GameObject m_shadowObject;
    private MeshFilter m_shadowFilter;
    private MeshRenderer m_shadowRenderer;
    private Mesh m_shadowMesh;
    private Mesh m_spriteMesh;

    public SpriteShadow(tk2dSprite sprite, SpriteShadowCaster caster)
    {
      this.shadowedSprite = sprite;
      this.m_caster = caster;
      this.m_casterTransform = caster.transform;
      this.shadowDepth = caster.shadowDepth;
      this.vertices = new List<Vector3>();
      this.m_spriteMesh = sprite.GetComponent<MeshFilter>().sharedMesh;
      this.spriteTransform = sprite.transform;
      this.spriteAnimator = sprite.GetComponent<tk2dSpriteAnimator>();
      this.cachedPosition = this.spriteTransform.position;
      if ((Object) this.spriteAnimator != (Object) null && this.spriteAnimator.CurrentClip != null)
      {
        this.cachedClip = this.spriteAnimator.CurrentClip;
        this.cachedFrame = this.spriteAnimator.CurrentFrame;
      }
      this.m_shadowObject = new GameObject("Shadow");
      this.m_shadowFilter = this.m_shadowObject.AddComponent<MeshFilter>();
      this.m_shadowRenderer = this.m_shadowObject.AddComponent<MeshRenderer>();
      this.m_shadowRenderer.sharedMaterial = this.m_caster.GetMaterialInstance();
      this.m_shadowRenderer.sharedMaterial.mainTexture = this.shadowedSprite.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
      this.m_shadowMesh = new Mesh();
      this.m_shadowMesh.vertices = new Vector3[10];
      this.m_shadowMesh.triangles = new int[24]
      {
        0,
        3,
        1,
        2,
        3,
        0,
        2,
        5,
        3,
        4,
        5,
        2,
        4,
        7,
        5,
        6,
        7,
        4,
        6,
        9,
        7,
        8,
        9,
        6
      };
      this.m_shadowMesh.uv = new Vector2[10];
      this.UpdateShadow(true);
    }

    public void Destroy() => Object.Destroy((Object) this.m_shadowObject);

    private Vector3 CollapseDepth(Vector3 input) => new Vector3(input.x, input.y, this.shadowDepth);

    public void UpdateShadow(bool force = false)
    {
      if (!force)
      {
        bool flag1 = this.cachedPosition != this.spriteTransform.position;
        bool flag2 = false;
        if ((Object) this.spriteAnimator != (Object) null && this.spriteAnimator.CurrentClip != null)
        {
          if (this.cachedClip != this.spriteAnimator.CurrentClip)
            flag2 = true;
          if (this.cachedFrame != this.spriteAnimator.CurrentFrame)
            flag2 = true;
        }
        if (!flag1 && !flag2)
          return;
      }
      float x = this.shadowedSprite.GetBounds().size.x;
      float y = this.shadowedSprite.GetBounds().size.y;
      Vector3 origin = this.CollapseDepth(this.m_casterTransform.position);
      Vector3 vector3 = this.CollapseDepth(this.spriteTransform.position);
      float magnitude = (vector3 + new Vector3(x / 2f, 0.0f, 0.0f) - origin).magnitude;
      Vector3 direction1 = vector3 - origin;
      Vector3 direction2 = vector3 + new Vector3(x, 0.0f, 0.0f) - origin;
      Ray ray1 = new Ray(origin, direction1);
      Ray ray2 = new Ray(origin, direction2);
      Vector3 point1 = ray1.GetPoint(magnitude);
      Vector3 point2 = ray2.GetPoint(magnitude);
      Vector3 point3 = ray1.GetPoint(magnitude + y * (float) ((double) magnitude / (double) this.m_caster.radius * 4.0));
      Vector3 point4 = ray2.GetPoint(magnitude + y * (float) ((double) magnitude / (double) this.m_caster.radius * 4.0));
      this.vertices.Clear();
      this.vertices.Add(point1);
      this.vertices.Add(point2);
      this.vertices.Add(point1 + (point3 - point1) / 4f);
      this.vertices.Add(point2 + (point4 - point2) / 4f);
      this.vertices.Add((point1 + point3) / 2f);
      this.vertices.Add((point2 + point4) / 2f);
      this.vertices.Add(point1 * 0.25f + point3 * 0.75f);
      this.vertices.Add(point2 * 0.25f + point4 * 0.75f);
      this.vertices.Add(point3);
      this.vertices.Add(point4);
      this.hasChanged = true;
      if ((double) origin.y > (double) vector3.y)
        this.m_shadowMesh.triangles = new int[24]
        {
          0,
          1,
          3,
          2,
          0,
          3,
          2,
          3,
          5,
          4,
          2,
          5,
          4,
          5,
          7,
          6,
          4,
          7,
          6,
          7,
          9,
          8,
          6,
          9
        };
      else
        this.m_shadowMesh.triangles = new int[24]
        {
          0,
          3,
          1,
          2,
          3,
          0,
          2,
          5,
          3,
          4,
          5,
          2,
          4,
          7,
          5,
          6,
          7,
          4,
          6,
          9,
          7,
          8,
          9,
          6
        };
      this.RebuildMesh();
      this.cachedPosition = this.spriteTransform.position;
      if (!((Object) this.spriteAnimator != (Object) null) || this.spriteAnimator.CurrentClip == null)
        return;
      this.cachedClip = this.spriteAnimator.CurrentClip;
      this.cachedFrame = this.spriteAnimator.CurrentFrame;
    }

    private void RebuildMesh()
    {
      this.m_shadowMesh.vertices = this.vertices.ToArray();
      Vector2[] uv = this.m_spriteMesh.uv;
      Vector2[] vector2Array = new Vector2[10];
      vector2Array[0] = uv[0];
      vector2Array[1] = uv[1];
      vector2Array[4] = (uv[0] + uv[2]) / 2f;
      vector2Array[5] = (uv[1] + uv[3]) / 2f;
      vector2Array[2] = (uv[0] + vector2Array[4]) / 2f;
      vector2Array[3] = (uv[1] + vector2Array[5]) / 2f;
      vector2Array[6] = uv[0] + (uv[2] - uv[0]) * 0.75f;
      vector2Array[7] = uv[1] + (uv[3] - uv[1]) * 0.75f;
      vector2Array[8] = uv[2];
      vector2Array[9] = uv[3];
      this.m_shadowMesh.uv = vector2Array;
      this.m_shadowMesh.RecalculateBounds();
      this.m_shadowFilter.sharedMesh = this.m_shadowMesh;
    }
  }

