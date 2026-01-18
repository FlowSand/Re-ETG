using System;
using UnityEngine;

#nullable disable

public class CustomTrailRenderer : BraveBehaviour
  {
    public SpeculativeRigidbody specRigidbody;
    public Material material;
    private Material instanceMaterial;
    public bool emit = true;
    private bool emittingDone;
    public float lifeTime = 1f;
    private float lifeTimeRatio = 1f;
    public Color[] colors;
    public float[] widths;
    public float maxAngle = 2f;
    public float minVertexDistance = 0.1f;
    public float maxVertexDistance = 1f;
    public float optimizeAngleInterval = 0.1f;
    public float optimizeDistanceInterval = 0.05f;
    public int optimizeCount = 30;
    private Mesh mesh;
    private CustomTrailRenderer.Point[] points = new CustomTrailRenderer.Point[100];
    private int numPoints;
    private float m_cachedMaxAngle;
    private float m_cachedMaxVertexDistance;
    private int m_cachedOptimizeCount;

    public void Awake()
    {
      this.m_cachedMaxAngle = this.maxAngle;
      this.m_cachedMaxVertexDistance = this.maxVertexDistance;
      this.m_cachedOptimizeCount = this.optimizeCount;
    }

    public void Start()
    {
      MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
      if (!(bool) (UnityEngine.Object) meshFilter)
        return;
      this.mesh = meshFilter.mesh;
      this.renderer = (Renderer) this.gameObject.AddComponent<MeshRenderer>();
      if (!(bool) (UnityEngine.Object) this.renderer)
        return;
      this.instanceMaterial = new Material(this.material);
      this.renderer.material = this.instanceMaterial;
      if (!(bool) (UnityEngine.Object) this.specRigidbody || !PhysicsEngine.HasInstance)
        return;
      PhysicsEngine.Instance.OnPostRigidbodyMovement += new System.Action(this.OnPostRigidbodyMovement);
    }

    public void Update()
    {
      if ((bool) (UnityEngine.Object) this.specRigidbody)
        return;
      this.UpdateMesh();
    }

    private void OnPostRigidbodyMovement()
    {
      if (!(bool) (UnityEngine.Object) this.specRigidbody)
        return;
      this.UpdateMesh();
    }

    public void Reenable()
    {
      this.emit = true;
      this.emittingDone = false;
      if ((bool) (UnityEngine.Object) this.renderer)
        this.renderer.enabled = true;
      this.maxAngle = this.m_cachedMaxAngle;
      this.maxVertexDistance = this.m_cachedMaxVertexDistance;
      this.optimizeCount = this.m_cachedOptimizeCount;
    }

    public void Clear()
    {
      for (int index = this.numPoints - 1; index >= 0; --index)
        this.points[index] = (CustomTrailRenderer.Point) null;
      this.numPoints = 0;
      if (!(bool) (UnityEngine.Object) this.mesh)
        return;
      this.mesh.Clear();
    }

    private void UpdateMesh()
    {
      if ((bool) (UnityEngine.Object) this.specRigidbody && (double) this.specRigidbody.transform.rotation.eulerAngles.z != 0.0)
        this.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -this.specRigidbody.transform.rotation.eulerAngles.z);
      if (!this.emit)
        this.emittingDone = true;
      if (this.emittingDone)
        this.emit = false;
      int num1 = 0;
      for (int index = this.numPoints - 1; index >= 0; --index)
      {
        CustomTrailRenderer.Point point = this.points[index];
        if (point == null || (double) point.timeAlive >= (double) this.lifeTime)
          ++num1;
        else
          break;
      }
      if (num1 > 1)
      {
        for (int index = this.numPoints - num1 + 1; this.numPoints > index; --this.numPoints)
          this.points[this.numPoints - 1] = (CustomTrailRenderer.Point) null;
      }
      if (this.numPoints > this.optimizeCount)
      {
        this.maxAngle += this.optimizeAngleInterval;
        this.maxVertexDistance += this.optimizeDistanceInterval;
        ++this.optimizeCount;
      }
      if (this.emit)
      {
        if (this.numPoints == 0)
        {
          this.points[this.numPoints++] = new CustomTrailRenderer.Point(this.transform);
          this.points[this.numPoints++] = new CustomTrailRenderer.Point(this.transform);
        }
        if (this.numPoints == 1)
          this.InsertPoint();
        bool flag = false;
        float sqrMagnitude = (this.points[1].position - this.transform.position).sqrMagnitude;
        if ((double) sqrMagnitude > (double) this.minVertexDistance * (double) this.minVertexDistance)
        {
          if ((double) sqrMagnitude > (double) this.maxVertexDistance * (double) this.maxVertexDistance)
            flag = true;
          else if ((double) Quaternion.Angle(this.transform.rotation, this.points[1].rotation) > (double) this.maxAngle)
            flag = true;
        }
        if (flag)
        {
          if (this.numPoints == this.points.Length)
            Array.Resize<CustomTrailRenderer.Point>(ref this.points, this.points.Length + 50);
          this.InsertPoint();
        }
        else
          this.points[0].Update(this.transform);
      }
      if (this.numPoints < 2)
      {
        this.renderer.enabled = false;
      }
      else
      {
        this.renderer.enabled = true;
        this.lifeTimeRatio = (double) this.lifeTime != 0.0 ? 1f / this.lifeTime : 0.0f;
        if (!this.emit)
        {
          if (this.numPoints != 0)
            ;
        }
        else
        {
          Vector3[] vector3Array = new Vector3[this.numPoints * 2];
          Vector2[] vector2Array = new Vector2[this.numPoints * 2];
          int[] numArray = new int[(this.numPoints - 1) * 6];
          Color[] colorArray = new Color[this.numPoints * 2];
          float num2 = (float) (1.0 / ((double) this.points[this.numPoints - 1].timeAlive - (double) this.points[0].timeAlive));
          for (int index1 = 0; index1 < this.numPoints; ++index1)
          {
            CustomTrailRenderer.Point point = this.points[index1];
            float t1 = point.timeAlive * this.lifeTimeRatio;
            Vector3 vector3_1 = index1 != 0 || this.numPoints <= 1 ? (index1 != this.numPoints - 1 || this.numPoints <= 1 ? (this.numPoints <= 2 ? Vector3.right : (this.points[index1 + 1].position - this.points[index1].position + (this.points[index1].position - this.points[index1 - 1].position)) * 0.5f) : this.points[index1].position - this.points[index1 - 1].position) : this.points[index1 + 1].position - this.points[index1].position;
            Color color;
            if (this.colors.Length == 0)
              color = Color.Lerp(Color.white, Color.clear, t1);
            else if (this.colors.Length == 1)
              color = Color.Lerp(this.colors[0], Color.clear, t1);
            else if (this.colors.Length == 2)
              color = Color.Lerp(this.colors[0], this.colors[1], t1);
            else if ((double) t1 <= 0.0)
              color = this.colors[0];
            else if ((double) t1 >= 1.0)
            {
              color = this.colors[this.colors.Length - 1];
            }
            else
            {
              float f = t1 * (float) (this.colors.Length - 1);
              int a = Mathf.Min(this.colors.Length - 2, (int) Mathf.Floor(f));
              float t2 = Mathf.InverseLerp((float) a, (float) (a + 1), f);
              if (a < 0 || a + 1 >= this.colors.Length)
                Debug.LogFormat("{0}, {1}, {2}, {3}", (object) f, (object) a, (object) t2, (object) (a + 1));
              color = Color.Lerp(this.colors[a], this.colors[a + 1], t2);
            }
            colorArray[index1 * 2] = color;
            colorArray[index1 * 2 + 1] = color;
            Vector3 vector3_2 = point.position;
            if (index1 > 0 && index1 == this.numPoints - 1)
            {
              float t3 = Mathf.InverseLerp(this.points[index1 - 1].timeAlive, point.timeAlive, this.lifeTime);
              vector3_2 = Vector3.Lerp(this.points[index1 - 1].position, point.position, t3);
            }
            float num3;
            if (this.widths.Length == 0)
              num3 = 1f;
            else if (this.widths.Length == 1)
              num3 = this.widths[0];
            else if (this.widths.Length == 2)
              num3 = Mathf.Lerp(this.widths[0], this.widths[1], t1);
            else if ((double) t1 <= 0.0)
              num3 = this.widths[0];
            else if ((double) t1 >= 1.0)
            {
              num3 = this.widths[this.widths.Length - 1];
            }
            else
            {
              float f = t1 * (float) (this.widths.Length - 1);
              int a = (int) Mathf.Floor(f);
              float t4 = Mathf.InverseLerp((float) a, (float) (a + 1), f);
              num3 = Mathf.Lerp(this.widths[a], this.widths[a + 1], t4);
            }
            Vector3 vector3_3 = vector3_1.normalized.RotateBy(Quaternion.Euler(0.0f, 0.0f, 90f)) * 0.5f * num3;
            vector3Array[index1 * 2] = vector3_2 - this.transform.position + vector3_3;
            vector3Array[index1 * 2 + 1] = vector3_2 - this.transform.position - vector3_3;
            float x = (point.timeAlive - this.points[0].timeAlive) * num2;
            vector2Array[index1 * 2] = new Vector2(x, 0.0f);
            vector2Array[index1 * 2 + 1] = new Vector2(x, 1f);
            if (index1 > 0)
            {
              int index2 = (index1 - 1) * 6;
              int num4 = index1 * 2;
              numArray[index2] = num4 - 2;
              numArray[index2 + 1] = num4 - 1;
              numArray[index2 + 2] = num4;
              numArray[index2 + 3] = num4 + 1;
              numArray[index2 + 4] = num4;
              numArray[index2 + 5] = num4 - 1;
            }
          }
          this.mesh.Clear();
          this.mesh.vertices = vector3Array;
          this.mesh.colors = colorArray;
          this.mesh.uv = vector2Array;
          this.mesh.triangles = numArray;
        }
      }
    }

    private void InsertPoint()
    {
      for (int numPoints = this.numPoints; numPoints > 0; --numPoints)
        this.points[numPoints] = this.points[numPoints - 1];
      this.points[0] = new CustomTrailRenderer.Point(this.transform);
      ++this.numPoints;
    }

    private class Point
    {
      public float timeCreated;
      public float fadeAlpha;
      public Vector3 position = Vector3.zero;
      public Quaternion rotation = Quaternion.identity;

      public Point(Transform trans)
      {
        this.position = trans.position;
        this.rotation = trans.rotation;
        this.timeCreated = BraveTime.ScaledTimeSinceStartup;
      }

      public float timeAlive => BraveTime.ScaledTimeSinceStartup - this.timeCreated;

      public void Update(Transform trans)
      {
        this.position = trans.position;
        this.rotation = trans.rotation;
        this.timeCreated = BraveTime.ScaledTimeSinceStartup;
      }
    }
  }

