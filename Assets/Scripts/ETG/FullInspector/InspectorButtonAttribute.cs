// Decompiled with JetBrains decompiler
// Type: FullInspector.InspectorButtonAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector
{
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class InspectorButtonAttribute : Attribute
  {
    [Obsolete("Please use InspectorName to get the custom name of the button")]
    public string DisplayName;

    public InspectorButtonAttribute()
    {
    }

    [Obsolete("Please use InspectorName to set the name of the button")]
    public InspectorButtonAttribute(string displayName)
    {
    }
  }
}
