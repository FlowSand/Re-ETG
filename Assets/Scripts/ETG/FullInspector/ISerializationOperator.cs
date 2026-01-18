using UnityEngine;

#nullable disable
namespace FullInspector
{
    public interface ISerializationOperator
    {
        Object RetrieveObjectReference(int storageId);

        int StoreObjectReference(Object obj);
    }
}
