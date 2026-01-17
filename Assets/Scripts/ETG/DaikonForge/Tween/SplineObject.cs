// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.SplineObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using DaikonForge.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween
{
  [AddComponentMenu("Daikon Forge/Tween/Spline Path")]
  [InspectorGroupOrder(new string[] {"General", "Path", "Control Points"})]
  [ExecuteInEditMode]
  public class SplineObject : MonoBehaviour
  {
    public Spline Spline;
    [Inspector("Path", Order = 0, Tooltip = "If set to TRUE, the end of the path will wrap around to the beginning")]
    public bool Wrap;
    [Inspector("Control Points", Order = 1, Tooltip = "Contains the list of Transforms that represent the control points of the path's curve")]
    public List<Transform> ControlPoints = new List<Transform>();

    public void Awake()
    {
      if (this.ControlPoints.Count == 0)
        this.ControlPoints.AddRange((IEnumerable<Transform>) ((IEnumerable<SplineNode>) this.transform.GetComponentsInChildren<SplineNode>()).Select<SplineNode, Transform>((Func<SplineNode, Transform>) (x => x.transform)).ToList<Transform>());
      this.CalculateSpline();
    }

    public Vector3 Evaluate(float time) => this.Spline.GetPosition(time);

    public float GetTimeAtNode(int nodeIndex)
    {
      this.CalculateSpline();
      return this.Spline.ControlPoints[nodeIndex].Time;
    }

    public SplineNode AddNode()
    {
      this.CalculateSpline();
      Vector3 position = this.transform.position;
      if (this.Spline.ControlPoints.Count > 2)
        position = this.Spline.ControlPoints.Last<Spline.ControlPoint>().Position + this.Spline.GetTangent(0.95f);
      return this.AddNode(position);
    }

    public SplineNode AddNode(Vector3 position)
    {
      GameObject gameObject1 = new GameObject();
      gameObject1.name = "SplineNode" + (object) this.Spline.ControlPoints.Count;
      GameObject gameObject2 = gameObject1;
      SplineNode splineNode = gameObject2.AddComponent<SplineNode>();
      gameObject2.transform.parent = this.transform;
      gameObject2.transform.position = position;
      this.ControlPoints.Add(gameObject2.transform);
      this.CalculateSpline();
      return splineNode;
    }

    public void CalculateSpline()
    {
      int index1 = 0;
      while (index1 < this.ControlPoints.Count)
      {
        if ((UnityEngine.Object) this.ControlPoints[index1] == (UnityEngine.Object) null)
          this.ControlPoints.RemoveAt(index1);
        else
          ++index1;
      }
      if (this.Spline == null)
        this.Spline = new Spline();
      this.Spline.Wrap = this.Wrap;
      this.Spline.ControlPoints.Clear();
      for (int index2 = 0; index2 < this.ControlPoints.Count; ++index2)
      {
        Transform controlPoint = this.ControlPoints[index2];
        if (!((UnityEngine.Object) controlPoint == (UnityEngine.Object) null))
          this.Spline.ControlPoints.Add(new Spline.ControlPoint(controlPoint.position, controlPoint.forward));
      }
      this.Spline.ComputeSpline();
    }

    public Bounds GetBounds()
    {
      Vector3 lhs1 = Vector3.one * float.MaxValue;
      Vector3 lhs2 = Vector3.one * float.MinValue;
      int num = 0;
      for (int index = 0; index < this.ControlPoints.Count; ++index)
      {
        if (!((UnityEngine.Object) this.ControlPoints[index] == (UnityEngine.Object) null))
        {
          ++num;
          Vector3 position = this.ControlPoints[index].transform.position;
          lhs1 = Vector3.Min(lhs1, position);
          lhs2 = Vector3.Max(lhs2, position);
        }
      }
      if (num == 0)
        return new Bounds(this.transform.position, Vector3.zero);
      Vector3 size = lhs2 - lhs1;
      return new Bounds(lhs1 + size * 0.5f, size);
    }
  }
}
