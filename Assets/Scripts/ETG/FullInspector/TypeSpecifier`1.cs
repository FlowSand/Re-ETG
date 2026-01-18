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
