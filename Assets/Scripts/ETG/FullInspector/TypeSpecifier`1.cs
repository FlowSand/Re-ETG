// Decompiled with JetBrains decompiler
// Type: FullInspector.TypeSpecifier`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector
{
  public class TypeSpecifier<TBaseType>
  {
    public Type Type;

    public TypeSpecifier()
    {
    }

    public TypeSpecifier(Type type) => this.Type = type;

    public static implicit operator Type(TypeSpecifier<TBaseType> specifier) => specifier.Type;

    public static implicit operator TypeSpecifier<TBaseType>(Type type)
    {
      return new TypeSpecifier<TBaseType>() { Type = type };
    }

    public override bool Equals(object obj)
    {
      return obj is TypeSpecifier<TBaseType> typeSpecifier && this.Type == typeSpecifier.Type;
    }

    public override int GetHashCode() => this.Type.GetHashCode();
  }
}
