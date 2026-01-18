// Decompiled with JetBrains decompiler
// Type: TweenComponentExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using DaikonForge.Tween;
using System;
using UnityEngine;

#nullable disable

  public static class TweenComponentExtensions
  {
    public static DaikonForge.Tween.Tween<float> TweenAlpha(this Component component)
    {
      if (component is TextMesh)
        return ((TextMesh) component).TweenAlpha();
      if (component is GUIText)
        return ((GUIText) component).material.TweenAlpha();
      if (component.GetComponent<Renderer>() is SpriteRenderer)
        return ((SpriteRenderer) component.GetComponent<Renderer>()).TweenAlpha();
      Material material = !((UnityEngine.Object) component.GetComponent<Renderer>() == (UnityEngine.Object) null) ? component.GetComponent<Renderer>().material : throw new NullReferenceException("Component does not have a Renderer assigned");
      return !((UnityEngine.Object) material == (UnityEngine.Object) null) ? material.TweenAlpha() : throw new NullReferenceException("Component does not have a Material assigned");
    }

    public static DaikonForge.Tween.Tween<Color> TweenColor(this Component component)
    {
      if (component is TextMesh)
        return ((TextMesh) component).TweenColor();
      if (component is GUIText)
        return ((GUIText) component).material.TweenColor();
      if (component.GetComponent<Renderer>() is SpriteRenderer)
        return ((SpriteRenderer) component.GetComponent<Renderer>()).TweenColor();
      Material material = !((UnityEngine.Object) component.GetComponent<Renderer>() == (UnityEngine.Object) null) ? component.GetComponent<Renderer>().material : throw new NullReferenceException("Component does not have a Renderer assigned");
      return !((UnityEngine.Object) material == (UnityEngine.Object) null) ? material.TweenColor() : throw new NullReferenceException("Component does not have a Material assigned");
    }

    public static DaikonForge.Tween.Tween<float> TweenPath(
      this Component component,
      IPathIterator path)
    {
      return component.TweenPath(path, true);
    }

    public static DaikonForge.Tween.Tween<float> TweenPath(
      this Component component,
      IPathIterator path,
      bool orientToPath)
    {
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        throw new ArgumentNullException(nameof (component));
      return component.transform.TweenPath(path);
    }

    public static TweenShake ShakePosition(this Component component)
    {
      return component.transform.ShakePosition();
    }

    public static TweenShake ShakePosition(this Component component, bool localPosition)
    {
      return component.transform.ShakePosition(localPosition);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenScaleFrom(
      this Component component,
      Vector3 startScale)
    {
      return component.TweenScale().SetStartValue(startScale);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenScaleTo(
      this Component component,
      Vector3 endScale)
    {
      return component.transform.TweenScale().SetEndValue(endScale);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenScale(this Component component)
    {
      return component.transform.TweenScale();
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotateFrom(
      this Component component,
      Vector3 startRotation)
    {
      return component.TweenRotateFrom(startRotation, true, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotateFrom(
      this Component component,
      Vector3 startRotation,
      bool useShortestPath)
    {
      return component.TweenRotateFrom(startRotation, useShortestPath, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotateFrom(
      this Component component,
      Vector3 startRotation,
      bool useShortestPath,
      bool useLocalRotation)
    {
      return component.transform.TweenRotation(useShortestPath, useLocalRotation).SetStartValue(startRotation);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotateTo(
      this Component component,
      Vector3 endRotation)
    {
      return component.TweenRotateTo(endRotation, true, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotateTo(
      this Component component,
      Vector3 endRotation,
      bool useShortestPath)
    {
      return component.TweenRotateTo(endRotation, useShortestPath, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotateTo(
      this Component component,
      Vector3 endRotation,
      bool useShortestPath,
      bool useLocalRotation)
    {
      return component.transform.TweenRotation(useShortestPath, useLocalRotation).SetEndValue(endRotation);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotation(this Component component)
    {
      return component.transform.TweenRotation(true, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotation(
      this Component component,
      bool useShortestPath)
    {
      return component.transform.TweenRotation(useShortestPath, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenRotation(
      this Component component,
      bool useShortestPath,
      bool useLocalRotation)
    {
      return component.transform.TweenRotation(useShortestPath, useLocalRotation);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenMoveFrom(
      this Component component,
      Vector3 startPosition)
    {
      return component.TweenMoveFrom(startPosition, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenMoveFrom(
      this Component component,
      Vector3 startPosition,
      bool useLocalPosition)
    {
      return component.TweenPosition(useLocalPosition).SetStartValue(startPosition);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenMoveTo(
      this Component component,
      Vector3 endPosition)
    {
      return component.TweenMoveTo(endPosition, false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenMoveTo(
      this Component component,
      Vector3 endPosition,
      bool useLocalPosition)
    {
      return component.TweenPosition(useLocalPosition).SetEndValue(endPosition);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenPosition(this Component component)
    {
      return component.transform.TweenPosition(false);
    }

    public static DaikonForge.Tween.Tween<Vector3> TweenPosition(
      this Component component,
      bool useLocalPosition)
    {
      return component.transform.TweenPosition(useLocalPosition);
    }
  }

