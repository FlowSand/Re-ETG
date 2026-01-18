using FullInspector;
using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

[Serializable]
public class EnemyDatabaseEntry : AssetBundleDatabaseEntry
  {
    [InspectorDisabled]
    public DungeonPlaceableBehaviour.PlaceableDifficulty difficulty;
    [InspectorDisabled]
    public int placeableWidth;
    [InspectorDisabled]
    public int placeableHeight;
    [InspectorDisabled]
    public bool isNormalEnemy;
    [FormerlySerializedAs("isBoss")]
    [InspectorDisabled]
    public bool isInBossTab;
    [InspectorDisabled]
    public string encounterGuid;
    [InspectorDisabled]
    public int ForcedPositionInAmmonomicon = -1;

    public EnemyDatabaseEntry()
    {
    }

    public EnemyDatabaseEntry(AIActor enemy)
    {
      this.myGuid = enemy.EnemyGuid;
      this.SetAll(enemy);
    }

    public override AssetBundle assetBundle => EnemyDatabase.AssetBundle;

    public override void DropReference() => base.DropReference();

    public T GetPrefab<T>() where T : UnityEngine.Object
    {
      if (!(bool) this.loadedPrefab)
        this.loadedPrefab = (UnityEngine.Object) this.assetBundle.LoadAsset<GameObject>(this.name + ".prefab").GetComponent<T>();
      return this.loadedPrefab as T;
    }

    public void SetAll(AIActor enemy)
    {
      this.difficulty = enemy.difficulty;
      this.placeableWidth = enemy.placeableWidth;
      this.placeableHeight = enemy.placeableHeight;
      this.isNormalEnemy = enemy.IsNormalEnemy;
      this.isInBossTab = enemy.InBossAmmonomiconTab;
      this.encounterGuid = !(bool) (UnityEngine.Object) enemy.encounterTrackable ? string.Empty : enemy.encounterTrackable.TrueEncounterGuid;
      this.ForcedPositionInAmmonomicon = enemy.ForcedPositionInAmmonomicon;
    }

    public bool Equals(AIActor other)
    {
      return !((UnityEngine.Object) other == (UnityEngine.Object) null) && this.difficulty == other.difficulty && this.placeableWidth == other.placeableWidth && this.placeableHeight == other.placeableHeight && this.isNormalEnemy == other.IsNormalEnemy && this.isInBossTab == other.InBossAmmonomiconTab && this.encounterGuid == (!(bool) (UnityEngine.Object) other.encounterTrackable ? string.Empty : other.encounterTrackable.TrueEncounterGuid) && this.ForcedPositionInAmmonomicon == other.ForcedPositionInAmmonomicon;
    }
  }

