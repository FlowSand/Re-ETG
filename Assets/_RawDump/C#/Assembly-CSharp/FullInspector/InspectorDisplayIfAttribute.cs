// Decompiled with JetBrains decompiler
// Type: FullInspector.InspectorDisplayIfAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector;

[Obsolete("Please use InspectorShowIfAttribute instead")]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class InspectorDisplayIfAttribute : Attribute
{
  public string ConditionalMemberName;

  public InspectorDisplayIfAttribute(string conditionalMemberName)
  {
    this.ConditionalMemberName = conditionalMemberName;
  }
}
