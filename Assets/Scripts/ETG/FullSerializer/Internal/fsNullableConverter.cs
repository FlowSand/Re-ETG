// Decompiled with JetBrains decompiler
// Type: FullSerializer.Internal.fsNullableConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullSerializer.Internal
{
  public class fsNullableConverter : fsConverter
  {
    public override bool CanProcess(Type type)
    {
      return type.Resolve().IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
    }

    public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
    {
      return this.Serializer.TrySerialize(Nullable.GetUnderlyingType(storageType), instance, out serialized);
    }

    public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
    {
      return this.Serializer.TryDeserialize(data, Nullable.GetUnderlyingType(storageType), ref instance);
    }

    public override object CreateInstance(fsData data, Type storageType) => (object) storageType;
  }
}
