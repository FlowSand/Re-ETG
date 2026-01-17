// Decompiled with JetBrains decompiler
// Type: FullSerializer.fsObjectProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullSerializer;

public abstract class fsObjectProcessor
{
  public virtual bool CanProcess(Type type) => throw new NotImplementedException();

  public virtual void OnBeforeSerialize(Type storageType, object instance)
  {
  }

  public virtual void OnAfterSerialize(Type storageType, object instance, ref fsData data)
  {
  }

  public virtual void OnBeforeDeserialize(Type storageType, ref fsData data)
  {
  }

  public virtual void OnBeforeDeserializeAfterInstanceCreation(
    Type storageType,
    object instance,
    ref fsData data)
  {
  }

  public virtual void OnAfterDeserialize(Type storageType, object instance)
  {
  }
}
