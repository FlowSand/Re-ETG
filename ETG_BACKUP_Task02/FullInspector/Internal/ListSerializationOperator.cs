// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.ListSerializationOperator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace FullInspector.Internal;

public class ListSerializationOperator : ISerializationOperator
{
  public List<UnityEngine.Object> SerializedObjects;

  public UnityEngine.Object RetrieveObjectReference(int storageId)
  {
    if (this.SerializedObjects == null)
      throw new InvalidOperationException("SerializedObjects cannot be  null");
    return storageId < 0 || storageId >= this.SerializedObjects.Count ? (UnityEngine.Object) null : this.SerializedObjects[storageId];
  }

  public int StoreObjectReference(UnityEngine.Object obj)
  {
    if (this.SerializedObjects == null)
      throw new InvalidOperationException("SerializedObjects cannot be null");
    if (object.ReferenceEquals((object) obj, (object) null))
      return -1;
    int count = this.SerializedObjects.Count;
    this.SerializedObjects.Add(obj);
    return count;
  }
}
