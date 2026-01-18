using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Object Pooling/Object Pool Manager")]
[Serializable]
public class dfPoolManager : MonoBehaviour, ILevelLoadedListener
  {
    public bool AutoPreload = true;
    public bool PreloadInBackground = true;
    [SerializeField]
    private List<dfPoolManager.ObjectPool> objectPools = new List<dfPoolManager.ObjectPool>();
    private bool poolsPreloaded;

    public event dfPoolManager.PoolManagerLoadingEvent LoadingStarted;

    public event dfPoolManager.PoolManagerLoadingEvent LoadingComplete;

    public event dfPoolManager.PoolManagerProgressEvent LoadingProgress;

    public static dfPoolManager Pool { get; private set; }

    private void Awake()
    {
      dfPoolManager.Pool = !((UnityEngine.Object) dfPoolManager.Pool != (UnityEngine.Object) null) ? this : throw new Exception($"Cannot have more than one instance of the {this.GetType().Name} class");
      if (!this.AutoPreload)
        return;
      this.Preload();
    }

    private void OnDestroy() => this.ClearAllPools();

    public void BraveOnLevelWasLoaded() => this.ClearAllPools();

    public void ClearAllPools()
    {
      this.poolsPreloaded = false;
      for (int index = 0; index < this.objectPools.Count; ++index)
        this.objectPools[index].Clear();
    }

    public void Preload()
    {
      if (this.poolsPreloaded)
        return;
      if (this.PreloadInBackground)
      {
        this.StartCoroutine(this.preloadPools());
      }
      else
      {
        IEnumerator enumerator = this.preloadPools();
        while (enumerator.MoveNext())
        {
          object current = enumerator.Current;
        }
      }
    }

    public void AddPool(string name, GameObject prefab)
    {
      if (this.objectPools.Any<dfPoolManager.ObjectPool>((Func<dfPoolManager.ObjectPool, bool>) (p => p.PoolName == name)))
        throw new Exception("Duplicate key: " + name);
      if (prefab.activeSelf)
        prefab.SetActive(false);
      this.objectPools.Add(new dfPoolManager.ObjectPool()
      {
        Prefab = prefab,
        PoolName = name
      });
    }

    [DebuggerHidden]
    private IEnumerator preloadPools()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new dfPoolManager__preloadPoolsc__Iterator0()
      {
        _this = this
      };
    }

    public dfPoolManager.ObjectPool this[string name]
    {
      get
      {
        for (int index = 0; index < this.objectPools.Count; ++index)
        {
          if (this.objectPools[index].PoolName == name)
            return this.objectPools[index];
        }
        throw new KeyNotFoundException("Object pool not found: " + name);
      }
    }

    public enum LimitReachedAction
    {
      Nothing,
      Error,
      Recycle,
    }

    public delegate void PoolManagerLoadingEvent();

    public delegate void PoolManagerProgressEvent(int TotalItems, int Current);

    [Serializable]
    public class ObjectPool
    {
      private dfList<GameObject> pool = dfList<GameObject>.Obtain();
      private dfList<GameObject> spawned = dfList<GameObject>.Obtain();
      [SerializeField]
      private string poolName = string.Empty;
      [SerializeField]
      private dfPoolManager.LimitReachedAction limitType;
      [SerializeField]
      private GameObject prefab;
      [SerializeField]
      private int maxInstances = -1;
      [SerializeField]
      private int initialPoolSize;
      [SerializeField]
      private bool allowReparenting = true;

      public string PoolName
      {
        get => this.poolName;
        set => this.poolName = value;
      }

      public dfPoolManager.LimitReachedAction LimitReached
      {
        get => this.limitType;
        set => this.limitType = value;
      }

      public GameObject Prefab
      {
        get => this.prefab;
        set => this.prefab = value;
      }

      public int MaxInstances
      {
        get => this.maxInstances;
        set => this.maxInstances = value;
      }

      public int InitialPoolSize
      {
        get => this.initialPoolSize;
        set => this.initialPoolSize = value;
      }

      public bool AllowReparenting
      {
        get => this.allowReparenting;
        set => this.allowReparenting = value;
      }

      public int Available
      {
        get => this.maxInstances == -1 ? int.MaxValue : Mathf.Max(this.pool.Count, this.maxInstances);
      }

      public void Clear()
      {
        while (this.spawned.Count > 0)
          this.pool.Enqueue(this.spawned.Dequeue());
        for (int index = 0; index < this.pool.Count; ++index)
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.pool[index]);
        this.pool.Clear();
      }

      public GameObject Spawn(
        Transform parent,
        Vector3 position,
        Quaternion rotation,
        bool activate)
      {
        GameObject gameObject = this.Spawn(position, rotation, activate);
        gameObject.transform.parent = parent;
        return gameObject;
      }

      public GameObject Spawn(Vector3 position, Quaternion rotation)
      {
        return this.Spawn(position, rotation, true);
      }

      public GameObject Spawn(Vector3 position, Quaternion rotation, bool activate)
      {
        GameObject gameObject = this.Spawn(false);
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        if (activate)
          gameObject.SetActive(true);
        return gameObject;
      }

      public GameObject Spawn(bool activate)
      {
        if (this.pool.Count > 0)
        {
          GameObject instance = this.pool.Dequeue();
          this.spawnInstance(instance, activate);
          return instance;
        }
        if (this.maxInstances == -1 || this.spawned.Count < this.maxInstances)
        {
          GameObject instance = this.Instantiate();
          this.spawnInstance(instance, activate);
          return instance;
        }
        if (this.limitType == dfPoolManager.LimitReachedAction.Nothing)
          return (GameObject) null;
        if (this.limitType == dfPoolManager.LimitReachedAction.Error)
          throw new Exception($"The {this.PoolName} object pool has already allocated its limit of {this.MaxInstances} objects");
        GameObject instance1 = this.spawned.Dequeue();
        this.spawnInstance(instance1, activate);
        return instance1;
      }

      public void Despawn(GameObject instance)
      {
        if (!this.spawned.Remove(instance))
          return;
        dfPooledObject component = instance.GetComponent<dfPooledObject>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.OnDespawned();
        instance.SetActive(false);
        this.pool.Enqueue(instance);
        if (!this.allowReparenting || !((UnityEngine.Object) dfPoolManager.Pool != (UnityEngine.Object) null))
          return;
        instance.transform.parent = dfPoolManager.Pool.transform;
      }

      internal void Preload() => this.Preload((System.Action) null);

      internal void Preload(System.Action callback)
      {
        if (this.prefab.activeSelf)
          this.prefab.SetActive(false);
        int num = Mathf.Min(this.initialPoolSize, this.maxInstances != -1 ? this.maxInstances : int.MaxValue);
        while (this.pool.Count + this.spawned.Count < num)
        {
          this.pool.Add(this.Instantiate());
          if (callback != null)
            callback();
        }
      }

      private void spawnInstance(GameObject instance, bool activate)
      {
        this.spawned.Enqueue(instance);
        dfPooledObject component = instance.GetComponent<dfPooledObject>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.OnSpawned();
        if (!activate)
          return;
        instance.SetActive(true);
      }

      private GameObject Instantiate()
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab);
        gameObject.name = $"{this.PoolName} {this.pool.Count + 1}";
        if (this.allowReparenting)
          gameObject.transform.parent = dfPoolManager.Pool.transform;
        dfPooledObject dfPooledObject = gameObject.GetComponent<dfPooledObject>();
        if ((UnityEngine.Object) dfPooledObject == (UnityEngine.Object) null)
          dfPooledObject = gameObject.AddComponent<dfPooledObject>();
        dfPooledObject.Pool = this;
        return gameObject;
      }
    }
  }

