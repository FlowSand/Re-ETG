// Decompiled with JetBrains decompiler
// Type: dfHelpAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public class dfHelpAttribute : Attribute
{
  public dfHelpAttribute(string url) => this.HelpURL = url;

  public string HelpURL { get; private set; }
}
