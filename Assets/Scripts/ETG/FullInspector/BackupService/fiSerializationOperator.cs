// Decompiled with JetBrains decompiler
// Type: FullInspector.BackupService.fiSerializationOperator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Internal;
using System;
using System.Collections.Generic;

#nullable disable
namespace FullInspector.BackupService
{
  public class fiSerializationOperator : ISerializationOperator
  {
    public List<fiUnityObjectReference> SerializedObjects;

    public UnityEngine.Object RetrieveObjectReference(int storageId)
    {
      if (this.SerializedObjects == null)
        throw new InvalidOperationException("SerializedObjects cannot be  null");
      if (storageId < 0 || storageId >= this.SerializedObjects.Count)
        return (UnityEngine.Object) null;
      fiUnityObjectReference serializedObject = this.SerializedObjects[storageId];
      return serializedObject == null || serializedObject.Target == (UnityEngine.Object) null ? (UnityEngine.Object) null : serializedObject.Target;
    }

    public int StoreObjectReference(UnityEngine.Object obj)
    {
      if (this.SerializedObjects == null)
        throw new InvalidOperationException("SerializedObjects cannot be null");
      if (obj == (UnityEngine.Object) null)
        return -1;
      int count = this.SerializedObjects.Count;
      this.SerializedObjects.Add(new fiUnityObjectReference(obj));
      return count;
    }
  }
}
