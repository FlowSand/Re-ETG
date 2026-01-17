// Decompiled with JetBrains decompiler
// Type: SpawnManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using Dungeonator;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class SpawnManager : MonoBehaviour
{
  public Transform Debris;
  public Transform Decals;
  public Transform ParticleSystems;
  public Transform Projectiles;
  public Transform VFX;
  [Header("Object Limit")]
  public int MaxObjects = (int) byte.MaxValue;
  public int CurrentObjects;
  public int MaxDecalPerArea = 5;
  public int MaxDecalAreaWidth = 2;
  [Header("Per-room Object Limit")]
  public bool UsesPerRoomObjectLimit;
  [ShowInInspectorIf("UsesPerRoomObjectLimit", false)]
  public int MaxObjectsPerRoom = 100;
  [ShowInInspectorIf("UsesPerRoomObjectLimit", false)]
  public int CurrentObjectsInRoom;
  private const int MAX_OBJECTS_HIGH = 800;
  private const int MAX_OBJECTS_MED = 300;
  private const int MAX_OBJECTS_LOW = 50;
  private static SpawnPool m_poolManager;
  private bool m_removalCoroutineRunning;
  private static SpawnManager m_instance;
  private LinkedList<EphemeralObject> m_objects = new LinkedList<EphemeralObject>();
  private Dictionary<EphemeralObject, RoomHandler> m_objectToRoomMap = new Dictionary<EphemeralObject, RoomHandler>();
  private Dictionary<RoomHandler, LinkedList<EphemeralObject>> m_objectsByRoom = new Dictionary<RoomHandler, LinkedList<EphemeralObject>>();

  public static SpawnManager Instance
  {
    get => SpawnManager.m_instance;
    set => SpawnManager.m_instance = value;
  }

  public static bool HasInstance => (UnityEngine.Object) SpawnManager.m_instance != (UnityEngine.Object) null;

  public static SpawnPool PoolManager
  {
    get
    {
      if ((UnityEngine.Object) SpawnManager.m_poolManager == (UnityEngine.Object) null)
        SpawnManager.m_poolManager = PathologicalGames.PoolManager.Pools.Create("SpawnManager Pool");
      return SpawnManager.m_poolManager;
    }
    set => SpawnManager.m_poolManager = value;
  }

  public static PrefabPool LastPrefabPool { get; set; }

  public void Awake()
  {
    SpawnManager.m_instance = this;
    this.CurrentObjects = 0;
    this.CurrentObjectsInRoom = 0;
    this.OnDebrisQuantityChanged();
  }

  public void OnDebrisQuantityChanged()
  {
    switch (GameManager.Options.DebrisQuantity)
    {
      case GameOptions.GenericHighMedLowOption.LOW:
        this.MaxObjects = 50;
        break;
      case GameOptions.GenericHighMedLowOption.MEDIUM:
        this.MaxObjects = 300;
        break;
      case GameOptions.GenericHighMedLowOption.HIGH:
        this.MaxObjects = 800;
        break;
      case GameOptions.GenericHighMedLowOption.VERY_LOW:
        this.MaxObjects = 0;
        break;
    }
  }

  public void Update()
  {
    this.CurrentObjects = this.m_objects.Count;
    if (!this.UsesPerRoomObjectLimit)
      return;
    if (GameManager.Instance.PrimaryPlayer.CurrentRoom != null && this.m_objectsByRoom.ContainsKey(GameManager.Instance.PrimaryPlayer.CurrentRoom))
      this.CurrentObjectsInRoom = this.m_objectsByRoom[GameManager.Instance.PrimaryPlayer.CurrentRoom].Count;
    else
      this.CurrentObjectsInRoom = 0;
  }

  public void OnDestroy() => SpawnManager.m_instance = (SpawnManager) null;

  public static void RegisterEphemeralObject(EphemeralObject obj)
  {
    if (!(bool) (UnityEngine.Object) SpawnManager.m_instance)
      return;
    SpawnManager.m_instance.AddObject(obj);
  }

  public static void DeregisterEphemeralObject(EphemeralObject obj)
  {
    if (!(bool) (UnityEngine.Object) SpawnManager.m_instance)
      return;
    SpawnManager.m_instance.RemoveObject(obj);
  }

  public static GameObject SpawnDebris(GameObject prefab)
  {
    return !(bool) (UnityEngine.Object) SpawnManager.m_instance ? (GameObject) null : SpawnManager.SpawnUnpooledInternal(prefab, Vector3.zero, Quaternion.identity, SpawnManager.m_instance.Debris);
  }

  public static GameObject SpawnDebris(GameObject prefab, Vector3 position, Quaternion rotation)
  {
    return !(bool) (UnityEngine.Object) SpawnManager.m_instance ? (GameObject) null : SpawnManager.SpawnUnpooledInternal(prefab, position, rotation, SpawnManager.m_instance.Debris);
  }

  public static GameObject SpawnDecal(GameObject prefab)
  {
    if (!(bool) (UnityEngine.Object) SpawnManager.m_instance)
      return (GameObject) null;
    GameObject gameObject = SpawnManager.Spawn(prefab, SpawnManager.m_instance.Decals);
    if (!(bool) (UnityEngine.Object) gameObject.GetComponent<DecalObject>())
      gameObject.AddComponent<DecalObject>().Priority = EphemeralObject.EphemeralPriority.Minor;
    return gameObject;
  }

  public static GameObject SpawnDecal(
    GameObject prefab,
    Vector3 position,
    Quaternion rotation,
    bool ignoresPools)
  {
    if (!(bool) (UnityEngine.Object) SpawnManager.m_instance)
      return (GameObject) null;
    DecalObject component1 = prefab.GetComponent<DecalObject>();
    EphemeralObject.EphemeralPriority priority = !(bool) (UnityEngine.Object) component1 ? EphemeralObject.EphemeralPriority.Ephemeral : component1.Priority;
    bool cancelAddition = false;
    SpawnManager.m_instance.ClearRoomForDecal(position.XY(), priority, out cancelAddition);
    if (cancelAddition)
      return (GameObject) null;
    GameObject gameObject = SpawnManager.Spawn(prefab, position, rotation, SpawnManager.m_instance.Decals, ignoresPools);
    if (!(bool) (UnityEngine.Object) gameObject.GetComponent<DecalObject>())
      gameObject.AddComponent<DecalObject>().Priority = EphemeralObject.EphemeralPriority.Ephemeral;
    tk2dBaseSprite component2 = gameObject.GetComponent<tk2dBaseSprite>();
    if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
    {
      component2.IsPerpendicular = true;
      component2.UpdateZDepth();
    }
    return gameObject;
  }

  public static GameObject SpawnParticleSystem(GameObject prefab)
  {
    return !(bool) (UnityEngine.Object) SpawnManager.m_instance ? (GameObject) null : SpawnManager.SpawnUnpooledInternal(prefab, Vector3.zero, Quaternion.identity, SpawnManager.m_instance.ParticleSystems);
  }

  public static GameObject SpawnParticleSystem(
    GameObject prefab,
    Vector3 position,
    Quaternion rotation)
  {
    return !(bool) (UnityEngine.Object) SpawnManager.m_instance ? (GameObject) null : SpawnManager.SpawnUnpooledInternal(prefab, position, rotation, SpawnManager.m_instance.ParticleSystems);
  }

  public static GameObject SpawnProjectile(
    GameObject prefab,
    Vector3 position,
    Quaternion rotation,
    bool ignoresPools = true)
  {
    return !(bool) (UnityEngine.Object) SpawnManager.m_instance ? (GameObject) null : SpawnManager.Spawn(prefab, position, rotation, SpawnManager.m_instance.Projectiles, ignoresPools);
  }

  public static GameObject SpawnProjectile(
    string resourcePath,
    Vector3 position,
    Quaternion rotation)
  {
    return SpawnManager.SpawnUnpooledInternal(BraveResources.Load<GameObject>(resourcePath), position, rotation, SpawnManager.m_instance.Projectiles);
  }

  public static GameObject SpawnVFX(GameObject prefab, Vector3 position, Quaternion rotation)
  {
    return !(bool) (UnityEngine.Object) SpawnManager.m_instance ? (GameObject) null : SpawnManager.Spawn(prefab, position, rotation, SpawnManager.m_instance.VFX);
  }

  public static GameObject SpawnVFX(GameObject prefab, bool ignoresPools = false)
  {
    return !(bool) (UnityEngine.Object) SpawnManager.m_instance ? (GameObject) null : SpawnManager.Spawn(prefab, SpawnManager.m_instance.VFX, ignoresPools);
  }

  public static GameObject SpawnVFX(
    GameObject prefab,
    Vector3 position,
    Quaternion rotation,
    bool ignoresPools)
  {
    return !(bool) (UnityEngine.Object) SpawnManager.m_instance ? (GameObject) null : SpawnManager.Spawn(prefab, position, rotation, SpawnManager.m_instance.VFX, ignoresPools);
  }

  public static bool Despawn(GameObject instance)
  {
    if ((UnityEngine.Object) SpawnManager.m_poolManager != (UnityEngine.Object) null)
    {
      GameObject prefab = SpawnManager.m_poolManager.GetPrefab(instance);
      if ((UnityEngine.Object) prefab != (UnityEngine.Object) null)
      {
        PrefabPool prefabPool = SpawnManager.m_poolManager.GetPrefabPool(prefab);
        Transform transform = instance.transform;
        if (prefabPool.despawned.Contains(transform))
          return true;
        if (prefabPool.spawned.Contains(transform))
        {
          SpawnManager.m_poolManager.Despawn(instance.transform);
          return true;
        }
      }
    }
    UnityEngine.Object.Destroy((UnityEngine.Object) instance);
    return false;
  }

  public static bool Despawn(GameObject instance, PrefabPool prefabPool)
  {
    if ((UnityEngine.Object) SpawnManager.m_poolManager != (UnityEngine.Object) null)
    {
      if (prefabPool == null)
      {
        GameObject prefab = SpawnManager.m_poolManager.GetPrefab(instance);
        if ((UnityEngine.Object) prefab != (UnityEngine.Object) null)
          prefabPool = SpawnManager.m_poolManager.GetPrefabPool(prefab);
      }
      if (prefabPool != null)
      {
        Transform transform = instance.transform;
        if (prefabPool.despawned.Contains(transform))
          return true;
        if (prefabPool.spawned.Contains(transform))
        {
          SpawnManager.m_poolManager.Despawn(instance.transform, prefabPool);
          return true;
        }
      }
    }
    UnityEngine.Object.Destroy((UnityEngine.Object) instance);
    return false;
  }

  public static void SpawnBulletScript(
    GameActor owner,
    BulletScriptSelector bulletScript,
    Vector2? pos = null,
    Vector2? direction = null,
    bool collidesWithEnemies = false,
    string ownerName = null)
  {
    if (!(bool) (UnityEngine.Object) owner || !(bool) (UnityEngine.Object) owner.bulletBank)
      return;
    Vector2 pos1 = !pos.HasValue ? owner.specRigidbody.GetUnitCenter(ColliderType.HitBox) : pos.Value;
    AIBulletBank bulletBank = owner.bulletBank;
    SpeculativeRigidbody specRigidbody = owner.specRigidbody;
    if (ownerName == null && (bool) (UnityEngine.Object) owner)
    {
      if ((bool) (UnityEngine.Object) owner.bulletBank)
        ownerName = owner.bulletBank.ActorName;
      else if (owner is AIActor)
        ownerName = (owner as AIActor).GetActorName();
    }
    SpawnManager.SpawnBulletScript(owner, pos1, bulletBank, bulletScript, ownerName, specRigidbody, direction, collidesWithEnemies);
  }

  public static void SpawnBulletScript(
    GameActor owner,
    Vector2 pos,
    AIBulletBank sourceBulletBank,
    BulletScriptSelector bulletScript,
    string ownerName,
    SpeculativeRigidbody sourceRigidbody = null,
    Vector2? direction = null,
    bool collidesWithEnemies = false,
    Action<Bullet, Projectile> OnBulletCreated = null)
  {
    GameObject gameObject = new GameObject("Temp BulletScript Spawner");
    gameObject.transform.position = (Vector3) pos;
    AIBulletBank aiBulletBank = gameObject.AddComponent<AIBulletBank>();
    aiBulletBank.Bullets = new List<AIBulletBank.Entry>();
    for (int index = 0; index < sourceBulletBank.Bullets.Count; ++index)
      aiBulletBank.Bullets.Add(new AIBulletBank.Entry(sourceBulletBank.Bullets[index]));
    aiBulletBank.useDefaultBulletIfMissing = sourceBulletBank.useDefaultBulletIfMissing;
    aiBulletBank.transforms = new List<Transform>((IEnumerable<Transform>) sourceBulletBank.transforms);
    aiBulletBank.PlayVfx = false;
    aiBulletBank.PlayAudio = false;
    aiBulletBank.CollidesWithEnemies = collidesWithEnemies;
    aiBulletBank.gameActor = owner;
    if (owner is AIActor)
      aiBulletBank.aiActor = owner as AIActor;
    aiBulletBank.ActorName = ownerName;
    if (OnBulletCreated != null)
      aiBulletBank.OnBulletSpawned += OnBulletCreated;
    aiBulletBank.SpecificRigidbodyException = sourceRigidbody;
    if (direction.HasValue)
      aiBulletBank.FixedPlayerPosition = new Vector2?(pos + direction.Value.normalized * 5f);
    BulletScriptSource bulletScriptSource = gameObject.AddComponent<BulletScriptSource>();
    bulletScriptSource.BulletManager = aiBulletBank;
    bulletScriptSource.BulletScript = bulletScript;
    bulletScriptSource.Initialize();
    gameObject.AddComponent<BulletSourceKiller>().BraveSource = bulletScriptSource;
  }

  public static bool IsSpawned(GameObject instance)
  {
    return (UnityEngine.Object) SpawnManager.m_poolManager != (UnityEngine.Object) null && SpawnManager.m_poolManager.IsSpawned(instance.transform);
  }

  public static bool IsPooled(GameObject instance)
  {
    if ((UnityEngine.Object) SpawnManager.m_poolManager != (UnityEngine.Object) null)
    {
      GameObject prefab = SpawnManager.m_poolManager.GetPrefab(instance);
      if ((UnityEngine.Object) prefab != (UnityEngine.Object) null)
      {
        PrefabPool prefabPool = SpawnManager.m_poolManager.GetPrefabPool(prefab);
        Transform transform = instance.transform;
        if (prefabPool.despawned.Contains(transform) || prefabPool.spawned.Contains(transform))
          return true;
      }
    }
    return false;
  }

  private static GameObject Spawn(GameObject prefab, Transform parent, bool ignoresPools = false)
  {
    return SpawnManager.Spawn(prefab, Vector3.zero, Quaternion.identity, parent, ignoresPools);
  }

  private static GameObject Spawn(
    GameObject prefab,
    Vector3 position,
    Quaternion rotation,
    Transform parent,
    bool ignoresPools = false)
  {
    if ((UnityEngine.Object) prefab == (UnityEngine.Object) null)
    {
      UnityEngine.Debug.LogError((object) "Attempting to spawn a null prefab!");
      return (GameObject) null;
    }
    if (ignoresPools)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, position, rotation);
      gameObject.transform.parent = parent;
      return gameObject;
    }
    if ((UnityEngine.Object) SpawnManager.m_poolManager == (UnityEngine.Object) null)
      SpawnManager.m_poolManager = PathologicalGames.PoolManager.Pools.Create("SpawnManager Pool");
    return SpawnManager.m_poolManager.Spawn(prefab.transform, position, rotation, parent).gameObject;
  }

  private static GameObject SpawnUnpooledInternal(
    GameObject prefab,
    Vector3 position,
    Quaternion rotation,
    Transform parent)
  {
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, position, rotation);
    gameObject.transform.parent = parent;
    return gameObject;
  }

  private void AddObject(EphemeralObject obj)
  {
    LinkedListNode<EphemeralObject> node1 = this.m_objects.First;
    bool flag1 = false;
    if (obj.Priority != EphemeralObject.EphemeralPriority.Critical)
      obj.Priority = EphemeralObject.EphemeralPriority.Minor;
    for (; node1 != null; node1 = node1.Next)
    {
      if (node1.Value.Priority >= obj.Priority)
      {
        this.m_objects.AddBefore(node1, obj);
        flag1 = true;
        break;
      }
    }
    if (!flag1)
      this.m_objects.AddLast(obj);
    if (this.UsesPerRoomObjectLimit)
    {
      RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(obj.transform.position.IntXY(VectorConversions.Floor));
      this.m_objectToRoomMap.Add(obj, roomFromPosition);
      if (!this.m_objectsByRoom.ContainsKey(roomFromPosition))
        this.m_objectsByRoom.Add(roomFromPosition, new LinkedList<EphemeralObject>());
      LinkedListNode<EphemeralObject> node2 = this.m_objectsByRoom[roomFromPosition].First;
      bool flag2 = false;
      for (; node2 != null; node2 = node2.Next)
      {
        if (node2.Value.Priority > obj.Priority)
        {
          this.m_objectsByRoom[roomFromPosition].AddBefore(node2, obj);
          flag2 = true;
          break;
        }
      }
      if (!flag2)
        this.m_objectsByRoom[roomFromPosition].AddLast(obj);
      while (this.m_objectsByRoom[roomFromPosition].Count > SpawnManager.m_instance.MaxObjectsPerRoom && this.m_objectsByRoom[roomFromPosition].Last.Value.Priority != EphemeralObject.EphemeralPriority.Critical)
        this.m_objectsByRoom[roomFromPosition].Last.Value.TriggerDestruction();
    }
    if (this.m_removalCoroutineRunning || SpawnManager.m_instance.m_objects.Count <= SpawnManager.m_instance.MaxObjects)
      return;
    this.StartCoroutine(this.DeferredRemovalOfObjectsAboveLimit());
  }

  [DebuggerHidden]
  private IEnumerator DeferredRemovalOfObjectsAboveLimit()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new SpawnManager.\u003CDeferredRemovalOfObjectsAboveLimit\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void RemoveObject(EphemeralObject obj)
  {
    if (this.UsesPerRoomObjectLimit && this.m_objectToRoomMap.ContainsKey(obj))
    {
      RoomHandler objectToRoom = this.m_objectToRoomMap[obj];
      this.m_objectToRoomMap.Remove(obj);
      this.m_objectsByRoom[objectToRoom].Remove(obj);
    }
    this.m_objects.Remove(obj);
  }

  public void ClearRectOfDecals(Vector2 minPos, Vector2 maxPos)
  {
    if (this.m_objects == null)
      return;
    LinkedListNode<EphemeralObject> next;
    for (LinkedListNode<EphemeralObject> linkedListNode = this.m_objects.First; linkedListNode != null; linkedListNode = next)
    {
      next = linkedListNode.Next;
      if (linkedListNode.Value is DecalObject && (double) linkedListNode.Value.transform.position.x > (double) minPos.x && (double) linkedListNode.Value.transform.position.x < (double) maxPos.x && (double) linkedListNode.Value.transform.position.y > (double) minPos.y && (double) linkedListNode.Value.transform.position.y < (double) maxPos.y)
        linkedListNode.Value.TriggerDestruction();
    }
  }

  private void ClearRoomForDecal(
    Vector2 pos,
    EphemeralObject.EphemeralPriority priority,
    out bool cancelAddition)
  {
    cancelAddition = false;
    float num1 = pos.x - (float) this.MaxDecalAreaWidth;
    float num2 = pos.x + (float) this.MaxDecalAreaWidth;
    float num3 = pos.y - (float) this.MaxDecalAreaWidth;
    float num4 = pos.y + (float) this.MaxDecalAreaWidth;
    int num5 = 0;
    LinkedListNode<EphemeralObject> next;
    for (LinkedListNode<EphemeralObject> linkedListNode = this.m_objects.First; linkedListNode != null; linkedListNode = next)
    {
      next = linkedListNode.Next;
      if (linkedListNode.Value is DecalObject && (double) linkedListNode.Value.transform.position.x > (double) num1 && (double) linkedListNode.Value.transform.position.x < (double) num2 && (double) linkedListNode.Value.transform.position.y > (double) num3 && (double) linkedListNode.Value.transform.position.y < (double) num4)
      {
        if (num5 < this.MaxDecalPerArea - 1)
          ++num5;
        else if (num5 == this.MaxDecalPerArea - 1)
        {
          ++num5;
          if (linkedListNode.Value.Priority < priority)
            cancelAddition = true;
          else
            linkedListNode.Value.TriggerDestruction();
        }
        else
          linkedListNode.Value.TriggerDestruction();
      }
    }
  }
}
