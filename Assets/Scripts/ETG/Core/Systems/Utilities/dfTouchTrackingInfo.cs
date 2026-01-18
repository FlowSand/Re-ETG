// Decompiled with JetBrains decompiler
// Type: dfTouchTrackingInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

internal class dfTouchTrackingInfo
  {
    private TouchPhase phase;
    private Vector2 position = Vector2.one * float.MinValue;
    private Vector2 deltaPosition = Vector2.zero;
    private float deltaTime;
    private float lastUpdateTime = UnityEngine.Time.realtimeSinceStartup;
    public bool IsActive;

    public int FingerID { get; set; }

    public TouchPhase Phase
    {
      get => this.phase;
      set
      {
        this.IsActive = true;
        this.phase = value;
        if (value != TouchPhase.Stationary)
          return;
        this.deltaTime = float.Epsilon;
        this.deltaPosition = Vector2.zero;
        this.lastUpdateTime = UnityEngine.Time.realtimeSinceStartup;
      }
    }

    public Vector2 Position
    {
      get => this.position;
      set
      {
        this.IsActive = true;
        this.deltaPosition = this.Phase != TouchPhase.Began ? value - this.position : Vector2.zero;
        this.position = value;
        float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
        this.deltaTime = realtimeSinceStartup - this.lastUpdateTime;
        this.lastUpdateTime = realtimeSinceStartup;
      }
    }

    public static implicit operator dfTouchInfo(dfTouchTrackingInfo info)
    {
      return new dfTouchInfo(info.FingerID, info.phase, info.phase != TouchPhase.Began ? 0 : 1, info.position, info.deltaPosition, info.deltaTime);
    }
  }

