// Decompiled with JetBrains decompiler
// Type: FullSerializer.fsObjectAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullSerializer;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class fsObjectAttribute : Attribute
{
  public Type[] PreviousModels;
  public string VersionString;
  public fsMemberSerialization MemberSerialization = fsMemberSerialization.Default;
  public Type Converter;
  public Type Processor;

  public fsObjectAttribute()
  {
  }

  public fsObjectAttribute(string versionString, params Type[] previousModels)
  {
    this.VersionString = versionString;
    this.PreviousModels = previousModels;
  }
}
