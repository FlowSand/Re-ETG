// Decompiled with JetBrains decompiler
// Type: WandOfWonderItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class WandOfWonderItem : PlayerItem
    {
      public float ItemChance = 0.25f;
      public float GunChance = 0.25f;
      public float EnemyChance = 0.5f;
      public float VanishChance = 0.25f;
      public GenericLootTable ItemTable;
      public GenericLootTable GunTable;
      public DungeonPlaceable EnemyPlaceable;
      public bool AffectsAllEnemiesInRoom;
      public int MaxItemsPerRoom = 1;
      public int MaxGunsPerRoom = 1;
      public GameObject OnEffectVFX;

      private AIActor GetTargetEnemy(PlayerController user)
      {
        if (user.CurrentRoom == null)
          return (AIActor) null;
        List<AIActor> activeEnemies = user.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        if (activeEnemies == null || activeEnemies.Count <= 0)
          return (AIActor) null;
        List<AIActor> list = activeEnemies.Where<AIActor>((Func<AIActor, bool>) (x => !x.healthHaver.IsBoss)).ToList<AIActor>();
        return list == null || list.Count <= 0 ? (AIActor) null : list[UnityEngine.Random.Range(0, list.Count)];
      }

      public override bool CanBeUsed(PlayerController user)
      {
        return !((UnityEngine.Object) this.GetTargetEnemy(user) == (UnityEngine.Object) null);
      }

      protected void ProcessSingleTarget(
        PlayerController user,
        AIActor randomEnemy,
        ref int spawnedItems,
        ref int spawnedGuns)
      {
        float num1 = this.ItemChance;
        float num2 = this.GunChance;
        if (spawnedItems == this.MaxItemsPerRoom)
          num1 = 0.0f;
        if (spawnedGuns == this.MaxGunsPerRoom)
          num2 = 0.0f;
        float num3 = UnityEngine.Random.value * (num1 + num2 + this.EnemyChance + this.VanishChance);
        Vector2 centerPosition = randomEnemy.CenterPosition;
        randomEnemy.EraseFromExistence();
        if ((UnityEngine.Object) this.OnEffectVFX != (UnityEngine.Object) null)
          SpawnManager.SpawnVFX(this.OnEffectVFX, (Vector3) centerPosition, Quaternion.identity);
        if ((double) num3 < (double) num1)
        {
          LootEngine.SpawnItem(this.ItemTable.SelectByWeight(), (Vector3) centerPosition, Vector2.up, 1f);
          ++spawnedItems;
        }
        else if ((double) num3 < (double) num1 + (double) num2)
        {
          LootEngine.SpawnItem(this.GunTable.SelectByWeight(), (Vector3) centerPosition, Vector2.up, 1f);
          ++spawnedGuns;
        }
        else
        {
          if ((double) num3 >= (double) num1 + (double) num2 + (double) this.EnemyChance)
            return;
          List<EnemyDatabaseEntry> list = EnemyDatabase.Instance.Entries.Where<EnemyDatabaseEntry>((Func<EnemyDatabaseEntry, bool>) (x => x != null && x.isNormalEnemy && !x.isInBossTab)).ToList<EnemyDatabaseEntry>();
          AIActor.Spawn(list[UnityEngine.Random.Range(0, list.Count)].GetPrefab<AIActor>(), centerPosition.ToIntVector2(VectorConversions.Floor), user.CurrentRoom, true);
        }
      }

      protected override void DoEffect(PlayerController user)
      {
        int spawnedItems = 0;
        int spawnedGuns = 0;
        if (this.AffectsAllEnemiesInRoom)
        {
          List<AIActor> activeEnemies = user.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
          if (activeEnemies == null || activeEnemies.Count <= 0)
            return;
          List<AIActor> list = activeEnemies.Where<AIActor>((Func<AIActor, bool>) (x => !x.healthHaver.IsBoss)).ToList<AIActor>();
          if (list == null || list.Count <= 0)
            return;
          for (int index = 0; index < list.Count; ++index)
            this.ProcessSingleTarget(user, list[index], ref spawnedItems, ref spawnedGuns);
        }
        else
        {
          AIActor targetEnemy = this.GetTargetEnemy(user);
          if ((UnityEngine.Object) targetEnemy == (UnityEngine.Object) null)
            return;
          this.ProcessSingleTarget(user, targetEnemy, ref spawnedItems, ref spawnedGuns);
        }
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
