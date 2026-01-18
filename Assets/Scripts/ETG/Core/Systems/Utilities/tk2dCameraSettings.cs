// Decompiled with JetBrains decompiler
// Type: tk2dCameraSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

[Serializable]
public class tk2dCameraSettings
  {
    public tk2dCameraSettings.ProjectionType projection;
    public float orthographicSize = 10f;
    public float orthographicPixelsPerMeter = 100f;
    public tk2dCameraSettings.OrthographicOrigin orthographicOrigin = tk2dCameraSettings.OrthographicOrigin.Center;
    public tk2dCameraSettings.OrthographicType orthographicType;
    public TransparencySortMode transparencySortMode;
    public float fieldOfView = 60f;
    public Rect rect = new Rect(0.0f, 0.0f, 1f, 1f);

    public enum ProjectionType
    {
      Orthographic,
      Perspective,
    }

    public enum OrthographicType
    {
      PixelsPerMeter,
      OrthographicSize,
    }

    public enum OrthographicOrigin
    {
      BottomLeft,
      Center,
    }
  }

