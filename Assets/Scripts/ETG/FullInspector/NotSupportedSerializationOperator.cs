using System;

#nullable disable
namespace FullInspector
{
  public class NotSupportedSerializationOperator : ISerializationOperator
  {
    public UnityEngine.Object RetrieveObjectReference(int storageId)
    {
      throw new NotSupportedException("UnityEngine.Object references are not supported with this serialization operator");
    }

    public int StoreObjectReference(UnityEngine.Object obj)
    {
      throw new NotSupportedException($"UnityEngine.Object references are not supported with this serialization operator (obj={(object) obj})");
    }
  }
}
