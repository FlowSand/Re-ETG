// Decompiled with JetBrains decompiler
// Type: FullSerializer.Internal.fsTypeConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullSerializer.Internal;

public class fsTypeConverter : fsConverter
{
  public override bool CanProcess(Type type) => typeof (Type).IsAssignableFrom(type);

  public override bool RequestCycleSupport(Type type) => false;

  public override bool RequestInheritanceSupport(Type type) => false;

  public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
  {
    Type type = (Type) instance;
    serialized = new fsData(type.FullName);
    return fsResult.Success;
  }

  public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
  {
    if (!data.IsString)
      return fsResult.Fail("Type converter requires a string");
    instance = (object) fsTypeLookup.GetType(data.AsString);
    return instance == null ? fsResult.Fail("Unable to find type " + data.AsString) : fsResult.Success;
  }

  public override object CreateInstance(fsData data, Type storageType) => (object) storageType;
}
