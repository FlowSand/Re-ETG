using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class PassiveItem : PickupObject, IPlayerInteractable
  {
    public static Dictionary<PlayerController, Dictionary<System.Type, int>> ActiveFlagItems = new Dictionary<PlayerController, Dictionary<System.Type, int>>();
    private static GameObject m_defaultIcon;
    protected bool m_pickedUp;
    protected bool m_pickedUpThisRun;
    [NonSerialized]
    public bool suppressPickupVFX;
    [SerializeField]
    public StatModifier[] passiveStatModifiers;
    [SerializeField]
    public int ArmorToGainOnInitialPickup;
    public GameObject minimapIcon;
    private RoomHandler m_minimapIconRoom;
    private GameObject m_instanceMinimapIcon;
    protected PlayerController m_owner;
    public Action<PlayerController> OnPickedUp;
    public Action<PlayerController> OnDisabled;

    public static void IncrementFlag(PlayerController player, System.Type flagType)
    {
      if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
        PassiveItem.ActiveFlagItems.Add(player, new Dictionary<System.Type, int>());
      if (!PassiveItem.ActiveFlagItems[player].ContainsKey(flagType))
        PassiveItem.ActiveFlagItems[player].Add(flagType, 1);
      else
        PassiveItem.ActiveFlagItems[player][flagType] = PassiveItem.ActiveFlagItems[player][flagType] + 1;
    }

    public static void DecrementFlag(PlayerController player, System.Type flagType)
    {
      if (!PassiveItem.ActiveFlagItems.ContainsKey(player) || !PassiveItem.ActiveFlagItems[player].ContainsKey(flagType))
        return;
      PassiveItem.ActiveFlagItems[player][flagType] = PassiveItem.ActiveFlagItems[player][flagType] - 1;
      if (PassiveItem.ActiveFlagItems[player][flagType] > 0)
        return;
      PassiveItem.ActiveFlagItems[player].Remove(flagType);
    }

    public static bool IsFlagSetAtAll(System.Type flagType)
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        if (PassiveItem.IsFlagSetForCharacter(GameManager.Instance.AllPlayers[index], flagType))
          return true;
      }
      return false;
    }

    public static bool IsFlagSetForCharacter(PlayerController player, System.Type flagType)
    {
      return PassiveItem.ActiveFlagItems.ContainsKey(player) && PassiveItem.ActiveFlagItems[player].ContainsKey(flagType) && PassiveItem.ActiveFlagItems[player][flagType] > 0;
    }

    public bool PickedUp => this.m_pickedUp;

    public PlayerController Owner => this.m_owner;

    private void Start()
    {
      if (!this.m_pickedUp)
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f);
      if (this.m_pickedUp)
        return;
      this.RegisterMinimapIcon();
    }

    public void RegisterMinimapIcon()
    {
      if ((double) this.transform.position.y < -300.0)
        return;
      if ((UnityEngine.Object) this.minimapIcon == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) PassiveItem.m_defaultIcon == (UnityEngine.Object) null)
          PassiveItem.m_defaultIcon = (GameObject) BraveResources.Load("Global Prefabs/Minimap_Item_Icon");
        this.minimapIcon = PassiveItem.m_defaultIcon;
      }
      if (!((UnityEngine.Object) this.minimapIcon != (UnityEngine.Object) null) || this.m_pickedUp)
        return;
      this.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor));
      this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_minimapIconRoom, this.minimapIcon);
    }

    public void GetRidOfMinimapIcon()
    {
      if (!((UnityEngine.Object) this.m_instanceMinimapIcon != (UnityEngine.Object) null))
        return;
      Minimap.Instance.DeregisterRoomIcon(this.m_minimapIconRoom, this.m_instanceMinimapIcon);
      this.m_instanceMinimapIcon = (GameObject) null;
    }

    public virtual DebrisObject Drop(PlayerController player)
    {
      this.m_pickedUp = false;
      this.m_pickedUpThisRun = true;
      this.HasBeenStatProcessed = true;
      this.DisableEffect(player);
      this.m_owner = (PlayerController) null;
      DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(this.gameObject, player.LockedApproximateSpriteCenter, (Vector2) (player.unadjustedAimPoint - player.LockedApproximateSpriteCenter), 4f);
      SpriteOutlineManager.AddOutlineToSprite(debrisObject.sprite, Color.black, 0.1f);
      this.RegisterMinimapIcon();
      return debrisObject;
    }

    public float GetDistanceToPoint(Vector2 point)
    {
      if (this.IsBeingSold || !(bool) (UnityEngine.Object) this.sprite)
        return 1000f;
      Bounds bounds = this.sprite.GetBounds();
      bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
      float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
      float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
      return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
    }

    public float GetOverrideMaxDistance() => -1f;

    protected virtual void Update()
    {
      if (this.m_pickedUp || !((UnityEngine.Object) this.m_owner == (UnityEngine.Object) null))
        return;
      this.HandlePickupCurseParticles();
      if (this.m_isBeingEyedByRat || UnityEngine.Time.frameCount % 51 != 0 || !this.ShouldBeTakenByRat(this.sprite.WorldCenter))
        return;
      GameManager.Instance.Dungeon.StartCoroutine(this.HandleRatTheft());
    }

    public void OnEnteredRange(PlayerController interactor)
    {
      if (!(bool) (UnityEngine.Object) this || !RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
        return;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white, 0.1f);
      this.sprite.UpdateZDepth();
      SquishyBounceWiggler component = this.GetComponent<SquishyBounceWiggler>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      component.WiggleHold = true;
    }

    public void OnExitRange(PlayerController interactor)
    {
      if (!(bool) (UnityEngine.Object) this)
        return;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
      if (this.m_pickedUp)
        return;
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f);
      this.sprite.UpdateZDepth();
      SquishyBounceWiggler component = this.GetComponent<SquishyBounceWiggler>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      component.WiggleHold = false;
    }

    public void Interact(PlayerController interactor)
    {
      if (GameStatsManager.HasInstance && GameStatsManager.Instance.IsRainbowRun)
      {
        if ((bool) (UnityEngine.Object) interactor && interactor.CurrentRoom != null && interactor.CurrentRoom == GameManager.Instance.Dungeon.data.Entrance && UnityEngine.Time.frameCount == PickupObject.s_lastRainbowPickupFrame)
          return;
        PickupObject.s_lastRainbowPickupFrame = UnityEngine.Time.frameCount;
      }
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
      this.Pickup(interactor);
    }

    public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
    {
      shouldBeFlipped = false;
      return string.Empty;
    }

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      if (RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
        RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable) this);
      if (!Dungeon.IsGenerating)
      {
        RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
        if (absoluteRoom.IsRegistered((IPlayerInteractable) this))
          absoluteRoom.DeregisterInteractable((IPlayerInteractable) this);
      }
      if (GameManager.Instance.InTutorial)
        GameManager.BroadcastRoomTalkDoerFsmEvent("playerAcquiredPassiveItem");
      this.OnSharedPickup();
      this.GetRidOfMinimapIcon();
      this.m_isBeingEyedByRat = false;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
      this.m_pickedUp = true;
      this.m_owner = player;
      if (this.OnPickedUp != null)
        this.OnPickedUp(this.m_owner);
      if (this.ShouldBeDestroyedOnExistence())
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
      else
      {
        if (!this.m_pickedUpThisRun)
        {
          this.HandleLootMods(player);
          this.HandleEncounterable(player);
          if (this.ArmorToGainOnInitialPickup > 0)
            player.healthHaver.Armor += (float) this.ArmorToGainOnInitialPickup;
          if (this.ItemSpansBaseQualityTiers || this.ItemRespectsHeartMagnificence)
            ++RewardManager.AdditionalHeartTierMagnificence;
        }
        else if ((bool) (UnityEngine.Object) this.encounterTrackable && this.encounterTrackable.m_doNotificationOnEncounter && !EncounterTrackable.SuppressNextNotification && !GameUIRoot.Instance.BossHealthBarVisible)
          GameUIRoot.Instance.notificationController.DoNotification(this.encounterTrackable, true);
        if (!this.m_pickedUpThisRun && player.characterIdentity == PlayableCharacters.Robot)
        {
          for (int index = 0; index < this.passiveStatModifiers.Length; ++index)
          {
            if (this.passiveStatModifiers[index].statToBoost == PlayerStats.StatType.Health && (double) this.passiveStatModifiers[index].amount > 0.0)
            {
              int amountToDrop = Mathf.FloorToInt(this.passiveStatModifiers[index].amount * (float) UnityEngine.Random.Range(GameManager.Instance.RewardManager.RobotMinCurrencyPerHealthItem, GameManager.Instance.RewardManager.RobotMaxCurrencyPerHealthItem + 1));
              LootEngine.SpawnCurrency(player.CenterPosition, amountToDrop);
            }
          }
        }
        if (!this.suppressPickupVFX && !PileOfDarkSoulsPickup.IsPileOfDarkSoulsPickup)
        {
          tk2dSprite component = UnityEngine.Object.Instantiate<GameObject>((GameObject) ResourceCache.Acquire("Global VFX/VFX_Item_Pickup")).GetComponent<tk2dSprite>();
          component.PlaceAtPositionByAnchor((Vector3) this.sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
          component.UpdateZDepth();
        }
        this.m_pickedUpThisRun = true;
        PlatformInterface.SetAlienFXColor((Color32) this.m_alienPickupColor, 1f);
        player.AcquirePassiveItem(this);
      }
    }

    protected virtual void DisableEffect(PlayerController disablingPlayer)
    {
      if (this.OnDisabled == null)
        return;
      this.OnDisabled(disablingPlayer);
    }

    public override void MidGameDeserialize(List<object> data)
    {
      base.MidGameDeserialize(data);
      for (int index = 0; index < this.passiveStatModifiers.Length; ++index)
      {
        if ((bool) (UnityEngine.Object) this.m_owner && this.passiveStatModifiers[index].statToBoost == PlayerStats.StatType.AdditionalBlanksPerFloor)
          this.m_owner.Blanks += Mathf.RoundToInt(this.passiveStatModifiers[index].amount);
      }
    }

    protected override void OnDestroy()
    {
      if (Minimap.HasInstance)
        this.GetRidOfMinimapIcon();
      this.DisableEffect(this.m_owner);
      this.m_owner = (PlayerController) null;
      base.OnDestroy();
    }
  }

