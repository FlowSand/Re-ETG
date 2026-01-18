using FullSerializer.Internal;
using System;
using System.Reflection;

#nullable disable
namespace FullInspector
{
  public abstract class BaseSerializer
  {
    public abstract string Serialize(
      MemberInfo storageType,
      object value,
      ISerializationOperator serializationOperator);

    public abstract object Deserialize(
      MemberInfo storageType,
      string serializedState,
      ISerializationOperator serializationOperator);

    public virtual bool SupportsMultithreading => false;

    protected static Type GetStorageType(MemberInfo member)
    {
      if (member is FieldInfo)
        return ((FieldInfo) member).FieldType;
      if (member is PropertyInfo)
        return ((PropertyInfo) member).PropertyType;
      return fsPortableReflection.IsType(member) ? fsPortableReflection.AsType(member) : throw new InvalidOperationException("Unknown member type " + (object) member);
    }
  }
}
