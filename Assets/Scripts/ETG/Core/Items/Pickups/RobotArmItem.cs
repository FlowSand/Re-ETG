// Decompiled with JetBrains decompiler
// Type: RobotArmItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Pickups
{
    public class RobotArmItem : PickupObject
    {
      private bool m_pickedUp;
      private GameObject minimapIcon;
      private RoomHandler m_minimapIconRoom;
      private GameObject m_instanceMinimapIcon;

      private void Start()
      {
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
        this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnPreCollision);
        if (this.m_pickedUp)
          return;
        this.RegisterMinimapIcon();
      }

      public void RegisterMinimapIcon()
      {
        if ((double) this.transform.position.y < -300.0)
          return;
        if ((Object) this.minimapIcon == (Object) null)
          this.minimapIcon = (GameObject) BraveResources.Load("Global Prefabs/Minimap_RobotArm_Icon");
        if (!((Object) this.minimapIcon != (Object) null) || this.m_pickedUp)
          return;
        this.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor));
        this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_minimapIconRoom, this.minimapIcon);
      }

      public void GetRidOfMinimapIcon()
      {
        if (!((Object) this.m_instanceMinimapIcon != (Object) null))
          return;
        Minimap.Instance.DeregisterRoomIcon(this.m_minimapIconRoom, this.m_instanceMinimapIcon);
        this.m_instanceMinimapIcon = (GameObject) null;
      }

      private void OnPreCollision(
        SpeculativeRigidbody otherRigidbody,
        SpeculativeRigidbody source,
        CollisionData collisionData)
      {
        if (this.m_pickedUp)
          return;
        PlayerController component = otherRigidbody.GetComponent<PlayerController>();
        if (!((Object) component != (Object) null))
          return;
        this.Pickup(component);
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_item_pickup_01", this.gameObject);
      }

      public bool CheckForCombination()
      {
        for (int index1 = 0; index1 < GameManager.Instance.AllPlayers.Length; ++index1)
        {
          for (int index2 = 0; index2 < GameManager.Instance.AllPlayers[index1].additionalItems.Count; ++index2)
          {
            if (GameManager.Instance.AllPlayers[index1].additionalItems[index2] is RobotArmBalloonsItem)
            {
              RobotArmQuestController.CombineBalloonsWithArm(GameManager.Instance.AllPlayers[index1].additionalItems[index2], (PickupObject) this, GameManager.Instance.AllPlayers[index1]);
              return true;
            }
          }
        }
        return false;
      }

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        this.m_pickedUp = true;
        this.GetRidOfMinimapIcon();
        if (this.CheckForCombination())
          return;
        if (!GameStatsManager.Instance.GetFlag(GungeonFlags.META_SHOP_EVER_SEEN_ROBOT_ARM))
        {
          GameManager.BroadcastRoomFsmEvent("armPickedUp", player.CurrentRoom);
          GameStatsManager.Instance.SetFlag(GungeonFlags.META_SHOP_EVER_SEEN_ROBOT_ARM, true);
          GameManager.Instance.Dungeon.data.DistributeComplexSecretPuzzleItems(new List<PickupObject>()
          {
            PickupObjectDatabase.GetById(GlobalItemIds.RobotBalloons)
          }, (RoomHandler) null, true);
        }
        this.specRigidbody.enabled = false;
        this.renderer.enabled = false;
        this.HandleEncounterable(player);
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        DebrisObject component = this.GetComponent<DebrisObject>();
        if ((Object) component != (Object) null)
        {
          Object.Destroy((Object) component);
          Object.Destroy((Object) this.specRigidbody);
          player.BloopItemAboveHead(this.sprite, string.Empty);
          player.AcquirePuzzleItem((PickupObject) this);
        }
        else
        {
          Object.Instantiate<GameObject>(this.gameObject);
          player.BloopItemAboveHead(this.sprite, string.Empty);
          player.AcquirePuzzleItem((PickupObject) this);
        }
        GameUIRoot.Instance.UpdatePlayerConsumables(player.carriedConsumables);
      }

      protected override void OnDestroy()
      {
        if (Minimap.HasInstance)
          this.GetRidOfMinimapIcon();
        base.OnDestroy();
      }
    }

}
