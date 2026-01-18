using FullInspector.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullInspector
{
  [AddComponentMenu("")]
  public abstract class fiBaseStorageComponent<T> : 
    MonoBehaviour,
    fiIEditorOnlyTag,
    ISerializationCallbackReceiver
  {
    [SerializeField]
    private List<UnityEngine.Object> _keys;
    [SerializeField]
    private List<T> _values;
    private IDictionary<UnityEngine.Object, T> _data;

    public IDictionary<UnityEngine.Object, T> Data
    {
      get
      {
        if (this._data == null)
          this._data = (IDictionary<UnityEngine.Object, T>) new Dictionary<UnityEngine.Object, T>();
        return this._data;
      }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
      if (this._keys == null || this._values == null)
        return;
      this._data = (IDictionary<UnityEngine.Object, T>) new Dictionary<UnityEngine.Object, T>();
      for (int index = 0; index < Math.Min(this._keys.Count, this._values.Count); ++index)
      {
        if (!object.ReferenceEquals((object) this._keys[index], (object) null))
          this.Data[this._keys[index]] = this._values[index];
      }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
      if (this._data == null)
      {
        this._keys = (List<UnityEngine.Object>) null;
        this._values = (List<T>) null;
      }
      else
      {
        this._keys = new List<UnityEngine.Object>(this._data.Count);
        this._values = new List<T>(this._data.Count);
        foreach (KeyValuePair<UnityEngine.Object, T> keyValuePair in (IEnumerable<KeyValuePair<UnityEngine.Object, T>>) this._data)
        {
          this._keys.Add(keyValuePair.Key);
          this._values.Add(keyValuePair.Value);
        }
      }
    }
  }
}
