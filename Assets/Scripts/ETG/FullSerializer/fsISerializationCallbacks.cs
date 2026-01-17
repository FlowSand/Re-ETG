// Decompiled with JetBrains decompiler
// Type: FullSerializer.fsISerializationCallbacks
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullSerializer;

public interface fsISerializationCallbacks
{
  void OnBeforeSerialize(Type storageType);

  void OnAfterSerialize(Type storageType, ref fsData data);

  void OnBeforeDeserialize(Type storageType, ref fsData data);

  void OnAfterDeserialize(Type storageType);
}
