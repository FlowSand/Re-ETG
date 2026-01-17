// Decompiled with JetBrains decompiler
// Type: FullInspector.InspectorRangeAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class InspectorRangeAttribute : Attribute
  {
    public float Min;
    public float Max;
    public float Step = float.NaN;

    public InspectorRangeAttribute(float min, float max)
    {
      this.Min = min;
      this.Max = max;
    }
  }
}
