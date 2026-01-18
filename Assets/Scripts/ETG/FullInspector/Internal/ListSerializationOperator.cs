using System;
using System.Collections.Generic;

#nullable disable
namespace FullInspector.Internal
{
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
}
