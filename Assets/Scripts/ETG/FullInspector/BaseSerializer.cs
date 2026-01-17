// Decompiled with JetBrains decompiler
// Type: FullInspector.BaseSerializer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
