// Decompiled with JetBrains decompiler
// Type: PileOfDarkSoulsPickup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Pickups
{
    public class PileOfDarkSoulsPickup : PickupObject, IPlayerInteractable
    {
      [NonSerialized]
      public List<PlayerItem> activeItems = new List<PlayerItem>();
      [NonSerialized]
      public List<PassiveItem> passiveItems = new List<PassiveItem>();
      [NonSerialized]
      public List<Gun> guns = new List<Gun>();
      [NonSerialized]
      public List<PickupObject> additionalItems = new List<PickupObject>();
      [NonSerialized]
      public int TargetPlayerID = -1;
      public int containedCurrency;
      public GameObject pickupVFX;
      public GameObject minimapIcon;
      private bool m_pickedUp;
      private RoomHandler m_minimapIconRoom;
      private GameObject m_instanceMinimapIcon;
      public static bool IsPileOfDarkSoulsPickup;

      private void Start()
      {
        if ((UnityEngine.Object) this.minimapIcon != (UnityEngine.Object) null && !this.m_pickedUp)
        {
          this.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor));
          this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_minimapIconRoom, this.minimapIcon);
        }
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f);
      }

      public void ToggleItems(bool val)
      {
        for (int index = 0; index < this.guns.Count; ++index)
        {
          this.guns[index].gameObject.SetActive(val);
          this.guns[index].GetRidOfMinimapIcon();
        }
        for (int index = 0; index < this.activeItems.Count; ++index)
        {
          this.activeItems[index].gameObject.SetActive(val);
          this.activeItems[index].GetRidOfMinimapIcon();
        }
        for (int index = 0; index < this.passiveItems.Count; ++index)
        {
          this.passiveItems[index].gameObject.SetActive(val);
          this.passiveItems[index].GetRidOfMinimapIcon();
        }
        for (int index = 0; index < this.additionalItems.Count; ++index)
          this.additionalItems[index].gameObject.SetActive(val);
      }

      private void GetRidOfMinimapIcon()
      {
        if (!((UnityEngine.Object) this.m_instanceMinimapIcon != (UnityEngine.Object) null))
          return;
        Minimap.Instance.DeregisterRoomIcon(this.m_minimapIconRoom, this.m_instanceMinimapIcon);
        this.m_instanceMinimapIcon = (GameObject) null;
      }

      protected override void OnDestroy()
      {
        base.OnDestroy();
        if (!Minimap.HasInstance)
          return;
        this.GetRidOfMinimapIcon();
      }

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        this.ToggleItems(true);
        player.HandleDarkSoulsHollowTransition(false);
        this.m_pickedUp = true;
        player.healthHaver.CursedMaximum = float.MaxValue;
        float currentHealth = player.healthHaver.GetCurrentHealth();
        player.carriedConsumables.Currency += this.containedCurrency;
        EncounterTrackable.SuppressNextNotification = true;
        PileOfDarkSoulsPickup.IsPileOfDarkSoulsPickup = true;
        bool flag = false;
        for (int index = 0; index < this.passiveItems.Count; ++index)
        {
          EncounterTrackable.SuppressNextNotification = true;
          this.passiveItems[index].Pickup(player);
          if (this.passiveItems[index] is ExtraLifeItem && !flag)
          {
            ExtraLifeItem passiveItem = this.passiveItems[index] as ExtraLifeItem;
            if (passiveItem.extraLifeMode == ExtraLifeItem.ExtraLifeMode.DARK_SOULS && (bool) (UnityEngine.Object) passiveItem.encounterTrackable)
            {
              flag = true;
              EncounterTrackable.SuppressNextNotification = false;
              GameUIRoot.Instance.notificationController.DoNotification(passiveItem.encounterTrackable);
              EncounterTrackable.SuppressNextNotification = true;
            }
          }
        }
        for (int index = 0; index < this.activeItems.Count; ++index)
        {
          EncounterTrackable.SuppressNextNotification = true;
          this.activeItems[index].Pickup(player);
        }
        for (int index = 0; index < this.guns.Count; ++index)
        {
          EncounterTrackable.SuppressNextNotification = true;
          this.guns[index].Pickup(player);
        }
        for (int index = 0; index < this.additionalItems.Count; ++index)
        {
          EncounterTrackable.SuppressNextNotification = true;
          this.additionalItems[index].Pickup(player);
        }
        player.ChangeGun(1);
        EncounterTrackable.SuppressNextNotification = false;
        PileOfDarkSoulsPickup.IsPileOfDarkSoulsPickup = false;
        player.healthHaver.ForceSetCurrentHealth(currentHealth);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        return !(bool) (UnityEngine.Object) this.sprite ? 1000f : Vector2.Distance(point, this.sprite.WorldCenter) / 2f;
      }

      public float GetOverrideMaxDistance() => -1f;

      public void OnEnteredRange(PlayerController interactor)
      {
        if (!(bool) (UnityEngine.Object) this || interactor.PlayerIDX != this.TargetPlayerID || !RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white, 0.1f);
        this.sprite.UpdateZDepth();
      }

      public void OnExitRange(PlayerController interactor)
      {
        if (!(bool) (UnityEngine.Object) this || interactor.PlayerIDX != this.TargetPlayerID)
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f);
        this.sprite.UpdateZDepth();
      }

      public void Interact(PlayerController interactor)
      {
        if (!(bool) (UnityEngine.Object) this || interactor.PlayerIDX != this.TargetPlayerID)
          return;
        if (RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
          RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable) this);
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
        this.Pickup(interactor);
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }
    }

}
