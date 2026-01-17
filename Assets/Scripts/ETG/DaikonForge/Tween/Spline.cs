// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Spline
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween;

public class Spline : IPathIterator, IEnumerable<Spline.ControlPoint>, IEnumerable
{
  public ISplineInterpolator SplineMethod = (ISplineInterpolator) new CatmullRomSpline();
  public List<Spline.ControlPoint> ControlPoints = new List<Spline.ControlPoint>();
  public bool Wrap;
  private float length;

  public float Length => this.length;

  public Vector3 GetPosition(float time)
  {
    time = Mathf.Abs(time) % 1f;
    float num1 = 1f / (float) this.ControlPoints.Count;
    if (!this.Wrap)
      time *= num1 * (float) (this.ControlPoints.Count - 1);
    int nodeIndex = Mathf.FloorToInt(time * (float) this.ControlPoints.Count);
    Spline.ControlPoint node1 = this.getNode(nodeIndex - 1);
    Spline.ControlPoint node2 = this.getNode(nodeIndex);
    Spline.ControlPoint node3 = this.getNode(nodeIndex + 1);
    Spline.ControlPoint node4 = this.getNode(nodeIndex + 2);
    float num2 = num1 * (float) nodeIndex;
    time -= num2;
    time /= num1;
    return this.SplineMethod.Evaluate(node1.Position, node2.Position, node3.Position, node4.Position, time);
  }

  public float AdjustTimeToConstant(float time)
  {
    int index = (double) time >= 0.0 && (double) time <= 1.0 ? this.getParameterIndex(time) : throw new ArgumentException("The length parameter must be a value between 0 and 1 (inclusive)");
    float num1 = 1f / (float) (this.ControlPoints.Count + (!this.Wrap ? -1 : 0));
    Spline.ControlPoint controlPoint = this.ControlPoints[index];
    float num2 = controlPoint.Length / this.length;
    float num3 = (time - controlPoint.Time) / num2;
    return (float) ((double) index * (double) num1 + (double) num3 * (double) num1);
  }

  public Vector3 GetOrientation(float time)
  {
    int index1 = this.ControlPoints.Count - (!this.Wrap ? 2 : 1);
    while ((double) this.ControlPoints[index1].Time > (double) time)
      --index1;
    int index2 = index1 != this.ControlPoints.Count - 1 ? index1 + 1 : 0;
    Spline.ControlPoint controlPoint1 = this.ControlPoints[index1];
    Spline.ControlPoint controlPoint2 = this.ControlPoints[index2];
    float num = controlPoint1.Length / this.length;
    float t = (time - controlPoint1.Time) / num;
    return Vector3.Lerp(controlPoint1.Orientation, controlPoint2.Orientation, t);
  }

  public Vector3 GetTangent(float time) => this.GetTangent(time, 0.01f);

  public Vector3 GetTangent(float time, float lookAhead)
  {
    if (!this.Wrap && (double) time > 1.0 - (double) lookAhead)
      time = 1f - lookAhead;
    Vector3 position = this.GetPosition(time);
    Vector3 tangent = this.GetPosition(time + lookAhead) - position;
    tangent.Normalize();
    return tangent;
  }

  public Spline AddNode(Spline.ControlPoint node)
  {
    this.ControlPoints.Add(node);
    return this;
  }

  public void ComputeSpline()
  {
    this.length = 0.0f;
    for (int index = 0; index < this.ControlPoints.Count; ++index)
    {
      this.ControlPoints[index].Time = 0.0f;
      this.ControlPoints[index].Length = 0.0f;
    }
    if (this.ControlPoints.Count < 2)
      return;
    int num1 = 16 /*0x10*/;
    int num2 = this.ControlPoints.Count + (!this.Wrap ? -1 : 0);
    float num3 = 1f / (float) num2;
    float num4 = num3 / (float) num1;
    for (int index1 = 0; index1 < num2; ++index1)
    {
      float num5 = (float) index1 * num3;
      Vector3 a = this.ControlPoints[index1].Position;
      for (int index2 = 1; index2 < num1; ++index2)
      {
        Vector3 position = this.GetPosition(num5 + (float) index2 * num4);
        this.ControlPoints[index1].Length += Vector3.Distance(a, position);
        a = position;
      }
    }
    this.length = 0.0f;
    for (int index = 0; index < this.ControlPoints.Count; ++index)
      this.length += this.ControlPoints[index].Length;
    float num6 = 0.0f;
    for (int index = 0; index < num2; ++index)
    {
      this.ControlPoints[index].Time = num6 / this.length;
      num6 += this.ControlPoints[index].Length;
    }
    if (this.Wrap)
      return;
    this.ControlPoints[this.ControlPoints.Count - 1].Time = 1f;
  }

  private int getParameterIndex(float time)
  {
    int parameterIndex = 0;
    for (int index = 1; index < this.ControlPoints.Count; ++index)
    {
      Spline.ControlPoint controlPoint = this.ControlPoints[index];
      if ((double) controlPoint.Length != 0.0 && (double) controlPoint.Time <= (double) time)
        parameterIndex = index;
      else
        break;
    }
    return parameterIndex;
  }

  private Spline.ControlPoint getNode(int nodeIndex)
  {
    if (this.Wrap)
    {
      if (nodeIndex < 0)
        nodeIndex += this.ControlPoints.Count;
      if (nodeIndex >= this.ControlPoints.Count)
        nodeIndex -= this.ControlPoints.Count;
    }
    else
      nodeIndex = Mathf.Clamp(nodeIndex, 0, this.ControlPoints.Count - 1);
    return this.ControlPoints[nodeIndex];
  }

  [DebuggerHidden]
  private IEnumerator<Spline.ControlPoint> GetCustomEnumerator()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<Spline.ControlPoint>) new Spline.\u003CGetCustomEnumerator\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public IEnumerator<Spline.ControlPoint> GetEnumerator() => this.GetCustomEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetCustomEnumerator();

  public class ControlPoint
  {
    public Vector3 Position;
    public Vector3 Orientation;
    public float Length;
    public float Time;

    public ControlPoint(Vector3 Position, Vector3 Tangent)
    {
      this.Position = Position;
      this.Orientation = Tangent;
      this.Length = 0.0f;
    }
  }
}
