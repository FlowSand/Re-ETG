// Decompiled with JetBrains decompiler
// Type: DaikonForge.Editor.InspectorGroupOrderAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace DaikonForge.Editor
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
  public class InspectorGroupOrderAttribute : Attribute
  {
    public List<string> Groups = new List<string>();

    public InspectorGroupOrderAttribute(params string[] groups)
    {
      this.Groups.AddRange((IEnumerable<string>) groups);
    }
  }
}
