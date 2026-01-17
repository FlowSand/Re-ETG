// Decompiled with JetBrains decompiler
// Type: PlayerItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Pickups
{
    public class PlayerItem : PickupObject, IPlayerInteractable
    {
      public static bool AllowDamageCooldownOnActive;
      private static GameObject m_defaultIcon;
      public bool consumable = true;
      [ShowInInspectorIf("consumable", false)]
      public bool consumableOnCooldownUse;
      [ShowInInspectorIf("consumable", false)]
      public bool consumableOnActiveUse;
      [ShowInInspectorIf("consumable", false)]
      public bool consumableHandlesOwnDuration;
      [ShowInInspectorIf("consumableHandlesOwnDuration", false)]
      public float customDestroyTime = -1f;
      public int numberOfUses = 1;
      public bool UsesNumberOfUsesBeforeCooldown;
      public bool canStack = true;
      public int roomCooldown = 1;
      public float timeCooldown;
      public float damageCooldown;
      public bool usableDuringDodgeRoll;
      public string useAnimation;
      [NonSerialized]
      public bool ForceAsExtant;
      [NonSerialized]
      public bool PreventCooldownBar;
      public GameObject minimapIcon;
      private RoomHandler m_minimapIconRoom;
      private GameObject m_instanceMinimapIcon;
      public Action<PlayerItem> OnActivationStatusChanged;
      [NonSerialized]
      protected bool m_isCurrentlyActive;
      private bool m_isDestroyed;
      [NonSerialized]
      protected float m_activeElapsed;
      [NonSerialized]
      protected float m_activeDuration;
      [NonSerialized]
      protected int m_cachedNumberOfUses;
      [NonSerialized]
      protected bool m_pickedUp;
      [NonSerialized]
      protected bool m_pickedUpThisRun;
      private int remainingRoomCooldown;
      private float remainingTimeCooldown;
      private float remainingDamageCooldown;
      public string OnActivatedSprite = string.Empty;
      public string OnCooldownSprite = string.Empty;
      [SerializeField]
      public StatModifier[] passiveStatModifiers;
      private int m_baseSpriteID = -1;
      [NonSerialized]
      public PlayerController LastOwner;
      [NonSerialized]
      protected float m_adjustedTimeScale = 1f;
      public Action<PlayerController> OnPickedUp;
      public System.Action OnPreDropEvent;

      public bool IsCurrentlyActive
      {
        get => this.m_isCurrentlyActive;
        protected set
        {
          if (value == this.m_isCurrentlyActive)
            return;
          this.m_isCurrentlyActive = value;
          if (this.OnActivationStatusChanged == null)
            return;
          this.OnActivationStatusChanged(this);
        }
      }

      public bool PickedUp => this.m_pickedUp;

      public int CurrentRoomCooldown
      {
        get => this.remainingRoomCooldown;
        set => this.remainingRoomCooldown = value;
      }

      public float CurrentTimeCooldown
      {
        get => this.remainingTimeCooldown;
        set => this.remainingTimeCooldown = value;
      }

      public float CurrentDamageCooldown
      {
        get => this.remainingDamageCooldown;
        set => this.remainingDamageCooldown = value;
      }

      public bool IsActive => this.IsCurrentlyActive;

      public bool IsOnCooldown
      {
        get
        {
          return this.remainingRoomCooldown > 0 || (double) this.remainingTimeCooldown > 0.0 || (double) this.remainingDamageCooldown > 0.0;
        }
      }

      public float ActivePercentage => Mathf.Clamp01(this.m_activeElapsed / this.m_activeDuration);

      public float CooldownPercentage
      {
        get
        {
          if (this.IsCurrentlyActive)
            return this.ActivePercentage;
          if (!this.IsOnCooldown)
            return 0.0f;
          if (this.remainingRoomCooldown > 0)
            return (float) this.remainingRoomCooldown / (float) this.roomCooldown;
          if ((double) this.remainingDamageCooldown > 0.0)
            return this.remainingDamageCooldown / this.damageCooldown;
          return (double) this.remainingTimeCooldown > 0.0 ? this.remainingTimeCooldown / this.timeCooldown : 0.0f;
        }
      }

      protected virtual void Start()
      {
        this.m_baseSpriteID = this.sprite.spriteId;
        this.m_cachedNumberOfUses = this.numberOfUses;
        if (this.m_pickedUp)
          return;
        this.renderer.enabled = true;
        if (!(this is SilencerItem))
          SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f);
        this.RegisterMinimapIcon();
      }

      public virtual void Update()
      {
        if (this.m_pickedUp)
        {
          if ((UnityEngine.Object) this.LastOwner == (UnityEngine.Object) null)
            this.LastOwner = this.GetComponentInParent<PlayerController>();
          if ((double) this.remainingTimeCooldown > 0.0 && (PlayerItem.AllowDamageCooldownOnActive || !this.IsCurrentlyActive))
            this.remainingTimeCooldown = Mathf.Max(0.0f, this.remainingTimeCooldown - BraveTime.DeltaTime);
          if (!this.IsCurrentlyActive)
            return;
          this.m_activeElapsed += BraveTime.DeltaTime * this.m_adjustedTimeScale;
          if (string.IsNullOrEmpty(this.OnActivatedSprite))
            return;
          this.sprite.SetSprite(this.OnActivatedSprite);
        }
        else
        {
          this.HandlePickupCurseParticles();
          if (this.m_isBeingEyedByRat || UnityEngine.Time.frameCount % 47 != 0 || !this.ShouldBeTakenByRat(this.sprite.WorldCenter))
            return;
          GameManager.Instance.Dungeon.StartCoroutine(this.HandleRatTheft());
        }
      }

      public void RegisterMinimapIcon()
      {
        if ((double) this.transform.position.y < -300.0)
          return;
        if ((UnityEngine.Object) this.minimapIcon == (UnityEngine.Object) null)
        {
          if ((UnityEngine.Object) PlayerItem.m_defaultIcon == (UnityEngine.Object) null)
            PlayerItem.m_defaultIcon = (GameObject) BraveResources.Load("Global Prefabs/Minimap_Item_Icon");
          this.minimapIcon = PlayerItem.m_defaultIcon;
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

      protected bool UseConsumableStack()
      {
        --this.numberOfUses;
        if (this.numberOfUses > 0)
          return false;
        this.m_isDestroyed = true;
        return true;
      }

      public virtual bool CanBeUsed(PlayerController user) => true;

      public void ResetSprite()
      {
        if (string.IsNullOrEmpty(this.OnCooldownSprite) && string.IsNullOrEmpty(this.OnActivatedSprite) || this.sprite.spriteId == this.m_baseSpriteID)
          return;
        this.sprite.SetSprite(this.m_baseSpriteID);
      }

      public bool Use(PlayerController user, out float destroyTime)
      {
        destroyTime = -1f;
        if (this.m_isDestroyed || !this.CanBeUsed(user))
          return false;
        if (this.IsCurrentlyActive)
        {
          this.DoActiveEffect(user);
          if (this.consumable && this.consumableOnActiveUse && this.UseConsumableStack())
            return true;
          if (!string.IsNullOrEmpty(this.OnActivatedSprite) && this.sprite.spriteId != this.m_baseSpriteID)
            this.sprite.SetSprite(this.m_baseSpriteID);
          return false;
        }
        if (this.IsOnCooldown)
        {
          this.DoOnCooldownEffect(user);
          if (this.consumable && this.consumableOnCooldownUse && this.UseConsumableStack())
            return true;
          if (!string.IsNullOrEmpty(this.OnCooldownSprite) && this.sprite.spriteId != this.m_baseSpriteID)
            this.sprite.SetSprite(this.m_baseSpriteID);
          return false;
        }
        this.DoEffect(user);
        if (!string.IsNullOrEmpty(this.useAnimation))
        {
          tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName(this.useAnimation);
          this.spriteAnimator.Play(clipByName);
          destroyTime = (float) clipByName.frames.Length / clipByName.fps;
        }
        if (this.consumable && !this.consumableOnCooldownUse && !this.consumableOnActiveUse)
        {
          bool flag = this.UseConsumableStack();
          if (this.consumableHandlesOwnDuration)
            destroyTime = this.customDestroyTime;
          if (flag)
            return true;
        }
        else if (this.UsesNumberOfUsesBeforeCooldown)
          --this.numberOfUses;
        if ((double) destroyTime >= 0.0)
          this.StartCoroutine(this.HandleAnimationReset(destroyTime));
        if (!this.UsesNumberOfUsesBeforeCooldown || this.numberOfUses <= 0)
        {
          if (this.UsesNumberOfUsesBeforeCooldown)
            this.numberOfUses = this.m_cachedNumberOfUses;
          this.ApplyCooldown(user);
          this.AfterCooldownApplied(user);
        }
        return false;
      }

      public void ForceApplyCooldown(PlayerController user)
      {
        this.ApplyCooldown(user);
        this.AfterCooldownApplied(user);
      }

      protected void ApplyCooldown(PlayerController user)
      {
        float num1 = 1f;
        if ((UnityEngine.Object) user != (UnityEngine.Object) null)
        {
          float num2 = user.stats.GetStatValue(PlayerStats.StatType.Coolness) * 0.05f;
          if (PassiveItem.IsFlagSetForCharacter(user, typeof (ChamberOfEvilItem)))
          {
            float num3 = user.stats.GetStatValue(PlayerStats.StatType.Curse) * 0.05f;
            num2 += num3;
          }
          float num4 = Mathf.Clamp(num2, 0.0f, 0.5f);
          num1 = Mathf.Max(0.0f, num1 - num4);
        }
        this.remainingRoomCooldown += this.roomCooldown;
        this.remainingTimeCooldown += this.timeCooldown * num1;
        this.remainingDamageCooldown += this.damageCooldown * num1;
        if (string.IsNullOrEmpty(this.OnCooldownSprite))
          return;
        this.sprite.SetSprite(this.OnCooldownSprite);
      }

      protected void ApplyAdditionalTimeCooldown(float addTimeCooldown)
      {
        this.remainingTimeCooldown += addTimeCooldown;
      }

      protected void ApplyAdditionalDamageCooldown(float addDamageCooldown)
      {
        this.remainingDamageCooldown += addDamageCooldown;
      }

      [DebuggerHidden]
      private IEnumerator HandleAnimationReset(float delay)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PlayerItem.\u003CHandleAnimationReset\u003Ec__Iterator0()
        {
          delay = delay,
          \u0024this = this
        };
      }

      public void ClearCooldowns()
      {
        this.remainingRoomCooldown = 0;
        this.remainingDamageCooldown = 0.0f;
        this.remainingTimeCooldown = 0.0f;
      }

      public void DidDamage(PlayerController Owner, float damageDone)
      {
        if (this.IsActive && !PlayerItem.AllowDamageCooldownOnActive)
          return;
        float num = 1f;
        GameLevelDefinition loadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
        if (loadedLevelDefinition != null)
          num /= loadedLevelDefinition.enemyHealthMultiplier;
        damageDone *= num;
        this.remainingDamageCooldown = Mathf.Max(0.0f, this.remainingDamageCooldown - damageDone);
      }

      public void ClearedRoom()
      {
        if (this.remainingRoomCooldown <= 0)
          return;
        --this.remainingRoomCooldown;
      }

      public virtual void OnItemSwitched(PlayerController user)
      {
      }

      protected virtual void DoEffect(PlayerController user)
      {
      }

      protected virtual void AfterCooldownApplied(PlayerController user)
      {
      }

      protected virtual void DoActiveEffect(PlayerController user)
      {
      }

      protected virtual void DoOnCooldownEffect(PlayerController user)
      {
      }

      protected virtual void OnPreDrop(PlayerController user)
      {
      }

      public DebrisObject Drop(PlayerController player, float overrideForce = 4f)
      {
        this.OnPreDrop(player);
        if (this.OnPreDropEvent != null)
          this.OnPreDropEvent();
        Vector2 spawnDirection = (Vector2) (player.unadjustedAimPoint - player.sprite.WorldCenter.ToVector3ZUp());
        if (player.CurrentRoom != null && player.CurrentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SPECIAL)
        {
          overrideForce = 2f;
          spawnDirection = Vector2.down;
        }
        Vector3 vector3Zup = player.sprite.WorldCenter.ToVector3ZUp();
        if (this is RobotUnlockTelevisionItem)
          vector3Zup += new Vector3(0.0f, -0.875f, 0.0f);
        DebrisObject debrisObject = LootEngine.SpawnItem(this.gameObject, vector3Zup, spawnDirection, overrideForce);
        PlayerItem component = debrisObject.GetComponent<PlayerItem>();
        component.m_baseSpriteID = this.m_baseSpriteID;
        component.m_pickedUp = false;
        component.m_pickedUpThisRun = true;
        component.HasBeenStatProcessed = true;
        component.HasProcessedStatMods = this.HasProcessedStatMods;
        component.remainingDamageCooldown = this.remainingDamageCooldown;
        component.remainingRoomCooldown = this.remainingRoomCooldown;
        component.remainingTimeCooldown = this.remainingTimeCooldown;
        component.ResetSprite();
        component.CopyStateFrom(this);
        player.stats.RecalculateStats(player);
        return debrisObject;
      }

      protected virtual void CopyStateFrom(PlayerItem other)
      {
      }

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        if (GameManager.Instance.InTutorial)
          GameManager.BroadcastRoomTalkDoerFsmEvent("playerAcquiredPlayerItem");
        if (RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
          RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable) this);
        this.OnSharedPickup();
        this.GetRidOfMinimapIcon();
        if (this.ShouldBeDestroyedOnExistence())
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        }
        else
        {
          if (!PileOfDarkSoulsPickup.IsPileOfDarkSoulsPickup)
          {
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_item_pickup_01", this.gameObject);
            tk2dSprite component = UnityEngine.Object.Instantiate<GameObject>((GameObject) ResourceCache.Acquire("Global VFX/VFX_Item_Pickup")).GetComponent<tk2dSprite>();
            component.PlaceAtPositionByAnchor((Vector3) this.sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
            component.UpdateZDepth();
          }
          if (!this.m_pickedUpThisRun)
          {
            this.HandleLootMods(player);
            this.HandleEncounterable(player);
          }
          else if ((bool) (UnityEngine.Object) this.encounterTrackable && this.encounterTrackable.m_doNotificationOnEncounter && !EncounterTrackable.SuppressNextNotification && !GameUIRoot.Instance.BossHealthBarVisible)
            GameUIRoot.Instance.notificationController.DoNotification(this.encounterTrackable, true);
          this.LastOwner = player;
          this.m_isBeingEyedByRat = false;
          DebrisObject component1 = this.GetComponent<DebrisObject>();
          if ((UnityEngine.Object) component1 != (UnityEngine.Object) null || this.ForceAsExtant)
          {
            if ((bool) (UnityEngine.Object) component1)
              UnityEngine.Object.Destroy((UnityEngine.Object) component1);
            this.m_pickedUp = true;
            this.m_pickedUpThisRun = true;
            SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
            this.renderer.enabled = false;
            SquishyBounceWiggler component2 = this.GetComponent<SquishyBounceWiggler>();
            if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
            {
              UnityEngine.Object.Destroy((UnityEngine.Object) component2);
              this.sprite.ForceBuild();
            }
            player.GetEquippedWith(this);
          }
          else
          {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.gameObject);
            PlayerItem component3 = gameObject.GetComponent<PlayerItem>();
            gameObject.GetComponent<Renderer>().enabled = false;
            gameObject.transform.position = player.transform.position;
            component3.m_pickedUp = true;
            component3.m_pickedUpThisRun = true;
            player.GetEquippedWith(component3);
          }
          if (this.OnPickedUp != null)
            this.OnPickedUp(player);
          PlatformInterface.SetAlienFXColor((Color32) this.m_alienPickupColor, 1f);
          player.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
        }
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

      public void OnEnteredRange(PlayerController interactor)
      {
        if (!(bool) (UnityEngine.Object) this || !RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        if (!(this is SilencerItem))
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
        if (!(this is SilencerItem))
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
        if (GameManager.Instance.InTutorial)
          EncounterTrackable.SuppressNextNotification = true;
        this.Pickup(interactor);
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      protected override void OnDestroy()
      {
        base.OnDestroy();
        if (!Minimap.HasInstance)
          return;
        this.GetRidOfMinimapIcon();
      }
    }

}
