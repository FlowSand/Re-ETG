// Decompiled with JetBrains decompiler
// Type: FullInspector.InspectorCollectionPagerAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class InspectorCollectionPagerAttribute : Attribute
  {
    public int PageMinimumCollectionLength;

    public InspectorCollectionPagerAttribute()
    {
      this.PageMinimumCollectionLength = fiSettings.DefaultPageMinimumCollectionLength;
    }

    public InspectorCollectionPagerAttribute(int pageMinimumCollectionLength)
    {
      this.PageMinimumCollectionLength = pageMinimumCollectionLength;
    }

    public bool AlwaysHide
    {
      set
      {
        if (value)
          this.PageMinimumCollectionLength = -1;
        else
          this.PageMinimumCollectionLength = fiSettings.DefaultPageMinimumCollectionLength;
      }
      get => this.PageMinimumCollectionLength < 0;
    }

    public bool AlwaysShow
    {
      set
      {
        if (value)
          this.PageMinimumCollectionLength = 0;
        else
          this.PageMinimumCollectionLength = fiSettings.DefaultPageMinimumCollectionLength;
      }
      get => this.PageMinimumCollectionLength == 0;
    }
  }
}
