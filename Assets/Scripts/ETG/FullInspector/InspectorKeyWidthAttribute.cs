// Decompiled with JetBrains decompiler
// Type: FullInspector.InspectorKeyWidthAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class InspectorKeyWidthAttribute : Attribute
{
  public float WidthPercentage;

  public InspectorKeyWidthAttribute(float widthPercentage)
  {
    this.WidthPercentage = (double) widthPercentage >= 0.0 && (double) widthPercentage < 1.0 ? widthPercentage : throw new ArgumentException("widthPercentage must be between [0,1]");
  }
}
