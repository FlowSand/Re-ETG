// Decompiled with JetBrains decompiler
// Type: ArbitraryCableDrawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class ArbitraryCableDrawer : MonoBehaviour
  {
    public Transform Attach1;
    public Vector2 Attach1Offset;
    public Transform Attach2;
    public Vector2 Attach2Offset;
    private Mesh m_mesh;
    private Vector3[] m_vertices;
    private MeshFilter m_stringFilter;

    public void Initialize(Transform t1, Transform t2)
    {
      this.Attach1 = t1;
      this.Attach2 = t2;
      this.m_mesh = new Mesh();
      this.m_vertices = new Vector3[20];
      this.m_mesh.vertices = this.m_vertices;
      int[] numArray = new int[54];
      Vector2[] vector2Array = new Vector2[20];
      int num = 0;
      for (int index = 0; index < 9; ++index)
      {
        numArray[index * 6] = num;
        numArray[index * 6 + 1] = num + 2;
        numArray[index * 6 + 2] = num + 1;
        numArray[index * 6 + 3] = num + 2;
        numArray[index * 6 + 4] = num + 3;
        numArray[index * 6 + 5] = num + 1;
        num += 2;
      }
      this.m_mesh.triangles = numArray;
      this.m_mesh.uv = vector2Array;
      GameObject gameObject = new GameObject("cableguy");
      this.m_stringFilter = gameObject.AddComponent<MeshFilter>();
      MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
      meshRenderer.material = BraveResources.Load("Global VFX/WhiteMaterial", ".mat") as Material;
      meshRenderer.material.SetColor("_OverrideColor", Color.black);
      this.m_stringFilter.mesh = this.m_mesh;
    }

    private void LateUpdate()
    {
      if (!(bool) (Object) this.Attach1 || !(bool) (Object) this.Attach2)
        return;
      Vector3 vector3 = this.Attach1.position.XY().ToVector3ZisY(-3f) + this.Attach1Offset.ToVector3ZisY();
      Vector3 p3 = this.Attach2.position.XY().ToVector3ZisY(-3f) + this.Attach2Offset.ToVector3ZisY();
      this.BuildMeshAlongCurve((Vector2) vector3, (Vector2) vector3, (Vector2) (p3 + new Vector3(0.0f, -2f, -2f)), (Vector2) p3);
      this.m_mesh.vertices = this.m_vertices;
      this.m_mesh.RecalculateBounds();
      this.m_mesh.RecalculateNormals();
    }

    private void OnDestroy()
    {
      if (!(bool) (Object) this.m_stringFilter)
        return;
      Object.Destroy((Object) this.m_stringFilter.gameObject);
    }

    private void BuildMeshAlongCurve(
      Vector2 p0,
      Vector2 p1,
      Vector2 p2,
      Vector2 p3,
      float meshWidth = 0.03125f)
    {
      Vector3[] vertices = this.m_vertices;
      Vector2? nullable1 = new Vector2?();
      for (int index = 0; index < 10; ++index)
      {
        Vector2 bezierPoint = BraveMathCollege.CalculateBezierPoint((float) index / 9f, p0, p1, p2, p3);
        Vector2? nullable2 = index != 9 ? new Vector2?(BraveMathCollege.CalculateBezierPoint((float) index / 9f, p0, p1, p2, p3)) : new Vector2?();
        Vector2 vector2 = Vector2.zero;
        if (nullable1.HasValue)
          vector2 += (Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) (bezierPoint - nullable1.Value)).XY().normalized;
        if (nullable2.HasValue)
          vector2 += (Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) (nullable2.Value - bezierPoint)).XY().normalized;
        vector2 = vector2.normalized;
        vertices[index * 2] = (bezierPoint + vector2 * meshWidth).ToVector3ZisY();
        vertices[index * 2 + 1] = (bezierPoint + -vector2 * meshWidth).ToVector3ZisY();
        nullable1 = new Vector2?(bezierPoint);
      }
    }
  }

