// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.CullableDictionary`3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal
{
  public class CullableDictionary<TKey, TValue, TDictionary> : ICullableDictionary<TKey, TValue> where TDictionary : IDictionary<TKey, TValue>, new()
  {
    [SerializeField]
    private TDictionary _primary;
    [SerializeField]
    private TDictionary _culled;
    [SerializeField]
    private bool _isCulling;

    public CullableDictionary()
    {
      this._primary = new TDictionary();
      this._culled = new TDictionary();
    }

    public TValue this[TKey key]
    {
      get
      {
        TValue obj;
        if (!this.TryGetValue(key, out obj))
          throw new KeyNotFoundException(string.Empty + (object) key);
        return obj;
      }
      set
      {
        this._culled.Remove(key);
        this._primary[key] = value;
      }
    }

    public IEnumerable<KeyValuePair<TKey, TValue>> Items
    {
      get
      {
        CullableDictionary_TKey_TValue_TDictionary___c__Iterator0 items = new CullableDictionary_TKey_TValue_TDictionary___c__Iterator0()
        {
          _this = this
        };
        items._PC = -2;
        return (IEnumerable<KeyValuePair<TKey, TValue>>) items;
      }
    }

    public void Add(TKey key, TValue value) => this._primary.Add(key, value);

    public bool TryGetValue(TKey key, out TValue value)
    {
      if (!this._culled.TryGetValue(key, out value))
        return this._primary.TryGetValue(key, out value);
      this._culled.Remove(key);
      this._primary.Add(key, value);
      return true;
    }

    public void BeginCullZone()
    {
      if (this._isCulling)
        return;
      fiUtility.Swap<TDictionary>(ref this._primary, ref this._culled);
      this._isCulling = true;
    }

    public void EndCullZone()
    {
      if (this._isCulling)
        this._isCulling = false;
      if (fiSettings.EmitGraphMetadataCulls && this._culled.Count > 0)
      {
        foreach (KeyValuePair<TKey, TValue> keyValuePair in this._culled)
          Debug.Log((object) $"fiGraphMetadata culling \"{(object) keyValuePair.Key}\"");
      }
      this._culled.Clear();
    }

    public bool IsEmpty => this._primary.Count == 0 && this._culled.Count == 0;
  }
}
