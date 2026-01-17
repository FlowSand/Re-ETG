// Decompiled with JetBrains decompiler
// Type: Tween
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using DaikonForge.Tween;
using UnityEngine;

#nullable disable
public static class Tween
{
  public static DaikonForge.Tween.Tween<UnityEngine.Color> Color(SpriteRenderer renderer)
  {
    return renderer.TweenColor();
  }

  public static DaikonForge.Tween.Tween<float> Alpha(SpriteRenderer renderer)
  {
    return renderer.TweenAlpha();
  }

  public static DaikonForge.Tween.Tween<UnityEngine.Color> Color(Material material)
  {
    return material.TweenColor();
  }

  public static DaikonForge.Tween.Tween<float> Alpha(Material material) => material.TweenAlpha();

  public static DaikonForge.Tween.Tween<Vector3> Position(Transform transform)
  {
    return global::Tween.Position(transform, false);
  }

  public static DaikonForge.Tween.Tween<Vector3> Position(
    Transform transform,
    bool useLocalPosition)
  {
    return transform.TweenPosition(useLocalPosition);
  }

  public static DaikonForge.Tween.Tween<Vector3> MoveTo(Transform transform, Vector3 endPosition)
  {
    return global::Tween.MoveTo(transform, endPosition, false);
  }

  public static DaikonForge.Tween.Tween<Vector3> MoveTo(
    Transform transform,
    Vector3 endPosition,
    bool useLocalPosition)
  {
    return transform.TweenMoveTo(endPosition, useLocalPosition);
  }

  public static DaikonForge.Tween.Tween<Vector3> MoveFrom(
    Transform transform,
    Vector3 startPosition)
  {
    return transform.TweenMoveFrom(startPosition, false);
  }

  public static DaikonForge.Tween.Tween<Vector3> MoveFrom(
    Transform transform,
    Vector3 startPosition,
    bool useLocalPosition)
  {
    return transform.TweenMoveFrom(startPosition, useLocalPosition);
  }

  public static DaikonForge.Tween.Tween<Vector3> RotateFrom(
    Transform transform,
    Vector3 startRotation)
  {
    return global::Tween.RotateFrom(transform, startRotation, true, false);
  }

  public static DaikonForge.Tween.Tween<Vector3> RotateFrom(
    Transform transform,
    Vector3 startRotation,
    bool useShortestPath)
  {
    return global::Tween.RotateFrom(transform, startRotation, useShortestPath, false);
  }

  public static DaikonForge.Tween.Tween<Vector3> RotateFrom(
    Transform transform,
    Vector3 startRotation,
    bool useShortestPath,
    bool useLocalRotation)
  {
    return transform.TweenRotateFrom(startRotation, useShortestPath, useLocalRotation);
  }

  public static DaikonForge.Tween.Tween<Vector3> RotateTo(Transform transform, Vector3 endRotation)
  {
    return global::Tween.RotateTo(transform, endRotation, true, false);
  }

  public static DaikonForge.Tween.Tween<Vector3> RotateTo(
    Transform transform,
    Vector3 endRotation,
    bool useShortestPath)
  {
    return global::Tween.RotateTo(transform, endRotation, useShortestPath, false);
  }

  public static DaikonForge.Tween.Tween<Vector3> RotateTo(
    Transform transform,
    Vector3 endRotation,
    bool useShortestPath,
    bool useLocalRotation)
  {
    return transform.TweenRotateTo(endRotation, useShortestPath, useLocalRotation);
  }

  public static DaikonForge.Tween.Tween<Vector3> Rotation(Transform transform)
  {
    return transform.TweenRotation();
  }

  public static DaikonForge.Tween.Tween<Vector3> Rotation(Transform transform, bool useShortestPath)
  {
    return global::Tween.Rotation(transform, useShortestPath, false);
  }

  public static DaikonForge.Tween.Tween<Vector3> Rotation(
    Transform transform,
    bool useShortestPath,
    bool useLocalRotation)
  {
    return transform.TweenRotation(useShortestPath, useLocalRotation);
  }

  public static DaikonForge.Tween.Tween<Vector3> ScaleFrom(Transform transform, Vector3 startScale)
  {
    return global::Tween.Scale(transform).SetStartValue(startScale);
  }

  public static DaikonForge.Tween.Tween<Vector3> ScaleTo(Transform transform, Vector3 endScale)
  {
    return global::Tween.Scale(transform).SetEndValue(endScale);
  }

  public static DaikonForge.Tween.Tween<Vector3> Scale(Transform transform)
  {
    return transform.TweenScale();
  }

  public static TweenShake Shake(Transform transform) => global::Tween.Shake(transform, false);

  public static TweenShake Shake(Transform transform, bool localPosition)
  {
    return transform.ShakePosition(localPosition);
  }

  public static DaikonForge.Tween.Tween<T> NamedProperty<T>(object target, string propertyName)
  {
    return TweenNamedProperty<T>.Obtain(target, propertyName);
  }
}
