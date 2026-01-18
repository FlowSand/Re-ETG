using DaikonForge.Tween;
using DaikonForge.Tween.Interpolation;
using System;
using UnityEngine;

#nullable disable

  public static class TweenTransformExtensions
  {
    public static TweenShake ShakePosition(this Transform transform)
    {
      return transform.ShakePosition(false);
    }

    public static TweenShake ShakePosition(this Transform transform, bool localPosition)
    {
      return TweenShake.Obtain().SetStartValue(!localPosition ? transform.position : transform.localPosition).SetShakeMagnitude(0.1f).SetDuration(1f).SetShakeSpeed(10f).OnExecute((TweenAssignmentCallback<Vector3>) (result =>
      {
        if (localPosition)
          transform.localPosition = result;
        else
          transform.position = result;
      }));
    }

    public static DaikonForge.Tween.Tween<float> TweenPath(
      this Transform transform,
      IPathIterator path)
    {
      return transform.TweenPath(path, true);
    }

    public static DaikonForge.Tween.Tween<float> TweenPath(
      this Transform transform,
      IPathIterator path,
      bool orientToPath)
    {
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null)
        throw new ArgumentNullException(nameof (transform));
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      DaikonForge.Tween.Tween<float> tween = (DaikonForge.Tween.Tween<float>) null;
      DaikonForge.Tween.Tween<float> tween1 = DaikonForge.Tween.Tween<float>.Obtain().SetStartValue(0.0f).SetEndValue(1f);
      // ISSUE: reference to a compiler-generated field
      if (TweenTransformExtensions._f__mg_cache0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TweenTransformExtensions._f__mg_cache0 = new TweenEasingCallback(TweenEasingFunctions.Linear);
      }
      // ISSUE: reference to a compiler-generated field
      TweenEasingCallback fMgCache0 = TweenTransformExtensions._f__mg_cache0;
      tween = tween1.SetEasing(fMgCache0).OnExecute((TweenAssignmentCallback<float>) (time =>
      {
        transform.position = path.GetPosition(time);
        if (!orientToPath)
          return;
        Vector3 tangent = path.GetTangent(time);
        if (tween.PlayDirection == TweenDirection.Forward)
          transform.forward = tangent;
        else
          transform.forward = -tangent;
      }));
      return tween;
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenScaleFrom(
      this Transform transform,
      Vector3 startScale)
    {
      return transform.TweenScale().SetStartValue(startScale);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenScaleTo(
      this Transform transform,
      Vector3 endScale)
    {
      return transform.TweenScale().SetEndValue(endScale);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenScale(this Transform transform)
    {
      return DaikonForge.Tween.Tween<Vector3>.Obtain().SetStartValue(transform.localScale).SetEndValue(transform.localScale).SetDuration(1f).OnExecute((TweenAssignmentCallback<Vector3>) (currentValue => transform.localScale = currentValue));
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotateFrom(
      this Transform transform,
      Vector3 startRotation)
    {
      return transform.TweenRotateFrom(startRotation, true, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotateFrom(
      this Transform transform,
      Vector3 startRotation,
      bool useShortestPath)
    {
      return transform.TweenRotateFrom(startRotation, useShortestPath, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotateFrom(
      this Transform transform,
      Vector3 startRotation,
      bool useShortestPath,
      bool useLocalRotation)
    {
      return transform.TweenRotation(useShortestPath, useLocalRotation).SetStartValue(startRotation);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotateTo(
      this Transform transform,
      Vector3 endRotation)
    {
      return transform.TweenRotateTo(endRotation, true, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotateTo(
      this Transform transform,
      Vector3 endRotation,
      bool useShortestPath)
    {
      return transform.TweenRotateTo(endRotation, useShortestPath, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotateTo(
      this Transform transform,
      Vector3 endRotation,
      bool useShortestPath,
      bool useLocalRotation)
    {
      return transform.TweenRotation(useShortestPath, useLocalRotation).SetEndValue(endRotation);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotation(this Transform transform)
    {
      return transform.TweenRotation(true, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotation(
      this Transform transform,
      bool useShortestPath)
    {
      return transform.TweenRotation(useShortestPath, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotation(
      this Transform transform,
      bool useShortestPath,
      bool useLocalRotation)
    {
      Interpolator<Vector3> interpolator = !useShortestPath ? Vector3Interpolator.Default : EulerInterpolator.Default;
      Vector3 vector3 = !useLocalRotation ? transform.eulerAngles : transform.localEulerAngles;
      TweenAssignmentCallback<Vector3> function = !useLocalRotation ? (TweenAssignmentCallback<Vector3>) (globalValue => transform.eulerAngles = globalValue) : (TweenAssignmentCallback<Vector3>) (localValue => transform.localEulerAngles = localValue);
      return DaikonForge.Tween.Tween<Vector3>.Obtain().SetStartValue(vector3).SetEndValue(vector3).SetDuration(1f).SetInterpolator(interpolator).OnExecute(function);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenMoveFrom(
      this Transform transform,
      Vector3 startPosition)
    {
      return transform.TweenMoveFrom(startPosition, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenMoveFrom(
      this Transform transform,
      Vector3 startPosition,
      bool useLocalPosition)
    {
      return transform.TweenPosition(useLocalPosition).SetStartValue(startPosition);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenMoveTo(
      this Transform transform,
      Vector3 endPosition)
    {
      return transform.TweenMoveTo(endPosition, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenMoveTo(
      this Transform transform,
      Vector3 endPosition,
      bool useLocalPosition)
    {
      return transform.TweenPosition(useLocalPosition).SetEndValue(endPosition);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenPosition(this Transform transform)
    {
      return transform.TweenPosition(false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenPosition(
      this Transform transform,
      bool useLocalPosition)
    {
      Vector3 vector3 = !useLocalPosition ? transform.position : transform.localPosition;
      TweenAssignmentCallback<Vector3> function = !useLocalPosition ? (TweenAssignmentCallback<Vector3>) (globalValue => transform.position = globalValue) : (TweenAssignmentCallback<Vector3>) (localValue => transform.localPosition = localValue);
      return DaikonForge.Tween.Tween<Vector3>.Obtain().SetStartValue(vector3).SetEndValue(vector3).SetDuration(1f).OnExecute(function);
    }
  }

