// Decompiled with JetBrains decompiler
// Type: FullSerializer.fsPropertyAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullSerializer
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class fsPropertyAttribute : Attribute
  {
    public string Name;
    public bool DeserializeOnly;

    public fsPropertyAttribute()
      : this(string.Empty)
    {
    }

    public fsPropertyAttribute(string name) => this.Name = name;

    public fsPropertyAttribute(bool deserializeOnly) => this.DeserializeOnly = deserializeOnly;
  }
}
