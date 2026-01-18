using Dungeonator;
using UnityEngine;

#nullable disable

public class AmmoPickup : PickupObject, IPlayerInteractable
  {
    public AmmoPickup.AmmoPickupMode mode = AmmoPickup.AmmoPickupMode.FULL_AMMO;
    public GameObject pickupVFX;
    public GameObject minimapIcon;
    public float SpreadAmmoCurrentGunPercent = 0.5f;
    public float SpreadAmmoOtherGunsPercent = 0.2f;
    [Header("Custom Ammo")]
    public bool AppliesCustomAmmunition;
    [ShowInInspectorIf("AppliesCustomAmmunition", false)]
    public float CustomAmmunitionDamageModifier = 1f;
    [ShowInInspectorIf("AppliesCustomAmmunition", false)]
    public float CustomAmmunitionSpeedModifier = 1f;
    [ShowInInspectorIf("AppliesCustomAmmunition", false)]
    public float CustomAmmunitionRangeModifier = 1f;
    private bool m_pickedUp;
    private RoomHandler m_minimapIconRoom;
    private GameObject m_instanceMinimapIcon;

    public bool pickedUp => this.m_pickedUp;

    private void Start()
    {
      if ((Object) this.minimapIcon != (Object) null && !this.m_pickedUp)
      {
        this.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor));
        this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_minimapIconRoom, this.minimapIcon);
      }
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f);
      if (!this.AppliesCustomAmmunition)
        return;
      this.sprite.usesOverrideMaterial = true;
      this.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/RainbowChestShader");
    }

    private void Update()
    {
      if (this.m_pickedUp || this.m_isBeingEyedByRat || !this.ShouldBeTakenByRat(this.sprite.WorldCenter))
        return;
      GameManager.Instance.Dungeon.StartCoroutine(this.HandleRatTheft());
    }

    private void GetRidOfMinimapIcon()
    {
      if (!((Object) this.m_instanceMinimapIcon != (Object) null))
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
      player.ResetTarnisherClipCapacity();
      if ((Object) player.CurrentGun == (Object) null || player.CurrentGun.ammo == player.CurrentGun.AdjustedMaxAmmo || !player.CurrentGun.CanGainAmmo)
        return;
      switch (this.mode)
      {
        case AmmoPickup.AmmoPickupMode.ONE_CLIP:
          player.CurrentGun.GainAmmo(player.CurrentGun.ClipCapacity);
          break;
        case AmmoPickup.AmmoPickupMode.FULL_AMMO:
          if (player.CurrentGun.AdjustedMaxAmmo > 0)
          {
            player.CurrentGun.GainAmmo(player.CurrentGun.AdjustedMaxAmmo);
            player.CurrentGun.ForceImmediateReload();
            string header = StringTableManager.GetString("#AMMO_SINGLE_GUN_REFILLED_HEADER");
            string description = $"{player.CurrentGun.GetComponent<EncounterTrackable>().journalData.GetPrimaryDisplayName()} {StringTableManager.GetString("#AMMO_SINGLE_GUN_REFILLED_BODY")}";
            tk2dBaseSprite sprite = player.CurrentGun.GetSprite();
            if (!GameUIRoot.Instance.BossHealthBarVisible)
            {
              GameUIRoot.Instance.notificationController.DoCustomNotification(header, description, sprite.Collection, sprite.spriteId);
              break;
            }
            break;
          }
          break;
        case AmmoPickup.AmmoPickupMode.SPREAD_AMMO:
          player.CurrentGun.GainAmmo(Mathf.CeilToInt((float) player.CurrentGun.AdjustedMaxAmmo * this.SpreadAmmoCurrentGunPercent));
          for (int index = 0; index < player.inventory.AllGuns.Count; ++index)
          {
            if ((bool) (Object) player.inventory.AllGuns[index] && (Object) player.CurrentGun != (Object) player.inventory.AllGuns[index])
              player.inventory.AllGuns[index].GainAmmo(Mathf.FloorToInt((float) player.inventory.AllGuns[index].AdjustedMaxAmmo * this.SpreadAmmoOtherGunsPercent));
          }
          player.CurrentGun.ForceImmediateReload();
          if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          {
            PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(player);
            if (!otherPlayer.IsGhost)
            {
              for (int index = 0; index < otherPlayer.inventory.AllGuns.Count; ++index)
              {
                if ((bool) (Object) otherPlayer.inventory.AllGuns[index])
                  otherPlayer.inventory.AllGuns[index].GainAmmo(Mathf.FloorToInt((float) otherPlayer.inventory.AllGuns[index].AdjustedMaxAmmo * this.SpreadAmmoOtherGunsPercent));
              }
              otherPlayer.CurrentGun.ForceImmediateReload();
            }
          }
          string header1 = StringTableManager.GetString("#AMMO_SINGLE_GUN_REFILLED_HEADER");
          string description1 = StringTableManager.GetString("#AMMO_SPREAD_REFILLED_BODY");
          tk2dBaseSprite sprite1 = this.sprite;
          if (!GameUIRoot.Instance.BossHealthBarVisible)
          {
            GameUIRoot.Instance.notificationController.DoCustomNotification(header1, description1, sprite1.Collection, sprite1.spriteId);
            break;
          }
          break;
      }
      this.m_pickedUp = true;
      this.m_isBeingEyedByRat = false;
      this.GetRidOfMinimapIcon();
      if ((Object) this.pickupVFX != (Object) null)
        player.PlayEffectOnActor(this.pickupVFX, Vector3.zero);
      Object.Destroy((Object) this.gameObject);
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", this.gameObject);
    }

    public float GetDistanceToPoint(Vector2 point)
    {
      if (!(bool) (Object) this.sprite)
        return 1000f;
      Bounds bounds = this.sprite.GetBounds();
      bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
      float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
      float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
      return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2))) / 1.5f;
    }

    public float GetOverrideMaxDistance() => -1f;

    public void OnEnteredRange(PlayerController interactor)
    {
      if (!(bool) (Object) this || !interactor.CurrentRoom.IsRegistered((IPlayerInteractable) this) && !RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
        return;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white, 0.1f);
      this.sprite.UpdateZDepth();
    }

    public void OnExitRange(PlayerController interactor)
    {
      if (!(bool) (Object) this)
        return;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f);
      this.sprite.UpdateZDepth();
    }

    public void Interact(PlayerController interactor)
    {
      if (!(bool) (Object) this)
        return;
      if ((Object) interactor.CurrentGun == (Object) null || interactor.CurrentGun.ammo == interactor.CurrentGun.AdjustedMaxAmmo || interactor.CurrentGun.InfiniteAmmo || interactor.CurrentGun.RequiresFundsToShoot)
      {
        if (!((Object) interactor.CurrentGun != (Object) null))
          return;
        GameUIRoot.Instance.InformNeedsReload(interactor, new Vector3(interactor.specRigidbody.UnitCenter.x - interactor.transform.position.x, 1.25f, 0.0f), 1f, "#RELOAD_FULL");
      }
      else
      {
        if (RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
          RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable) this);
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
        this.Pickup(interactor);
      }
    }

    public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
    {
      shouldBeFlipped = false;
      return string.Empty;
    }

    public enum AmmoPickupMode
    {
      ONE_CLIP,
      FULL_AMMO,
      SPREAD_AMMO,
    }
  }

