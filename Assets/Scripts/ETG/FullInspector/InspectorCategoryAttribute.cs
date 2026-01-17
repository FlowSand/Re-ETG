// Decompiled with JetBrains decompiler
// Type: FullInspector.InspectorCategoryAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
  public sealed class InspectorCategoryAttribute : Attribute
  {
    public string Category;

    public InspectorCategoryAttribute(string category) => this.Category = category;
  }
}
