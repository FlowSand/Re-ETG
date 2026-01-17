// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiGraphMetadataSerializer`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
