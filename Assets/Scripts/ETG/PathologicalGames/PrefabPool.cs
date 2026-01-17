// Decompiled with JetBrains decompiler
// Type: PathologicalGames.PrefabPool
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

[Serializable]
public class PrefabPool
{
  public Transform prefab;
  internal GameObject prefabGO;
  public int preloadAmount = 1;
  public bool preloadTime;
  public int preloadFrames = 2;
  public float preloadDelay;
  public bool limitInstances;
  public int limitAmount = 100;
  public bool limitFIFO;
  public bool cullDespawned;
  public int cullAbove = 50;
  public int cullDelay = 60;
  public int cullMaxPerPass = 5;
  public bool _logMessages;
  private bool forceLoggingSilent;
  public SpawnPool spawnPool;
  private bool cullingActive;
  internal List<Transform> _spawned = new List<Transform>();
  internal List<Transform> _despawned = new List<Transform>();
  private bool _preloaded;

  public PrefabPool(Transform prefab)
  {
    this.prefab = prefab;
    this.prefabGO = prefab.gameObject;
  }

  public PrefabPool()
  {
  }

  public bool logMessages
  {
    get
    {
      if (this.forceLoggingSilent)
        return false;
      return this.spawnPool.logMessages ? this.spawnPool.logMessages : this._logMessages;
    }
  }

  internal void inspectorInstanceConstructor()
  {
    this.prefabGO = this.prefab.gameObject;
    this._spawned = new List<Transform>();
    this._despawned = new List<Transform>();
  }

  internal void SelfDestruct()
  {
    this.prefab = (Transform) null;
    this.prefabGO = (GameObject) null;
    this.spawnPool = (SpawnPool) null;
    foreach (Transform transform in this._despawned)
    {
      if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) transform.gameObject);
    }
    foreach (Transform transform in this._spawned)
    {
      if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) transform.gameObject);
    }
    this._spawned.Clear();
    this._despawned.Clear();
  }

  public List<Transform> spawned => this._spawned;

  public List<Transform> despawned => this._despawned;

  public int totalCount => 0 + this._spawned.Count + this._despawned.Count;

  internal bool preloaded
  {
    get => this._preloaded;
    private set => this._preloaded = value;
  }

  internal bool DespawnInstance(Transform xform) => this.DespawnInstance(xform, true);

  internal void RemoveInstance(Transform xform)
  {
    this._spawned.Remove(xform);
    this._despawned.Remove(xform);
  }

  internal bool DespawnInstance(Transform xform, bool sendEventMessage)
  {
    if (this.logMessages)
      UnityEngine.Debug.Log((object) $"SpawnPool {this.spawnPool.poolName} ({this.prefab.name}): Despawning '{xform.name}'");
    this._spawned.Remove(xform);
    this._despawned.Add(xform);
    if (sendEventMessage)
      xform.gameObject.BroadcastMessage("OnDespawned", (object) this.spawnPool, SendMessageOptions.DontRequireReceiver);
    PoolManagerUtils.SetActive(xform.gameObject, false);
    if (!this.cullingActive && this.cullDespawned && this.totalCount > this.cullAbove)
    {
      this.cullingActive = true;
      this.spawnPool.StartCoroutine(this.CullDespawned());
    }
    return true;
  }

  [DebuggerHidden]
  internal IEnumerator CullDespawned()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new PrefabPool.<CullDespawned>c__Iterator0()
    {
      _this = this
    };
  }

  internal Transform SpawnInstance(Vector3 pos, Quaternion rot)
  {
    SpawnManager.LastPrefabPool = this;
    if (this.limitInstances && this.limitFIFO && this._spawned.Count >= this.limitAmount)
    {
      Transform xform = this._spawned[0];
      if (this.logMessages)
        UnityEngine.Debug.Log((object) $"SpawnPool {this.spawnPool.poolName} ({this.prefab.name}): LIMIT REACHED! FIFO=True. Calling despawning for {xform}...");
      this.DespawnInstance(xform);
      this.spawnPool._spawned.Remove(xform);
    }
    Transform transform;
    if (this._despawned.Count == 0)
    {
      transform = this.SpawnNew(pos, rot);
    }
    else
    {
      transform = (Transform) null;
      while ((UnityEngine.Object) transform == (UnityEngine.Object) null)
      {
        if (this._despawned.Count == 0)
        {
          transform = this.SpawnNew(pos, rot);
        }
        else
        {
          transform = this._despawned[0];
          this._despawned.RemoveAt(0);
          if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
            this._spawned.Add(transform);
        }
      }
      if (this.logMessages)
        UnityEngine.Debug.Log((object) $"SpawnPool {this.spawnPool.poolName} ({this.prefab.name}): respawning '{transform.name}'.");
      transform.position = pos;
      transform.rotation = rot;
      PoolManagerUtils.SetActive(transform.gameObject, true);
    }
    return transform;
  }

  public Transform SpawnNew() => this.SpawnNew(Vector3.zero, Quaternion.identity);

  public Transform SpawnNew(Vector3 pos, Quaternion rot)
  {
    if (this.limitInstances && this.totalCount >= this.limitAmount)
    {
      if (this.logMessages)
        UnityEngine.Debug.Log((object) $"SpawnPool {this.spawnPool.poolName} ({this.prefab.name}): LIMIT REACHED! Not creating new instances! (Returning null)");
      return (Transform) null;
    }
    if (pos == Vector3.zero)
      pos = this.spawnPool.group.position;
    if (rot == Quaternion.identity)
      rot = this.spawnPool.group.rotation;
    Transform transform = UnityEngine.Object.Instantiate<Transform>(this.prefab, pos, rot);
    this.nameInstance(transform);
    if (!this.spawnPool.dontReparent)
      transform.parent = this.spawnPool.group;
    if (this.spawnPool.matchPoolScale)
      transform.localScale = Vector3.one;
    if (this.spawnPool.matchPoolLayer)
      this.SetRecursively(transform, this.spawnPool.gameObject.layer);
    this._spawned.Add(transform);
    if (this.logMessages)
      UnityEngine.Debug.Log((object) $"SpawnPool {this.spawnPool.poolName} ({this.prefab.name}): Spawned new instance '{transform.name}'.");
    return transform;
  }

  private void SetRecursively(Transform xform, int layer)
  {
    xform.gameObject.layer = layer;
    foreach (Transform xform1 in xform)
      this.SetRecursively(xform1, layer);
  }

  internal void AddUnpooled(Transform inst, bool despawn)
  {
    this.nameInstance(inst);
    if (despawn)
    {
      PoolManagerUtils.SetActive(inst.gameObject, false);
      this._despawned.Add(inst);
    }
    else
      this._spawned.Add(inst);
  }

  internal void PreloadInstances()
  {
    if (this.preloaded)
      UnityEngine.Debug.Log((object) $"SpawnPool {this.spawnPool.poolName} ({this.prefab.name}): Already preloaded! You cannot preload twice. If you are running this through code, make sure it isn't also defined in the Inspector.");
    else if ((UnityEngine.Object) this.prefab == (UnityEngine.Object) null)
    {
      UnityEngine.Debug.LogError((object) $"SpawnPool {this.spawnPool.poolName} ({this.prefab.name}): Prefab cannot be null.");
    }
    else
    {
      if (this.limitInstances && this.preloadAmount > this.limitAmount)
      {
        UnityEngine.Debug.LogWarning((object) $"SpawnPool {this.spawnPool.poolName} ({this.prefab.name}): You turned ON 'Limit Instances' and entered a 'Limit Amount' greater than the 'Preload Amount'! Setting preload amount to limit amount.");
        this.preloadAmount = this.limitAmount;
      }
      if (this.cullDespawned && this.preloadAmount > this.cullAbove)
        UnityEngine.Debug.LogWarning((object) $"SpawnPool {this.spawnPool.poolName} ({this.prefab.name}): You turned ON Culling and entered a 'Cull Above' threshold greater than the 'Preload Amount'! This will cause the culling feature to trigger immediatly, which is wrong conceptually. Only use culling for extreme situations. See the docs.");
      if (this.preloadTime)
      {
        if (this.preloadFrames > this.preloadAmount)
        {
          UnityEngine.Debug.LogWarning((object) $"SpawnPool {this.spawnPool.poolName} ({this.prefab.name}): Preloading over-time is on but the frame duration is greater than the number of instances to preload. The minimum spawned per frame is 1, so the maximum time is the same as the number of instances. Changing the preloadFrames value...");
          this.preloadFrames = this.preloadAmount;
        }
        this.spawnPool.StartCoroutine(this.PreloadOverTime());
      }
      else
      {
        this.forceLoggingSilent = true;
        while (this.totalCount < this.preloadAmount)
          this.DespawnInstance(this.SpawnNew(), false);
        this.forceLoggingSilent = false;
      }
    }
  }

  [DebuggerHidden]
  private IEnumerator PreloadOverTime()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new PrefabPool.<PreloadOverTime>c__Iterator1()
    {
      _this = this
    };
  }

  public bool Contains(Transform transform)
  {
    if ((UnityEngine.Object) this.prefabGO == (UnityEngine.Object) null)
      UnityEngine.Debug.LogError((object) $"SpawnPool {this.spawnPool.poolName}: PrefabPool.prefabGO is null");
    return this._spawned.Contains(transform) || this._despawned.Contains(transform);
  }

  private void nameInstance(Transform instance)
  {
    instance.name += (this.totalCount + 1).ToString("#000");
  }
}
