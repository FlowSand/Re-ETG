// Decompiled with JetBrains decompiler
// Type: FullSerializer.Internal.fsVersionedType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullSerializer.Internal;

public struct fsVersionedType
{
  public fsVersionedType[] Ancestors;
  public string VersionString;
  public Type ModelType;

  public object Migrate(object ancestorInstance)
  {
    return Activator.CreateInstance(this.ModelType, ancestorInstance);
  }

  public override string ToString()
  {
    return $"fsVersionedType [ModelType={(object) this.ModelType}, VersionString={this.VersionString}, Ancestors.Length={(object) this.Ancestors.Length}]";
  }

  public static bool operator ==(fsVersionedType a, fsVersionedType b)
  {
    return a.ModelType == b.ModelType;
  }

  public static bool operator !=(fsVersionedType a, fsVersionedType b)
  {
    return a.ModelType != b.ModelType;
  }

  public override bool Equals(object obj)
  {
    return obj is fsVersionedType fsVersionedType && this.ModelType == fsVersionedType.ModelType;
  }

  public override int GetHashCode() => this.ModelType.GetHashCode();
}
