// Decompiled with JetBrains decompiler
// Type: PathologicalGames.PrefabsDict
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace PathologicalGames;

public class PrefabsDict : 
  IDictionary<string, Transform>,
  ICollection<KeyValuePair<string, Transform>>,
  IEnumerable<KeyValuePair<string, Transform>>,
  IEnumerable
{
  private Dictionary<string, Transform> _prefabs = new Dictionary<string, Transform>();

  public override string ToString()
  {
    string[] array = new string[this._prefabs.Count];
    this._prefabs.Keys.CopyTo(array, 0);
    return $"[{string.Join(", ", array)}]";
  }

  internal void _Add(string prefabName, Transform prefab) => this._prefabs.Add(prefabName, prefab);

  internal bool _Remove(string prefabName) => this._prefabs.Remove(prefabName);

  internal void _Clear() => this._prefabs.Clear();

  public int Count => this._prefabs.Count;

  public bool ContainsKey(string prefabName) => this._prefabs.ContainsKey(prefabName);

  public bool TryGetValue(string prefabName, out Transform prefab)
  {
    return this._prefabs.TryGetValue(prefabName, out prefab);
  }

  public void Add(string key, Transform value) => throw new NotImplementedException("Read-Only");

  public bool Remove(string prefabName) => throw new NotImplementedException("Read-Only");

  public bool Contains(KeyValuePair<string, Transform> item)
  {
    throw new NotImplementedException("Use Contains(string prefabName) instead.");
  }

  public Transform this[string key]
  {
    get
    {
      try
      {
        return this._prefabs[key];
      }
      catch (KeyNotFoundException ex)
      {
        throw new KeyNotFoundException($"A Prefab with the name '{key}' not found. \nPrefabs={this.ToString()}");
      }
    }
    set => throw new NotImplementedException("Read-only.");
  }

  public ICollection<string> Keys => (ICollection<string>) this._prefabs.Keys;

  public ICollection<Transform> Values => (ICollection<Transform>) this._prefabs.Values;

  private bool IsReadOnly => true;

  bool ICollection<KeyValuePair<string, Transform>>.System\u002ECollections\u002EGeneric\u002EICollection<System\u002ECollections\u002EGeneric\u002EKeyValuePair<string\u002CUnityEngine\u002ETransform>>\u002EIsReadOnly
  {
    get => true;
  }

  public void Add(KeyValuePair<string, Transform> item)
  {
    throw new NotImplementedException("Read-only");
  }

  public void Clear() => throw new NotImplementedException();

  private void CopyTo(KeyValuePair<string, Transform>[] array, int arrayIndex)
  {
    throw new NotImplementedException("Cannot be copied");
  }

  void ICollection<KeyValuePair<string, Transform>>.System\u002ECollections\u002EGeneric\u002EICollection<System\u002ECollections\u002EGeneric\u002EKeyValuePair<string\u002CUnityEngine\u002ETransform>>\u002ECopyTo(
    KeyValuePair<string, Transform>[] array,
    int arrayIndex)
  {
    throw new NotImplementedException("Cannot be copied");
  }

  public bool Remove(KeyValuePair<string, Transform> item)
  {
    throw new NotImplementedException("Read-only");
  }

  public IEnumerator<KeyValuePair<string, Transform>> GetEnumerator()
  {
    return (IEnumerator<KeyValuePair<string, Transform>>) this._prefabs.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._prefabs.GetEnumerator();
}
