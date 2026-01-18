// Decompiled with JetBrains decompiler
// Type: HealthPickup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class HealthPickup : PickupObject, IPlayerInteractable
  {
    public string pickupName;
    public float healAmount = 1f;
    public int armorAmount;
    public GameObject healVFX;
    public GameObject armorVFX;
    public GameObject minimapIcon;
    private bool m_pickedUp;
    private RoomHandler m_minimapIconRoom;
    private GameObject m_instanceMinimapIcon;
    private bool m_placedInWorld;

    private void Awake()
    {
      if (!Dungeon.IsGenerating)
        return;
      this.m_placedInWorld = true;
    }

    public void Start()
    {
      this.specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.TriggerWasEntered);
      this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnTrigger);
      if (!((Object) this.minimapIcon != (Object) null) || this.m_pickedUp)
        return;
      this.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor));
      this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_minimapIconRoom, this.minimapIcon);
    }

    private void TriggerWasEntered(
      SpeculativeRigidbody otherRigidbody,
      SpeculativeRigidbody selfRigidbody,
      CollisionData collisionData)
    {
      if (this.m_pickedUp)
        return;
      if ((Object) otherRigidbody.GetComponent<PlayerController>() != (Object) null)
      {
        this.PrePickupLogic(otherRigidbody, selfRigidbody);
      }
      else
      {
        if (!((Object) otherRigidbody.GetComponent<PickupObject>() != (Object) null) || !(bool) (Object) this.debris)
          return;
        this.debris.ApplyVelocity((selfRigidbody.UnitCenter - otherRigidbody.UnitCenter).normalized);
        selfRigidbody.RegisterGhostCollisionException(otherRigidbody);
      }
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
      if (GameUIRoot.HasInstance)
        this.ToggleLabel(false);
      if (!Minimap.HasInstance)
        return;
      this.GetRidOfMinimapIcon();
    }

    public void OnTrigger(
      SpeculativeRigidbody otherRigidbody,
      SpeculativeRigidbody selfRigidbody,
      CollisionData collisionData)
    {
      if (this.m_pickedUp || !((Object) otherRigidbody.GetComponent<PlayerController>() != (Object) null))
        return;
      this.PrePickupLogic(otherRigidbody, selfRigidbody);
    }

    private void PrePickupLogic(
      SpeculativeRigidbody otherRigidbody,
      SpeculativeRigidbody selfRigidbody)
    {
      PlayerController component = otherRigidbody.GetComponent<PlayerController>();
      if (component.IsGhost)
        return;
      HealthHaver healthHaver = otherRigidbody.healthHaver;
      if (component.HealthAndArmorSwapped)
      {
        if ((double) healthHaver.GetCurrentHealth() == (double) healthHaver.GetMaxHealth() && this.armorAmount > 0)
        {
          if (!(bool) (Object) this.debris)
            return;
          this.debris.ApplyVelocity(otherRigidbody.Velocity / 4f);
          selfRigidbody.RegisterTemporaryCollisionException(otherRigidbody, 0.25f);
        }
        else
        {
          this.Pickup(component);
          Object.Destroy((Object) this.gameObject);
        }
      }
      else if ((double) healthHaver.GetCurrentHealth() == (double) healthHaver.GetMaxHealth() && this.armorAmount == 0)
      {
        if (component.HasActiveBonusSynergy(CustomSynergyType.COIN_KING_OF_HEARTS))
        {
          this.m_pickedUp = true;
          int num = (int) AkSoundEngine.PostEvent("Play_OBJ_coin_medium_01", this.gameObject);
          int amountToDrop = (double) this.healAmount >= 1.0 ? Random.Range(5, 12) : Random.Range(3, 7);
          LootEngine.SpawnCurrency(!(bool) (Object) this.sprite ? component.CenterPosition : this.sprite.WorldCenter, amountToDrop);
          this.GetRidOfMinimapIcon();
          this.ToggleLabel(false);
          Object.Destroy((Object) this.gameObject);
        }
        else
        {
          if (!(bool) (Object) this.debris)
            return;
          this.debris.ApplyVelocity(otherRigidbody.Velocity / 4f);
          selfRigidbody.RegisterTemporaryCollisionException(otherRigidbody, 0.25f);
        }
      }
      else
      {
        this.Pickup(healthHaver.GetComponent<PlayerController>());
        Object.Destroy((Object) this.gameObject);
      }
    }

    public virtual void Update()
    {
      if (this.armorAmount <= 0 || (double) this.healAmount > 0.0 || this.m_pickedUp || this.m_isBeingEyedByRat || UnityEngine.Time.frameCount % 47 != 0 || this.m_placedInWorld || !this.ShouldBeTakenByRat(this.sprite.WorldCenter))
        return;
      GameManager.Instance.Dungeon.StartCoroutine(this.HandleRatTheft());
    }

    public override void Pickup(PlayerController player)
    {
      if (player.IsGhost)
        return;
      this.HandleEncounterable(player);
      this.GetRidOfMinimapIcon();
      this.ToggleLabel(false);
      this.m_pickedUp = true;
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", this.gameObject);
      if (this.armorAmount > 0 && (double) this.healAmount > 0.0)
      {
        bool flag = (double) player.healthHaver.GetCurrentHealth() != (double) player.healthHaver.GetMaxHealth();
        if (player.HealthAndArmorSwapped)
        {
          player.healthHaver.Armor += (float) Mathf.CeilToInt(this.healAmount);
          player.healthHaver.ApplyHealing((float) this.armorAmount);
        }
        else
        {
          player.healthHaver.ApplyHealing(this.healAmount);
          player.healthHaver.Armor += (float) this.armorAmount;
        }
        if (flag && (Object) this.healVFX != (Object) null)
          player.PlayEffectOnActor(this.healVFX, Vector3.zero);
        else if ((Object) this.armorVFX != (Object) null)
          player.PlayEffectOnActor(this.armorVFX, Vector3.zero);
        else if ((Object) this.healVFX != (Object) null)
          player.PlayEffectOnActor(this.healVFX, Vector3.zero);
      }
      else if (this.armorAmount > 0)
      {
        if ((Object) this.armorVFX != (Object) null)
          player.PlayEffectOnActor(this.armorVFX, Vector3.zero);
        if (player.HealthAndArmorSwapped)
          player.healthHaver.ApplyHealing((float) this.armorAmount);
        else
          player.healthHaver.Armor += (float) this.armorAmount;
      }
      else
      {
        if ((Object) this.healVFX != (Object) null)
          player.PlayEffectOnActor(this.healVFX, Vector3.zero);
        if (player.HealthAndArmorSwapped)
          player.healthHaver.Armor += (float) Mathf.CeilToInt(this.healAmount);
        else
          player.healthHaver.ApplyHealing(this.healAmount);
      }
      Object.Destroy((Object) this.gameObject);
    }

    public float GetDistanceToPoint(Vector2 point)
    {
      if (this.IsBeingSold || this.m_pickedUp || !(bool) (Object) this.sprite || this.armorAmount > 0)
        return 1000f;
      Bounds bounds = this.sprite.GetBounds();
      bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
      float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
      float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
      return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
    }

    public void ToggleLabel(bool enabledValue)
    {
      if (enabledValue)
      {
        dfLabel componentInChildren = GameUIRoot.Instance.RegisterDefaultLabel(this.transform, new Vector3(1f, 3f / 16f, 0.0f), StringTableManager.GetString("#SAVE_FOR_LATER")).GetComponentInChildren<dfLabel>();
        componentInChildren.ColorizeSymbols = false;
        componentInChildren.ProcessMarkup = true;
      }
      else
      {
        if (GameManager.Instance.IsLoadingLevel || !(bool) (Object) GameUIRoot.Instance)
          return;
        GameUIRoot.Instance.DeregisterDefaultLabel(this.transform);
      }
    }

    public void OnEnteredRange(PlayerController interactor)
    {
      if (!(bool) (Object) this || this.m_pickedUp || this.armorAmount > 0 || !HeartDispenser.DispenserOnFloor || !RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this) || (double) interactor.healthHaver.GetCurrentHealthPercentage() < 1.0)
        return;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white, 0.1f);
      this.sprite.UpdateZDepth();
      this.ToggleLabel(true);
    }

    public void OnExitRange(PlayerController interactor)
    {
      if (!(bool) (Object) this || this.armorAmount > 0 || !HeartDispenser.DispenserOnFloor)
        return;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
      if (this.m_pickedUp)
        return;
      this.sprite.UpdateZDepth();
      this.ToggleLabel(false);
    }

    public void Interact(PlayerController interactor)
    {
      if (this.m_pickedUp || !HeartDispenser.DispenserOnFloor || this.armorAmount > 0 || (double) interactor.healthHaver.GetCurrentHealthPercentage() < 1.0)
        return;
      this.ToggleLabel(false);
      this.spriteAnimator.PlayAndDestroyObject((double) this.healAmount <= 0.5 ? "heart_small_teleport" : "heart_big_teleport");
      if ((double) this.healAmount > 0.5)
        HeartDispenser.CurrentHalfHeartsStored += 2;
      else
        ++HeartDispenser.CurrentHalfHeartsStored;
      this.m_pickedUp = true;
    }

    public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
    {
      shouldBeFlipped = false;
      return string.Empty;
    }

    public float GetOverrideMaxDistance() => -1f;

    private RoomHandler FindShop()
    {
      RoomHandler shop = (RoomHandler) null;
      for (int index1 = 0; index1 < GameManager.Instance.Dungeon.data.rooms.Count; ++index1)
      {
        RoomHandler room = GameManager.Instance.Dungeon.data.rooms[index1];
        if (room.IsShop)
        {
          BaseShopController[] componentsInChildren = room.hierarchyParent.GetComponentsInChildren<BaseShopController>();
          for (int index2 = 0; index2 < componentsInChildren.Length; ++index2)
          {
            if ((bool) (Object) componentsInChildren[index2] && componentsInChildren[index2].baseShopType == BaseShopController.AdditionalShopType.NONE)
            {
              shop = room;
              break;
            }
          }
          if (shop != null)
            break;
        }
      }
      return shop;
    }
  }

