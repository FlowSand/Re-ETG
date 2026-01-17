// Decompiled with JetBrains decompiler
// Type: PathologicalGames.SpawnPoolsDict
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace PathologicalGames;

public class SpawnPoolsDict : 
  IDictionary<string, SpawnPool>,
  ICollection<KeyValuePair<string, SpawnPool>>,
  IEnumerable<KeyValuePair<string, SpawnPool>>,
  IEnumerable
{
  internal Dictionary<string, SpawnPoolsDict.OnCreatedDelegate> onCreatedDelegates = new Dictionary<string, SpawnPoolsDict.OnCreatedDelegate>();
  private Dictionary<string, SpawnPool> _pools = new Dictionary<string, SpawnPool>();

  public void AddOnCreatedDelegate(
    string poolName,
    SpawnPoolsDict.OnCreatedDelegate createdDelegate)
  {
    if (!this.onCreatedDelegates.ContainsKey(poolName))
      this.onCreatedDelegates.Add(poolName, createdDelegate);
    else
      this.onCreatedDelegates[poolName] += createdDelegate;
  }

  public void RemoveOnCreatedDelegate(
    string poolName,
    SpawnPoolsDict.OnCreatedDelegate createdDelegate)
  {
    if (!this.onCreatedDelegates.ContainsKey(poolName))
      throw new KeyNotFoundException($"No OnCreatedDelegates found for pool name '{poolName}'.");
    this.onCreatedDelegates[poolName] -= createdDelegate;
  }

  public SpawnPool Create(string poolName)
  {
    return new GameObject(poolName + "Pool").AddComponent<SpawnPool>();
  }

  public SpawnPool Create(string poolName, GameObject owner)
  {
    if (!this.assertValidPoolName(poolName))
      return (SpawnPool) null;
    string name = owner.gameObject.name;
    try
    {
      owner.gameObject.name = poolName;
      return owner.AddComponent<SpawnPool>();
    }
    finally
    {
      owner.gameObject.name = name;
    }
  }

  private bool assertValidPoolName(string poolName)
  {
    string str = poolName.Replace("Pool", string.Empty);
    if (str != poolName)
    {
      Debug.LogWarning((object) $"'{poolName}' has the word 'Pool' in it. This word is reserved for GameObject defaul naming. The pool name has been changed to '{str}'");
      poolName = str;
    }
    if (!this.ContainsKey(poolName))
      return true;
    Debug.Log((object) $"A pool with the name '{poolName}' already exists");
    return false;
  }

  public override string ToString()
  {
    string[] array = new string[this._pools.Count];
    this._pools.Keys.CopyTo(array, 0);
    return $"[{string.Join(", ", array)}]";
  }

  public bool Destroy(string poolName)
  {
    SpawnPool spawnPool;
    if (!this._pools.TryGetValue(poolName, out spawnPool))
    {
      Debug.LogError((object) $"PoolManager: Unable to destroy '{poolName}'. Not in PoolManager");
      return false;
    }
    UnityEngine.Object.Destroy((UnityEngine.Object) spawnPool.gameObject);
    return true;
  }

  public void DestroyAll()
  {
    foreach (KeyValuePair<string, SpawnPool> pool in this._pools)
      UnityEngine.Object.Destroy((UnityEngine.Object) pool.Value);
    this._pools.Clear();
  }

  internal void Add(SpawnPool spawnPool)
  {
    if (this.ContainsKey(spawnPool.poolName))
    {
      Debug.LogError((object) $"A pool with the name '{spawnPool.poolName}' already exists. This should only happen if a SpawnPool with this name is added to a scene twice.");
    }
    else
    {
      this._pools.Add(spawnPool.poolName, spawnPool);
      if (!this.onCreatedDelegates.ContainsKey(spawnPool.poolName))
        return;
      this.onCreatedDelegates[spawnPool.poolName](spawnPool);
    }
  }

  public void Add(string key, SpawnPool value)
  {
    throw new NotImplementedException("SpawnPools add themselves to PoolManager.Pools when created, so there is no need to Add() them explicitly. Create pools using PoolManager.Pools.Create() or add a SpawnPool component to a GameObject.");
  }

  internal bool Remove(SpawnPool spawnPool)
  {
    if (!this.ContainsKey(spawnPool.poolName))
    {
      Debug.LogError((object) $"PoolManager: Unable to remove '{spawnPool.poolName}'. Pool not in PoolManager");
      return false;
    }
    this._pools.Remove(spawnPool.poolName);
    return true;
  }

  public bool Remove(string poolName)
  {
    throw new NotImplementedException("SpawnPools can only be destroyed, not removed and kept alive outside of PoolManager. There are only 2 legal ways to destroy a SpawnPool: Destroy the GameObject directly, if you have a reference, or use PoolManager.Destroy(string poolName).");
  }

  public int Count => this._pools.Count;

  public bool ContainsKey(string poolName) => this._pools.ContainsKey(poolName);

  public bool TryGetValue(string poolName, out SpawnPool spawnPool)
  {
    return this._pools.TryGetValue(poolName, out spawnPool);
  }

  public bool Contains(KeyValuePair<string, SpawnPool> item)
  {
    throw new NotImplementedException("Use PoolManager.Pools.Contains(string poolName) instead.");
  }

  public SpawnPool this[string key]
  {
    get
    {
      try
      {
        return this._pools[key];
      }
      catch (KeyNotFoundException ex)
      {
        throw new KeyNotFoundException($"A Pool with the name '{key}' not found. \nPools={this.ToString()}");
      }
    }
    set
    {
      throw new NotImplementedException("Cannot set PoolManager.Pools[key] directly. SpawnPools add themselves to PoolManager.Pools when created, so there is no need to set them explicitly. Create pools using PoolManager.Pools.Create() or add a SpawnPool component to a GameObject.");
    }
  }

  public ICollection<string> Keys
  {
    get => throw new NotImplementedException("If you need this, please request it.");
  }

  public ICollection<SpawnPool> Values
  {
    get => throw new NotImplementedException("If you need this, please request it.");
  }

  private bool IsReadOnly => true;

  bool ICollection<KeyValuePair<string, SpawnPool>>.System\u002ECollections\u002EGeneric\u002EICollection<System\u002ECollections\u002EGeneric\u002EKeyValuePair<string\u002CPathologicalGames\u002ESpawnPool>>\u002EIsReadOnly
  {
    get => true;
  }

  public void Add(KeyValuePair<string, SpawnPool> item)
  {
    throw new NotImplementedException("SpawnPools add themselves to PoolManager.Pools when created, so there is no need to Add() them explicitly. Create pools using PoolManager.Pools.Create() or add a SpawnPool component to a GameObject.");
  }

  public void Clear()
  {
    throw new NotImplementedException("Use PoolManager.Pools.DestroyAll() instead.");
  }

  private void CopyTo(KeyValuePair<string, SpawnPool>[] array, int arrayIndex)
  {
    throw new NotImplementedException("PoolManager.Pools cannot be copied");
  }

  void ICollection<KeyValuePair<string, SpawnPool>>.System\u002ECollections\u002EGeneric\u002EICollection<System\u002ECollections\u002EGeneric\u002EKeyValuePair<string\u002CPathologicalGames\u002ESpawnPool>>\u002ECopyTo(
    KeyValuePair<string, SpawnPool>[] array,
    int arrayIndex)
  {
    throw new NotImplementedException("PoolManager.Pools cannot be copied");
  }

  public bool Remove(KeyValuePair<string, SpawnPool> item)
  {
    throw new NotImplementedException("SpawnPools can only be destroyed, not removed and kept alive outside of PoolManager. There are only 2 legal ways to destroy a SpawnPool: Destroy the GameObject directly, if you have a reference, or use PoolManager.Destroy(string poolName).");
  }

  public IEnumerator<KeyValuePair<string, SpawnPool>> GetEnumerator()
  {
    return (IEnumerator<KeyValuePair<string, SpawnPool>>) this._pools.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._pools.GetEnumerator();

  public delegate void OnCreatedDelegate(SpawnPool pool);
}
