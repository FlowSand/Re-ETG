// Decompiled with JetBrains decompiler
// Type: FullSerializer.fsMissingVersionConstructorException
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullSerializer;

public sealed class fsMissingVersionConstructorException(Type versionedType, Type constructorType) : 
  Exception($"{(object) versionedType} is missing a constructor for previous model type {(object) constructorType}")
{
}
