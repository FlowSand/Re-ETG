using UnityEngine;

#nullable disable
namespace FullInspector.Internal
{
    public class fiGraphMetadataSerializer<TPersistentData> : 
        fiIGraphMetadataStorage,
        ISerializationCallbackReceiver
        where TPersistentData : IGraphMetadataItemPersistent
    {
        [SerializeField]
        private string[] _keys;
        [SerializeField]
        private TPersistentData[] _values;
        [SerializeField]
        private Object _target;

        public void RestoreData(Object target)
        {
            this._target = target;
            if (this._keys == null || this._values == null)
                return;
            fiPersistentMetadata.GetMetadataFor(this._target).Deserialize<TPersistentData>(this._keys, this._values);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (this._target == (Object) null)
                return;
            fiGraphMetadata metadataFor = fiPersistentMetadata.GetMetadataFor(this._target);
            if (!metadataFor.ShouldSerialize())
                return;
            metadataFor.Serialize<TPersistentData>(out this._keys, out this._values);
        }
    }
}
