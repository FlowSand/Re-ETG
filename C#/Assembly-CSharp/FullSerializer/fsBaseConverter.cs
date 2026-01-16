// Decompiled with JetBrains decompiler
// Type: FullSerializer.fsBaseConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace FullSerializer;

public abstract class fsBaseConverter
{
  public fsSerializer Serializer;

  public virtual object CreateInstance(fsData data, Type storageType)
  {
    return !this.RequestCycleSupport(storageType) ? (object) storageType : throw new InvalidOperationException($"Please override CreateInstance for {this.GetType().FullName}; the object graph for {(object) storageType} can contain potentially contain cycles, so separated instance creation is needed");
  }

  public virtual bool RequestCycleSupport(Type storageType)
  {
    if (storageType == typeof (string))
      return false;
    return storageType.Resolve().IsClass || storageType.Resolve().IsInterface;
  }

  public virtual bool RequestInheritanceSupport(Type storageType)
  {
    return !storageType.Resolve().IsSealed;
  }

  public abstract fsResult TrySerialize(object instance, out fsData serialized, Type storageType);

  public abstract fsResult TryDeserialize(fsData data, ref object instance, Type storageType);

  protected fsResult FailExpectedType(fsData data, params fsDataType[] types)
  {
    return fsResult.Fail($"{this.GetType().Name} expected one of {string.Join(", ", ((IEnumerable<fsDataType>) types).Select<fsDataType, string>((Func<fsDataType, string>) (t => t.ToString())).ToArray<string>())} but got {(object) data.Type} in {(object) data}");
  }

  protected fsResult CheckType(fsData data, fsDataType type)
  {
    if (data.Type == type)
      return fsResult.Success;
    return fsResult.Fail($"{this.GetType().Name} expected {(object) type} but got {(object) data.Type} in {(object) data}");
  }

  protected fsResult CheckKey(fsData data, string key, out fsData subitem)
  {
    return this.CheckKey(data.AsDictionary, key, out subitem);
  }

  protected fsResult CheckKey(Dictionary<string, fsData> data, string key, out fsData subitem)
  {
    if (data.TryGetValue(key, out subitem))
      return fsResult.Success;
    return fsResult.Fail($"{this.GetType().Name} requires a <{key}> key in the data {(object) data}");
  }

  protected fsResult SerializeMember<T>(Dictionary<string, fsData> data, string name, T value)
  {
    fsData data1;
    fsResult fsResult = this.Serializer.TrySerialize(typeof (T), (object) value, out data1);
    if (fsResult.Succeeded)
      data[name] = data1;
    return fsResult;
  }

  protected fsResult DeserializeMember<T>(
    Dictionary<string, fsData> data,
    string name,
    out T value)
  {
    fsData data1;
    if (!data.TryGetValue(name, out data1))
    {
      value = default (T);
      return fsResult.Fail($"Unable to find member \"{name}\"");
    }
    object result = (object) null;
    fsResult fsResult = this.Serializer.TryDeserialize(data1, typeof (T), ref result);
    value = (T) result;
    return fsResult;
  }
}
