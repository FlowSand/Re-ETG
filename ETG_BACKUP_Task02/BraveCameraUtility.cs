// Decompiled with JetBrains decompiler
// Type: BraveCameraUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public static class BraveCameraUtility
{
  public static float? OverrideAspect;
  private static int m_cachedMultiple = -1;

  public static float ASPECT
  {
    get
    {
      if (BraveCameraUtility.OverrideAspect.HasValue)
        return BraveCameraUtility.OverrideAspect.Value;
      return GameManager.Options.CurrentPreferredScalingMode == GameOptions.PreferredScalingMode.FORCE_PIXEL_PERFECT || GameManager.Options.CurrentPreferredScalingMode == GameOptions.PreferredScalingMode.PIXEL_PERFECT ? 1.77777779f : Mathf.Max(1.77777779f, (float) Screen.width / (float) Screen.height);
    }
    set
    {
    }
  }

  public static int H_PIXELS
  {
    get
    {
      return Mathf.RoundToInt((float) (480.0 * ((double) BraveCameraUtility.ASPECT / 1.7777777910232544)));
    }
  }

  public static int V_PIXELS
  {
    get => Mathf.RoundToInt((float) BraveCameraUtility.H_PIXELS / BraveCameraUtility.ASPECT);
  }

  public static Rect GetRect()
  {
    if (GameManager.Options.CurrentPreferredScalingMode != GameOptions.PreferredScalingMode.UNIFORM_SCALING && GameManager.Options.CurrentPreferredScalingMode != GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST)
    {
      int num1 = Mathf.Min(Mathf.FloorToInt((float) Screen.width / (float) BraveCameraUtility.H_PIXELS), Mathf.FloorToInt((float) Screen.height / (float) BraveCameraUtility.V_PIXELS));
      int num2 = BraveCameraUtility.H_PIXELS * num1;
      int num3 = BraveCameraUtility.V_PIXELS * num1;
      float num4 = (float) (1.0 - (double) num2 / (double) Screen.width);
      float num5 = (float) (1.0 - (double) num3 / (double) Screen.height);
      return new Rect(num4 / 2f, num5 / 2f, 1f - num4, 1f - num5);
    }
    float num6 = (float) Screen.width / (float) Screen.height;
    float num7 = 0.0f;
    float num8 = 0.0f;
    if (Screen.width % 16 /*0x10*/ == 0 && Screen.height % 9 == 0 && Screen.width / 16 /*0x10*/ == Screen.height / 9)
      return new Rect(0.0f, 0.0f, 1f, 1f);
    if ((double) num6 > (double) BraveCameraUtility.ASPECT)
      num7 = (float) (1.0 - (double) BraveCameraUtility.ASPECT / (double) num6);
    else if ((double) num6 < (double) BraveCameraUtility.ASPECT)
      num8 = (float) (1.0 - (double) num6 / (double) BraveCameraUtility.ASPECT);
    return new Rect(num7 / 2f, num8 / 2f, 1f - num7, 1f - num8);
  }

  public static Vector2 GetRectSize()
  {
    if (GameManager.Options.CurrentPreferredScalingMode != GameOptions.PreferredScalingMode.UNIFORM_SCALING && GameManager.Options.CurrentPreferredScalingMode != GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST)
    {
      int num = Mathf.Min(Mathf.FloorToInt((float) Screen.width / (float) BraveCameraUtility.H_PIXELS), Mathf.FloorToInt((float) Screen.height / (float) BraveCameraUtility.V_PIXELS));
      return new Vector2(1f - (float) (1.0 - (double) (BraveCameraUtility.H_PIXELS * num) / (double) Screen.width), 1f - (float) (1.0 - (double) (BraveCameraUtility.V_PIXELS * num) / (double) Screen.height));
    }
    float num1 = (float) Screen.width / (float) Screen.height;
    float num2 = 0.0f;
    float num3 = 0.0f;
    if (Screen.width % 16 /*0x10*/ == 0 && Screen.height % 9 == 0 && Screen.width / 16 /*0x10*/ == Screen.height / 9)
      return Vector2.one;
    if ((double) num1 > (double) BraveCameraUtility.ASPECT)
      num2 = (float) (1.0 - (double) BraveCameraUtility.ASPECT / (double) num1);
    else if ((double) num1 < (double) BraveCameraUtility.ASPECT)
      num3 = (float) (1.0 - (double) num1 / (double) BraveCameraUtility.ASPECT);
    return new Vector2(1f - num2, 1f - num3);
  }

  public static Vector2 ConvertGameViewportToScreenViewport(Vector2 pos)
  {
    Rect rect = BraveCameraUtility.GetRect();
    return new Vector2(pos.x * rect.width + rect.x, pos.y * rect.height + rect.y);
  }

  public static IntVector2 GetTargetScreenResolution(IntVector2 inResolution)
  {
    float num1 = (float) inResolution.x / (float) inResolution.y;
    if (inResolution.x % 16 /*0x10*/ == 0 && inResolution.y % 9 == 0 && inResolution.x / 16 /*0x10*/ == inResolution.y / 9)
      return inResolution;
    if ((double) num1 > (double) BraveCameraUtility.ASPECT)
    {
      float num2 = num1 / BraveCameraUtility.ASPECT;
      float f = (float) inResolution.y * num2;
      return new IntVector2(inResolution.x, Mathf.RoundToInt(f));
    }
    if ((double) num1 >= (double) BraveCameraUtility.ASPECT)
      return inResolution;
    float num3 = num1 / BraveCameraUtility.ASPECT;
    return new IntVector2(Mathf.RoundToInt((float) inResolution.x / num3), inResolution.y);
  }

  public static void MaintainCameraAspect(Camera c)
  {
    c.transparencySortMode = TransparencySortMode.Orthographic;
    if (GameManager.Options == null || GameManager.Options.CurrentPreferredScalingMode != GameOptions.PreferredScalingMode.UNIFORM_SCALING && GameManager.Options.CurrentPreferredScalingMode != GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST)
    {
      int num1 = Mathf.Max(1, Mathf.Min(Mathf.FloorToInt((float) Screen.width / (float) BraveCameraUtility.H_PIXELS), Mathf.FloorToInt((float) Screen.height / (float) BraveCameraUtility.V_PIXELS)));
      int num2 = BraveCameraUtility.H_PIXELS * num1;
      int num3 = BraveCameraUtility.V_PIXELS * num1;
      float num4 = (float) (1.0 - (double) num2 / (double) Screen.width);
      float num5 = (float) (1.0 - (double) num3 / (double) Screen.height);
      c.rect = new Rect(num4 / 2f, num5 / 2f, 1f - num4, 1f - num5);
      if (BraveCameraUtility.m_cachedMultiple != num1)
        dfGUIManager.ForceResolutionUpdates();
      BraveCameraUtility.m_cachedMultiple = num1;
    }
    else
    {
      float num6 = (float) Screen.width / (float) Screen.height;
      float aspect = BraveCameraUtility.ASPECT;
      float num7 = 0.0f;
      float num8 = 0.0f;
      bool flag = false;
      if (Screen.width % 16 /*0x10*/ == 0 && Screen.height % 9 == 0 && Screen.width / 16 /*0x10*/ == Screen.height / 9)
      {
        c.rect = new Rect(0.0f, 0.0f, 1f, 1f);
        flag = true;
      }
      else if ((double) num6 > (double) aspect)
        num7 = (float) (1.0 - (double) aspect / (double) num6);
      else if ((double) num6 < (double) aspect)
        num8 = (float) (1.0 - (double) num6 / (double) aspect);
      if (!flag)
        c.rect = new Rect(num7 / 2f, num8 / 2f, 1f - num7, 1f - num8);
    }
    float num = Mathf.Clamp01(GameManager.Options.DisplaySafeArea);
    float width = c.rect.width;
    float height = c.rect.height;
    Rect rect = new Rect(c.rect.xMin + width * (float) (0.5 * (1.0 - (double) num)), c.rect.yMin + height * (float) (0.5 * (1.0 - (double) num)), width * num, height * num);
    c.rect = rect;
  }

  public static void MaintainCameraAspectForceAspect(Camera c, float forcedAspect)
  {
    c.transparencySortMode = TransparencySortMode.Orthographic;
    if (GameManager.Options == null || GameManager.Options.CurrentPreferredScalingMode != GameOptions.PreferredScalingMode.UNIFORM_SCALING && GameManager.Options.CurrentPreferredScalingMode != GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST)
    {
      int num1 = Mathf.Max(1, Mathf.Min(Mathf.FloorToInt((float) Screen.width / 480f), Mathf.FloorToInt((float) Screen.height / 270f)));
      int num2 = 480 * num1;
      int num3 = 270 * num1;
      float num4 = (float) (1.0 - (double) num2 / (double) Screen.width);
      float num5 = (float) (1.0 - (double) num3 / (double) Screen.height);
      c.rect = new Rect(num4 / 2f, num5 / 2f, 1f - num4, 1f - num5);
      if (BraveCameraUtility.m_cachedMultiple != num1)
        dfGUIManager.ForceResolutionUpdates();
      BraveCameraUtility.m_cachedMultiple = num1;
    }
    else
    {
      float num6 = (float) Screen.width / (float) Screen.height;
      float num7 = 0.0f;
      float num8 = 0.0f;
      bool flag = false;
      if (Screen.width % 16 /*0x10*/ == 0 && Screen.height % 9 == 0 && Screen.width / 16 /*0x10*/ == Screen.height / 9)
      {
        c.rect = new Rect(0.0f, 0.0f, 1f, 1f);
        flag = true;
      }
      else if ((double) num6 > (double) forcedAspect)
        num7 = (float) (1.0 - (double) forcedAspect / (double) num6);
      else if ((double) num6 < (double) forcedAspect)
        num8 = (float) (1.0 - (double) num6 / (double) forcedAspect);
      if (!flag)
        c.rect = new Rect(num7 / 2f, num8 / 2f, 1f - num7, 1f - num8);
    }
    float num = Mathf.Clamp01(GameManager.Options.DisplaySafeArea);
    float width = c.rect.width;
    float height = c.rect.height;
    Rect rect = new Rect(c.rect.xMin + width * (float) (0.5 * (1.0 - (double) num)), c.rect.yMin + height * (float) (0.5 * (1.0 - (double) num)), width * num, height * num);
    c.rect = rect;
  }

  public static Camera GenerateBackgroundCamera(Camera c)
  {
    Camera component = new GameObject("BackgroundCamera", new System.Type[1]
    {
      typeof (Camera)
    }).GetComponent<Camera>();
    component.transform.position = new Vector3(-1000f, -1000f, 0.0f);
    component.orthographic = true;
    component.orthographicSize = 1f;
    component.depth = -5f;
    component.clearFlags = CameraClearFlags.Color;
    component.backgroundColor = Color.black;
    component.cullingMask = -1;
    component.rect = new Rect(0.0f, 0.0f, 1f, 1f);
    return component;
  }
}
