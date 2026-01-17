// Decompiled with JetBrains decompiler
// Type: FullSerializer.Internal.fsGuidConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullSerializer.Internal
{
  public class fsGuidConverter : fsConverter
  {
    public override bool CanProcess(Type type) => type == typeof (Guid);

    public override bool RequestCycleSupport(Type storageType) => false;

    public override bool RequestInheritanceSupport(Type storageType) => false;

    public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
    {
      Guid guid = (Guid) instance;
      serialized = new fsData(guid.ToString());
      return fsResult.Success;
    }

    public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
    {
      if (!data.IsString)
        return fsResult.Fail("fsGuidConverter encountered an unknown JSON data type");
      instance = (object) new Guid(data.AsString);
      return fsResult.Success;
    }

    public override object CreateInstance(fsData data, Type storageType) => (object) new Guid();
  }
}
