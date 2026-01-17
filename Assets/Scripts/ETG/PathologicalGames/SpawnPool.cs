// Decompiled with JetBrains decompiler
// Type: PathologicalGames.SpawnPool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace PathologicalGames;

[AddComponentMenu("Path-o-logical/PoolManager/SpawnPool")]
public sealed class SpawnPool : 
  MonoBehaviour,
  IList<Transform>,
  ICollection<Transform>,
  IEnumerable<Transform>,
  IEnumerable
{
  public string poolName = string.Empty;
  public bool matchPoolScale;
  public bool matchPoolLayer;
  public bool dontReparent;
  public bool _dontDestroyOnLoad;
  public bool logMessages;
  public List<PrefabPool> _perPrefabPoolOptions = new List<PrefabPool>();
  public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();
  public float maxParticleDespawnTime = 300f;
  public PrefabsDict prefabs = new PrefabsDict();
  public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();
  private List<PrefabPool> _prefabPools = new List<PrefabPool>();
  internal List<Transform> _spawned = new List<Transform>();

  public bool dontDestroyOnLoad
  {
    get => this._dontDestroyOnLoad;
    set
    {
      this._dontDestroyOnLoad = value;
      if (!((UnityEngine.Object) this.group != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.group.gameObject);
    }
  }

  public Transform group { get; private set; }

  public Dictionary<string, PrefabPool> prefabPools
  {
    get
    {
      Dictionary<string, PrefabPool> prefabPools = new Dictionary<string, PrefabPool>();
      for (int index = 0; index < this._prefabPools.Count; ++index)
        prefabPools[this._prefabPools[index].prefabGO.name] = this._prefabPools[index];
      return prefabPools;
    }
  }

  private void Awake()
  {
    if (this._dontDestroyOnLoad)
      UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
    this.group = this.transform;
    if (this.poolName == string.Empty)
    {
      this.poolName = this.group.name.Replace("Pool", string.Empty);
      this.poolName = this.poolName.Replace("(Clone)", string.Empty);
    }
    if (this.logMessages)
      UnityEngine.Debug.Log((object) $"SpawnPool {this.poolName}: Initializing..");
    for (int index = 0; index < this._perPrefabPoolOptions.Count; ++index)
    {
      if ((UnityEngine.Object) this._perPrefabPoolOptions[index].prefab == (UnityEngine.Object) null)
      {
        UnityEngine.Debug.LogWarning((object) $"Initialization Warning: Pool '{this.poolName}' contains a PrefabPool with no prefab reference. Skipping.");
      }
      else
      {
        this._perPrefabPoolOptions[index].inspectorInstanceConstructor();
        this.CreatePrefabPool(this._perPrefabPoolOptions[index]);
      }
    }
    PoolManager.Pools.Add(this);
  }

  private void OnDestroy()
  {
    if (this.logMessages)
      UnityEngine.Debug.Log((object) $"SpawnPool {this.poolName}: Destroying...");
    PoolManager.Pools.Remove(this);
    this.StopAllCoroutines();
    this._spawned.Clear();
    foreach (PrefabPool prefabPool in this._prefabPools)
      prefabPool.SelfDestruct();
    this._prefabPools.Clear();
    this.prefabs._Clear();
  }

  public void CreatePrefabPool(PrefabPool prefabPool)
  {
    if (this.GetPrefabPool(prefabPool.prefab) == null)
    {
      prefabPool.spawnPool = this;
      this._prefabPools.Add(prefabPool);
      if (this.prefabs.ContainsKey(prefabPool.prefab.name))
        UnityEngine.Debug.LogError((object) ("Duplicate prefab name: " + prefabPool.prefab.name));
      else
        this.prefabs._Add(prefabPool.prefab.name, prefabPool.prefab);
    }
    if (prefabPool.preloaded)
      return;
    if (this.logMessages)
      UnityEngine.Debug.Log((object) $"SpawnPool {this.poolName}: Preloading {prefabPool.preloadAmount} {prefabPool.prefab.name}");
    prefabPool.PreloadInstances();
  }

  public void Add(Transform instance, string prefabName, bool despawn, bool parent)
  {
    for (int index = 0; index < this._prefabPools.Count; ++index)
    {
      if ((UnityEngine.Object) this._prefabPools[index].prefabGO == (UnityEngine.Object) null)
      {
        UnityEngine.Debug.LogError((object) "Unexpected Error: PrefabPool.prefabGO is null");
        return;
      }
      if (this._prefabPools[index].prefabGO.name == prefabName)
      {
        this._prefabPools[index].AddUnpooled(instance, despawn);
        if (this.logMessages)
          UnityEngine.Debug.Log((object) $"SpawnPool {this.poolName}: Adding previously unpooled instance {instance.name}");
        if (parent)
          instance.parent = this.group;
        if (despawn)
          return;
        this._spawned.Add(instance);
        return;
      }
    }
    UnityEngine.Debug.LogError((object) $"SpawnPool {this.poolName}: PrefabPool {prefabName} not found.");
  }

  public void Add(Transform item)
  {
    throw new NotImplementedException("Use SpawnPool.Spawn() to properly add items to the pool.");
  }

  public void Remove(Transform item)
  {
    for (int index = 0; index < this._prefabPools.Count; ++index)
    {
      if (this._prefabPools[index]._spawned.Contains(item) || this._prefabPools[index]._despawned.Contains(item))
        this._prefabPools[index].RemoveInstance(item);
    }
    this._spawned.Remove(item);
  }

  public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, Transform parent)
  {
    for (int index = 0; index < this._prefabPools.Count; ++index)
    {
      if ((UnityEngine.Object) this._prefabPools[index].prefabGO == (UnityEngine.Object) prefab.gameObject)
      {
        Transform transform = this._prefabPools[index].SpawnInstance(pos, rot);
        if ((UnityEngine.Object) transform == (UnityEngine.Object) null)
          return (Transform) null;
        if ((UnityEngine.Object) parent != (UnityEngine.Object) null)
          transform.parent = parent;
        else if (!this.dontReparent && (UnityEngine.Object) transform.parent != (UnityEngine.Object) this.group)
          transform.parent = this.group;
        this._spawned.Add(transform);
        transform.gameObject.BroadcastMessage("OnSpawned", (object) this, SendMessageOptions.DontRequireReceiver);
        return transform;
      }
    }
    PrefabPool prefabPool = new PrefabPool(prefab);
    this.CreatePrefabPool(prefabPool);
    Transform transform1 = prefabPool.SpawnInstance(pos, rot);
    transform1.parent = !((UnityEngine.Object) parent != (UnityEngine.Object) null) ? this.group : parent;
    this._spawned.Add(transform1);
    transform1.gameObject.BroadcastMessage("OnSpawned", (object) this, SendMessageOptions.DontRequireReceiver);
    return transform1;
  }

  public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
  {
    Transform transform = this.Spawn(prefab, pos, rot, (Transform) null);
    return (UnityEngine.Object) transform == (UnityEngine.Object) null ? (Transform) null : transform;
  }

  public Transform Spawn(Transform prefab) => this.Spawn(prefab, Vector3.zero, Quaternion.identity);

  public Transform Spawn(Transform prefab, Transform parent)
  {
    return this.Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
  }

  public Transform Spawn(string prefabName) => this.Spawn(this.prefabs[prefabName]);

  public Transform Spawn(string prefabName, Transform parent)
  {
    return this.Spawn(this.prefabs[prefabName], parent);
  }

  public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot)
  {
    return this.Spawn(this.prefabs[prefabName], pos, rot);
  }

  public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot, Transform parent)
  {
    return this.Spawn(this.prefabs[prefabName], pos, rot, parent);
  }

  public AudioSource Spawn(AudioSource prefab, Vector3 pos, Quaternion rot)
  {
    return this.Spawn(prefab, pos, rot, (Transform) null);
  }

  public AudioSource Spawn(AudioSource prefab)
  {
    return this.Spawn(prefab, Vector3.zero, Quaternion.identity, (Transform) null);
  }

  public AudioSource Spawn(AudioSource prefab, Transform parent)
  {
    return this.Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
  }

  public AudioSource Spawn(AudioSource prefab, Vector3 pos, Quaternion rot, Transform parent)
  {
    Transform transform = this.Spawn(prefab.transform, pos, rot, parent);
    if ((UnityEngine.Object) transform == (UnityEngine.Object) null)
      return (AudioSource) null;
    AudioSource component = transform.GetComponent<AudioSource>();
    component.Play();
    this.StartCoroutine(this.ListForAudioStop(component));
    return component;
  }

  public ParticleSystem Spawn(ParticleSystem prefab, Vector3 pos, Quaternion rot)
  {
    return this.Spawn(prefab, pos, rot, (Transform) null);
  }

  public ParticleSystem Spawn(
    ParticleSystem prefab,
    Vector3 pos,
    Quaternion rot,
    Transform parent)
  {
    Transform transform = this.Spawn(prefab.transform, pos, rot, parent);
    if ((UnityEngine.Object) transform == (UnityEngine.Object) null)
      return (ParticleSystem) null;
    ParticleSystem component = transform.GetComponent<ParticleSystem>();
    this.StartCoroutine(this.ListenForEmitDespawn(component));
    return component;
  }

  public void Despawn(Transform instance, PrefabPool prefabPool = null)
  {
    bool flag = false;
    if (prefabPool != null)
    {
      if (prefabPool._spawned.Contains(instance))
        flag = prefabPool.DespawnInstance(instance);
      else if (prefabPool._despawned.Contains(instance))
      {
        UnityEngine.Debug.LogError((object) $"SpawnPool {this.poolName}: {instance.name} has already been despawned. You cannot despawn something more than once!");
        return;
      }
    }
    if (!flag)
    {
      for (int index = 0; index < this._prefabPools.Count; ++index)
      {
        if (this._prefabPools[index]._spawned.Contains(instance))
        {
          flag = this._prefabPools[index].DespawnInstance(instance);
          break;
        }
        if (this._prefabPools[index]._despawned.Contains(instance))
        {
          UnityEngine.Debug.LogError((object) $"SpawnPool {this.poolName}: {instance.name} has already been despawned. You cannot despawn something more than once!");
          return;
        }
      }
    }
    if (!flag)
      UnityEngine.Debug.LogError((object) $"SpawnPool {this.poolName}: {instance.name} not found in SpawnPool");
    else
      this._spawned.Remove(instance);
  }

  public void Despawn(Transform instance, Transform parent)
  {
    instance.parent = parent;
    this.Despawn(instance);
  }

  public void Despawn(Transform instance, float seconds)
  {
    this.StartCoroutine(this.DoDespawnAfterSeconds(instance, seconds, false, (Transform) null));
  }

  public void Despawn(Transform instance, float seconds, Transform parent)
  {
    this.StartCoroutine(this.DoDespawnAfterSeconds(instance, seconds, true, parent));
  }

  [DebuggerHidden]
  private IEnumerator DoDespawnAfterSeconds(
    Transform instance,
    float seconds,
    bool useParent,
    Transform parent)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new SpawnPool.<DoDespawnAfterSeconds>c__Iterator1()
    {
      instance = instance,
      seconds = seconds,
      useParent = useParent,
      parent = parent,
      $this = this
    };
  }

  public void DespawnAll()
  {
    List<Transform> transformList = new List<Transform>((IEnumerable<Transform>) this._spawned);
    for (int index = 0; index < transformList.Count; ++index)
      this.Despawn(transformList[index]);
  }

  public bool IsSpawned(Transform instance) => this._spawned.Contains(instance);

  public PrefabPool GetPrefabPool(Transform prefab)
  {
    for (int index = 0; index < this._prefabPools.Count; ++index)
    {
      if ((UnityEngine.Object) this._prefabPools[index].prefabGO == (UnityEngine.Object) null)
        UnityEngine.Debug.LogError((object) $"SpawnPool {this.poolName}: PrefabPool.prefabGO is null");
      if ((UnityEngine.Object) this._prefabPools[index].prefabGO == (UnityEngine.Object) prefab.gameObject)
        return this._prefabPools[index];
    }
    return (PrefabPool) null;
  }

  public PrefabPool GetPrefabPool(GameObject prefab)
  {
    for (int index = 0; index < this._prefabPools.Count; ++index)
    {
      if ((UnityEngine.Object) this._prefabPools[index].prefabGO == (UnityEngine.Object) null)
        UnityEngine.Debug.LogError((object) $"SpawnPool {this.poolName}: PrefabPool.prefabGO is null");
      if ((UnityEngine.Object) this._prefabPools[index].prefabGO == (UnityEngine.Object) prefab)
        return this._prefabPools[index];
    }
    return (PrefabPool) null;
  }

  public Transform GetPrefab(Transform instance)
  {
    for (int index = 0; index < this._prefabPools.Count; ++index)
    {
      if (this._prefabPools[index].Contains(instance))
        return this._prefabPools[index].prefab;
    }
    return (Transform) null;
  }

  public GameObject GetPrefab(GameObject instance)
  {
    for (int index = 0; index < this._prefabPools.Count; ++index)
    {
      if (this._prefabPools[index].Contains(instance.transform))
        return this._prefabPools[index].prefabGO;
    }
    return (GameObject) null;
  }

  [DebuggerHidden]
  private IEnumerator ListForAudioStop(AudioSource src)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new SpawnPool.<ListForAudioStop>c__Iterator2()
    {
      src = src,
      $this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator ListenForEmitDespawn(ParticleSystem emitter)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new SpawnPool.<ListenForEmitDespawn>c__Iterator3()
    {
      emitter = emitter,
      $this = this
    };
  }

  public override string ToString()
  {
    List<string> stringList = new List<string>();
    foreach (Transform transform in this._spawned)
      stringList.Add(transform.name);
    return string.Join(", ", stringList.ToArray());
  }

  public Transform this[int index]
  {
    get => this._spawned[index];
    set => throw new NotImplementedException("Read-only.");
  }

  public bool Contains(Transform item)
  {
    throw new NotImplementedException("Use IsSpawned(Transform instance) instead.");
  }

  public void CopyTo(Transform[] array, int arrayIndex) => this._spawned.CopyTo(array, arrayIndex);

  public int Count => this._spawned.Count;

  [DebuggerHidden]
  public IEnumerator<Transform> GetEnumerator()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<Transform>) new SpawnPool.<GetEnumerator>c__Iterator4()
    {
      $this = this
    };
  }

  [DebuggerHidden]
  IEnumerator IEnumerable.GetEnumerator()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new SpawnPool.<System_Collections_IEnumerable_GetEnumerator>c__Iterator0()
    {
      $this = this
    };
  }

  public int IndexOf(Transform item) => throw new NotImplementedException();

  public void Insert(int index, Transform item) => throw new NotImplementedException();

  public void RemoveAt(int index) => throw new NotImplementedException();

  public void Clear() => throw new NotImplementedException();

  public bool IsReadOnly => throw new NotImplementedException();

  bool ICollection<Transform>.Remove(Transform item) => throw new NotImplementedException();
}
