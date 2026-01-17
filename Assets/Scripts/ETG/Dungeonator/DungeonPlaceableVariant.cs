// Decompiled with JetBrains decompiler
// Type: Dungeonator.DungeonPlaceableVariant
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace Dungeonator;

[Serializable]
public class DungeonPlaceableVariant
{
  [SerializeField]
  public float percentChance = 0.1f;
  [SerializeField]
  public Vector2 unitOffset = Vector2.zero;
  [SerializeField]
  [FormerlySerializedAs("nonenemyPlaceable")]
  public GameObject nonDatabasePlaceable;
  [FormerlySerializedAs("enemyGuid")]
  [EnemyIdentifier]
  public string enemyPlaceableGuid;
  [PickupIdentifier]
  public int pickupObjectPlaceableId = -1;
  [SerializeField]
  public bool forceBlackPhantom;
  [SerializeField]
  public bool addDebrisObject;
  [SerializeField]
  public DungeonPrerequisite[] prerequisites;
  [SerializeField]
  public DungeonPlaceableRoomMaterialRequirement[] materialRequirements;
  [NonSerialized]
  public float percentChanceMultiplier = 1f;

  public float GetPercentChance() => this.percentChance;

  public GameObject GetOrLoadPlaceableObject
  {
    get
    {
      if ((bool) (UnityEngine.Object) this.nonDatabasePlaceable)
        return this.nonDatabasePlaceable;
      if (!string.IsNullOrEmpty(this.enemyPlaceableGuid))
      {
        AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(this.enemyPlaceableGuid);
        if ((bool) (UnityEngine.Object) orLoadByGuid)
          return orLoadByGuid.gameObject;
      }
      if (this.pickupObjectPlaceableId >= 0)
      {
        PickupObject byId = PickupObjectDatabase.GetById(this.pickupObjectPlaceableId);
        if ((bool) (UnityEngine.Object) byId)
          return byId.gameObject;
      }
      return (GameObject) null;
    }
  }

  public DungeonPlaceableBehaviour.PlaceableDifficulty difficulty
  {
    get
    {
      if ((UnityEngine.Object) this.nonDatabasePlaceable != (UnityEngine.Object) null)
      {
        DungeonPlaceableBehaviour component = this.nonDatabasePlaceable.GetComponent<DungeonPlaceableBehaviour>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          return component.difficulty;
      }
      if (string.IsNullOrEmpty(this.enemyPlaceableGuid))
        return DungeonPlaceableBehaviour.PlaceableDifficulty.BASE;
      EnemyDatabaseEntry entry = EnemyDatabase.GetEntry(this.enemyPlaceableGuid);
      return entry == null ? DungeonPlaceableBehaviour.PlaceableDifficulty.BASE : entry.difficulty;
    }
  }

  public int difficultyRating => (int) this.difficulty;
}
