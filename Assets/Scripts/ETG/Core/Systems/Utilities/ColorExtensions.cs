// Decompiled with JetBrains decompiler
// Type: ColorExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public static class ColorExtensions
    {
      public static bool EqualsNonAlloc(this Color32 color, Color32 other)
      {
        return (int) color.r == (int) other.r && (int) color.g == (int) other.g && (int) color.b == (int) other.b && (int) color.a == (int) other.a;
      }

      public static Color SmoothStep(Color start, Color end, float t)
      {
        return new Color(Mathf.SmoothStep(start.r, end.r, t), Mathf.SmoothStep(start.g, end.g, t), Mathf.SmoothStep(start.b, end.b, t), Mathf.SmoothStep(start.a, end.a, t));
      }
    }

}
