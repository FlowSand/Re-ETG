// Decompiled with JetBrains decompiler
// Type: dfMobileTouchInputSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class dfMobileTouchInputSource : IDFTouchInputSource
  {
    private static dfMobileTouchInputSource instance;
    private List<dfTouchInfo> activeTouches = new List<dfTouchInfo>();

    public static dfMobileTouchInputSource Instance
    {
      get
      {
        if (dfMobileTouchInputSource.instance == null)
          dfMobileTouchInputSource.instance = new dfMobileTouchInputSource();
        return dfMobileTouchInputSource.instance;
      }
    }

    public int TouchCount => Input.touchCount;

    public IList<dfTouchInfo> Touches => (IList<dfTouchInfo>) this.activeTouches;

    public dfTouchInfo GetTouch(int index) => (dfTouchInfo) Input.GetTouch(index);

    public void Update()
    {
      this.activeTouches.Clear();
      for (int index = 0; index < this.TouchCount; ++index)
        this.activeTouches.Add(this.GetTouch(index));
    }
  }

