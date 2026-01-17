// Decompiled with JetBrains decompiler
// Type: tk2dUITime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    public static class tk2dUITime
    {
      private static double lastRealTime;
      private static float _deltaTime = 0.0166666675f;

      public static float deltaTime => tk2dUITime._deltaTime;

      public static void Init()
      {
        tk2dUITime.lastRealTime = (double) UnityEngine.Time.realtimeSinceStartup;
        tk2dUITime._deltaTime = UnityEngine.Time.maximumDeltaTime;
      }

      public static void Update()
      {
        float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
        tk2dUITime._deltaTime = (double) UnityEngine.Time.timeScale >= 1.0 / 1000.0 ? BraveTime.DeltaTime / UnityEngine.Time.timeScale : Mathf.Min(0.06666667f, realtimeSinceStartup - (float) tk2dUITime.lastRealTime);
        tk2dUITime.lastRealTime = (double) realtimeSinceStartup;
      }
    }

}
