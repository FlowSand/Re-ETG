// Decompiled with JetBrains decompiler
// Type: InControl.SingletonPrefabAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace InControl
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true)]
  public class SingletonPrefabAttribute : Attribute
  {
    public readonly string Name;

    public SingletonPrefabAttribute(string name) => this.Name = name;
  }
}
