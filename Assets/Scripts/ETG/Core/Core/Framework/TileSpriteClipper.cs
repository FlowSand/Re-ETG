// Decompiled with JetBrains decompiler
// Type: TileSpriteClipper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class TileSpriteClipper : BraveBehaviour
  {
    public bool doOptimize = true;
    public bool updateEveryFrame;
    public TileSpriteClipper.ClipMode clipMode;
    [ShowInInspectorIf("clipMode", 3, false)]
    public float clipY;
    private Vector3[] m_vertices;
    private int[] m_triangles;
    private Vector2[] m_uvs;

    private void Start() => this.DoClip();

    private void OnEnable() => this.Start();

    private void OnDisable()
    {
      if (!(bool) (Object) this.sprite)
        return;
      this.sprite.ForceBuild();
    }

    private void LateUpdate()
    {
      if (this.updateEveryFrame)
        this.DoClip();
      if (!(bool) (Object) this.sprite || (bool) (Object) this.sprite.attachParent)
        return;
      this.sprite.UpdateZDepth();
    }

    protected override void OnDestroy() => base.OnDestroy();

    public void DoClip()
    {
      if (this.clipMode == TileSpriteClipper.ClipMode.ClipBelowY)
        this.ClipToY();
      else
        this.ClipToTileBounds();
    }

    private void ClipToY()
    {
      if (BraveUtility.isLoadingLevel || GameManager.Instance.IsLoadingLevel || !(bool) (Object) this.sprite)
        return;
      Transform transform = this.transform;
      Bounds bounds = this.sprite.GetBounds();
      Vector2 b1 = bounds.min.XY();
      Vector2 b2 = bounds.max.XY();
      Vector3 vector3_1 = new Vector3(Mathf.Sign(transform.lossyScale.x), Mathf.Sign(transform.lossyScale.y), Mathf.Sign(transform.lossyScale.z));
      Vector2 lhs1 = Vector2.Scale(vector3_1.XY(), b1);
      Vector2 rhs = Vector2.Scale(vector3_1.XY(), b2);
      Vector2 vector2_1 = transform.position.XY() + Vector2.Min(lhs1, rhs);
      Vector2 vector2_2 = transform.position.XY() + Vector2.Max(lhs1, rhs);
      tk2dSpriteDefinition spriteDefinition = this.sprite.Collection.spriteDefinitions[this.sprite.spriteId];
      Vector2 lhs2 = new Vector2(float.MaxValue, float.MaxValue);
      Vector2 lhs3 = new Vector2(float.MinValue, float.MinValue);
      for (int index = 0; index < spriteDefinition.uvs.Length; ++index)
      {
        lhs2 = Vector2.Min(lhs2, spriteDefinition.uvs[index]);
        lhs3 = Vector2.Max(lhs3, spriteDefinition.uvs[index]);
      }
      if (this.m_vertices == null || this.m_vertices.Length != 4)
        this.m_vertices = new Vector3[4];
      if (this.m_triangles == null || this.m_triangles.Length != 6)
        this.m_triangles = new int[6];
      if (this.m_uvs == null || this.m_uvs.Length != 4)
        this.m_uvs = new Vector2[4];
      float x1 = vector2_1.x - transform.position.x;
      float x2 = vector2_2.x - transform.position.x;
      float y1 = Mathf.Max(vector2_1.y, Mathf.Min(this.clipY, vector2_2.y)) - transform.position.y;
      float y2 = vector2_2.y - transform.position.y;
      Vector3 b3 = new Vector3(x1, y1, 0.0f);
      Vector3 b4 = new Vector3(x2, y1, 0.0f);
      Vector3 b5 = new Vector3(x1, y2, 0.0f);
      Vector3 b6 = new Vector3(x2, y2, 0.0f);
      Vector3 vector3_2 = Vector3.Scale(vector3_1, b3);
      Vector3 vector3_3 = Vector3.Scale(vector3_1, b4);
      Vector3 vector3_4 = Vector3.Scale(vector3_1, b5);
      Vector3 vector3_5 = Vector3.Scale(vector3_1, b6);
      this.m_vertices[0] = vector3_2;
      this.m_vertices[1] = vector3_3;
      this.m_vertices[2] = vector3_4;
      this.m_vertices[3] = vector3_5;
      if (this.sprite.ShouldDoTilt)
      {
        for (int index = 0; index < 4; ++index)
          this.m_vertices[index] = !this.sprite.IsPerpendicular ? this.m_vertices[index].WithZ(this.m_vertices[index].z + this.m_vertices[index].y) : this.m_vertices[index].WithZ(this.m_vertices[index].z - this.m_vertices[index].y);
      }
      this.m_triangles[0] = 0;
      this.m_triangles[1] = 2;
      this.m_triangles[2] = 1;
      this.m_triangles[3] = 2;
      this.m_triangles[4] = 3;
      this.m_triangles[5] = 1;
      float t1 = (float) (((double) x1 + (double) transform.position.x - (double) vector2_1.x) / ((double) vector2_2.x - (double) vector2_1.x));
      float t2 = (float) (((double) x2 + (double) transform.position.x - (double) vector2_1.x) / ((double) vector2_2.x - (double) vector2_1.x));
      float t3 = (float) (((double) y1 + (double) transform.position.y - (double) vector2_1.y) / ((double) vector2_2.y - (double) vector2_1.y));
      float t4 = (float) (((double) y2 + (double) transform.position.y - (double) vector2_1.y) / ((double) vector2_2.y - (double) vector2_1.y));
      if (spriteDefinition.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)
      {
        Vector2 vector2_3 = new Vector2(Mathf.Lerp(lhs2.x, lhs3.x, t3), Mathf.Lerp(lhs2.y, lhs3.y, t1));
        Vector2 vector2_4 = new Vector2(Mathf.Lerp(lhs2.x, lhs3.x, t4), Mathf.Lerp(lhs2.y, lhs3.y, t2));
        this.m_uvs[0] = new Vector2(vector2_3.x, vector2_3.y);
        this.m_uvs[1] = new Vector2(vector2_3.x, vector2_4.y);
        this.m_uvs[2] = new Vector2(vector2_4.x, vector2_3.y);
        this.m_uvs[3] = new Vector2(vector2_4.x, vector2_4.y);
      }
      else
      {
        float x3 = Mathf.Lerp(lhs2.x, lhs3.x, t1);
        float x4 = Mathf.Lerp(lhs2.x, lhs3.x, t2);
        float y3 = Mathf.Lerp(lhs2.y, lhs3.y, t3);
        float y4 = Mathf.Lerp(lhs2.y, lhs3.y, t4);
        this.m_uvs[0] = new Vector2(x3, y3);
        this.m_uvs[1] = new Vector2(x4, y3);
        this.m_uvs[2] = new Vector2(x3, y4);
        this.m_uvs[3] = new Vector2(x4, y4);
      }
      MeshFilter component = this.GetComponent<MeshFilter>();
      Mesh mesh = component.mesh;
      if ((Object) mesh == (Object) null)
        mesh = new Mesh();
      else if (mesh.vertexCount != this.m_vertices.Length)
        mesh.Clear(false);
      mesh.vertices = this.m_vertices;
      mesh.triangles = this.m_triangles;
      mesh.uv = this.m_uvs;
      mesh.RecalculateBounds();
      mesh.RecalculateNormals();
      component.mesh = mesh;
    }

    private void ClipToTileBounds()
    {
      if (BraveUtility.isLoadingLevel || GameManager.Instance.IsLoadingLevel || !(bool) (Object) this.sprite)
        return;
      Transform transform = this.transform;
      Bounds bounds = this.sprite.GetBounds();
      Vector2 b1 = bounds.min.XY();
      Vector2 b2 = bounds.max.XY();
      Vector3 vector3 = new Vector3(Mathf.Sign(transform.lossyScale.x), Mathf.Sign(transform.lossyScale.y), Mathf.Sign(transform.lossyScale.z));
      Vector2 lhs1 = Vector2.Scale(vector3.XY(), b1);
      Vector2 rhs = Vector2.Scale(vector3.XY(), b2);
      Vector2 vector1 = transform.position.XY() + Vector2.Min(lhs1, rhs);
      Vector2 vector2 = transform.position.XY() + Vector2.Max(lhs1, rhs);
      IntVector2 intVector2_1 = vector1.ToIntVector2(VectorConversions.Floor);
      IntVector2 intVector2_2 = vector2.ToIntVector2(VectorConversions.Floor);
      tk2dSpriteDefinition spriteDefinition = this.sprite.Collection.spriteDefinitions[this.sprite.spriteId];
      Vector2 lhs2 = new Vector2(float.MaxValue, float.MaxValue);
      Vector2 lhs3 = new Vector2(float.MinValue, float.MinValue);
      for (int index = 0; index < spriteDefinition.uvs.Length; ++index)
      {
        lhs2 = Vector2.Min(lhs2, spriteDefinition.uvs[index]);
        lhs3 = Vector2.Max(lhs3, spriteDefinition.uvs[index]);
      }
      int length = (intVector2_2.x - intVector2_1.x + 2) * (intVector2_2.y - intVector2_1.y + 2) * 4 * 2;
      if (this.m_vertices != null && length / 2 > this.m_vertices.Length)
      {
        this.m_vertices = (Vector3[]) null;
        this.m_triangles = (int[]) null;
        this.m_uvs = (Vector2[]) null;
      }
      if (this.m_vertices == null)
        this.m_vertices = new Vector3[length];
      if (this.m_triangles == null)
        this.m_triangles = new int[this.m_vertices.Length / 4 * 6];
      if (this.m_uvs == null)
        this.m_uvs = new Vector2[length];
      int index1 = 0;
      int index2 = 0;
      for (int x1 = intVector2_1.x; x1 <= intVector2_2.x; ++x1)
      {
        for (int y1 = intVector2_1.y; y1 <= intVector2_2.y; ++y1)
        {
          if (x1 >= 0 && x1 < GameManager.Instance.Dungeon.data.Width && y1 >= 0 && y1 < GameManager.Instance.Dungeon.data.Height)
          {
            CellData cellData = GameManager.Instance.Dungeon.data.cellData[x1][y1];
            if (cellData != null)
            {
              if (this.clipMode == TileSpriteClipper.ClipMode.GroundDecal)
              {
                if (cellData.type != CellType.FLOOR && !cellData.fallingPrevented || cellData.cellVisualData.floorType == CellVisualData.CellFloorType.Water)
                  continue;
              }
              else if (this.clipMode == TileSpriteClipper.ClipMode.WallEnterer)
              {
                if (cellData.type == CellType.WALL && !GameManager.Instance.Dungeon.data.isFaceWallLower(x1, y1))
                  continue;
              }
              else if (this.clipMode == TileSpriteClipper.ClipMode.PitBounds && (cellData.type != CellType.PIT || cellData.fallingPrevented))
                continue;
              int num = index1;
              float x2 = Mathf.Max((float) x1, vector1.x) - transform.position.x;
              float x3 = Mathf.Min((float) (x1 + 1), vector2.x) - transform.position.x;
              float y2 = Mathf.Max((float) y1, vector1.y) - transform.position.y;
              float y3 = Mathf.Min((float) (y1 + 1), vector2.y) - transform.position.y;
              Vector3 b3 = new Vector3(x2, y2, 0.0f);
              Vector3 b4 = new Vector3(x3, y2, 0.0f);
              Vector3 b5 = new Vector3(x2, y3, 0.0f);
              Vector3 b6 = new Vector3(x3, y3, 0.0f);
              b3 = Vector3.Scale(vector3, b3);
              b4 = Vector3.Scale(vector3, b4);
              b5 = Vector3.Scale(vector3, b5);
              b6 = Vector3.Scale(vector3, b6);
              this.m_vertices[index1] = b3;
              this.m_vertices[index1 + 1] = b4;
              this.m_vertices[index1 + 2] = b5;
              this.m_vertices[index1 + 3] = b6;
              if (this.sprite.ShouldDoTilt)
              {
                for (int index3 = index1; index3 < index1 + 4; ++index3)
                  this.m_vertices[index3] = !this.sprite.IsPerpendicular ? this.m_vertices[index3].WithZ(this.m_vertices[index3].z + this.m_vertices[index3].y) : this.m_vertices[index3].WithZ(this.m_vertices[index3].z - this.m_vertices[index3].y);
              }
              this.m_triangles[index2] = num;
              this.m_triangles[index2 + 1] = num + 2;
              this.m_triangles[index2 + 2] = num + 1;
              this.m_triangles[index2 + 3] = num + 2;
              this.m_triangles[index2 + 4] = num + 3;
              this.m_triangles[index2 + 5] = num + 1;
              float t1 = (float) (((double) x2 + (double) transform.position.x - (double) vector1.x) / ((double) vector2.x - (double) vector1.x));
              float t2 = (float) (((double) x3 + (double) transform.position.x - (double) vector1.x) / ((double) vector2.x - (double) vector1.x));
              float t3 = (float) (((double) y2 + (double) transform.position.y - (double) vector1.y) / ((double) vector2.y - (double) vector1.y));
              float t4 = (float) (((double) y3 + (double) transform.position.y - (double) vector1.y) / ((double) vector2.y - (double) vector1.y));
              if (spriteDefinition.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)
              {
                Vector2 vector2_1 = new Vector2(Mathf.Lerp(lhs2.x, lhs3.x, t3), Mathf.Lerp(lhs2.y, lhs3.y, t1));
                Vector2 vector2_2 = new Vector2(Mathf.Lerp(lhs2.x, lhs3.x, t4), Mathf.Lerp(lhs2.y, lhs3.y, t2));
                this.m_uvs[index1] = new Vector2(vector2_1.x, vector2_1.y);
                this.m_uvs[index1 + 1] = new Vector2(vector2_1.x, vector2_2.y);
                this.m_uvs[index1 + 2] = new Vector2(vector2_2.x, vector2_1.y);
                this.m_uvs[index1 + 3] = new Vector2(vector2_2.x, vector2_2.y);
              }
              else
              {
                float x4 = Mathf.Lerp(lhs2.x, lhs3.x, t1);
                float x5 = Mathf.Lerp(lhs2.x, lhs3.x, t2);
                float y4 = Mathf.Lerp(lhs2.y, lhs3.y, t3);
                float y5 = Mathf.Lerp(lhs2.y, lhs3.y, t4);
                this.m_uvs[index1] = new Vector2(x4, y4);
                this.m_uvs[index1 + 1] = new Vector2(x5, y4);
                this.m_uvs[index1 + 2] = new Vector2(x4, y5);
                this.m_uvs[index1 + 3] = new Vector2(x5, y5);
              }
              index1 += 4;
              index2 += 6;
            }
          }
        }
      }
      for (int index4 = index1; index4 < this.m_vertices.Length; ++index4)
      {
        this.m_vertices[index4] = Vector3.zero;
        this.m_uvs[index4] = Vector2.zero;
      }
      for (int index5 = index2; index5 < this.m_triangles.Length; ++index5)
        this.m_triangles[index5] = 0;
      MeshFilter component = this.GetComponent<MeshFilter>();
      Mesh mesh = component.mesh;
      if ((Object) mesh == (Object) null)
        mesh = new Mesh();
      mesh.vertices = this.m_vertices;
      mesh.triangles = this.m_triangles;
      mesh.uv = this.m_uvs;
      mesh.RecalculateBounds();
      mesh.RecalculateNormals();
      component.mesh = mesh;
    }

    public enum ClipMode
    {
      GroundDecal,
      WallEnterer,
      PitBounds,
      ClipBelowY,
    }
  }

