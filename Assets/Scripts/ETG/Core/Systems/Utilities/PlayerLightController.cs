// Decompiled with JetBrains decompiler
// Type: PlayerLightController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class PlayerLightController : MonoBehaviour
  {
    public int resolution = 1000;
    public float maxDistance = 10f;
    public float distortionMax = 0.5f;
    public Material shadowMaterial;
    private MeshFilter mf;
    private MeshRenderer mr;
    private Mesh m;
    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Vector2> uvs;
    private Vector3[] directionCache;
    private int layerMask = -1025;

    private void Start()
    {
      this.mf = this.GetComponent<MeshFilter>();
      if ((Object) this.mf == (Object) null)
        this.mf = this.gameObject.AddComponent<MeshFilter>();
      this.mr = this.GetComponent<MeshRenderer>();
      if ((Object) this.mr == (Object) null)
        this.mr = this.gameObject.AddComponent<MeshRenderer>();
      this.vertices = new List<Vector3>();
      this.triangles = new List<int>();
      this.uvs = new List<Vector2>();
      this.directionCache = new Vector3[this.resolution];
      this.CacheDirections();
      this.UpdateVertices(true);
      this.m = new Mesh();
      this.m.vertices = this.vertices.ToArray();
      this.m.triangles = this.triangles.ToArray();
      this.m.uv = this.uvs.ToArray();
      this.m.RecalculateBounds();
      this.m.RecalculateNormals();
      this.mf.sharedMesh = this.m;
      this.mr.material = this.shadowMaterial;
    }

    private void CacheDirections()
    {
      for (int index = 0; index < this.resolution; ++index)
      {
        Vector3 vector3 = Quaternion.Euler(0.0f, 0.0f, (float) index * (360f / (float) this.resolution)) * Vector3.up;
        this.directionCache[index] = vector3.normalized;
      }
    }

    private void UpdateVertices(bool generateTrisAndUVs)
    {
      this.vertices.Clear();
      if (generateTrisAndUVs)
      {
        this.triangles.Clear();
        this.uvs.Clear();
      }
      for (int index = 0; index < this.resolution; ++index)
      {
        Ray ray = new Ray(this.transform.position, this.directionCache[index]);
        RaycastHit hitInfo = new RaycastHit();
        Vector3 point1;
        Vector3 point2;
        float num;
        if (Physics.Raycast(ray, out hitInfo, this.maxDistance, this.layerMask))
        {
          point1 = hitInfo.point;
          point2 = ray.GetPoint(this.maxDistance + 1f);
          num = Mathf.Clamp01(1f - Mathf.Max(hitInfo.distance / this.maxDistance, 0.5f));
        }
        else
        {
          point1 = ray.GetPoint(this.maxDistance);
          point2 = ray.GetPoint(this.maxDistance + 1f);
          num = 0.0f;
        }
        this.vertices.Add(this.transform.InverseTransformPoint(point1) + this.directionCache[index] * (this.distortionMax * num));
        this.vertices.Add(this.transform.InverseTransformPoint(point2));
        if (generateTrisAndUVs)
        {
          if (index > 1)
          {
            this.triangles.Add(index * 2 - 1);
            this.triangles.Add(index * 2 - 2);
            this.triangles.Add(index * 2);
            this.triangles.Add(index * 2);
            this.triangles.Add(index * 2 + 1);
            this.triangles.Add(index * 2 - 1);
          }
          this.uvs.Add(Vector2.zero);
          this.uvs.Add(Vector2.zero);
        }
      }
      if (!generateTrisAndUVs)
        return;
      this.triangles.Add(this.vertices.Count - 1);
      this.triangles.Add(this.vertices.Count - 2);
      this.triangles.Add(0);
      this.triangles.Add(0);
      this.triangles.Add(1);
      this.triangles.Add(this.vertices.Count - 1);
    }

    private void LateUpdate()
    {
      this.UpdateVertices(false);
      this.m.vertices = this.vertices.ToArray();
    }
  }

