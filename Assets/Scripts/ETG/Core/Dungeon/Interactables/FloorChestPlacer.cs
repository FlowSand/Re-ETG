// Decompiled with JetBrains decompiler
// Type: FloorChestPlacer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class FloorChestPlacer : DungeonPlaceableBehaviour, IPlaceConfigurable
    {
      public bool OverrideItemQuality;
      [ShowInInspectorIf("OverrideItemQuality", false)]
      public PickupObject.ItemQuality ItemQuality;
      public float OverrideMimicChance = -1f;
      [DwarfConfigurable]
      public int xPixelOffset;
      [DwarfConfigurable]
      public int yPixelOffset;
      public bool CenterChestInRegion;
      public bool OverrideLockChance;
      public bool ForceUnlockedIfWooden;
      public float LockChance = 0.5f;
      public bool UseOverrideChest;
      public DungeonPrerequisite OverrideChestPrereq;
      public Chest OverrideChestPrefab;

      public void ConfigureOnPlacement(RoomHandler room)
      {
        IntVector2 positionInRoom = this.transform.position.IntXY() - room.area.basePosition;
        Chest chest = !this.UseOverrideChest || !this.OverrideChestPrereq.CheckConditionsFulfilled() ? GameManager.Instance.RewardManager.GenerationSpawnRewardChestAt(positionInRoom, room, !this.OverrideItemQuality ? new PickupObject.ItemQuality?() : new PickupObject.ItemQuality?(this.ItemQuality), this.OverrideMimicChance) : Chest.Spawn(this.OverrideChestPrefab, this.transform.position.IntXY());
        if (this.CenterChestInRegion && (bool) (Object) chest)
        {
          SpeculativeRigidbody component = chest.GetComponent<SpeculativeRigidbody>();
          if ((bool) (Object) component)
          {
            Vector2 vector2 = component.UnitCenter - chest.transform.position.XY();
            Vector2 vector = this.transform.position.XY() + new Vector2((float) this.xPixelOffset / 16f, (float) this.yPixelOffset / 16f) + new Vector2((float) this.placeableWidth / 2f, (float) this.placeableHeight / 2f) - vector2;
            chest.transform.position = vector.ToVector3ZisY().Quantize(1f / 16f);
            component.Reinitialize();
          }
        }
        if (this.OverrideLockChance && (bool) (Object) chest)
        {
          if ((double) Random.value < (double) this.LockChance || this.ForceUnlockedIfWooden && (double) chest.lootTable.D_Chance == 1.0)
            chest.ForceUnlock();
          else
            chest.IsLocked = true;
        }
        Object.Destroy((Object) this.gameObject);
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
