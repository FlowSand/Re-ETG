// Decompiled with JetBrains decompiler
// Type: ExtraLifeItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class ExtraLifeItem : PassiveItem
    {
      public static GameObject LastActivatedBonfire;
      private static List<RoomHandler> s_bonfiredRooms = new List<RoomHandler>();
      public ExtraLifeItem.ExtraLifeMode extraLifeMode;
      public bool consumedOnUse = true;
      [ShowInInspectorIf("extraLifeMode", 1, false)]
      public bool DropDarkSoulsItems;
      [ShowInInspectorIf("extraLifeMode", 1, false)]
      public int DarkSoulsCursedHealthMax = -1;
      [PickupIdentifier]
      public int[] ExcludedPickupIDs;
      public bool DoesBonfireSynergy;
      public GameObject BonfireSynergyBonfire;

      public static void ClearPerLevelData()
      {
        ExtraLifeItem.s_bonfiredRooms.Clear();
        ExtraLifeItem.LastActivatedBonfire = (GameObject) null;
      }

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        player.healthHaver.OnPreDeath += new Action<Vector2>(this.HandlePreDeath);
        base.Pickup(player);
      }

      protected override void Update()
      {
        base.Update();
        if (!this.DoesBonfireSynergy || !(bool) (UnityEngine.Object) this.m_owner || !this.m_owner.HasActiveBonusSynergy(CustomSynergyType.THE_REAL_DARK_SOULS) || this.m_owner.CurrentRoom == null || ExtraLifeItem.s_bonfiredRooms.Contains(this.m_owner.CurrentRoom) || GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.NONE)
          return;
        RoomHandler currentRoom = this.m_owner.CurrentRoom;
        if (!((currentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SPECIAL && currentRoom.area.PrototypeRoomSpecialSubcategory == PrototypeDungeonRoom.RoomSpecialSubCategory.STANDARD_SHOP) | currentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.EXIT))
          return;
        bool success = false;
        IntVector2 visibleClearSpot = currentRoom.GetCenteredVisibleClearSpot(4, 4, out success);
        if (success)
        {
          ExtraLifeItem.LastActivatedBonfire = UnityEngine.Object.Instantiate<GameObject>(this.BonfireSynergyBonfire, (visibleClearSpot + new IntVector2(1, 1)).ToVector2().ToVector3ZisY(), Quaternion.identity);
          LootEngine.DoDefaultSynergyPoof(visibleClearSpot.ToVector2() + new Vector2(2f, 2f));
          PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(ExtraLifeItem.LastActivatedBonfire.GetComponent<SpeculativeRigidbody>());
        }
        ExtraLifeItem.s_bonfiredRooms.Add(currentRoom);
      }

      private void HandlePreDeath(Vector2 damageDirection)
      {
        if ((bool) (UnityEngine.Object) this.m_owner)
        {
          if (this.m_owner.IsInMinecart)
            this.m_owner.currentMineCart.EvacuateSpecificPlayer(this.m_owner, true);
          for (int index = 0; index < this.m_owner.passiveItems.Count; ++index)
          {
            if (this.m_owner.passiveItems[index] is CompanionItem && this.m_owner.passiveItems[index].DisplayName == "Pig" || this.m_owner.passiveItems[index] is ExtraLifeItem && this.extraLifeMode != ExtraLifeItem.ExtraLifeMode.DARK_SOULS && (this.m_owner.passiveItems[index] as ExtraLifeItem).extraLifeMode == ExtraLifeItem.ExtraLifeMode.DARK_SOULS)
              return;
          }
        }
        if (this.m_owner.IsInMinecart)
          this.m_owner.currentMineCart.EvacuateSpecificPlayer(this.m_owner, true);
        switch (this.extraLifeMode)
        {
          case ExtraLifeItem.ExtraLifeMode.ESCAPE_ROPE:
            this.HandleEscapeRopeStyle();
            break;
          case ExtraLifeItem.ExtraLifeMode.DARK_SOULS:
            this.HandleDarkSoulsStyle();
            break;
          case ExtraLifeItem.ExtraLifeMode.CLONE:
            this.HandleCloneStyle();
            return;
        }
        if (!this.consumedOnUse)
          return;
        this.m_owner.RemovePassiveItem(this.PickupObjectId);
      }

      private void HandleEscapeRopeStyle()
      {
        this.m_owner.healthHaver.FullHeal();
        this.m_owner.EscapeRoom(PlayerController.EscapeSealedRoomStyle.NONE, true);
      }

      private void HandleCloneStyle() => this.m_owner.HandleCloneItem(this);

      private void HandleDarkSoulsStyle()
      {
        this.m_owner.TriggerDarkSoulsReset(this.DropDarkSoulsItems, this.DarkSoulsCursedHealthMax);
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        player.healthHaver.OnPreDeath -= new Action<Vector2>(this.HandlePreDeath);
        debrisObject.GetComponent<ExtraLifeItem>().m_pickedUpThisRun = true;
        return debrisObject;
      }

      protected override void OnDestroy()
      {
        if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
          this.m_owner.healthHaver.OnPreDeath -= new Action<Vector2>(this.HandlePreDeath);
        base.OnDestroy();
      }

      public enum ExtraLifeMode
      {
        ESCAPE_ROPE,
        DARK_SOULS,
        CLONE,
      }
    }

}
